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
    public class SearchPageModelTests
    {
        private HorrorDbContext _context;
        private SearchPageModel _pageModel;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HorrorDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            _context = new HorrorDbContext(options);

            _context.Movies.AddRange(new List<Movie>
            {
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
                    title = "Dracula Returns",
                    publishingYear = 1945,
                    description = "Sequel",
                    director = "Someone",
                    writer = "Writer"
                },
                new Movie
                {
                    id = 3,
                    title = "Frankenstein",
                    publishingYear = 1931,
                    description = "Monster",
                    director = "Whale",
                    writer = "Garrett"
                }
            });

            _context.SaveChanges();

            _pageModel = new SearchPageModel(_context);
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
        public async Task OnPostSearchAsync_EmptySearch_ReturnsEmptyResults()
        {
            var result = await _pageModel.OnPostSearchAsync("");

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsEmpty(_pageModel.results);
        }

        [Test]
        public async Task OnPostSearchAsync_NoMatch_ReturnsEmptyList()
        {
            var result = await _pageModel.OnPostSearchAsync("Alien");

            Assert.IsInstanceOf<PageResult>(result);
            Assert.AreEqual(0, _pageModel.results.Count);
        }

        [Test]
        public async Task OnPostSearchAsync_FindsMatchingMovies()
        {
            var result = await _pageModel.OnPostSearchAsync("Dracula");

            Assert.IsInstanceOf<PageResult>(result);
            Assert.AreEqual(2, _pageModel.results.Count);
        }

        [Test]
        public async Task OnPostSearchAsync_OrdersByYearDescending()
        {
            await _pageModel.OnPostSearchAsync("Dracula");

            var years = _pageModel.results
                .Select(m => m.publishingYear)
                .ToList();

            Assert.That(years, Is.Ordered.Descending);
        }

        [Test]
        public async Task OnPostSearchAsync_TrimsAndNormalizesWhitespace()
        {
            await _pageModel.OnPostSearchAsync("   Dracula    ");

            Assert.AreEqual("Dracula", _pageModel.wantedMovie);
            Assert.AreEqual(2, _pageModel.results.Count);
        }

        [Test]
        public async Task OnPostSearchAsync_NullInput_TreatedAsEmpty()
        {
            var result = await _pageModel.OnPostSearchAsync(null);

            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsEmpty(_pageModel.results);
        }
    }
}
