namespace DataAccess;

using Microsoft.EntityFrameworkCore;
using Entities;

public class BankaDbContext : DbContext
{
    public DbSet<Musteri> Musteriler => Set<Musteri>();
    public DbSet<Hesap> Hesaplar => Set<Hesap>();
    public DbSet<Odeme> Odemeler => Set<Odeme>();
    public DbSet<MuhasebeDefteri> MuhasebeDefteri => Set<MuhasebeDefteri>();
    public DbSet<Log> Loglar => Set<Log>();

    public BankaDbContext(DbContextOptions<BankaDbContext> options) : base(options)
    {
    }

    public BankaDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=banka.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Musteri>()
            .HasIndex(m => m.TCKN)
            .IsUnique();

        modelBuilder.Entity<Hesap>()
            .HasIndex(h => h.HesapNo)
            .IsUnique();
    }
}