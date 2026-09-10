namespace DataAccess;

public interface ILogRepository
{
    void LogEkle(string mesaj, string seviye);
}