using System.ComponentModel.DataAnnotations;

namespace Projet_7.Core.DTO
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        public string Password { get; set; }

        public string? FullName { get; set; }

        [Required(ErrorMessage = "Le rôle est obligatoire.")]
        public string Role { get; set; }
    }
}
