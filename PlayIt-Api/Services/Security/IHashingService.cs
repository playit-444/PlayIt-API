namespace PlayIt_Api.Services.Security
{
    public interface IHashingService
    {
        /// <summary>
        /// Creates a fresh hash based on the input string, as well as some freshy generated salt
        /// </summary>
        /// <param name="input">the input bytes for the hash computation</param>
        /// <param name="salt">the salt bytes for the hash computation</param>
        /// <returns>the finished hash</returns>
        byte[] CreateHash(in byte[] input, out byte[] salt);

        /// <summary>
        /// Compares a pre-existing hash to a newly generated one
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        /// <param name="expectedHash"></param>
        /// <returns>whether the input is equal to the resulting hash value</returns>
        bool CompareHash(in byte[] input, in byte[] salt, in byte[] expectedHash);
    }
}
