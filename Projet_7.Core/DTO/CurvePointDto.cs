using System.ComponentModel.DataAnnotations;

namespace Projet_7.Core.DTO
{
    public class CurvePointDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ne doit pas être vide.")]
        public byte CurveId { get; set; }

        public double? Term { get; set; }

        public double? CurvePointValue { get; set; }
    }
}
