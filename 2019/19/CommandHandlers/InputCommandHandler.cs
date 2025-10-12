/// <summary>
/// Handles the input instruction (opcode 3) in the Intcode computer.
/// Reads a value from the IO channel and stores it at the resolved memory location.
/// </summary>
class InputCommandHandler : ACommandHandler
{
    private IIntCodeIO ioChannel;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputCommandHandler"/> class.
    /// </summary>
    /// 
    /// <param name="pIOChannel">The IO channel used for reading input.</param>
    public InputCommandHandler(IIntCodeIO pIOChannel)
    {
        ioChannel = pIOChannel;
    }

    /// <inheritdoc />
    public override void Execute(Context pContext)
    {
        long singleIntegerAsInput = ioChannel.Read();
        pContext.program[GetIndex(pContext, 1)] = singleIntegerAsInput;
        pContext.instructionPointer += 2;
    }
}
