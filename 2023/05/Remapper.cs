using RemapValue = (long src, long dst, long rng);
using RangeValue = (long src, long rng);

class Remapper {

    private List<RemapValue> remapValues = new();

    public Remapper (string pRemapDataBlock)
    {
        // The input blocks look like this:
        // seed-to-soil map:
        // 2067746708 2321931404 124423068
        // 2774831547 3357841131 95865403
        // 3776553292 3323317283 34523848
        // 4167907733 3453706534 116376261

        // I'll first process this into an array of (source, dest, range) tuples:
        remapValues = pRemapDataBlock
            .Substring(pRemapDataBlock.IndexOf(Environment.NewLine) + 1)                // Cut of seed-to-soil etc
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)        // Split into separate lines
            .Select(x => x.Split(" "))                                                // Convert the lines into 3 string numbers
            .Select(
                x => (long.Parse(x[1]), long.Parse(x[0]), long.Parse(x[2]))
            ).ToList();

        // Also due to the nature of the remap setup, these ranges should never overlap...
        // We're not going to test that (only checked it in the debugger),
        // but I am going to use that property for some optimization later on ...

        remapValues.Sort ((a, b) => a.src.CompareTo(b.src));
    }

    public long Remap(long pInputValue)
    {
        //How do we remap? 

        // First we find the range our input value belongs in
        // Every range describes values from src to src+rng inclusive
        // However the moment src is bigger than our input value we can stop comparing ...

        RemapValue? remapValue = GetRemapData(pInputValue);

        // No remap data found?
        if (remapValue == null) return pInputValue;

        // If we've found a range, remap our input value 

        return Remap(remapValue.Value, pInputValue);
    }

    private long Remap (RemapValue rRemapValue,  long pInputValue)
    {
        return rRemapValue.dst + pInputValue - rRemapValue.src;
    }

    private RemapValue? GetRemapData(long pInputValue)
    {
        foreach (var remapData in remapValues)
        {
            // Missed the range we should be in, return null (since we are sorted ...)
            if (pInputValue < remapData.src) return null;
            // If the input value is in the range, return it
            if (pInputValue >= remapData.src && pInputValue < remapData.src + remapData.rng) return remapData;
        }

        return null;
    }

    //Overload to remap a range instead of single value

    public List<RangeValue> Remap(RangeValue pInValue)
    {
        List<RangeValue> outputRanges = new();

        // SO how do we do this ...
        //
        // This is the situation:
        // we have single continuous in range, parts of which might overlap with the src range of remap data in our (sorted) remap data list.
        //
        // What are the different overlap situations that we can have? These:
        //
        //  IN RANGE                                             [-------]
        //  1) REMAP SRC RANGE - NO OVERLAP BEFORE   [---------]                                   -> SKIP RemapData and continue
        //  2) REMAP SRC RANGE - OVERLAP BEFORE             [------]                               -> CUT OFF the overlapping part from IN and remap it (1 new, 1 updated in)
        //  3) REMAP SRC RANGE - OVERLAP ALL                   [------------]                      -> JUST replace the whole range and remap it
        //  4) REMAP SRC RANGE - OVERLAP IN                         [---]                          -> CUT into 3 parts, 1 old done, 1 remapped new done, 1 still to check (updated in)
        //  5) REMAP SRC RANGE - OVERLAP AFTER                        [-------]                    -> CUT into 2 parts, 1 old done, 1 remapped new, nothing left to check
        //  6) REMAP SRC RANGE - NO OVERLAP AFTER                                [----]            -> SINCE the remap ranges are sorted, we're done, pass on whatever is left
        //  7) NO REMAP SRC RANGE LEFT ?                                                           -> DONE, pass on whatever is left

        // Note:
        // - remember any part of the in range which is not overlapping with a remap range will be remapped to itself...
        // - perhaps there is some way to condense the cases and approach this in a smarter way
        //   but I can't wrap my head around it and it would be unreadable and messy anyway, so separate cases it is!

        foreach (var remapValue in remapValues)
        {
            long remapEnd = remapValue.src + remapValue.rng;
            long srcEnd = pInValue.src + pInValue.rng;


            // 1) REMAP SRC RANGE - NO OVERLAP BEFORE:  SKIP RemapData and continue
            if (remapEnd < pInValue.src) continue;

            // 2) REMAP SRC RANGE - OVERLAP BEFORE:     CUT OFF the overlapping part from IN and remap it (1 new, 1 updated in) 
            else if (remapValue.src <= pInValue.src && remapEnd >= pInValue.src && remapEnd < srcEnd)
            {
                //Looking at situation 2 above: overlap is from start of pInValue to the end of the remap src range, but it needs to be relative to pInValue
                //so we do remapEnd-pInvalue.src. IN ADDITION, we need the remapped start value so we don't use pInValue.src but Remap (..., pInvalue.src)
                RangeValue newRange = (Remap (remapValue, pInValue.src), remapEnd - pInValue.src);
                pInValue.src += newRange.rng;
                pInValue.rng -= newRange.rng;

                outputRanges.Add(newRange);

                // At this point pInValue.rng should be bigger than 0!!
                if (pInValue.rng <= 0) throw new Exception("Invalid in range left!");
            }

            // 3) REMAP SRC RANGE - OVERLAP ALL:        JUST replace the whole range and remap it
            else if (remapValue.src <= pInValue.src && remapEnd >= srcEnd)
            {
                //Looking at situation 3 above: overlap is from start of pInValue to the end of the pInvalue range, but it needs to be relative to pInValue
                RangeValue newRange = (Remap(remapValue, pInValue.src), pInValue.rng);
                //Mark the invalue as nothing left to do...
                pInValue.rng = 0;

                outputRanges.Add(newRange);

                break;
            }

            // 4) REMAP SRC RANGE - OVERLAP IN:        CUT into 3 parts, 1 old done, 1 remapped new done, 1 still to check (updated in)
            else if (remapValue.src < pInValue.src && remapEnd < srcEnd)
            {
                // This is the most complicated one ...

                // So now we have three ranges to take care of:
                // The start -> unremapped, passed on to the output as is, from the invalue.src to where the remapValue.src starts...
                RangeValue unalteredStart = (pInValue.src, remapValue.src - pInValue.src); 
                outputRanges.Add(unalteredStart);

                // The inner part which should be completely remapped
                RangeValue innerPart = (Remap(remapValue, remapValue.src), remapValue.rng);
                outputRanges.Add(innerPart);

                // And the left over part:
                pInValue.src = remapEnd + 1;
                pInValue.rng = srcEnd-pInValue.src;

                // At this point pInValue.rng should be bigger than 0!!
                if (pInValue.rng <= 0) throw new Exception("Invalid in range left!");

            }

            //  5) REMAP SRC RANGE - OVERLAP AFTER -> CUT into 2 parts, 1 old done, 1 remapped new, nothing left to check
            else if (remapValue.src <= srcEnd && remapEnd > srcEnd)
            {
                RangeValue unalteredStart = (pInValue.src, remapValue.src - pInValue.src);
                outputRanges.Add(unalteredStart);

                RangeValue remappedEnd = (Remap (remapValue, remapValue.src), srcEnd - remapValue.src);
                outputRanges.Add(remappedEnd);

                pInValue.rng = 0;
                break;
            }
            else // we're done!
            {
                break;
            }

        }

        // Still unmapped stuff left?
        if (pInValue.rng > 0)
        {
            outputRanges.Add(pInValue);
        }
        
        return outputRanges;
    }



}

