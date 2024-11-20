using Microsoft.EntityFrameworkCore;
using TreeStructure.Models;
using TreeStructure.VM;

namespace TreeStructure.Services
{
    public class TreeService : ITreeService
    {
        private readonly TreeDBContext _context;

        public TreeService(TreeDBContext context)
        {
            _context = context;
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
        // Logowanie błędu
        Console.WriteLine($"Błąd podczas dodawania elementu: {ex.Message}");
        return false;
    }
}


        public async Task<bool> DeleteElementAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var node = GetNodeById(id);
                if (node == null)
                {
                    return false;
                }

                await DeleteChildrenRecursively(node);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> EditElementAsync(int id, string name)
        {
            if (id == 0 || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var node = GetNodeById(id);
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

            var node = GetNodeById(id);
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

        private Tree? GetNodeById(int id)
        {
            return _context.Trees
                .Include(x => x.InverseParent)
                .FirstOrDefault(x => x.Id == id);
        }

        private async Task DeleteChildrenRecursively(Tree node)
        {
            foreach (var child in node.InverseParent.ToList())
            {
                await DeleteChildrenRecursively(child);
                _context.Remove(child);
            }

            _context.Remove(node);
            await _context.SaveChangesAsync();
        }
    }
}
