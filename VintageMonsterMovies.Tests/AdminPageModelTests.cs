using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VintageMonsterMovies.Pages;
using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace VintageMonsterMovies.Tests
{
    [TestFixture]
    public class AdminPageModelTests
    {
        private AdminPageModel _pageModel;
        private HorrorDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HorrorDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new HorrorDbContext(options);

            _context.Movies.AddRange(
                new Movie
                {
                    id = 1,
                    title = "Godzilla",
                    publishingYear = 1954,
                    description = "Giant monster attacks Japan",
                    director = "Ishirō Honda",
                    writer = "Shigeru Kayama"
                },
                new Movie
                {
                    id = 2,
                    title = "King Kong",
                    publishingYear = 1933,
                    description = "Giant ape on Skull Island",
                    director = "Merian C. Cooper",
                    writer = "Edgar Wallace"
                }
            );
            _context.SaveChanges();

            _pageModel = new AdminPageModel(_context)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task OnGetAsync_PopulatesMovieList()
        {
            var result = await _pageModel.OnGetAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.AreEqual(2, _pageModel.movieList.Count);
        }

        [Test]
        public async Task OnPostAsync_ValidMovie_AddsMovie()
        {
            _pageModel.movie = new Movie
            {
                id = 3,
                title = "Frankenstein",
                publishingYear = 1931,
                description = "Scientist creates monster",
                director = "James Whale",
                writer = "John L. Balderston"
            };

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<RedirectToPageResult>(result);

            var allMovies = await _context.Movies.ToListAsync();
            Assert.AreEqual(3, allMovies.Count);
            Assert.AreEqual("The movie is successfully added to the database.", _pageModel.TempData["Message"]);
        }

        [Test]
        public async Task OnPostAsync_InvalidModel_ReturnsPage()
        {
            _pageModel.ModelState.AddModelError("title", "Required");

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);

            var allMovies = await _context.Movies.ToListAsync();
            Assert.AreEqual(2, allMovies.Count);
        }

        [Test]
        public async Task OnPostDeleteAsync_RemovesMovie()
        {
            var result = await _pageModel.OnPostDeleteAsync(1);

            Assert.IsInstanceOf<RedirectToPageResult>(result);

            var allMovies = await _context.Movies.ToListAsync();
            Assert.AreEqual(1, allMovies.Count);
            Assert.AreEqual("The movie is successfully deleted.", _pageModel.TempData["Message"]);
        }

        [Test]
        public async Task OnPostDeleteAsync_InvalidId_DoesNothing()
        {
            var result = await _pageModel.OnPostDeleteAsync(999);

            Assert.IsInstanceOf<RedirectToPageResult>(result);

            var allMovies = await _context.Movies.ToListAsync();
            Assert.AreEqual(2, allMovies.Count);
            Assert.AreEqual("The movie is successfully deleted.", _pageModel.TempData["Message"]);
        }
    }
}
