using System;

namespace wUAL.WPF.Models.ProgressBar
{
    public class ProgressBarDataTemplateSelector 
        : ProgressBarModels.DataTemplateSelector<ProgressBarModels>
    {
        protected override ProgressBarModels SelectorModels
            => ProgressBarModels.Current;
    }
}