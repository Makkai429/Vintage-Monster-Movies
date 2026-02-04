using System.Collections.Generic;
using System.Linq;
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
    public class DetailsModelTests
    {
        private HorrorDbContext _context;
        private DetailsModel _pageModel;

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

            _pageModel = new DetailsModel(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task OnGetAsync_ExistingMovie_LoadsMovieAndDetail()
        {
            var result = await _pageModel.OnGetAsync(1);

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsNotNull(_pageModel.movie);
            Assert.AreEqual("Dracula", _pageModel.movie.title);

            Assert.IsNotNull(_pageModel.detail);
            Assert.AreEqual(1, _pageModel.detail.movieId);
        }

        [Test]
        public async Task OnGetAsync_ExistingMovie_NoDetail_SetsDetailNull()
        {
            var result = await _pageModel.OnGetAsync(2);

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsNotNull(_pageModel.movie);
            Assert.AreEqual("Frankenstein", _pageModel.movie.title);

            Assert.IsNull(_pageModel.detail);
        }

        [Test]
        public async Task OnGetAsync_NonExistingMovie_ReturnsNotFound()
        {
            var result = await _pageModel.OnGetAsync(999);

            Assert.IsInstanceOf<NotFoundResult>(result);
            Assert.IsNull(_pageModel.movie);
            Assert.IsNull(_pageModel.detail);
        }
    }
}
