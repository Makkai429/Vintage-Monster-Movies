using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Pages
{
    public class SearchPageModel : PageModel
    {
        private HorrorDbContext _context;

        [BindProperty]
        public string wantedMovie {  get; set; }=string.Empty;

        public List<Movie> results { get; set; } = new List<Movie>();

        public SearchPageModel(HorrorDbContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSearchAsync(string typedMovie)
        {
           
            var q = (typedMovie ?? string.Empty).Trim();
            q = System.Text.RegularExpressions.Regex.Replace(q, @"\s+", " "); 
            wantedMovie = q;

            if (string.IsNullOrWhiteSpace(q))
            {
                results = new List<Movie>();
                return Page();
            }

            var like = $"%{q}%";

            var query = _context.Movies
                .AsNoTracking()
                .Where(m => m.title != null && EF.Functions.Like(m.title, like))
                .OrderByDescending(m => m.publishingYear);

            results = await query.ToListAsync();
            return Page();
        }
    }
}
