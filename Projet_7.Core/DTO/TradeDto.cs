using System.ComponentModel.DataAnnotations;

namespace Projet_7.Core.DTO
{
    public class TradeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le compte est obligatoire.")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Le type de transaction est obligatoire.")]
        public string DealType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantité d'offre ne peut pas être négative.")]
        public double? BuyQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La quantité d'offre ne peut pas être négative.")]
        public double? SellQuantity { get; set; }
    }

}
