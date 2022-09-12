using System.ComponentModel.DataAnnotations;

namespace TreeStructure.Models
{
    public class Tree
    {
        public Tree()
        {
            InverseParent = new HashSet<Tree>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana")]
        [StringLength(40)]
        [RegularExpression("[a-zA-Z0-9-]+")]
        public string Folder { get; set; } = null!;
        public int? ParentId { get; set; }

        public virtual Tree? Parent { get; set; }
        public virtual ICollection<Tree> InverseParent { get; set; }
    }
}
