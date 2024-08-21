using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoommateMatcher.Services
{
	public interface IEncryptionService
	{
		public Task<string> EncryptAsync(string plainText);
		public Task<string> DecryptAsync(string encryptedText);
		
	}
}