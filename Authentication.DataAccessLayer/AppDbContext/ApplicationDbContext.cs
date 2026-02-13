using Microsoft.EntityFrameworkCore;


namespace Authentication.DataAccessLayer.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<LoginAttempt> LoginAttempts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Skip snake_case for EF Migrations table
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (string.Equals(entity.GetTableName(), "__EFMigrationsHistory", StringComparison.OrdinalIgnoreCase))
                {
                    entity.SetTableName("__EFMigrationsHistory"); // keep PascalCase table
                    foreach (var property in entity.GetProperties())
                    {
                        property.SetColumnName(property.Name); // keep PascalCase columns
                    }
                }
            }

            base.OnModelCreating(modelBuilder);

            // User  Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.UserId);
                entity.HasIndex(x => x.Email).IsUnique();
                entity.Property(x => x.Email).HasMaxLength(255);
                entity.Property(x => x.FirstName).HasMaxLength(200);
                entity.Property(x => x.MiddleName).HasMaxLength(100);
                entity.Property(x => x.LastName).HasMaxLength(100);
                entity.Property(x => x.IsEmailVerified).HasDefaultValue(false);
                entity.Property(x => x.FailedLoginAttempts).HasDefaultValue(0);
            });

            // RefreshToken  Configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(x => x.RefreshTokenId);
                entity.HasIndex(x => x.Token).IsUnique();
                entity.Property(x => x.Token).HasMaxLength(500);
                entity.Property(x => x.IpAddress).HasMaxLength(50);
                entity.Property(x => x.UserAgent).HasMaxLength(500);
                entity.Property(x => x.IsRevoked).HasDefaultValue(false);

                entity.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            });

            // LoginAttempt  Configuration
            modelBuilder.Entity<LoginAttempt>(entity =>
            {
                entity.HasKey(x => x.LoginAttemptId);
                entity.Property(x => x.IpAddress).HasMaxLength(50);
                entity.Property(x => x.UserAgent).HasMaxLength(500);
                entity.Property(x => x.FailureReason).HasMaxLength(200);             

                entity.HasOne(x => x.User)
                .WithMany(x => x.LoginAttempts)
                 .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
