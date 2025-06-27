/// <summary>
/// Represents the runtime state of the Intcode computer,
/// including memory, instruction pointer, and relative base.
/// </summary>
class Context
{
    /// <summary>
    /// The memory of the Intcode program, extended dynamically as needed.
    /// Keys are memory addresses, values are contents.
    /// </summary>
    public Dictionary<long, long> program;

    /// <summary>
    /// The current instruction pointer, representing the next opcode to execute.
    /// </summary>
    public long instructionPointer;

    /// <summary>
    /// The current relative base, used for resolving parameters in relative mode (mode 2).
    /// </summary>
    public long relativeBase;

    /// <summary>
    /// Gets the parameter mode for the given parameter index (starting at 1).
    /// </summary>
    /// 
    /// <param name="pParam">The 1-based parameter index (e.g. 1 for the first parameter).</param>
    /// 
    /// <returns>The parameter mode: 0 (position), 1 (immediate), or 2 (relative).</returns>
    public int GetParamMode(int pParam)
    {
        return (int)((program[instructionPointer] / Math.Pow(10, pParam + 1)) % 10);
    }
}
