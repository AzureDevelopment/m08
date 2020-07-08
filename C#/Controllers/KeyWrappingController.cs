using System.Text;
using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;

namespace C_.Controllers
{
    [ApiController]
    public class KeyWrappingController : ControllerBase
    {
        private KeyClient _keyClient;
        private SecretClient _secretClient;

        public KeyWrappingController()
        {
            string vaultUri = "https://m08.vault.azure.net/";
            _keyClient = new KeyClient(new Uri(vaultUri), new DefaultAzureCredential());
            _secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Wrap(EncryptDto toEncrypt)
        {
            byte[] keyToWrap = Encoding.UTF8.GetBytes(toEncrypt.Payload);
            KeyVaultKey key = await _keyClient.GetKeyAsync("test");
            CryptographyClient crypto = new CryptographyClient(key.Id, new DefaultAzureCredential());

            WrapResult result = await crypto.WrapKeyAsync(KeyWrapAlgorithm.RsaOaep256, keyToWrap);
            KeyVaultSecret secret = await _secretClient.SetSecretAsync(new KeyVaultSecret(toEncrypt.Name, Convert.ToBase64String(result.EncryptedKey)));
            return Ok(secret);
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Unwrap(EncryptDto toDecrypt)
        {
            KeyVaultKey key = await _keyClient.GetKeyAsync("test");
            CryptographyClient crypto = new CryptographyClient(key.Id, new DefaultAzureCredential());
            KeyVaultSecret wrappedKey = await _secretClient.GetSecretAsync(toDecrypt.Name);
            UnwrapResult result = await crypto.UnwrapKeyAsync(KeyWrapAlgorithm.RsaOaep256, Convert.FromBase64String(wrappedKey.Value));

            return Ok(Encoding.UTF8.GetString(result.Key));
        }
    }
}