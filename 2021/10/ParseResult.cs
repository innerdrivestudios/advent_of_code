public class ParseResult {

    public enum ParseStatus { Complete, Incomplete, Invalid };

    public readonly ParseStatus status;
    public readonly string chunk;
    public readonly Stack<char> stack;
    public readonly int charIndex;

    public ParseResult(ParseStatus pStatus, string pChunk, Stack<char> pStack = null, int pCharIndex = -1) { 
        status = pStatus;
        chunk = pChunk;
        stack = pStack;
        charIndex = pCharIndex;
    }

    public char expected => ParseUtil.GetClosingChar(stack.Peek());
    public char found => chunk[charIndex];

    public int invalidCharScore => found switch
    {
        ')' => 3,
        ']' => 57,
        '}' => 1197,
        '>' => 25137,
        _ => 0
    };

    public long GetAutoCompleteScore()
    {
        //Ok we run this one little bit differently... (note we could also just map ([{< ...)
        //               01234
        string scores = " )]}>";

        long autocompletionScore = 0;

        while (stack.Count > 0)
        {
            autocompletionScore *= 5;
            //If we had mapped ([{ etc we would not need to get the closing char but anyway, for clarity...
            autocompletionScore += scores.IndexOf(ParseUtil.GetClosingChar (stack.Pop()));
        }

        return autocompletionScore;
    }
}

