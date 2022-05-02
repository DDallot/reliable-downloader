using ReliableDownloader.Contracts.Validations;
using ReliableDownloader.Models;
using System;
using System.Linq;

namespace ReliableDownloader.Validations
{
    public class ValidateChecksum : IValidate
    {
		public bool IsValid(string localFilePath, FileHeader fileHeader)
        {
			var checksum = GetMD5Checksum(localFilePath);
			return checksum.SequenceEqual(fileHeader.ContentMD5);
        }

		private byte[] GetMD5Checksum(string filename)
		{
			using (var md5 = System.Security.Cryptography.MD5.Create())
			{
				using (var stream = System.IO.File.OpenRead(filename))
				{
					return md5.ComputeHash(stream);
				}
			}
		}
	}
}
