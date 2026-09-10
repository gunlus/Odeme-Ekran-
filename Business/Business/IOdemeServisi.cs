namespace Business;

public interface IOdemeServisi
{
    (bool basarili, string mesaj) OdemeYap(int odemeId);
}