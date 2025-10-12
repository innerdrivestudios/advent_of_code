/// <summary>
/// Abstract base class for all Intcode instruction handlers.
/// Provides utility methods for resolving parameter values and indexes based on parameter modes.
/// </summary>
abstract class ACommandHandler
{
    /// <summary>
    /// Executes the command represented by this handler.
    /// Modifies the execution <see cref="Context"/> based on instruction semantics.
    /// </summary>
    /// 
    /// <param name="pContext">The current execution context, including memory, instruction pointer, and IO.</param>
    public abstract void Execute(Context pContext);

    /// <summary>
    /// Gets the resolved value of a parameter at the given offset.
    /// Uses the parameter mode to determine how to interpret the parameter.
    /// </summary>
    /// 
    /// <param name="pContext">The current execution context.</param>
    /// <param name="pOffset">The offset from the instruction pointer for the parameter.</param>
    /// 
    /// <returns>The value resolved from memory.</returns>
    protected long GetValue(Context pContext, int pOffset)
    {
        return pContext.program.GetValueOrDefault(GetIndex(pContext, pOffset));
    }

    /// <summary>
    /// Resolves the memory index for a parameter based on its mode.
    /// </summary>
    /// 
    /// <param name="pContext">The current execution context.</param>
    /// <param name="pOffset">The offset from the instruction pointer for the parameter.</param>
    /// 
    /// <returns>The effective memory index to read from or write to.</returns>
    /// 
    /// <exception cref="InvalidDataException">Thrown if the parameter mode is unsupported.</exception>
    protected long GetIndex(Context pContext, int pOffset)
    {
        int paramMode = pContext.GetParamMode(pOffset);
        long index = pContext.instructionPointer + pOffset;
        var program = pContext.program;

        return paramMode switch
        {
            0 => program.GetValueOrDefault(index),                           // Position mode
            1 => index,                                                      // Immediate mode
            2 => pContext.relativeBase + program.GetValueOrDefault(index),   // Relative mode
            _ => throw new InvalidDataException($"Parameter mode {paramMode} not supported.")
        };
    }
}
