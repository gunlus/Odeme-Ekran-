namespace Business;

using Dtos;
using DataAccess;

public class HesapServisi : IHesapServisi
{
    private readonly IHesapRepository _hesapRepository;

    public HesapServisi(IHesapRepository hesapRepository)
    {
        _hesapRepository = hesapRepository;
    }

    public HesapDTO? HesapDetayGetir(string hesapNo)
    {
        if (string.IsNullOrWhiteSpace(hesapNo))
            throw new ArgumentException("Hesap numarası boş olamaz.");
        return _hesapRepository.GetirDtoile(hesapNo);
    }

    public List<HesapDTO> MusteriHesaplariniGetir(int musteriId)
    {
        return _hesapRepository.ListeleDtoile(musteriId);
    }

    public void YeniHesapOlustur(HesapDTO hesapDto)
    {
        if (string.IsNullOrWhiteSpace(hesapDto.HesapNo))
            throw new ArgumentException("Hesap numarası boş bırakılamaz.");
        if (hesapDto.Bakiye < 0)
            throw new ArgumentOutOfRangeException(nameof(hesapDto.Bakiye), "Başlangıç bakiyesi negatif olamaz.");

        _hesapRepository.Ekle(hesapDto);
    }

    public List<OdemeDTO> BekleyenOdemeleriListele(int hesapId)
    {
        return _hesapRepository.BekleyenOdemeleriGetir(hesapId);
    }
}