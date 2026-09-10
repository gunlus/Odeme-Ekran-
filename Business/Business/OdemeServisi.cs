namespace Business;

using DataAccess;

public class OdemeServisi : IOdemeServisi
{
    private readonly IOdemeRepository _odemeRepository;
    private readonly ILogServisi _logServisi; // Log servisi eklendi

    public OdemeServisi(IOdemeRepository odemeRepository, ILogServisi logServisi)
    {
        _odemeRepository = odemeRepository;
        _logServisi = logServisi;
    }

    public (bool basarili, string mesaj) OdemeYap(int odemeId)
    {
        if (odemeId <= 0)
        {
            _logServisi.Logla($"Geçersiz ödeme ID denemesi: {odemeId}", "Warning");
            return (false, "Geçersiz ödeme ID değeri.");
        }

        var sonuc = _odemeRepository.OdemeIsleminiGerceklestir(odemeId);

        // İşlem sonucuna göre veritabanına log atılıyor
        if (sonuc.basarili)
        {
            _logServisi.Logla($"Ödeme başarıyla gerçekleştirildi. Odeme ID: {odemeId}", "Info");
        }
        else
        {
            _logServisi.Logla($"Ödeme işlemi başarısız oldu. Odeme ID: {odemeId}, Mesaj: {sonuc.mesaj}", "Error");
        }

        return sonuc;
    }
}