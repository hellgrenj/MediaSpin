using Microsoft.EntityFrameworkCore;
using storage.Domain.Models;

namespace storage.Persistence
{
    public class StorageDbContext : DbContext
    {
        public DbSet<Source> Sources { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Sentence> Sentences { get; set; }

        public StorageDbContext(DbContextOptions<StorageDbContext> options) : base(options) { }
    }
}