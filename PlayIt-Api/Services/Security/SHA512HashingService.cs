using System;
using System.Security.Cryptography;

namespace PlayIt_Api.Services.Security
{
    public class SHA512HashingService : IHashingService
    {
        private Org.BouncyCastle.Security.SecureRandom random = new Org.BouncyCastle.Security.SecureRandom();

        public bool CompareHash(in byte[] input, in byte[] salt, in byte[] expectedHash)
        {
            //check params
            if (input == null || input.Length == 0)
                throw new ArgumentNullException(nameof(input));
            if (expectedHash == null || expectedHash.Length == 0)
                throw new ArgumentNullException(nameof(expectedHash));

            //compute hash
            byte[] freshHash;
            using (SHA512 sha512 = new SHA512Managed())
            {
                byte[] data = new byte[input.Length + 32];
                Array.Copy(input, 0, data, 0, input.Length);
                Array.Copy(salt, 0, data, input.Length, 32);
                freshHash = sha512.ComputeHash(data);
            }

            //compare hashes
            for (int i = 0; i < expectedHash.Length; i++)
            {
                if (expectedHash[i] != freshHash[i]) return false;
            }

            return true;
        }

        public byte[] CreateHash(in byte[] input, out byte[] salt)
        {
            //assign salt
            salt = new byte[32];
            //check params
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (input.Length < 1)
                throw new ArgumentOutOfRangeException(nameof(input));

            //generate salt
            random.NextBytes(salt);
            //compute hash
            byte[] hash;
            using (SHA512 sha = new SHA512Managed())
            {
                byte[] data = new byte[input.Length + 32];
                Array.Copy(input, 0, data, 0, input.Length);
                Array.Copy(salt, 0, data, input.Length, 32);
                hash = sha.ComputeHash(data);
            }

            //return finished hash
            return hash;
        }
    }
}
