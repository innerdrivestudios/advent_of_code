/// <summary>
/// Interface representing input/output operations for the Intcode computer.
/// Implementations provide mechanisms for reading and writing values.
/// </summary>
interface IIntCodeIO
{
    /// <summary>
    /// Reads a value to be used as Intcode input.
    /// </summary>
    /// 
    /// <returns>A <see cref="long"/> value for input.</returns>
    long Read();

    /// <summary>
    /// Writes an output value from the Intcode program.
    /// </summary>
    /// 
    /// <param name="value">The <see cref="long"/> value to output.</param>
    void Write(long value);
}
