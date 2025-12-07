namespace Application.Helper
{
    public enum Level
    {
        Service,
        API,
        Infrastructure,
    }

    public static class ServiceLogger
    {
        public static void Logging(Level level, string message)
        {
            Console.WriteLine(Format(level, message));
        }

        private static string Format(Level level, string message)
        {
            return $"[{level}]: {message}";
        }
    }
}
