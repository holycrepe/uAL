using System.ComponentModel;

namespace wUAL.UserControls.CheckedComboBox
{
    public interface ICheckedComboBoxDataItem<T> : INotifyPropertyChanged
    {
        bool IsSelected { get; set; }
        string Text { get; set; }
        string Value { get; set; }
    }
}