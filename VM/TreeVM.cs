using TreeStructure.Models;

namespace TreeStructure.VM
{
    public class TreeVM
    {

        public int Id { get; set; }
        public string Folder { get; set; } = null!;
        public int? ParentId { get; set; }

        public virtual ICollection<Tree>? InverseParent { get; set; }
    }
}
