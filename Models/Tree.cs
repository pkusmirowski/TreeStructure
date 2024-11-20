using System.ComponentModel.DataAnnotations;

namespace TreeStructure.Models
{
    /// <summary>
    /// Model reprezentujący węzeł drzewa w strukturze danych.
    /// </summary>
    public class Tree
    {
        /// <summary>
        /// Identyfikator węzła drzewa.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nazwa folderu w drzewie.
        /// </summary>
        [Required(ErrorMessage = "Pole 'Folder' jest wymagane.")]
        [StringLength(40, ErrorMessage = "Pole 'Folder' może mieć maksymalnie 40 znaków.")]
        [RegularExpression("^[a-zA-Z0-9-]+$", ErrorMessage = "Pole 'Folder' może zawierać tylko litery, cyfry i myślniki.")]
        public string Folder { get; set; } = null!;

        /// <summary>
        /// Identyfikator rodzica (jeśli istnieje).
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Referencja do nadrzędnego węzła.
        /// </summary>
        public virtual Tree? Parent { get; set; }

        /// <summary>
        /// Kolekcja dzieci węzła.
        /// </summary>
        public virtual ICollection<Tree> InverseParent { get; set; } = new HashSet<Tree>();
    }
}
