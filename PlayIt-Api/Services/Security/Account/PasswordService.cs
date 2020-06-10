using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace PlayIt_Api.Services.Security.Account
{
    public class PasswordService : IPasswordService
    {
        private readonly string _passwordPattern;
        private int _minLength, _maxLength;
        private readonly IHashingService _hashingService;

        public PasswordService(
            [FromServices]IHashingService service,
            int min,
            int max)
        {
            this.MinLength = min;
            this.MaxLength = max;
            this._hashingService = service;

            this._passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{" + _minLength + "," + _maxLength + "}$";
        }

        public int MinLength { get => _minLength; private set => _minLength = value; }
        public int MaxLength { get => _maxLength; private set => _maxLength = value; }
        public string PasswordPattern { get => _passwordPattern;}

        public bool ComparePasswords(in string password, in byte[] hash, in byte[] salt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            if (hash == null || hash.Length == 0)
                throw new ArgumentNullException(nameof(hash));
            if (salt == null || salt.Length == 0)
                throw new ArgumentNullException(nameof(salt));

            return _hashingService.CompareHash(Encoding.UTF8.GetBytes(password), salt, hash);
        }

        public byte[] CreatePassword(in string password, out byte[] salt)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            if (password.Length < MinLength || password.Length > MaxLength)
                throw new ArgumentOutOfRangeException(nameof(password));

            if (!Regex.IsMatch(password, _passwordPattern))
                throw new ArgumentOutOfRangeException(nameof(password));
            else
                return _hashingService.CreateHash(Encoding.UTF8.GetBytes(password), out salt);
        }
    }
}
