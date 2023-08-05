using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RoommateMatcher.Models
{
	public class AppDbContext:IdentityDbContext<AppUser,AppRole,string>
	{
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppUserPreferences> UserPreferences { get; set; }
        public DbSet<AppUserAddress> UserAddresses { get; set; }
        public DbSet<AppUserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<AppUserFollows> UserFollows { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Preferences)
                .WithOne(p => p.User)
                .HasForeignKey<AppUserPreferences>(p => p.UserId);

            modelBuilder.Entity<AppUserPreferences>()
               .HasOne(u => u.Address)
               .WithOne(p => p.Preferences) 
               .HasForeignKey<AppUserAddress>(p => p.PreferencesId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

