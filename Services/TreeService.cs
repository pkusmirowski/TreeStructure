using TreeStructure.ExtensionMethods;
using TreeStructure.Models;
using TreeStructure.VM;

namespace TreeStructure.Services
{
    public class TreeService
    {
        private readonly TreeDBContext _context;

        public TreeService(TreeDBContext context)
        {
            _context = context;
        }

        public TreeVM DisplayTree()
        {
            var lookupId = _context.Trees.ToLookup(x => x.ParentId);
            var treeElements = lookupId[null].SelectRecursive(x => lookupId[x.Id]).ToList();

            var root = treeElements[0];

            return new TreeVM
            {
                Id = root.Id,
                Folder = root.Folder,
                ParentId = root.Id,
                InverseParent = root.InverseParent
            };
        }

        public async Task<bool> AddElement(int id, string name)
        {
            if (id == 0)
            {
                return false;
            }

            var newElement = new Tree
            {
                Folder = name,
                ParentId = id
            };

            await _context.AddAsync(newElement);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteElement(int id)
        {
            if (id == 0)
            {
                return false;
            }

            var parent = GetParent(id);

            if (parent.Count > 0)
            {
                var parentNode = parent[0];

                if (parentNode.InverseParent.Count > 0)
                {
                    await DeleteChild(parentNode.InverseParent);
                }

                _context.Remove(parentNode);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> EditElement(int id, string name)
        {
            if (id == 0)
            {
                return false;
            }

            var parent = GetParent(id);

            if (parent.Count > 0)
            {
                parent[0].Folder = name;
                _context.Update(parent[0]);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> MoveElement(int id, int newId)
        {
            if (id == 0 || newId == 0)
            {
                return false;
            }

            var parent = GetParent(id);

            if (parent.Count > 0)
            {
                parent[0].ParentId = newId;
                _context.Update(parent[0]);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private List<Tree> GetParent(int id)
        {
            var lookupId = _context.Trees.ToLookup(x => x.ParentId);
            var treeElements = lookupId[null].SelectRecursive(x => lookupId[x.Id]).ToList();
            return treeElements.Where(res => res.Id == id).ToList();
        }

        private async Task DeleteChild(ICollection<Tree> child)
        {
            foreach (var children in child)
            {
                if (children.InverseParent.Count > 0)
                {
                    await DeleteChild(children.InverseParent);
                }

                _context.Remove(children);
            }

            await _context.SaveChangesAsync();
        }
    }
}
