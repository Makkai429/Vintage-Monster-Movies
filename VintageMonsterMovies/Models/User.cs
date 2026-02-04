using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Models
{
    [Index(nameof(username), IsUnique = true)]
    [Index(nameof(email), IsUnique = true)]
    public class User {
        public int id { get; set; }

        [Required(ErrorMessage = "The username is required")]
        public string username { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress(ErrorMessage = "The email is in wrong format.")]
        public string email { get; set; }

        [Required(ErrorMessage = "The password is required.")]
        public string password { get; set; }

        public string status { get; set; } = "user";
    }
}
