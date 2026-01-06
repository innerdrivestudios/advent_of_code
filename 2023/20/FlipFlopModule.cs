using System.Resources;

class FlipFlopModule : AbstractModule
{
    public bool state = false;

    public FlipFlopModule(string pModuleSpecification) : base(pModuleSpecification) {}

    protected override void ProcessPulse(bool pHigh, AbstractModule pSender)
    {
        base.ProcessPulse(pHigh, pSender);

        if (pHigh) return;

        //flip
        state = !state;
        QueuePulse(state);
    }

    protected override void ResetInternal()
    {
        base.ResetInternal();
        state = false;
    }
}
