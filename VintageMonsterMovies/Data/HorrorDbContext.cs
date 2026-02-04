using Microsoft.EntityFrameworkCore;
using VintageMonsterMovies.Models;

namespace VintageMonsterMovies.Data
{
    public class HorrorDbContext: DbContext
    {
        public HorrorDbContext(DbContextOptions<HorrorDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieDetail> MovieDetails { get; set; }
    }
}
