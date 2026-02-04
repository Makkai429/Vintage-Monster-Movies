using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Pages
{
    public class UserPageModel : PageModel
    {
        private HorrorDbContext _context;

        public List<Movie> movieList { get; set; } = new List<Movie>();

        public UserPageModel(HorrorDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            movieList=await _context.Movies.ToListAsync();
            return Page();
        }
    }
}
