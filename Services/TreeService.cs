using Microsoft.EntityFrameworkCore;
using TreeStructure.Models;
using TreeStructure.VM;
using Microsoft.Extensions.Logging;

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
            var lookupId = (await _context.Trees.ToListAsync()).ToLookup(x => x.ParentId);
            var root = lookupId[null].FirstOrDefault();

            if (root == null)
            {
                return null;
            }

            return new TreeVM
            {
                Id = root.Id,
                Folder = root.Folder,
                ParentId = root.ParentId,
                InverseParent = root.InverseParent
            };
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
                _logger.LogError($"Error adding element: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteElementAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var node = await GetNodeByIdAsync(id);
                if (node == null)
                {
                    return false;
                }

                await DeleteChildrenRecursively(node);

                _context.Remove(node);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error deleting element: {ex.Message}");
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
            if (id == 0 || newId == 0)
            {
                return false;
            }

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

        public async Task<List<TreeVM>> GetTreeListAsync()
        {
            var trees = await _context.Trees.ToListAsync();

            var treeList = trees.Select(tree => new TreeVM
            {
                Id = tree.Id,
                Folder = tree.Folder,
                ParentId = tree.ParentId
            }).ToList();

            return treeList;
        }

        private async Task<Tree?> GetNodeByIdAsync(int id)
        {
            return await _context.Trees
                .Include(x => x.InverseParent)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task DeleteChildrenRecursively(Tree node)
        {
            foreach (var child in node.InverseParent.ToList())
            {
                await DeleteChildrenRecursively(child);
                _context.Remove(child);
            }
        }
    }
}
