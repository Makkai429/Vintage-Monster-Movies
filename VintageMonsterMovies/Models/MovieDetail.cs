using System.ComponentModel.DataAnnotations;

namespace VintageMonsterMovies.Models
{
    public class MovieDetail
    {
        public int id { get; set; }

        [Required(ErrorMessage = "The ID of the movie is required.")]
        public int movieId { get; set; }

        [Required(ErrorMessage = "The stars of the movie are required.")]
        public string stars { get; set; }

        [Required(ErrorMessage = "The lenght of the movie is required .")]
        public int runtime { get; set; }

        [Required(ErrorMessage = "The storyline is required.")]
        public string storyline { get; set; }

        [Required(ErrorMessage = "The company is required.")]
        public string company { get; set; }

    }
}
