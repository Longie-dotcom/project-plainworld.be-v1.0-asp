namespace Application.ApplicationException
{
    public abstract class ApplicationExceptionBase : Exception
    {
        protected ApplicationExceptionBase(string message) : base(message) { }
    }

    public class PlayerNotFound : ApplicationExceptionBase
    {
        public PlayerNotFound(string message) : base(message) { }
    }
}
