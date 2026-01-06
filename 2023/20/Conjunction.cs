class Conjunction : AbstractModule
{
    private Dictionary<AbstractModule, bool> inputToLastPulse = new();

    public Conjunction(string pModuleSpecification) : base(pModuleSpecification) {}

    public bool queuePulse = false;

    protected override void ProcessPulse(bool pHigh, AbstractModule pSender)
    {
        base.ProcessPulse(pHigh, pSender);

        inputToLastPulse[pSender] = pHigh;

        int trueCount = inputToLastPulse.Count(x => x.Value);
        bool allHigh = trueCount == connectedInputModules.Count;
        QueuePulse(!allHigh);

        queuePulse = !allHigh;
    }

    protected override void ResetInternal()
    {
        base.ResetInternal();
        inputToLastPulse.Clear();
    }
}
