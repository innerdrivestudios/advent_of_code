/// <summary>
/// Provides a console-based implementation of the <see cref="IIntCodeIO"/> interface.
/// Used for interactive input/output with the Intcode computer.
/// </summary>
class ConsoleIO : IIntCodeIO
{
    /// <summary>
    /// Reads a long value from the console. Retries until valid input is provided.
    /// </summary>
    /// 
    /// <returns>The parsed <see cref="long"/> value entered by the user.</returns>
    public long Read()
    {
        while (true)
        {
            if (long.TryParse(Console.ReadLine(), out var value))
            {
                return value;
            }
            else
            {
                Console.WriteLine("Please enter a long...");
            }
        }
    }

    /// <summary>
    /// Writes a <see cref="long"/> value to the console.
    /// </summary>
    /// 
    /// <param name="value">The value to write.</param>
    public void Write(long value)
    {
        Console.WriteLine(value);
    }
}
