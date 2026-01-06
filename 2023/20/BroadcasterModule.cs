
using System.Reflection.Metadata.Ecma335;

class BroadcasterModule : AbstractModule
{
    public BroadcasterModule(string pModuleSpecification) : base(pModuleSpecification)
    {
    }

    protected override void ResetInternal()
    {
        lowPulseCount = highPulseCount = 0;
    }

    public bool Operate ()
    {
        return ProcessModules();
    }

    protected override void ProcessPulse(bool pHigh, AbstractModule pSender)
    {
        base.ProcessPulse(pHigh, pSender);
        QueuePulse(pHigh);
    }

    public void DebugFlipFlops()
    {
        base.DebugFlipFlops();
    }

    public void StepByStepDebugging()
    {
        debug = true;

        Console.WriteLine("Press key to send a single low pulse to the broadcaster...");
        Console.ReadKey();

        while (true)
        {
            // Push button...
            ReceivePulse(false, null);

            do
            {
                Console.WriteLine("Press key to operate...");
                Console.ReadKey();
            } while (!Operate());
        }
    }

    public long Part1()
    {
        Reset();

        for (int i = 1; i <= 1000; i++)
        {
            ReceivePulse(false, null);
            while (!Operate()) { }
        }

        return lowPulseCount * highPulseCount;
    }

    public long Part2()
    {
        // Solved this part by reverse engineering the given schematic.

        // Looking in our schematic RX is only one output.
        // So theoretically we can just repeat sending pulses until RX turns false.
        // Unfortunately that turned out to be unfeasible after a couple million button pushes ;).

        // However... in the schematic we can see rx is connected to a conjunction (aka AND-ish) gate.
        // which is connected to a couple of other conjunction gates.
        //
        // In my case (SOME OTHER GATES) -> &(cl, rp, lb, nj) -> &lx -> rx
        //
        // What does the conjunction gate do again?
        // If ALL inputs are HIGH at the same time after a button push, send a LOW otherwise a HIGH.
        // SO for RX to be LOW... LX needs to sent out a LOW (since RX is not a CONJUNCTION gate).
        // For LX to sent out a LOW all inputs (cl, rp, lb, nj) need to send out a high.
        // For cl, rp, lb, nj (which are also conjunction gates) their inputs all need to be low at the same time...
        // However inspecting my puzzle input, these modules only have one input, so the question becomes, 
        // when does each module receive a low (sometimes that repeats on a steady phase)

        // So this is basically all we need to know... when do the channels cl, rp, lb and nj all become LOW...
        // and more importantly WHEN do they do so at the same time...
        // Because IF they are all low, they will all send out a HIGH, if they all send out a HIGH, lx will be low.
        // (Still following?)

        Reset();

        List<AbstractModule> lxConnections = moduleRegistry["lx"].connectedInputModules;
        List<long> requiredButtonPresses = new();

        int buttonPresses = 0;

        Console.WriteLine("Resolving input channels for modules connected to rx through lx...");

        while (lxConnections.Count > 0)
        {
            buttonPresses++;
            ReceivePulse(false, null);
            while (!Operate()) { }

            for (int i = lxConnections.Count-1; i >= 0; i--)
            {
                //If we went low this "frame" we were still high at buttonPresses - 1
                if (lxConnections[i].lowReceived)
                {
                    long lowAt = buttonPresses;
                    requiredButtonPresses.Add(lowAt);
                    Console.WriteLine("Resolved " + lxConnections[i].channelName + " at " + lowAt + " button presses.");
                    lxConnections.RemoveAt(i);
                }
            }
        }

        // Now just to be sure that we don't have any common multiples (turns out i didn't in the end...):
        long endResult = 1;

        for (int i = 0; i < requiredButtonPresses.Count;i++)
        {
            endResult *= requiredButtonPresses[i];
            //if (i > 0) endResult /= NumberUtil.GCD(requiredButtonPresses[i], requiredButtonPresses[i - 1]);
        }

        return endResult;
    }
}
