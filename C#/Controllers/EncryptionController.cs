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
    public class EncryptionController : ControllerBase
    {
        private KeyClient _keyClient;
        private SecretClient _secretClient;

        public EncryptionController()
        {
            string vaultUri = "https://m08.vault.azure.net/";
            _keyClient = new KeyClient(new Uri(vaultUri), new DefaultAzureCredential());
            _secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Encrypt(EncryptDto toEncrypt)
        {
            byte[] toEncryptInBytes = Encoding.UTF8.GetBytes(toEncrypt.Payload);

            if (toEncryptInBytes.Length > 245)
            {
                return BadRequest();
            }

            KeyVaultKey key = await _keyClient.GetKeyAsync("test");
            CryptographyClient crypto = new CryptographyClient(key.Id, new DefaultAzureCredential());

            EncryptResult result = await crypto.EncryptAsync(EncryptionAlgorithm.RsaOaep256, toEncryptInBytes);

            return new OkObjectResult(Convert.ToBase64String(result.Ciphertext));
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Decrypt(EncryptDto toDecrypt)
        {
            byte[] toDecryptInBytes = Convert.FromBase64String(toDecrypt.Payload);
            KeyVaultKey key = await _keyClient.GetKeyAsync("test");
            CryptographyClient crypto = new CryptographyClient(key.Id, new DefaultAzureCredential());

            DecryptResult result = await crypto.DecryptAsync(EncryptionAlgorithm.RsaOaep256, toDecryptInBytes);

            return new OkObjectResult(Encoding.UTF8.GetString(result.Plaintext));
        }
    }
}