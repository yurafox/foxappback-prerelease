using Microsoft.AspNetCore.DataProtection;

namespace Wsds.WebApp.Auth.Protection
{
    public class FSCryptoProvider:ICrypto
    {
        private readonly string _key;
        private readonly IDataProtectionProvider _provider;

        public FSCryptoProvider(IDataProtectionProvider provider)
        {
            _provider = provider;
            _key = AuthOpt.Key;
        }

        public string Encrypt(string str)
        {
            var protector = _provider.CreateProtector(_key);
            return protector.Protect(str);
        }

        public string Decrypt(string eStr)
        {
            var protector = _provider.CreateProtector(_key);
            return protector.Unprotect(eStr);
        }
    }
}
