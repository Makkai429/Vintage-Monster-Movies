using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Pages
{
    public class EditDetailsModel : PageModel
    {
        private readonly HorrorDbContext _context;

        [BindProperty]
        public MovieDetail detail { get; set; } = default!;

        public Movie movie { get; set; }

        public EditDetailsModel(HorrorDbContext context)
        {
            _context = context;
        }

        public string movieTitle { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int id)
        {
            detail = await _context.MovieDetails.FirstOrDefaultAsync(d => d.movieId == id);
            movie = await _context.Movies.FirstOrDefaultAsync(m => m.id == id);

            movieTitle = $"{movie.title} ({movie.publishingYear})";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var entity = await _context.MovieDetails.FirstOrDefaultAsync(d => d.movieId == detail.movieId);

            if (entity == null)
                return NotFound();

            entity.stars = detail.stars;
            entity.runtime = detail.runtime;
            entity.company = detail.company;
            entity.storyline = detail.storyline;

            await _context.SaveChangesAsync();
            return RedirectToPage("/CreateDetails", new { movieId = detail.movieId });
        }
    }
}
