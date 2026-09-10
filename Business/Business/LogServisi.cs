namespace Business;

using DataAccess;
using Dtos;
using Entities;

public class LogServisi : ILogServisi
{
    private readonly ILogRepository _logRepository;

    public LogServisi(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public void Logla(string mesaj, string seviye)
    {
        
        _logRepository.LogEkle(mesaj, seviye);
    }
}