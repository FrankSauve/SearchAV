using Microsoft.EntityFrameworkCore;

namespace RC_SpeechToText.Models
{
    public partial class SearchAVContext : DbContext
    {
        public SearchAVContext()
        {
        }

        public SearchAVContext(DbContextOptions<SearchAVContext> options)
            : base(options)
        {
        }

        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<Version> Version { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>().ToTable("File");
            modelBuilder.Entity<Version>().ToTable("Version");
            modelBuilder.Entity<User>().ToTable("User");
        }
    }
}
