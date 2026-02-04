using System.Collections.Generic;
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
    public class CreateDetailsModelTests
    {
        private HorrorDbContext _context;
        private CreateDetailsModel _pageModel;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HorrorDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            _context = new HorrorDbContext(options);

            _context.Movies.AddRange(
                new Movie
                {
                    id = 1,
                    title = "Dracula",
                    publishingYear = 1931,
                    description = "Classic",
                    director = "Browning",
                    writer = "Balderston"
                },
                new Movie
                {
                    id = 2,
                    title = "Frankenstein",
                    publishingYear = 1931,
                    description = "Monster",
                    director = "Whale",
                    writer = "Garrett"
                });

            _context.MovieDetails.Add(new MovieDetail
            {
                id = 1,
                movieId = 1,
                stars = "Bela Lugosi",
                runtime = 75,
                storyline = "Vampire story",
                company = "Universal"
            });

            _context.SaveChanges();

            _pageModel = new CreateDetailsModel(_context)
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
        public async Task OnGetAsync_LoadsMovieOptions()
        {
            await _pageModel.OnGetAsync();

            Assert.AreEqual(2, _pageModel.MovieOptions.Count);
        }

        [Test]
        public async Task OnGetAsync_LoadsOnlyMoviesWithDetails()
        {
            await _pageModel.OnGetAsync();

            Assert.AreEqual(1, _pageModel.moviesWithDetails.Count);
            Assert.AreEqual(1, _pageModel.moviesWithDetails.First().id);
        }

        [Test]
        public async Task OnPostAsync_InvalidModel_ReturnsPage()
        {
            _pageModel.ModelState.AddModelError("x", "error");

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
        }

        [Test]
        public async Task OnPostAsync_NonExistingMovie_ReturnsError()
        {
            _pageModel.detail = new MovieDetail
            {
                movieId = 999,
                stars = "Test",
                runtime = 100,
                storyline = "Test",
                company = "Test"
            };

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsTrue(_pageModel.ModelState.ErrorCount > 0);
        }

        [Test]
        public async Task OnPostAsync_AlreadyExists_ReturnsError()
        {
            _pageModel.detail = new MovieDetail
            {
                movieId = 1,
                stars = "Test",
                runtime = 100,
                storyline = "Test",
                company = "Test"
            };

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsTrue(_pageModel.ModelState.ErrorCount > 0);
        }

        [Test]
        public async Task OnPostAsync_ValidData_SavesAndRedirects()
        {
            _pageModel.detail = new MovieDetail
            {
                movieId = 2,
                stars = "Boris Karloff",
                runtime = 90,
                storyline = "Monster experiment",
                company = "Universal"
            };

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<RedirectToPageResult>(result);

            var saved = _context.MovieDetails.FirstOrDefault(d => d.movieId == 2);
            Assert.IsNotNull(saved);

            Assert.AreEqual("A részletek sikeresen mentve.", _pageModel.TempData["Message"]);
        }
    }
}
