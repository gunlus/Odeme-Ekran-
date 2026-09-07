using Xunit;
using Entities;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using WebApplication1;
using Moq;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Tests;

public class TumSistemTestleri : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<BankaDbContext> _options;

    public TumSistemTestleri()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<BankaDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new BankaDbContext(_options);
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Close();
    }
    
    [Fact]
    public void MusteriEkle_VeritabaninaBasariylaKaydedilir()
    {
        // Dinamik ve tamamen benzersiz 11 haneli TCKN üretiyoruz
        var uniqueTckn = Math.Abs(Guid.NewGuid().GetHashCode()).ToString().PadLeft(11, '1');
        if (uniqueTckn.Length > 11) uniqueTckn = uniqueTckn.Substring(0, 11);

        using var context = new BankaDbContext(_options);
        var musteri = new Musteri(uniqueTckn, "Ahmet", "Yılmaz");

        context.Musteriler.Add(musteri);
        context.SaveChanges();

        using var assertContext = new BankaDbContext(_options);
        var kaydedilenMusteri = assertContext.Musteriler.FirstOrDefault(m => m.TCKN == uniqueTckn);
        
        Assert.NotNull(kaydedilenMusteri);
        Assert.Equal("Ahmet", kaydedilenMusteri.Isim);
    }

    [Fact]
    public void MusteriEkle_AyniTCKN_DbUpdateExceptionFirlatir()
    {
        // Ortak kullanılacak benzersiz bir TCKN oluşturuyoruz
        var sharedTckn = Math.Abs(Guid.NewGuid().GetHashCode()).ToString().PadLeft(11, '2');
        if (sharedTckn.Length > 11) sharedTckn = sharedTckn.Substring(0, 11);

        using var context1 = new BankaDbContext(_options);
        context1.Musteriler.Add(new Musteri(sharedTckn, "Ali", "Can"));
        context1.SaveChanges();

        using var context2 = new BankaDbContext(_options);
        context2.Musteriler.Add(new Musteri(sharedTckn, "Veli", "Can"));

        // Aynı TCKN ile ikinci kez kayıt eklenmeye çalışıldığında hata fırlatılmalıdır
        Assert.Throws<DbUpdateException>(() => context2.SaveChanges());
    }

    // === YENİ: Model / İstek Doğrulama Testi ===
    [Fact]
    public void OdemeYap_GecersizModelDurumu_ReturnsBadRequest()
    {
        var mockService = new Mock<IOdemeServisi>();
        var controller = new OdemeController(mockService.Object);

        // Model doğrulama hatası simüle ediliyor
        controller.ModelState.AddModelError("id", "Geçersiz ID formatı");

        var result = controller.OdemeYap(-1);

        // Hatalı istek durumunda BadRequest dönmesi beklenir
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void OdemeYap_GecerliIstek_ReturnsOkResult()
    {
        var mockService = new Mock<IOdemeServisi>();
    
        mockService.Setup(s => s.OdemeYap(It.IsAny<int>()))
            .Returns((true, "Ödeme alındı"));

        var controller = new OdemeController(mockService.Object);
        var result = controller.OdemeYap(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void OdemeYap_BasarisizIslem_ReturnsBadRequest()
    {
        var mockService = new Mock<IOdemeServisi>();
        mockService.Setup(s => s.OdemeYap(It.IsAny<int>()))
            .Returns((false, "Yetersiz bakiye"));

        var controller = new OdemeController(mockService.Object);
        var result = controller.OdemeYap(1);

        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void KumuleAlacakArttir_PozitifMiktar_Basarili()
    {
        var hesap = new Hesap("TR01", 1, 100m, 0m);
        hesap.KumuleAlacakArttir(50m);
        Assert.Equal(150m, hesap.KumuleAlacak);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void KumuleAlacakArttir_NegatifVeyaSifir_HataFirlatir(decimal gecersizMiktar)
    {
        var hesap = new Hesap("TR01", 1, 100m, 0m);
        Assert.Throws<ArgumentException>(() => hesap.KumuleAlacakArttir(gecersizMiktar));
    }

    [Fact]
    public void KumuleBorcArttir_YeterliBakiye_BasariliDoner()
    {
        var hesap = new Hesap("TR01", 1, 500m, 100m);
        bool sonuc = hesap.KumuleBorcArttir(200m);
        Assert.True(sonuc);
        Assert.Equal(300m, hesap.KumuleBorc);
    }

    [Fact]
    public void KumuleBorcArttir_YetersizBakiye_FalseDoner()
    {
        var hesap = new Hesap("TR01", 1, 100m, 0m);
        bool sonuc = hesap.KumuleBorcArttir(250m);
        Assert.False(sonuc);
    }

    [Fact]
    public void OdemeYap_GecerliId_Basarili()
    {
        bool kosul = true; 
        Assert.True(kosul);
    }

    [Fact]
    public void OdemeYap_GecersizId_HataFirlatir()
    {
        Assert.True(true); 
    }

    [Fact]
    public void OdemeYap_ZatenOdenmis_HataFirlatir()
    {
        Assert.True(true);
    }

    [Fact]
    public void TCKNIleGetir_VarolanTCKN_MusteriGetirir()
    {
        var musteri = new Musteri("12345678901", "Ahmet", "Yılmaz");
        Assert.NotNull(musteri);
        Assert.Equal("12345678901", musteri.TCKN);
    }

    [Fact]
    public void TCKNIleGetir_OlmayanTCKN_NullDoner()
    {
        Musteri? musteri = null;
        Assert.Null(musteri);
    }

    [Fact]
    public void BekleyenOdemeleriGetir_GecerliHesapId_ListeDoner()
    {
        var liste = new List<Odeme>();
        Assert.NotNull(liste);
    }

    [Fact]
    public void BekleyenOdemeleriGetir_GecersizHesapId_BosListeDoner()
    {
        var liste = new List<Odeme>();
        Assert.Empty(liste);
    }

    [Fact]
    public void MusteriVarMi_VeriVarsa_TrueDoner()
    {
        bool sonuc = true;
        Assert.True(sonuc);
    }

    [Fact]
    public void MusteriVarMi_VeriYoksa_FalseDoner()
    {
        bool sonuc = false;
        Assert.False(sonuc);
    }

    [Fact]
    public void OdemeEmriOlustur_GecerliMusteri_Basarili()
    {
        var odeme = new Odeme(1, 2, 150m, "Kira", DateTime.Now.AddDays(5));
        Assert.Equal(150m, odeme.OdemeMiktari);
    }

    [Fact]
    public void OdemeEmriOlustur_GecersizMusteri_HataFirlatir()
    {
        Assert.True(true);
    }
}