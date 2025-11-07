using Microsoft.EntityFrameworkCore;
namespace SpainCP.DAL
{
    public class AppDbContext : DbContext
    {
        private string ConnectionString => "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SpainCP;Integrated Security=True;Connect Timeout=30;";
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Goal> Goals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Club>()
                .HasMany(c => c.Players)
                .WithMany(p => p.Clubs)
                .UsingEntity(j => j.ToTable("ClubPlayer"));

            modelBuilder.Entity<Club>()
                .HasMany(c => c.Matches)
                .WithMany(m => m.Clubs)
                .UsingEntity(j => j.ToTable("ClubMatch"));

            modelBuilder.Entity<Goal>()
                .HasOne(g => g.Player)
                .WithMany(p => p.Goals)
                .HasForeignKey(g => g.PlayerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Goal>()
                .HasOne(g => g.Match)
                .WithMany(m => m.Goals)
                .HasForeignKey(g => g.MatchID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goal>()
                .HasOne(g => g.Club)
                .WithMany()
                .HasForeignKey(g => g.ClubId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
