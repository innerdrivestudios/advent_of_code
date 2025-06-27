/// <summary>
/// Handles the output instruction (opcode 4) in the Intcode computer.
/// Writes a resolved value to the IO channel.
/// </summary>
class OutputCommandHandler : ACommandHandler
{
    private IIntCodeIO ioChannel;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputCommandHandler"/> class.
    /// </summary>
    /// 
    /// <param name="pIOChannel">The IO channel used for output.</param>
    public OutputCommandHandler(IIntCodeIO pIOChannel)
    {
        ioChannel = pIOChannel;
    }

    /// <inheritdoc />
    public override void Execute(Context pContext)
    {
        ioChannel.Write(GetValue(pContext, 1));
        pContext.instructionPointer += 2;
    }
}
