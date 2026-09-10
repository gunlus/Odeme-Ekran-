namespace Business;

using Dtos;

public interface IMusteriServisi
{
    MusteriDTO? TCKNIleGetir(string tckn);
    List<MusteriDTO> TumMusterileriGetir();
    void MusteriEkle(MusteriDTO musteriDTO);
    bool MusteriVarMi();
}