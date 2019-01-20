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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.Property(e => e.FileId).HasColumnName("FileId");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.FilePath)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                // TODO: check how to add foreign key to entity
                // entity.HasForeignKey(e => e.TranscriptionId);

                entity.Property(e => e.TranscriptionId).HasColumnName("TranscriptionId");

                entity.Property(e => e.DateAdded).HasColumnType("date");

                entity.Property(e => e.Type)
                   .HasMaxLength(256)
                   .IsUnicode(false);

                // TODO: check how to add foreign key to entity
                // entity.HasForeignKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("UserId");

            });

            modelBuilder.Entity<Version>(entity =>
            {
                entity.HasKey(e => e.VersionId);

                entity.Property(e => e.VersionId).HasColumnName("VersionId");

                entity.Property(e => e.UserId).HasColumnName("UserId");

                entity.Property(e => e.TranscriptionId).HasColumnName("TranscriptionId");

                entity.Property(e => e.Transcription)
                    .IsUnicode(false);

                entity.Property(e => e.Active).HasColumnName("Active");
            });
        }
    }
}
