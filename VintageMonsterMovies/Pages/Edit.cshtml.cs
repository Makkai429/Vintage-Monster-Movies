using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Pages
{
    public class EditModel : PageModel
    {
        private HorrorDbContext _context;

        [BindProperty]
        public Movie movie { get; set; } = default!;
        public EditModel(HorrorDbContext context) { 
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var entity=await _context.Movies.FirstOrDefaultAsync(m=>m.id==id);
            if (entity==null) { 
                return NotFound(); 
            }

            movie = entity;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var entity = await _context.Movies.FirstOrDefaultAsync(m => m.id == movie.id);

            if(entity == null) {
                return NotFound();
            }

            entity.title = movie.title;
            entity.publishingYear = movie.publishingYear;
            entity.writer = movie.writer;
            entity.director = movie.director;
            entity.description = movie.description;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Message"] = "The movie is successfully updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "An error happened during the updating. Try again, please!");
                return Page();
            }

            return RedirectToPage("AdminPage");
        }
    }
}
