namespace DataAccess;

using Entities;

public class LogRepository : ILogRepository
{
    private readonly BankaDbContext _db;

    public LogRepository(BankaDbContext db)
    {
        _db = db;
    }

    public void LogEkle(string mesaj, string seviye)
    {
        var log = new Log
        {
            Mesaj = mesaj,
            Seviye = seviye,
            Tarih = DateTime.Now
        };

        _db.Loglar.Add(log);
        _db.SaveChanges();
    }
}