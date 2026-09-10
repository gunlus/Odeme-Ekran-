using Dtos;
namespace DataAccess;

public interface IMusteriRepository
{
    MusteriDTO? TCKNileGetir(string tckn);
    List<MusteriDTO> TumMusterileriGetir();
    void MusteriEkle(MusteriDTO musteriDTO);
    bool MusteriVarMi();
}