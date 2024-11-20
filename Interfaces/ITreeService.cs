using TreeStructure.VM;

namespace TreeStructure.Services
{
    public interface ITreeService
    {
        Task<TreeVM?> DisplayTreeAsync();
        Task<bool> AddElementAsync(int id, string name);
        Task<bool> DeleteElementAsync(int id);
        Task<bool> EditElementAsync(int id, string name);
        Task<bool> MoveElementAsync(int id, int newId);
        Task<List<TreeVM>> GetTreeListAsync();
    }
}
