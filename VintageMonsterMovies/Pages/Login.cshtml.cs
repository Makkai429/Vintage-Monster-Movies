using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace VintageMonsterMovies.Pages
{
    public class LoginModel : PageModel
    {
        private HorrorDbContext _context;

        [BindProperty]
        public string password { get; set; }

        [BindProperty]
        public string username { get; set; }

        public User newUser { get; set; } = new User();

        public LoginModel(HorrorDbContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            var user = await _context.Users.Where(f => f.username == username).FirstOrDefaultAsync();

            if (user == null)
            {
                ModelState.AddModelError("username", "The username is incorrect.");
                return Page();
            }

            else if (user.password != password)
            {
                ModelState.AddModelError("password", "The username is incorrect.");
                return Page();
            }
            if (user.status == "user")
            {
                TempData["username"] = user.username;
                return RedirectToPage("UserPage");
            }
            else if (user.status == "admin")
            {
                TempData["username"] = user.username;
                return RedirectToPage("AdminPage");
            }
            else
            {
                ModelState.AddModelError("", "Unidentified user status.");
                return Page();
            }
        }
    }
}
