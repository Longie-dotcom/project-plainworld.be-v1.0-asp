namespace Application.ApplicationException
{
    public class PlayerNotFound : Exception
    {
        public PlayerNotFound(string message) : base(message) { }
    }
}
