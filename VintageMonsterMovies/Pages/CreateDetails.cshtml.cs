
using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Pages
{
    public class CreateDetailsModel : PageModel
    {
        private readonly HorrorDbContext _context;

        public CreateDetailsModel(HorrorDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MovieDetail detail { get; set; } = new();

        public List<SelectListItem> MovieOptions { get; set; } = new();

        public List<MovieDetail> detailList { get; set; } = new();

        public List<Movie> moviesWithDetails { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            MovieOptions = await _context.Movies
                .OrderBy(m => m.title)
                .Select(m => new SelectListItem
                {
                    Value = m.id.ToString(),
                    Text = $"{m.title} ({m.publishingYear})"
                })
                .ToListAsync();
            moviesWithDetails = await _context.Movies.Where(m => _context.MovieDetails.Any(d => d.movieId == m.id)).ToListAsync();
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadMovieOptionsAsync();
                return Page();
            }

            var exists = await _context.Movies.AnyAsync(m => m.id == detail.movieId);
            if (!exists)
            {
                ModelState.AddModelError(nameof(detail.movieId), "A kiválasztott film nem létezik.");
                await LoadMovieOptionsAsync();
                return Page();
            }

            var already = await _context.MovieDetails.AnyAsync(md => md.movieId == detail.movieId);
            if (already)
            {
                ModelState.AddModelError(string.Empty, "Ehhez a filmhez már létezik MovieDetails bejegyzés.");
                await LoadMovieOptionsAsync();
                return Page();
            }

            _context.MovieDetails.Add(detail);
            await _context.SaveChangesAsync();

            TempData["Message"] = "A részletek sikeresen mentve.";
          
            return RedirectToPage("/AdminPage");
        }

        private async Task LoadMovieOptionsAsync()
        {
            MovieOptions = await _context.Movies
                .OrderBy(m => m.title)
                .Select(m => new SelectListItem
                {
                    Value = m.id.ToString(),
                    Text = $"{m.title} ({m.publishingYear})"
                })
                .ToListAsync();
        }
    }
}
