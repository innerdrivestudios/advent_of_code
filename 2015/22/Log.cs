static class Log {

    public static bool enabled = true;

    public static void WriteLine (string pMessage = "")
    {
        if (enabled) Console.WriteLine(pMessage);
    }
}
