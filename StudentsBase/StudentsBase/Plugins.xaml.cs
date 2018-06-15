using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Library;
using System.IO;
using System.Configuration;

namespace StudentsBase
{
    public partial class Plugins : Window
    {
        MainWindow prevWindow;
        List<Plugin> myPlugins = new List<Plugin>();
        public delegate void LoadPluginsHandler();

        AppDomain pluginDomain;
        AppDomainSetup setup;

        AppSettingsReader reader = new AppSettingsReader();
        string readConfig;

        public Plugins(MainWindow _prevWindow)
        {
            InitializeComponent();
            prevWindow = _prevWindow;
            try
            {
                readConfig = (string)reader.GetValue("pluginPath", typeof(string));

                string[] files = System.IO.Directory.GetFiles(Directory.GetCurrentDirectory() + readConfig /*@"\Plugins"*/);
                for (int x = 0; x < files.Length; x++)
                {
                    string dllName = files[x].Remove(0, files[x].LastIndexOf(@"\") + 1);
                    dllName = dllName.Remove(dllName.Length - 4, 4);
                    if (dllName != "Library")
                        myPlugins.Add(new Plugin(dllName, files[x]));
                }

                PluginList.ItemsSource = myPlugins;
                PluginList.Items.Refresh();
            }
            catch(Exception)
            {
                MessageBox.Show("Can't Load Plugins", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void JustRun()
        {
            try
            {
                LoadPluginsHandler AsyncPlugin = new LoadPluginsHandler(LoadPlugins);
                Dispatcher.BeginInvoke(AsyncPlugin);
            }
            catch(Exception)
            {
                MessageBox.Show("Can't Load Plugins", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadPluginsHandler AsyncPlugin = new LoadPluginsHandler(LoadPlugins);
                Dispatcher.BeginInvoke(AsyncPlugin);
            }
            catch(Exception)
            {
                MessageBox.Show("Can't Load Plugins", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadPlugins()
        {
            setup = new AppDomainSetup();
            string path = @Directory.GetCurrentDirectory() + @"\Plugins";
            setup.ApplicationBase = path;
            pluginDomain = AppDomain.CreateDomain("plugin", null, setup);
            try
            {
                pluginDomain.DoCallBack(ConnectClass.ConnectWProgram);
            }
            catch
            {
                MessageBox.Show("One of your plugins have an error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddPlugin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "*.dll | *.dll;";
                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    string dllName = filename.Remove(0, filename.LastIndexOf(@"\") + 1);

                    File.Copy(filename, Directory.GetCurrentDirectory() + @"\Plugins\" + dllName, true);
                    dllName = dllName.Remove(dllName.Length - 4, 4);
                    filename = Directory.GetCurrentDirectory() + @"\Plugins";

                    bool flag = false;
                    for (int i = 0; i < myPlugins.Count(); i++)
                    {
                        if (myPlugins[i].Name == dllName)
                        {
                            myPlugins[i] = new Plugin(dllName, filename);
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        PluginList.ItemsSource = myPlugins;
                        PluginList.Items.Refresh();
                        return;
                    }
                    else
                        myPlugins.Add(new Plugin(dllName, filename));
                }
            }
            catch
            {
                MessageBox.Show("Can't move file", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            PluginList.ItemsSource = myPlugins;
            PluginList.Items.Refresh();
        }

        private void DeletePlugin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pluginDomain != null)
                {
                    AppDomain.Unload(pluginDomain);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
            catch
            { }
            Plugin item = (Plugin)PluginList.SelectedItem;
            if (item != null && item.Path != null)
            {
                try
                {
                    File.Delete(item.Path);
                }
                catch
                {
                    MessageBox.Show("Please, reload your program to delete this plugin ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            myPlugins.Remove(item);
            PluginList.Items.Refresh();

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }

    public class Plugin
    {
        public Plugin(string name,string path)
        {
            Name = name;
            Path = path;
        }
        public string Path { get; set; }
        public string Name { get; set; }
    }
}
