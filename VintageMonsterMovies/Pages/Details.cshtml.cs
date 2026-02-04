using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Pages
{
    public class DetailsModel : PageModel
    {
        private HorrorDbContext _context;

        public Movie movie { get; set; }

        public MovieDetail detail { get; set; }

        public DetailsModel(HorrorDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            movie = await _context.Movies.FirstOrDefaultAsync(m => m.id == id);

            if (movie == null)
                return NotFound();

            detail = await _context.MovieDetails.FirstOrDefaultAsync(m=>m.movieId == id);

            return Page();
        }
    }
}
