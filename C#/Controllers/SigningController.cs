using System.IO;
using System.Text;
using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.AspNetCore.Http;

namespace C_.Controllers
{
    [ApiController]
    public class SigningController : ControllerBase
    {
        private KeyClient _keyClient;
        private SecretClient _secretClient;

        public SigningController()
        {
            string vaultUri = "https://m08.vault.azure.net/";
            _keyClient = new KeyClient(new Uri(vaultUri), new DefaultAzureCredential());
            _secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Sign()
        {
            IFormFile file = Request.Form.Files[0];
            byte[] fileBytes = ReadFully(file.OpenReadStream());
            KeyVaultKey key = await _keyClient.GetKeyAsync("test");
            CryptographyClient crypto = new CryptographyClient(key.Id, new DefaultAzureCredential());
            SignResult result = await crypto.SignDataAsync(SignatureAlgorithm.RS512, fileBytes);
            return Ok(result.Signature);
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Verify([FromForm] string signature)
        {
            KeyVaultKey key = await _keyClient.GetKeyAsync("test");
            CryptographyClient crypto = new CryptographyClient(key.Id, new DefaultAzureCredential());
            IFormFile file = Request.Form.Files[0];
            byte[] fileBytes = ReadFully(file.OpenReadStream());
            VerifyResult result = await crypto.VerifyDataAsync(SignatureAlgorithm.RS512, fileBytes, Convert.FromBase64String(signature));

            return Ok(result);
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}