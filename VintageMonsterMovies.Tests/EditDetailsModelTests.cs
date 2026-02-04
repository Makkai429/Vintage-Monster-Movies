using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using VintageMonsterMovies.Data;
using VintageMonsterMovies.Models;
using VintageMonsterMovies.Pages;

namespace VintageMonsterMovies.Tests
{
    [TestFixture]
    public class EditDetailsModelTests
    {
        private HorrorDbContext _context;
        private EditDetailsModel _pageModel;

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

            _pageModel = new EditDetailsModel(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task OnGetAsync_ExistingMovieId_LoadsDetailAndMovie()
        {
            var result = await _pageModel.OnGetAsync(1);

            Assert.IsInstanceOf<PageResult>(result);
            Assert.AreEqual(1, _pageModel.detail.movieId);
            Assert.AreEqual("Dracula", _pageModel.movie.title);
            Assert.AreEqual("Dracula (1931)", _pageModel.movieTitle);
        }

        [Test]
        public async Task OnPostAsync_ExistingDetail_UpdatesAndRedirects()
        {
            _pageModel.detail = new MovieDetail
            {
                movieId = 1,
                stars = "Updated Star",
                runtime = 80,
                storyline = "Updated Story",
                company = "Updated Company"
            };

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<RedirectToPageResult>(result);

            var updated = _context.MovieDetails.First(d => d.movieId == 1);

            Assert.AreEqual("Updated Star", updated.stars);
            Assert.AreEqual(80, updated.runtime);
            Assert.AreEqual("Updated Story", updated.storyline);
            Assert.AreEqual("Updated Company", updated.company);

            var redirect = result as RedirectToPageResult;
            Assert.AreEqual("/CreateDetails", redirect.PageName);
            Assert.AreEqual(1, redirect.RouteValues["movieId"]);
        }

        [Test]
        public async Task OnPostAsync_NonExistingDetail_ReturnsNotFound()
        {
            _pageModel.detail = new MovieDetail
            {
                movieId = 999,
                stars = "X",
                runtime = 50,
                storyline = "X",
                company = "X"
            };

            var result = await _pageModel.OnPostAsync();

            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
