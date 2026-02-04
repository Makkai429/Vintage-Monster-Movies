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
    public class EditModelTests
    {
        private HorrorDbContext _context;
        private EditModel _pageModel;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HorrorDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            _context = new HorrorDbContext(options);

            _context.Movies.Add(new Movie
            {
                id = 1,
                title = "Old Title",
                publishingYear = 1950,
                description = "Old description",
                director = "Old director",
                writer = "Old writer"
            });

            _context.SaveChanges();

            _pageModel = new EditModel(_context)
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
        public async Task OnGetAsync_ValidId_ReturnsPage_AndLoadsMovie()
        {
            var result = await _pageModel.OnGetAsync(1);

            Assert.IsInstanceOf<PageResult>(result);
            Assert.AreEqual(1, _pageModel.movie.id);
            Assert.AreEqual("Old Title", _pageModel.movie.title);
        }

        [Test]
        public async Task OnGetAsync_InvalidId_ReturnsNotFound()
        {
            var result = await _pageModel.OnGetAsync(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task OnPostAsync_InvalidModel_ReturnsPage()
        {
            _pageModel.ModelState.AddModelError("title", "Required");

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
        }

        [Test]
        public async Task OnPostAsync_EntityNotFound_ReturnsNotFound()
        {
            _pageModel.movie = new Movie
            {
                id = 999,
                title = "New",
                publishingYear = 2000,
                description = "d",
                director = "d",
                writer = "d"
            };

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task OnPostAsync_ValidUpdate_UpdatesDatabase_AndRedirects()
        {
            _pageModel.movie = new Movie
            {
                id = 1,
                title = "Updated Title",
                publishingYear = 2001,
                description = "Updated description",
                director = "New director",
                writer = "New writer"
            };

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<RedirectToPageResult>(result);

            var updated = await _context.Movies.FirstAsync();

            Assert.AreEqual("Updated Title", updated.title);
            Assert.AreEqual(2001, updated.publishingYear);
            Assert.AreEqual("New director", updated.director);
            Assert.AreEqual("The movie is successfully updated.", _pageModel.TempData["Message"]);
        }
    }
}
