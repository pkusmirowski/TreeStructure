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
            List<Tree> parent = GetParent(id);

            if (parent[0].InverseParent.Count == 0)
            {
                _context.Remove(parent[0]);
                await _context.SaveChangesAsync();
            }
            else
            {
                foreach (var child in parent)
                {
                    if (child.InverseParent.Count > 0)
                    {
                        await DeleteChild(child.InverseParent);
                    }
                }
                await DeleteElement(id);
            }
            return true;
        }

        public async Task<bool> EditElement(int id, string name)
        {
            List<Tree> parent = GetParent(id);

            parent[0].Folder = name;
            _context.Update(parent[0]);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveElement(int id, int newId)
        {
            List<Tree> parent = GetParent(id);
            parent[0].ParentId = newId;
            _context.Update(parent[0]);
            await _context.SaveChangesAsync();

            return true;
        }

        private List<Tree> GetParent(int id)
        {
            var lookupId = _context.Trees.ToLookup(x => x.ParentId);
            var treeElements = lookupId[null].SelectRecursive(x => lookupId[x.Id]).ToList();
            return treeElements.Where(res => res.Id == id).ToList();
        }

        private async Task<bool> DeleteChild(ICollection<Tree> child)
        {
            foreach (var children in child)
            {
                if (children.InverseParent.Count == 0)
                {
                    _context.Remove(children);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await DeleteChild(children.InverseParent);
                }
            }
            return true;
        }
    }
}
