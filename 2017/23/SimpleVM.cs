using System.Diagnostics;

class SimpleVM {

    public enum VMState { Stepped, Ended, Waiting };

    private long instructionPointer = 0;
    private Dictionary<string, long> registers = new();
    private string[][] instructions;

    private Dictionary<string, int> count = new();

    public bool log = false;

    public SimpleVM(string[][] pInstructions) {
        instructions = pInstructions;

        foreach (char c in "abcdefgh") registers[""+c] = 0;
    }

    public VMState Run() {
        if (instructionPointer >= instructions.Length) return VMState.Ended;
        
        var instruction = instructions[instructionPointer];
        string opcode= instruction[0];

        count[opcode] = count.GetValueOrDefault(opcode, 0) + 1;

        VMState returnValue = VMState.Ended;

        if (log) Console.Write($"Executing [{instructionPointer}] -> {string.Join(" ", instruction)}\t");

        switch (opcode)
        {
            case "set": HandleSet(instruction); returnValue = VMState.Stepped; break;
            case "sub": HandleSub(instruction); returnValue = VMState.Stepped; break;
            case "mul": HandleMul(instruction); returnValue = VMState.Stepped; break;
            case "mul1": HandleMul(instruction); returnValue = VMState.Stepped; break;
            case "mul2": HandleMul(instruction); returnValue = VMState.Stepped; break;
            case "jnz": HandleJNZ(instruction); returnValue = VMState.Stepped; break;
        }

        if (log)
        {
            foreach (var kv in registers) Console.Write(kv.Key + "=" + ("" + kv.Value).PadRight(10, ' '));
            Console.WriteLine();
            Console.ReadKey();
        }

        return returnValue;
    }

    // And all the helper methods ...

    void HandleSet(string[] pInstruction)
    {
        registers[pInstruction[1]] = GetValue(pInstruction[2]);
        instructionPointer++;
    }

    void HandleSub(string[] pInstruction)
    {
        registers[pInstruction[1]] -= GetValue(pInstruction[2]);
        instructionPointer++;
    }

    void HandleMul(string[] pInstruction)
    {
        registers[pInstruction[1]] *= GetValue(pInstruction[2]);
        instructionPointer++;
    }

    void HandleJNZ(string[] pInstruction)
    {
        long valueX = GetValue(pInstruction[1]);
        long valueY = GetValue(pInstruction[2]);

        if (valueX != 0) instructionPointer += valueY;
        else instructionPointer++;
    }

    long GetValue(string pInput)
    {
        if (long.TryParse(pInput, out var value)) return value;
        else return registers.GetValueOrDefault(pInput, 0);
    }

    public int GetInstructionCount (string pOpcode)
    {
        return count[pOpcode];
    }

    public void SetRegisterValue (string pRegister, long pValue)
    {
        registers[pRegister] = pValue;
    }

    public long GetRegisterValue(string pRegister)
    {
        return registers[pRegister];
    }

}
