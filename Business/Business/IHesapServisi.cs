namespace Business;

using Dtos;

public interface IHesapServisi
{
    HesapDTO? HesapDetayGetir(string hesapNo);
    List<HesapDTO> MusteriHesaplariniGetir(int musteriId);
    void YeniHesapOlustur(HesapDTO hesapDto);
    List<OdemeDTO> BekleyenOdemeleriListele(int hesapId);
}