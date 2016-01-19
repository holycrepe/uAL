/*
 * Created by SharpDevelop.
 * User: Avi
 * Date: 1/10/2016
 * Time: 10:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace wUAL.UserControls
{
    /// <summary>
    /// Interaction logic for QueueGridHeaderControl.xaml
    /// </summary>
    public partial class QueueGridHeaderControl : UserControl
	{

        #region Initialization
        public QueueGridHeaderControl() : this(null)
        {
            
        }

        public QueueGridHeaderControl(string label, string iconPath = null, ImageSource icon = null) 
        {
            InitializeComponent();
            SetLabel(label, iconPath, icon);
        }
        void SetLabel(string label, string iconPath = null, ImageSource icon = null)
        {
            if (label == null && iconPath == null && icon == null) { 
                return;                
            }
            if (label != null) {
                SetValue(QueueLabelProperty, label);
            }
            SetIconFromPath(iconPath, icon);
        }

        void SetIconFromPath(string iconPath = null, ImageSource icon = null)
        {
            if (QueueIcon != null)
            {
                return;
            }
            icon = icon ?? GetIconFromPath(iconPath ?? QueueLabel);
            if (icon != null)
            {
                QueueIcon = icon;
            }
        }

        ImageSource GetIconFromPath(string iconPath)
        {
            iconPath = iconPath.Replace(" ", "");
            var iconResource = Application.Current.TryFindResource(iconPath + "Icon");
            if (iconResource == null)
            {
                iconResource = Application.Current.TryFindResource(iconPath);
            }
            if (iconResource != null)
            {
                return (ImageSource)iconResource;
            }
            return null;
        }
        #endregion

        #region QueueIconName Dependency Property

        /// <summary>
        /// Gets or sets the QueueIconName which is displayed next to the field
        /// </summary>
        public string QueueIconName
        {
            get { return (string)GetValue(QueueIconNameProperty); }
            set {
                SetIconFromPath(value);
                SetValue(QueueIconNameProperty, value);
            }
        }

        /// <summary>
        /// Identified the QueueIconName dependency property
        /// </summary>
        public static readonly DependencyProperty QueueIconNameProperty =
            DependencyProperty.Register("QueueIconName", typeof(string),
              typeof(QueueGridHeaderControl), new PropertyMetadata());

        #endregion

        #region QueueIcon Dependency Property

        ImageSource _queueIcon;

        /// <summary>
        /// Gets or sets the QueueIcon which is displayed next to the field
        /// </summary>
        public ImageSource QueueIcon
        {
            get {
                var value = (ImageSource)GetValue(QueueIconProperty);
                if (value != null)
                {
                    return value;
                }
                if (_queueIcon != null)
                {
                    return _queueIcon;
                }
                if (QueueIconName != null)
                {
                    return _queueIcon = GetIconFromPath(QueueIconName);
                }
                return null;
            }
            set {
                SetValue(QueueIconProperty, value);
            }
        }

        /// <summary>
        /// Identified the QueueIcon dependency property
        /// </summary>
        public static readonly DependencyProperty QueueIconProperty =
            DependencyProperty.Register("QueueIcon", typeof(ImageSource),
              typeof(QueueGridHeaderControl), new PropertyMetadata());

        #endregion

        #region QueueLabel Dependency Property

        /// <summary>
        /// Gets or sets the QueueLabel which is displayed next to the field
        /// </summary>
        public string QueueLabel
        {
            get { return (string)GetValue(QueueLabelProperty); }
            set {
                SetIconFromPath();
                SetValue(QueueLabelProperty, value);
            }
        }

        /// <summary>
        /// Identified the QueueLabel dependency property
        /// </summary>
        public static readonly DependencyProperty QueueLabelProperty =
            DependencyProperty.Register("QueueLabel", typeof(string),
              typeof(QueueGridHeaderControl), new PropertyMetadata());

        #endregion

    }
}