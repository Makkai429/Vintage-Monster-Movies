using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using VintageMonsterMovies.Pages;

namespace VintageMonsterMovies.Tests
{
    [TestFixture]
    public class LoginModelTests
    {
        private HorrorDbContext _context;
        private LoginModel _pageModel;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HorrorDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            _context = new HorrorDbContext(options);

            _context.Users.AddRange(
                new User { id = 1, username = "user1", email = "user1@test.com", password = "pass1", status = "user" },
                new User { id = 2, username = "admin1", email = "admin1@test.com", password = "pass2", status = "admin" },
                new User { id = 3, username = "unknown", email = "unknown@test.com", password = "pass3", status = "other" }
            );
            _context.SaveChanges();

            _pageModel = new LoginModel(_context)
            {
                TempData = new TempDataDictionary(
                    new DefaultHttpContext(),
                    Mock.Of<ITempDataProvider>())
            };
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
        public async Task OnPostLoginAsync_InvalidUsername_ReturnsPageWithError()
        {
            _pageModel.username = "nonexistent";
            _pageModel.password = "any";

            var result = await _pageModel.OnPostLoginAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsTrue(_pageModel.ModelState.ContainsKey("username"));
        }

        [Test]
        public async Task OnPostLoginAsync_WrongPassword_ReturnsPageWithError()
        {
            _pageModel.username = "user1";
            _pageModel.password = "wrongpass";

            var result = await _pageModel.OnPostLoginAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsTrue(_pageModel.ModelState.ContainsKey("password"));
        }

        [Test]
        public async Task OnPostLoginAsync_ValidUser_RedirectsToUserPage()
        {
            _pageModel.username = "user1";
            _pageModel.password = "pass1";

            var result = await _pageModel.OnPostLoginAsync();

            Assert.IsInstanceOf<RedirectToPageResult>(result);
            var redirect = result as RedirectToPageResult;
            Assert.AreEqual("UserPage", redirect.PageName);
            Assert.AreEqual("user1", _pageModel.TempData["username"]);
        }

        [Test]
        public async Task OnPostLoginAsync_ValidAdmin_RedirectsToAdminPage()
        {
            _pageModel.username = "admin1";
            _pageModel.password = "pass2";

            var result = await _pageModel.OnPostLoginAsync();

            Assert.IsInstanceOf<RedirectToPageResult>(result);
            var redirect = result as RedirectToPageResult;
            Assert.AreEqual("AdminPage", redirect.PageName);
            Assert.AreEqual("admin1", _pageModel.TempData["username"]);
        }

        [Test]
        public async Task OnPostLoginAsync_UnknownStatus_ReturnsPageWithError()
        {
            _pageModel.username = "unknown";
            _pageModel.password = "pass3";

            var result = await _pageModel.OnPostLoginAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsTrue(_pageModel.ModelState.ErrorCount > 0);
        }
    }
}
