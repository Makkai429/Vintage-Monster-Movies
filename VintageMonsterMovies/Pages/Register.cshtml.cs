using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace VintageMonsterMovies.Pages
{
    public class RegisterModel : PageModel
    {
        private HorrorDbContext _context;

        [BindProperty]
        public User user { get; set; }= new User();

        [BindProperty]
        public string repeatedPassword { get; set; }=string.Empty;

        public bool sent { get; set; } = false;
        public bool successful { get; set; } = false;

        public RegisterModel(HorrorDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            sent = true;

            if (!ModelState.IsValid)
            {
                sent = false;
                return Page();
            }

            if (user.password != repeatedPassword)
            {
                ModelState.AddModelError("repeatedPassword", "Passwords do not match.");
                successful = false;
                return Page();
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                successful = true;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Saving error: {ex.Message}");
                successful = false;
            }

            return Page();
        }
    }
}
