using System.IO;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Ame.Modules.Windows.Serializer
{
    public static class DockLayoutSerializer
    {
        #region properties

        private static readonly DependencyProperty LoadLayoutCommandProperty =
            DependencyProperty.RegisterAttached("LoadLayoutCommand",
            typeof(ICommand),
            typeof(DockLayoutSerializer),
            new PropertyMetadata(null, OnLoadLayoutCommandChanged));

        private static readonly DependencyProperty SaveLayoutCommandProperty =
            DependencyProperty.RegisterAttached("SaveLayoutCommand",
            typeof(ICommand),
            typeof(DockLayoutSerializer),
            new PropertyMetadata(null, OnSaveLayoutCommandChanged));

        #endregion properties


        #region methods

        #region Load Layout

        public static ICommand GetLoadLayoutCommand(DependencyObject obj)
        {
            return obj.GetValue(LoadLayoutCommandProperty) as ICommand;
        }

        public static void SetLoadLayoutCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadLayoutCommandProperty, value);
        }

        private static void OnLoadLayoutCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement framworkElement = obj as FrameworkElement;
            framworkElement.Loaded -= OnFrameworkElementLoaded;

            var command = e.NewValue as ICommand;
            if (command != null)
            {
                framworkElement.Loaded += OnFrameworkElementLoaded;
            }
        }

        private static void OnFrameworkElementLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }
            ICommand loadLayoutCommand = DockLayoutSerializer.GetLoadLayoutCommand(frameworkElement);
            if (loadLayoutCommand == null)
            {
                return;
            }

            if (loadLayoutCommand is RoutedCommand)
            {
                (loadLayoutCommand as RoutedCommand).Execute(frameworkElement, frameworkElement);
            }
            else
            {
                loadLayoutCommand.Execute(frameworkElement);
            }
        }

        #endregion Load Layout

        #region Save Layout

        public static ICommand GetSaveLayoutCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(SaveLayoutCommandProperty);
        }

        public static void SetSaveLayoutCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SaveLayoutCommandProperty, value);
        }

        private static void OnSaveLayoutCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement framworkElement = d as FrameworkElement;
            framworkElement.Unloaded -= OnFrameworkElementSaved;

            var command = e.NewValue as ICommand;
            if (command != null)
            {
                framworkElement.Unloaded += OnFrameworkElementSaved;
            }
        }

        private static void OnFrameworkElementSaved(object sender, RoutedEventArgs e)
        {
            DockingManager frameworkElement = sender as DockingManager;
            if (frameworkElement == null)
            {
                return;
            }
            ICommand SaveLayoutCommand = GetSaveLayoutCommand(frameworkElement);
            if (SaveLayoutCommand == null)
            {
                return;
            }

            string xmlLayoutString = string.Empty;
            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(frameworkElement);
                xmlLayout.Serialize(fs);
                xmlLayoutString = fs.ToString();
            }
            if (SaveLayoutCommand is RoutedCommand)
            {
                (SaveLayoutCommand as RoutedCommand).Execute(xmlLayoutString, frameworkElement);
            }
            else
            {
                SaveLayoutCommand.Execute(xmlLayoutString);
            }
        }

        #endregion Save Layout

        #endregion methods
    }
}
