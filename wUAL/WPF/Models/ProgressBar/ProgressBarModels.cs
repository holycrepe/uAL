namespace wUAL.WPF.Models.ProgressBar
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Markup;
    using Torrent;
    using uAL.Extensions;
    using UserControls;
    using WPF.Models.Base;

    [ContentProperty(nameof(Models))]
    public class ProgressBarModels : SelectorModelsBase<UTorrentJob, ProgressBarModel, ProgressBar.States, ProgressBar.Element, ProgressBar.Template>
    {
        private static ProgressBarModels _model;
        public override IEnumerable<ProgressBar.States> GetMatchingStates(UTorrentJob item)
        {
            if (item != null)
            {
                if (item.Torrent.IsActive)
                    yield return ProgressBar.States.Active;
                if (item.Torrent.IsRunning && item.Torrent.PercentComplete > 0)
                    yield return ProgressBar.States.Resuming;
                if (item.Torrent.IsRunning)
                    yield return ProgressBar.States.Pending;
                if (item.Torrent.PercentComplete > 0)
                    yield return ProgressBar.States.Partial;
            }
            yield return ProgressBar.States.Default;
        }

        #region Overrides of SelectorModelsBase<UTorrentJob,ProgressBarModel,States,Elements,Template>

        public override void MakeStyle(ProgressBar.States state, Dictionary<ProgressBar.Element, Style> styles)
        {
            var model = this.Models.Get(state);

            var element = ProgressBar.Element.ProgressBar;
            styles[element] = CreateStyle(element)
                .AddForeground(model.Foreground);

            element = ProgressBar.Element.Label;
            styles[element] = CreateStyle(element)
            .AddSetter(MyProgressBar.FillProperty, model.Fill)
            .AddSetter(MyProgressBar.StrokeProperty, model.StrokeGradient);

            if (model.Label == null)
                model.Label = state + "...";
        }

        #endregion
        [DebuggerNonUserCode]
        public static ProgressBarModels Current
            => _model ?? (_model = Load<ProgressBarModels>());

    }
}