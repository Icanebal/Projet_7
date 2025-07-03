using System.ComponentModel.DataAnnotations;

namespace Projet_7.Core.DTO
{
    public class BidDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le compte est obligatoire.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Le type de l'offre est obligatoire.")]
        public string BidType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantité d'offre ne peut pas être négative.")]
        public double? BidQuantity { get; set; }
    }
}

