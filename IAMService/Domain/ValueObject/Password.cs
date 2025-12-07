using Domain.DomainException;

namespace Domain.ValueObject
{
    public class Password
    {
        #region Attributes
        #endregion

        #region Properties 
        public string Hashed { get; private set; } = string.Empty;
        #endregion

        private Password(string hashed)
        {
            Hashed = hashed;
        }

        #region Methods
        public static Password FromPlain(string plain)
        {
            ValidatePlain(plain);
            var hashed = BCrypt.Net.BCrypt.HashPassword(plain);
            return new Password(hashed);
        }

        public static Password FromHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new InvalidPasswordOVException("Hash cannot be empty.");
            return new Password(hash);
        }

        public bool Verify(string plain)
        {
            return BCrypt.Net.BCrypt.Verify(plain, Hashed);
        }
        #endregion

        #region Private Helpers 
        private static void ValidatePlain(string plain)
        {
            if (string.IsNullOrWhiteSpace(plain))
                throw new InvalidPasswordOVException("Password cannot be empty.");
            if (plain.Length < 6)
                throw new InvalidPasswordOVException("Password must be at least 6 characters.");
        }
        #endregion
    }
}
