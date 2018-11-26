using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        public virtual DbSet<Videos> Videos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=tcp:cap-project.database.windows.net,1433;Initial Catalog=SearchAV;Persist Security Info=False;User ID= searchav;Password= Capstone1819;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Videos>(entity =>
            {
                entity.HasKey(e => e.VideoId);

                entity.Property(e => e.VideoId).HasColumnName("VideoID");

                entity.Property(e => e.DateAdded).HasColumnType("date");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.TranscriptionPath)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.VideoPath)
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });
        }
    }
}
