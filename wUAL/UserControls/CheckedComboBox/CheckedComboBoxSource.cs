using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace wUAL.UserControls.CheckedComboBox
{
    public class CheckedComboBoxSource<T> : ObservableCollection<ICheckedComboBoxDataItem<T>>
        
    {

        private static int indexBoolStat = 0;
        private string text;
        private List<string> value;


        /// <summary>   
        /// Gets or sets SelectedItemsText.   
        /// </summary>   
        /// 
        public CheckedComboBoxSource()
        {
            this.indexBool = indexBoolStat;
            indexBoolStat++;

        }

        private int indexBool;
        public int IndexBool
        {
            get
            {
                return this.indexBool;
            }
            set
            {
                if (this.indexBool != value)
                {
                    this.indexBool = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("indexBool"));
                }
            }
        }


        public string SelectedItemsText
        {
            get
            {
                return this.text;
            }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItemsText"));
                }
            }
        }


        public List<string> SelectedItemsValue
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItemsValue"));
                }
            }
        }



        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ICheckedComboBoxDataItem<T> item in e.NewItems)
                {
                    item.PropertyChanged += new PropertyChangedEventHandler(OnItemPropertyChanged);
                }
            }
            if (e.OldItems != null)
            {
                foreach (ICheckedComboBoxDataItem<T> item in e.OldItems)
                {
                    item.PropertyChanged -= new PropertyChangedEventHandler(OnItemPropertyChanged);
                }
            }
            base.OnCollectionChanged(e);
            this.UpdateSelectedText();
        }

        public void UpdateSelectedText()
        {
            int itemsCount = this.Items.Count;
            StringBuilder sb = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();
            this.SelectedItemsValue = new List<string>();

            for (int i = 1; i < itemsCount; i++)
            {
                var item = this.Items[i];
                if (item.IsSelected)
                {
                    sb.AppendFormat("{0}, ", item.Text);
                    SelectedItemsValue.Add(item.Value);
                }
            }

            if (sb.Length > 2)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            this.SelectedItemsText = sb.ToString();


        }


        //function used to update the content of the combobox source
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //this case means the user changed the all item (first item)
            if (e.PropertyName == "IsSelectedAll")
            {
                this.UpdateAll();
            }
            else
                if (e.PropertyName == "IsSelected")
            {
                this.UpdateSelectedText();
                this.checkAll();
            }
        }


        //function used to verify, if all the other items are checked or not
        //if yes the all item(first item) will be set to the checked state
        //otherwise the state will be unchecked
        public void checkAll()
        {

            //in order to avoid the behavior the property changed of the all item (first item),
            // if we change its state
            CheckedComboBoxDataItem<T>.setloadAllNeeded(false, indexBool);
            int itemsCount = this.Items.Count;

            bool allChecked = true;
            for (int i = 1; i < itemsCount && allChecked; i++)
            {
                var item = this.Items[i];
                if (!item.IsSelected)
                {
                    allChecked = false;
                }
            }
            if (allChecked && !this.Items[0].IsSelected) this.Items[0].IsSelected = true;
            else
                if (!allChecked && this.Items[0].IsSelected) this.Items[0].IsSelected = false;

            CheckedComboBoxDataItem<T>.setloadAllNeeded(true, indexBool);
        }



        //function used to check or uncheck all the other item
        //when the user checked or unchecked the all item (first item)
        public void UpdateAll()
        {

            //in order to avoid the behavior the property changed of the other items,
            // if we change its state
            CheckedComboBoxDataItem<T>.setloadAllNeeded(false, indexBool);
            int itemsCount = this.Items.Count;

            if (this.Items[0].IsSelected)
            {
                for (int i = 1; i < itemsCount; i++)
                {
                    var item = this.Items[i];
                    this.Items[i].IsSelected = true;
                }

            }
            else
            {
                for (int i = 1; i < itemsCount; i++)
                {
                    var item = this.Items[i];
                    this.Items[i].IsSelected = false;
                }
            }
            this.UpdateSelectedText();
            CheckedComboBoxDataItem<T>.setloadAllNeeded(true, indexBool);
        }


    }
}
