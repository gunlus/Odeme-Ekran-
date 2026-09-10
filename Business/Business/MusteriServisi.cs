namespace Business;

using Dtos;
using DataAccess;

public class MusteriServisi : IMusteriServisi
{
    private readonly IMusteriRepository _musteriRepository;

    public MusteriServisi(IMusteriRepository musteriRepository)
    {
        _musteriRepository = musteriRepository;
    }

    public MusteriDTO? TCKNIleGetir(string tckn)
    {
        return _musteriRepository.TCKNileGetir(tckn);
    }

    public List<MusteriDTO> TumMusterileriGetir()
    {
        return _musteriRepository.TumMusterileriGetir();
    }

    public void MusteriEkle(MusteriDTO musteriDTO)
    {
        if (string.IsNullOrWhiteSpace(musteriDTO.TCKN) || musteriDTO.TCKN.Length != 11)
            throw new ArgumentException("Kimlik numarası 11 haneli olmalıdır.");

        var mevcut = _musteriRepository.TCKNileGetir(musteriDTO.TCKN);
        if (mevcut != null)
            throw new InvalidOperationException("Bu TCKN ile kayıtlı müşteri zaten mevcut.");

        _musteriRepository.MusteriEkle(musteriDTO);
    }

    public bool MusteriVarMi()
    {
        return _musteriRepository.MusteriVarMi();
    }
}