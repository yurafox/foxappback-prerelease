namespace Wsds.WebApp.Auth.Protection
{
    public interface ICrypto
    {
        string Encrypt(string str);
        string Decrypt(string eStr);
    }
}
