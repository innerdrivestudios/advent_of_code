// This is a refactor step of part 1 with minimal changes (e.g. it could have been done better ;))

class SimpleVM {

    public enum VMState { Stepped, Ended, Waiting };
    public long sendCount {  get; private set; }    
    public SimpleVM other;

    private long instructionPointer = 0;
    private Dictionary<string, long> registers = new();
    private string[][] instructions;
    private Queue<long> receiveQueue = new ();


    public SimpleVM(string[][] pInstructions, int pId) {
        instructions = pInstructions;
        registers["p"] = pId;
    }

    public VMState Run() {
        if (instructionPointer >= instructions.Length) return VMState.Ended;

        var instruction = instructions[instructionPointer];

        switch (instruction[0])
        {
            case "snd": HandleSnd(instruction); return VMState.Stepped;
            case "set": HandleSet(instruction); return VMState.Stepped;
            case "add": HandleAdd(instruction); return VMState.Stepped;
            case "mul": HandleMul(instruction); return VMState.Stepped;
            case "mod": HandleMod(instruction); return VMState.Stepped;
            case "rcv": return HandleRcv(instruction); 
            case "jgz": HandleJGZ(instruction); return VMState.Stepped;
        }

        return VMState.Ended;
    }

    // And all the helper methods ...

    void HandleSnd(string[] instruction)
    {
        other.ReceiveValue(GetValue(instruction[1]));
        instructionPointer++;
        sendCount++;
    }

    void HandleSet(string[] instruction)
    {
        registers[instruction[1]] = GetValue(instruction[2]);
        instructionPointer++;
    }

    void HandleAdd(string[] instruction)
    {
        registers[instruction[1]] = registers[instruction[1]] + GetValue(instruction[2]);
        instructionPointer++;
    }

    void HandleMul(string[] instruction)
    {
        registers[instruction[1]] = registers[instruction[1]] * GetValue(instruction[2]);
        instructionPointer++;
    }

    void HandleMod(string[] instruction)
    {
        registers[instruction[1]] = registers[instruction[1]] % GetValue(instruction[2]);
        instructionPointer++;
    }

    VMState HandleRcv(string[] instruction)
    {
        if (receiveQueue.Count > 0)
        {
            long value = receiveQueue.Dequeue();
            registers[instruction[1]] = value;
            instructionPointer++;
            return VMState.Stepped;
        }
        else
        {
            return VMState.Waiting;
        }
    }

    void HandleJGZ(string[] instruction)
    {
        long valueX = GetValue(instruction[1]);
        long valueY = GetValue(instruction[2]);

        if (valueX > 0) instructionPointer += valueY;
        else instructionPointer++;
    }

    long GetValue(string pInput)
    {
        if (long.TryParse(pInput, out var value)) return value;
        else return registers.GetValueOrDefault(pInput, 0);
    }

    public void ReceiveValue (long pValue)
    {
        receiveQueue.Enqueue(pValue);
    }

}
