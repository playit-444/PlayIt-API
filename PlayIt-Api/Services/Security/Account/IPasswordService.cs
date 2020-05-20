namespace PlayIt_Api.Services.Security.Account
{
    public interface IPasswordService
    {
        string PasswordPattern { get;  }
        int MinLength { get; }
        int MaxLength { get; }

        /// <summary>
        /// Creates a password based on an input string
        /// </summary>
        /// <param name="password">the password string</param>
        /// <param name="salt">the salt which is returned from the hashing process</param>
        /// <returns>the finished byte hash of the password</returns>
        byte[] CreatePassword(in string password, out byte[] salt);
        /// <summary>
        /// Compares the source string password with the hashed password from the database
        /// </summary>
        /// <param name="password">the plaintext password</param>
        /// <param name="hash">the hash of the password provided from the database</param>
        /// <param name="salt">the salt used in the original salt operation</param>
        /// <returns>whether the passwords match</returns>
        bool ComparePasswords(in string password, in byte[] hash, in byte[] salt);
    }
}
