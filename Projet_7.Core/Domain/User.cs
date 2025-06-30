namespace Projet_7.Core.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
    }

}