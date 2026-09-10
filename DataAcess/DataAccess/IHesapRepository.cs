using Dtos;
namespace DataAccess;
public interface IHesapRepository
{
    HesapDTO? GetirDtoile(string hesapNo);
    List<HesapDTO> ListeleDtoile(int musteriId);
    void Ekle(HesapDTO hesapDto);
    List<OdemeDTO> BekleyenOdemeleriGetir(int hesapId);
}