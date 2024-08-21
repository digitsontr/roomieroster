using System.Security.Cryptography;

namespace RoommateMatcher.Services
{
	public class EncryptionService : IEncryptionService
	{
		private readonly IConfiguration _configuration;
		
		public EncryptionService(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		
		public async Task<string> DecryptAsync(string encryptedText)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = Convert.FromBase64String(_configuration["Chat:EncryptionKey"] ?? "");
				aes.IV = Convert.FromBase64String(_configuration["Chat:EncryptionIV"] ?? "");

				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);

				using (MemoryStream ms = new MemoryStream(cipherTextBytes))
				{
					using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader sr = new StreamReader(cs))
						{
							return await sr.ReadToEndAsync();
						}
					}
				}
			}
		}

		public Task<string> EncryptAsync(string plainText)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = Convert.FromBase64String(_configuration["Chat:EncryptionKey"] ?? "");
				aes.IV = Convert.FromBase64String(_configuration["Chat:EncryptionIV"] ?? "");

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter sw = new StreamWriter(cs))
						{
							sw.Write(plainText);
						}
					}

					return Task.FromResult(Convert.ToBase64String(ms.ToArray()));
				}
			}
		}
	}
}