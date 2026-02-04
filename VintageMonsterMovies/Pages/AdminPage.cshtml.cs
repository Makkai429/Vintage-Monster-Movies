using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Pages
{
    public class AdminPageModel : PageModel
    {
        private readonly HorrorDbContext _context;

        [BindProperty]
        public Movie movie { get; set; } = new Movie();

        public List<Movie> movieList { get; set; }=new List<Movie>();

        public AdminPageModel(HorrorDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            movieList = await _context.Movies.ToListAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                movieList = await _context.Movies.OrderBy(m=>m.publishingYear).ToListAsync();
                return Page();
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            TempData["Message"] = "The movie is successfully added to the database.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var deleted = await _context.Movies.FirstOrDefaultAsync(m => m.id == id);

            if (deleted != null)
            {
                _context.Movies.Remove(deleted);
                await _context.SaveChangesAsync();
            }

            TempData["Message"] = "The movie is successfully deleted.";
            return RedirectToPage();
        }
    }
}
