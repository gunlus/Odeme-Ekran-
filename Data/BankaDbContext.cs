using Microsoft.EntityFrameworkCore;
using Odeme_Projesi.Models;

namespace Odeme_Projesi.Data;

public class BankaDbContext : DbContext
{
    public DbSet<Musteri> Musteriler { get; set; }
    public DbSet<Hesap> Hesaplar { get; set; }
    public DbSet<Odeme> Odemeler { get; set; }
    
    public DbSet<Log> Loglar { get; set; }
    
    public DbSet<MuhasebeDefteri>  MuhasebeDefteri { get; set; }
    
    // ===== MUHASEBE DEFTERİ =====
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=banka.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== MUSTERI =====
        modelBuilder.Entity<Musteri>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.TCKN).IsUnique();
        });

        // ===== HESAP =====
        modelBuilder.Entity<Hesap>(entity =>
{
    // ✅ Birincil anahtar artık Id
    entity.HasKey(h => h.Id);

    // ✅ HesapNo hala benzersiz olsun
    entity.HasIndex(h => h.HesapNo).IsUnique();

    // Musteri ile ilişki
    entity.HasOne(h => h.Musteri)
          .WithMany(m => m.Hesaplar)
          .HasForeignKey(h => h.MusteriId)
          .OnDelete(DeleteBehavior.Restrict);
});

       // ===== ODEME =====
modelBuilder.Entity<Odeme>(entity =>
{
    entity.HasKey(o => o.Id);

    entity.HasOne(o => o.AlacakliHesap)
          .WithMany()
          .HasForeignKey(o => o.AlacakliHesapId)
          .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(o => o.BorcluHesap)
          .WithMany()
          .HasForeignKey(o => o.BorcluHesapId)
          .OnDelete(DeleteBehavior.Restrict);
});

// ===== MUHASEBE DEFTERI =====
modelBuilder.Entity<MuhasebeDefteri>(entity =>
{
    entity.HasKey(m => m.Id);

    entity.HasOne(m => m.Odeme)
          .WithMany()
          .HasForeignKey(m => m.OdemeId)
          .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(m => m.Hesap)
          .WithMany()
          .HasForeignKey(m => m.HesapId)
          .OnDelete(DeleteBehavior.Restrict);

    entity.Property(m => m.Tutar).IsRequired();
    entity.Property(m => m.BakiyeOnce).IsRequired();
    entity.Property(m => m.BakiyeSonra).IsRequired();
    entity.Property(m => m.Tarih).IsRequired();
    entity.Property(m => m.IslemTipi).IsRequired();
    });
    }
}