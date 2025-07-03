using System.ComponentModel.DataAnnotations;

namespace Projet_7.Core.DTO
{
    public class RatingDto
    {
        public int Id { get; set; }

        public string? MoodysRating { get; set; }

        public string? SandPRating { get; set; }

        public string? FitchRating { get; set; }

        public byte? OrderNumber { get; set; }
    }
}
