using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Xml.Serialization;
using static Torrent.Helpers.Utils.DebugUtils;
namespace wUAL.Properties.Settings.AppSettings
{
    using static AppSettings;
    using static AllSettings.AllSettings;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Persists a Window's Size, Location and WindowState to UserScopeSettings 
    /// </summary>
    public partial class WindowSettings
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
        public static List<WindowSettings> Instances { get; } = new List<WindowSettings>();        

        private Window _window;
        public string Key 
            => _window.GetType().FullName;

        public WindowSettings(Window window)
        {
            _window = window;
        }

        /// <summary>
        /// Register the "Save" attached property and the "OnSaveInvalidated" callback 
        /// </summary>
        public static readonly DependencyProperty SaveProperty
            = DependencyProperty.RegisterAttached("Save", typeof(bool), typeof(WindowSettings),
                                                  new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSaveInvalidated)));

        public static void SetSave(DependencyObject dependencyObject, bool enabled)
            => dependencyObject.SetValue(SaveProperty, enabled);

        /// <summary>
        /// Called when Save is changed on an object.
        /// </summary>
        private static void OnSaveInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var window = dependencyObject as Window;
            if (window == null || !((bool)e.NewValue)) return;
            var settings = new WindowSettings(window);
            settings.Attach();
            Instances.Add(settings);
        }

        /// <summary>
        /// Load the Window Size Location and State from the settings object
        /// </summary>
        public virtual void LoadWindowState()
        {
            LoadWindowPlacement();
            WINDOWPLACEMENT? placement = null;
            if (AppSetting?.Placements?.TryGetValue(Key, out placement) != true || placement == null)
            {
                return;
            }

            try
            {
                // Load window placement details for previous application session from application settings
                // if window was closed on a monitor that is now disconnected from the computer,
                // SetWindowPlacement will place the window onto a visible monitor.
                var wp = placement.Value;

                wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                wp.flags = 0;
                wp.UnminimizeState();
                var hwnd = new WindowInteropHelper(_window).Handle;
                SetWindowPlacement(hwnd, ref wp);
            }
            catch (Exception ex)
            {
                DEBUG.WriteLine($"Failed to load window state:\r\n{ex}");
            }
        }

        /// <summary>
        /// Save the Window Size, Location and State to the settings object
        /// </summary>
        public virtual void SaveWindowState()
        {
            WINDOWPLACEMENT wp;
            var hwnd = new WindowInteropHelper(_window).Handle;
            GetWindowPlacement(hwnd, out wp);
            if (AppSetting?.Placements != null)
            {
                AppSetting.Placements[Key] = wp;
            }
        }

        protected virtual void Save()
            => SaveAllSettings();

        private void Attach()
        {
            if (_window == null) return;
            _window.Closing += WindowClosing;
            _window.SourceInitialized += WindowSourceInitialized;
        }

        void WindowSourceInitialized(object sender, EventArgs e)
            => LoadWindowState();

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            Save();
            _window.Closing -= WindowClosing;
            _window.SourceInitialized -= WindowSourceInitialized;
            _window = null;
        }
    }
    
}