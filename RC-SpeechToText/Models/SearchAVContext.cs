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

        public virtual DbSet<Video> Video { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(e => e.VideoId);

                entity.Property(e => e.VideoId).HasColumnName("VideoID");
                
                entity.Property(e => e.DateAdded).HasColumnType("date");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Transcription)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.VideoPath)
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });
        }
    }
}
