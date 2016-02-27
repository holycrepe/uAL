namespace wUAL.WPF.Models.ProgressBar
{
    public class ProgressBar
    {
        public enum States
        {
            Default,
            Active,
            Resuming,
            Pending,
            Partial
        }

        public enum Element
        {
            ProgressBar,
            Label
        }

        public enum Template
        {
            Default,
            NotRunning,
            Progress,
            PercentComplete
        }
    }
}