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
    public class UserPageModelTests
    {
        private HorrorDbContext _context;
        private UserPageModel _pageModel;

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
                    description = "Classic vampire horror",
                    director = "Tod Browning",
                    writer = "Garrett Fort"
                },
                new Movie
                {
                    id = 2,
                    title = "The Mummy",
                    publishingYear = 1932,
                    description = "Ancient curse unleashed",
                    director = "Karl Freund",
                    writer = "John Balderston"
                }
            );

            _context.SaveChanges();

            _pageModel = new UserPageModel(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task OnGetAsync_ReturnsPageResult()
        {
            var result = await _pageModel.OnGetAsync();

            Assert.IsInstanceOf<PageResult>(result);
        }

        [Test]
        public async Task OnGetAsync_LoadsAllMoviesIntoMovieList()
        {
            await _pageModel.OnGetAsync();

            Assert.AreEqual(2, _pageModel.movieList.Count);
        }

        [Test]
        public async Task OnGetAsync_MovieListContainsCorrectTitles()
        {
            await _pageModel.OnGetAsync();

            var titles = _pageModel.movieList.Select(m => m.title).ToList();

            CollectionAssert.Contains(titles, "Dracula");
            CollectionAssert.Contains(titles, "The Mummy");
        }

        [Test]
        public async Task OnGetAsync_EmptyDatabase_ReturnsEmptyList()
        {
            var options = new DbContextOptionsBuilder<HorrorDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            using var emptyContext = new HorrorDbContext(options);
            var emptyPage = new UserPageModel(emptyContext);

            await emptyPage.OnGetAsync();

            Assert.AreEqual(0, emptyPage.movieList.Count);
        }
    }
}
