using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PlayIt_Api.Models.Entities
{
    public partial class PlayItContext : DbContext
    {
        public PlayItContext()
        {
        }

        public PlayItContext(DbContextOptions<PlayItContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }
        public virtual DbSet<GameType> GameType { get; set; }
        public virtual DbSet<LoginAttempt> LoginAttempt { get; set; }
        public virtual DbSet<Token> Token { get; set; }
        public virtual DbSet<TokenType> TokenType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("server=PIETRAS.DK\\PIETRASSQL,52384;database=PlayIt;user=PlayIt;pwd=pVG553c@Yia7Ft7u3D*mvGi*ay!z5LLi");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.AvatarFilePath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsFixedLength();

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GameType>(entity =>
            {
                entity.Property(e => e.GameTypeId).ValueGeneratedOnAdd();

                entity.Property(e => e.BackgroundPath)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CoverPath)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.LogoPath)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LoginAttempt>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Ipv4)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.Property(e => e.TokenId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.Expiration).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Token)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Token__AccountId__3C69FB99");

                entity.HasOne(d => d.TokenType)
                    .WithMany(p => p.Token)
                    .HasForeignKey(d => d.TokenTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Token__TokenType__3B75D760");
            });

            modelBuilder.Entity<TokenType>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
