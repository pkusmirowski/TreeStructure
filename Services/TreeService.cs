using Microsoft.EntityFrameworkCore;
using TreeStructure.Models;
using TreeStructure.VM;

namespace TreeStructure.Services
{
    public class TreeService : ITreeService
    {
        private readonly TreeDBContext _context;
        private readonly ILogger<TreeService> _logger;

        public TreeService(TreeDBContext context, ILogger<TreeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TreeVM?> DisplayTreeAsync()
        {
            var root = await _context.Trees
                .Include(t => t.InverseParent)
                .FirstOrDefaultAsync(t => t.ParentId == null);

            if (root == null)
            {
                return null;
            }

            return new TreeVM
            {
                Id = root.Id,
                Folder = root.Folder,
                ParentId = root.ParentId,
                InverseParent = await GetChildrenAsync(root.Id)
            };
        }

        private async Task<List<Tree>> GetChildrenAsync(int parentId)
        {
            var children = await _context.Trees
                .Where(t => t.ParentId == parentId)
                .Include(t => t.InverseParent)
                .ToListAsync();

            foreach (var child in children)
            {
                child.InverseParent = await GetChildrenAsync(child.Id);
            }

            return children;
        }

        public async Task<bool> AddElementAsync(int id, string name)
        {
            if (id == 0 || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            try
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
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error adding element");
                return false;
            }
        }

        private async Task DeleteChildrenRecursively(Tree node)
        {
            var children = node.InverseParent?.ToList(); // Pobieramy listę dzieci

            if (children != null)
            {
                foreach (var child in children)
                {
                    await DeleteChildrenRecursively(child); // Rekurencyjne usunięcie dzieci
                }

                _context.Remove(node); // Usunięcie węzła
            }
        }

        public async Task<bool> DeleteElementAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var node = await GetNodeByIdAsync(id);
                if (node == null)
                {
                    return false;
                }

                await DeleteChildrenRecursively(node); // Usunięcie dzieci
                await _context.SaveChangesAsync(); // Zapisujemy zmiany do bazy

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting element");
                return false;
            }
        }

        public async Task<bool> EditElementAsync(int id, string name)
        {
            if (id == 0 || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var node = await GetNodeByIdAsync(id);
            if (node == null)
            {
                return false;
            }

            node.Folder = name;
            _context.Update(node);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MoveElementAsync(int id, int newId)
        {
            var node = await GetNodeByIdAsync(id);
            if (node == null)
            {
                return false;
            }

            node.ParentId = newId;
            _context.Update(node);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<Tree?> GetNodeByIdAsync(int id)
        {
            return await _context.Trees
                .Include(x => x.InverseParent)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<TreeVM>> GetTreeListAsync()
        {
            var trees = await _context.Trees.ToListAsync();

            var treeList = trees.ConvertAll(tree => new TreeVM
            {
                Id = tree.Id,
                Folder = tree.Folder,
                ParentId = tree.ParentId
            });

            return treeList;
        }
        public async Task<bool> AddNodeAsync(int parentId, string nodeName)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                return false;
            }

            try
            {
                var newNode = new Tree
                {
                    Folder = nodeName,
                    ParentId = parentId
                };

                await _context.AddAsync(newNode);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error adding node");
                return false;
            }
        }

        public async Task<List<Tree>> SearchTreeAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<Tree>();
            }

            var trees = await _context.Trees
                .Where(t => t.Folder != null && t.Folder.Contains(name))
                .ToListAsync();

            return trees;
        }
    }
}
