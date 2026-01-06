abstract class AbstractModule
{
    public static bool debug = false;

    // Simply static registry for modules to access other modules,
    // deferring the need to know a module at construction time...
    protected static Dictionary<string, AbstractModule> moduleRegistry = new();
    
    // Variables to keep track of low and high pulse count during a "run"
    protected static long lowPulseCount = 0;
    protected static long highPulseCount = 0;

    // Pulses need to be processed in the order in which they are queue,
    // so we queue both a pulse and the module "owning" the pulse,
    // meaning if a module receives 2 pulses, it will have a pulse queue of size 2
    // and it will be in the module queue twice, since these 2 pulses might be to be processed
    // at a different moment in time.
    protected static readonly Queue<AbstractModule> modulesToProcess = new();
    
    // Info about this specific module, we start setting up the connected output module names and our own channel name
    private List<string> connectedOutputModuleNames = new ();
    public readonly string channelName;

    // So we can resolve links to our actual input and output module instances during initialization
    // Little bit dirty to make this public but I got to finish this sometime...
    public readonly List<AbstractModule> connectedOutputModules = new ();
    public readonly List<AbstractModule> connectedInputModules = new ();

    // The queue pulse for this module
    protected Queue<bool> pulseQueue = new();

    // Do we still need to trigger the initialization of our output modules?
    private bool initializedOutputModules = false;

    public bool lowReceived { get; private set; } = false;
    public bool highReceived { get; private set; } = false;

    public AbstractModule (string pModuleSpecification)
    {
        string[] moduleSpecParts = pModuleSpecification.Splat(["->", ","]);
        channelName = moduleSpecParts[0];
        connectedOutputModuleNames = moduleSpecParts.Skip(1).ToList ();

        // Register ourselves
        moduleRegistry.Add (channelName, this);

        Debug("Creating a(n) " + this + " named: " + channelName + " with connections to " + string.Join(",", connectedOutputModuleNames));
    }

    public void Initialize(AbstractModule pParent)
    {
        // Check if we still need to recursively initiaze
        if (!initializedOutputModules)
        {
            initializedOutputModules = true;

            foreach (string moduleName in connectedOutputModuleNames)
            {
                // If a module doesn't appear on the left side in the schematic no module
                // will be created for it, so we'll fix this here, with a 'sink' module
                // that doesn't lead anywhere...
                if (!moduleRegistry.ContainsKey(moduleName))
                {
                    moduleRegistry[moduleName] = new BroadcasterModule(moduleName);
                }

                Debug("Connected " + channelName + " to output module " + moduleName);
                connectedOutputModules.Add(moduleRegistry[moduleName]);
                moduleRegistry[moduleName].Initialize(this);
            }

            Debug("Connected " + channelName + " to " + connectedOutputModules.Count + " output modules.");
        }

        connectedInputModules.Add(pParent);
        Debug("Connected " + channelName + " to input module " + pParent?.channelName);
        Debug("Connected " + channelName + " to " + connectedOutputModules.Count + " input modules.");
    }

    // Tells module to accept a pulse and process it...
    public virtual void ReceivePulse (bool pHigh, AbstractModule pSender)
    {
        if (!initializedOutputModules) throw new Exception("Please initialize module before sending pulses");
        Debug (channelName + " received pulse " + (pHigh ? "high" : "low"));

        if (pHigh) highReceived = true;
        else lowReceived = true;

        // By default we process it (count it) and leave the rest up to the subclass
        ProcessPulse (pHigh, pSender);
    }

    protected virtual void ProcessPulse(bool pHigh, AbstractModule pSender)
    {
        if (pHigh) highPulseCount++; else lowPulseCount++;
    }

    // Helper method to queue a pulse for processing, called from subclasses
    protected void QueuePulse (bool pHigh)
    {
        pulseQueue.Enqueue(pHigh);
        modulesToProcess.Enqueue(this);
    }

    // Helper method to process all modules until done
    // Only accessible through our root broadcaster module
    protected bool ProcessModules()
    {
        if (modulesToProcess.Count == 0) return true;
        modulesToProcess.Dequeue().PropagatePulse();
        return false;
    }

    // Called when a module is processed we pass on the queued pulse to our outputs..
    protected void PropagatePulse()
    {
        if (pulseQueue.Count == 0) return;

        bool pulse = pulseQueue.Dequeue ();

        foreach (AbstractModule module in connectedOutputModules) {
            Debug(channelName + " -" + (pulse ? "high" : "low") + "-> " + module.channelName);
            module.ReceivePulse(pulse, this);
        }
    }

    // Used this to reverse engineer stuff for part two...
    protected void DebugFlipFlops()
    {
        foreach (AbstractModule module in moduleRegistry.Values)
        {
            if (module is FlipFlopModule ff)
            Console.WriteLine(module.channelName+" "+ff.state);
        }
    }

    protected static void Debug(string pMessage) {
        if (!debug) return;
        Console.WriteLine(pMessage);
    }

    protected void Reset()
    {
        foreach (AbstractModule module in moduleRegistry.Values)
        {
            module.ResetInternal();
        }
    }

    protected virtual void ResetInternal() { }



}
