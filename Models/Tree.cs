using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeStructure.Models
{
    public class Tree
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Folder name is required.")]
        [StringLength(40, ErrorMessage = "Folder name can be a maximum of 40 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "Folder name can only contain letters, numbers, and hyphens.")]
        public string? Folder { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Tree? Parent { get; set; }

        public virtual ICollection<Tree> InverseParent { get; set; } = new List<Tree>();
    }
}
