using DataAccess;
using Entities;

namespace DataAcess;

public class DatabaseSetupServisi
{
    private readonly MusteriServisi _musteriServisi;

    public DatabaseSetupServisi()
    {
       // _musteriServisi = new MusteriServisi();
    }

    public void InitializeDatabase()
    {
        var db = new BankaDbContext();
        db.Database.EnsureCreated();
        
        if (!_musteriServisi.MusteriVarMi())
        {
            DatabaseInitializer.TestVerileriEkle();
        }
    }
}