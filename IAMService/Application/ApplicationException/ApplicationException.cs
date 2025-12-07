namespace Application.ApplicationException
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }

    public class RoleNotFound : ApplicationExceptionBase
    {
        public RoleNotFound(string message) : base(message) { }
    }

    public class UserNotFound : ApplicationExceptionBase
    {
        public UserNotFound(string message) : base(message) { }
    }

    public class UserEmailNotFound : ApplicationExceptionBase
    {
        public UserEmailNotFound(string message) : base(message) { }
    }

    public class PrivilegeNotFound : ApplicationExceptionBase
    {
        public PrivilegeNotFound(string message) : base(message) { }
    }

    public class UserAlreadyExists : ApplicationExceptionBase
    {
        public UserAlreadyExists(string message) : base(message) { }
    }

    public class RoleCodeAlreadyExists : ApplicationExceptionBase
    {
        public RoleCodeAlreadyExists(string message) : base(message) { }
    }

    public class InvalidTokenException : ApplicationExceptionBase
    {
        public InvalidTokenException(string message) : base(message) { }
    }

    public class InvalidPassword : ApplicationExceptionBase
    {
        public InvalidPassword(string message) : base(message) { }
    }

    public class InvalidRole : ApplicationExceptionBase
    {
        public InvalidRole(string message) : base(message) { }
    }

    public class InvalidResetPassword : ApplicationExceptionBase
    {
        public InvalidResetPassword(string message) : base(message) { }
    }

    public class InvalidChangePassword : ApplicationExceptionBase
    {
        public InvalidChangePassword(string message) : base(message) { }
    }

    public class PrivilegeAlreadyExists : Exception
    {
        public PrivilegeAlreadyExists(string message) : base(message) { }
    }

    public class InvalidOwner : Exception
    {
        public InvalidOwner(string message) : base(message) { }
    }
}
