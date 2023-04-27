using System.ComponentModel.DataAnnotations;

namespace TreeStructure.Models
{
    public class Tree
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana")]
        [StringLength(40, ErrorMessage = "Nazwa może mieć maksymalnie 40 znaków")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "Nazwa może zawierać tylko litery, cyfry i myślniki")]
        public string Folder { get; set; } = null!;

        public int? ParentId { get; set; }

        public virtual Tree? Parent { get; set; }
        public virtual ICollection<Tree> InverseParent { get; set; } = new HashSet<Tree>();
    }
}
