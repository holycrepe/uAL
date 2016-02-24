namespace wUAL.WPF.Selectors.Models.ProgressBar2
{
    public enum ProgressStates
    {
        Default,
        Active,
        Resuming,
        Pending,
        Partial
    }
    public class ProgressBarStates<T>
    {
        public virtual T Default { get; set; }
        public virtual T Active { get; set; }
        public virtual T Resuming { get; set; }
        public virtual T Pending { get; set; }
        public virtual T Partial { get; set; }
    }
}