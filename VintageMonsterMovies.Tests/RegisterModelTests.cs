using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using VintageMonsterMovies.Pages;

namespace VintageMonsterMovies.Tests
{
    [TestFixture]
    public class RegisterModelTests
    {
        private HorrorDbContext _context;
        private RegisterModel _pageModel;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HorrorDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            _context = new HorrorDbContext(options);

            _pageModel = new RegisterModel(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void OnGet_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _pageModel.OnGet());
        }

        [Test]
        public async Task OnPostAsync_InvalidModelState_ReturnsPageAndSentFalse()
        {
            _pageModel.ModelState.AddModelError("x", "error");

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsFalse(_pageModel.sent);
            Assert.IsFalse(_pageModel.successful);
        }

        [Test]
        public async Task OnPostAsync_PasswordsDoNotMatch_ReturnsPageWithModelError()
        {
            _pageModel.user = new User
            {
                id = 1,
                username = "testuser",
                email = "test@example.com",
                password = "Password123"
            };
            _pageModel.repeatedPassword = "Mismatch";

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsTrue(_pageModel.ModelState.ContainsKey("repeatedPassword"));
            Assert.IsFalse(_pageModel.successful);
            Assert.IsTrue(_pageModel.sent);
        }

        [Test]
        public async Task OnPostAsync_ValidUser_SavesSuccessfully()
        {
            _pageModel.user = new User
            {
                id = 1,
                username = "testuser",
                email = "testuser@example.com",
                password = "Password123"
            };
            _pageModel.repeatedPassword = "Password123";

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsTrue(_pageModel.successful);
            Assert.IsTrue(_pageModel.sent);

            var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.username == "testuser");
            Assert.IsNotNull(savedUser);
            Assert.AreEqual("Password123", savedUser.password);
            Assert.AreEqual("testuser@example.com", savedUser.email);
        }

    }
}
