using System.ComponentModel.DataAnnotations;

namespace VintageMonsterMovies.Models
{
    public class Movie
    {
        public int id { get; set; }

        [Required(ErrorMessage = "The title is required.")]
        public string title { get; set; }

        [Required(ErrorMessage = "The publishing year is required.")]
        public int publishingYear { get; set; }

        [Required(ErrorMessage = "The description is required.")]
        public string description { get; set; }

        [Required(ErrorMessage = "The director is required.")]
        public string director { get; set; }

        [Required(ErrorMessage = "The writer is required.")]
        public string writer { get; set; }
    }
}
