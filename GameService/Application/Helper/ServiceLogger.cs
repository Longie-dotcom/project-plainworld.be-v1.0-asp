namespace Application.Helper
{
    public enum Level
    {
        Service,
        API,
        Infrastructure,
    }

    public enum Type
    {
        Log,
        Warn,
        Error,
    }

    public static class ServiceLogger
    {
        public static void Logging(Level level, string message)
        {
            Console.WriteLine(Format(Type.Log, level, message));
        }

        public static void Warning(Level level, string message)
        {
            Console.WriteLine(Format(Type.Warn, level, message));
        }

        public static void Error(Level level, string message)
        {
            Console.WriteLine(Format(Type.Error, level, message));
        }

        private static string Format(Type type, Level level, string message)
        {
            return $"[{type}]-[{level}]: {message}";
        }
    }
}
