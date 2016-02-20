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
    public class CheckedComboBoxDataItem<T> : ICheckedComboBoxDataItem<T>
    {
        //bool used to know if we access to every checkbox
        //if it's enabled, we don't process the property changed of IsSelected
        //see setter of IsSelected property
        static private List<bool> loadsAllNeeded = new List<bool>();

        private int indexLoadNeeded;

        //getter
        static public bool getloadAllNeeded(int index)
        {
            return loadsAllNeeded[index];
        }

        //setter
        static public void setloadAllNeeded(bool value, int index)
        {
            loadsAllNeeded[index] = value;
        }

        //constructor
        public CheckedComboBoxDataItem(String val, String txt, bool selected, int index)
        {
            this.value = val;
            this.text = txt;
            this.selected = selected;
            this.indexLoadNeeded = index;

            if (index <= loadsAllNeeded.Count)
            {
                loadsAllNeeded.Add(true);
            }
        }

        //default constructor
        public CheckedComboBoxDataItem()
        {
            this.value = "All";
            this.text = "All";
            this.selected = true;
            this.indexLoadNeeded = 0;
        }

        /// <summary>   
        ///     Called when the value of a property changes.   
        /// </summary>   
        /// <param name="propertyName">The name of the property that has changed.</param>   
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>   
        ///     Raised when the value of one of the properties changes.   
        /// </summary>   
        public event PropertyChangedEventHandler PropertyChanged;





        private string text;
        /// <summary>   
        /// Gets or sets Text.   
        /// </summary>   
        public string Text
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
                    OnPropertyChanged("Text");
                }
            }
        }

        private string value = "";
        /// <summary>   
        /// Gets or sets Text.   
        /// </summary>   
        public string Value
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
                    OnPropertyChanged("Value");
                }
            }
        }

        private bool selected;
        /// <summary>   
        /// Gets or sets IsSelected.   
        /// </summary>   
        public bool IsSelected
        {
            get
            {

                return this.selected;
            }
            set
            {
                if (this.selected != value)
                {
                    this.selected = value;

                    //if we changed the all checked item (first item)
                    if (this.value.Equals("All") && getloadAllNeeded(indexLoadNeeded))
                        OnPropertyChanged("IsSelectedAll");
                    else
                        OnPropertyChanged("IsSelected");
                }
            }
        }


    }    
}
