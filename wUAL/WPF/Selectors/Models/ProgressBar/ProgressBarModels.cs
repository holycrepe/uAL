using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using AddGenericConstraint;
using Torrent;
using uAL.Extensions;
using wUAL.UserControls;
using wUAL.WPF.Selectors.Models.Base;

namespace wUAL.WPF.Selectors.Models.ProgressBar
{
    using static ProgressBar;
    using static ProgressBar.States;

    [ContentProperty(nameof(Models))]
    public class ProgressBarModels : SelectorModelsBase<UTorrentJob, ProgressBarModel, States, Element, Template>
    {
        private static ProgressBarModels _model;
        public override IEnumerable<States> GetMatchingStates(UTorrentJob item)
        {
            if (item.Torrent.IsActive)
                yield return Active;
            if (item.Torrent.IsRunning && item.Torrent.PercentComplete > 0)
                yield return Resuming;
            if (item.Torrent.IsRunning)
                yield return Pending;
            if (item.Torrent.PercentComplete > 0)
                yield return Partial;
            yield return Default;
        }

        #region Overrides of SelectorModelsBase<UTorrentJob,ProgressBarModel,States,Elements,Template>

        public override void MakeStyle(States state, Dictionary<Element, Style> styles)
        {
            var model = Models.Get(state);

            var element = Element.ProgressBar;
            styles[element] = CreateStyle(element)
                .AddForeground(model.Foreground);

            element = Element.Label;
            styles[element] = CreateStyle(element)
            .AddSetter(OutlinedTextBlock.FillProperty, model.Fill)
            .AddSetter(OutlinedTextBlock.StrokeProperty, model.StrokeGradient);

            if (model.Label == null)
                model.Label = state + "...";
        }

        #endregion

        public static ProgressBarModels Model
            => _model ?? (_model = Load<ProgressBarModels>());

    }
}