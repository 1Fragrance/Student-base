using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
using System.Xml;

namespace StudentsBase
{
    public partial class SettingsWindow : Window
    {
        MainWindow prevWindow;
        AppSettingsReader reader = new AppSettingsReader();
        List<string> defaultSettings = new List<string>()
        {"Ctrl+N","Ctrl+L","Ctrl+S","Esc","F1","F2","F9"/*,@"\Plugins"*/};

        public SettingsWindow(MainWindow _prevWindow)
        {
            InitializeComponent();
            prevWindow = _prevWindow;

            try
            {
                NewFileBox.Text = (string)reader.GetValue("newFile", typeof(string));
                OpenFileBox.Text = (string)reader.GetValue("openFile", typeof(string));
                SaveFileBox.Text = (string)reader.GetValue("saveFile", typeof(string));
                CloseBox.Text = (string)reader.GetValue("closeWindow", typeof(string));
                PluginsMenuBox.Text = (string)reader.GetValue("pluginsMenu", typeof(string));
                RunPluginsBox.Text = (string)reader.GetValue("runPlugins", typeof(string));
                SettingsBox.Text = (string)reader.GetValue("settings", typeof(string));
                //FolderBox.Text =/* Directory.GetCurrentDirectory() + */  (string)reader.GetValue("pluginPath", typeof(string));
            }
            catch(Exception)
            {
                NewFileBox.Text = "Ctrl+N";
                OpenFileBox.Text = "Ctrl+L";
                SaveFileBox.Text = "Ctrl+S";
                CloseBox.Text = "Esc";
                PluginsMenuBox.Text = "F1";
                RunPluginsBox.Text = "F2";
                SettingsBox.Text = "F9";
                //FolderBox.Text = /*Directory.GetCurrentDirectory() +*/ @"\Plugins";
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            foreach (Control ctl in Field.Children)
            {
                if (ctl.GetType() == typeof(TextBox))
                {
                    foreach(Control ctl2 in Field.Children)
                    {
                        if (ctl2.GetType() == typeof(TextBox))
                        {
                            if (((TextBox)ctl).Text == ((TextBox)ctl2).Text)
                            {
                                if (ctl != ctl2)
                                {
                                    MessageBox.Show("Please, delete similar hotkeys", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(NewFileBox.Text)  && !String.IsNullOrWhiteSpace(OpenFileBox.Text)  && !String.IsNullOrWhiteSpace(SaveFileBox.Text)  &&
                !String.IsNullOrWhiteSpace(CloseBox.Text)  && !String.IsNullOrWhiteSpace(PluginsMenuBox.Text)  &&
                !String.IsNullOrWhiteSpace(RunPluginsBox.Text)  && !String.IsNullOrWhiteSpace(SettingsBox.Text)  /*&& !String.IsNullOrWhiteSpace(FolderBox.Text) */)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    config.AppSettings.Settings["newFile"].Value = NewFileBox.Text;
                    config.AppSettings.Settings["openFile"].Value = OpenFileBox.Text;
                    config.AppSettings.Settings["saveFile"].Value = SaveFileBox.Text;
                    config.AppSettings.Settings["closeWindow"].Value = CloseBox.Text;
                    config.AppSettings.Settings["pluginsMenu"].Value = PluginsMenuBox.Text;
                    config.AppSettings.Settings["runPlugins"].Value = RunPluginsBox.Text;
                    config.AppSettings.Settings["settings"].Value = SettingsBox.Text;
                    //config.AppSettings.Settings["pluginPath"].Value = FolderBox.Text;

                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    MessageBox.Show("Config file changed", "Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Incorrect Config File", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Fill all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DefaultSettings.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            }
        }

        private void DefaultSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["newFile"].Value = defaultSettings[0];
                config.AppSettings.Settings["openFile"].Value = defaultSettings[1];
                config.AppSettings.Settings["saveFile"].Value = defaultSettings[2];
                config.AppSettings.Settings["closeWindow"].Value = defaultSettings[3];
                config.AppSettings.Settings["pluginsMenu"].Value = defaultSettings[5];
                config.AppSettings.Settings["runPlugins"].Value = defaultSettings[4];
                config.AppSettings.Settings["settings"].Value = defaultSettings[6];
                //config.AppSettings.Settings["pluginPath"].Value = defaultSettings[7];

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                NewFileBox.Text = defaultSettings[0];
                OpenFileBox.Text = defaultSettings[1];
                SaveFileBox.Text = defaultSettings[2];
                CloseBox.Text = defaultSettings[3];
                PluginsMenuBox.Text = defaultSettings[5];
                RunPluginsBox.Text = defaultSettings[4];
                SettingsBox.Text = defaultSettings[6];
                //FolderBox.Text = defaultSettings[7];


                MessageBox.Show("Config file restored", "Completed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Incorrect Config File", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewFileBox_KeyUp(object sender, KeyEventArgs e)
        {
            CheckInput(e, NewFileBox);
        }


        private void CheckInput(KeyEventArgs e, TextBox box)
        {
            if (e.Key < (Key)34 || e.Key > (Key)113)
                return;
            if (e.Key > (Key)69 && e.Key < (Key)90)
                return;
            if (Keyboard.Modifiers == ModifierKeys.None || Keyboard.Modifiers == ModifierKeys.Windows)
            {
                if (!(e.Key >= (Key)90 && e.Key <= (Key)113))
                {
                    box.Text = null;
                    return;
                }
                else
                {
                    box.Text = null;
                    box.Text = e.Key.ToString();
                    return;
                }

            }
            string output;
            if (e.Key > (Key)33 && e.Key < (Key)44)
                output = (Convert.ToInt32(e.Key) - 34).ToString();
            else
                output = e.Key.ToString();

            box.Text = null;
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                box.Text = "Ctrl+" + output;
                return;
            }
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                box.Text = "Alt+" + output;
                return;
            }
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                box.Text = "Shift+" + output;
                return;
            }
        }

        private void OpenFileBox_KeyUp(object sender, KeyEventArgs e)
        {
            CheckInput(e, OpenFileBox);
        }

        private void SaveFileBox_KeyUp(object sender, KeyEventArgs e)
        {
            CheckInput(e, SaveFileBox);
        }

        private void CloseBox_KeyUp(object sender, KeyEventArgs e)
        {
            CheckInput(e, CloseBox);
        }

        private void PluginsMenuBox_KeyUp(object sender, KeyEventArgs e)
        {
            CheckInput(e, PluginsMenuBox);
        }

        private void RunPluginsBox_KeyUp(object sender, KeyEventArgs e)
        {
            CheckInput(e, RunPluginsBox);
        }

        private void SettingsBox_KeyUp(object sender, KeyEventArgs e)
        {
            CheckInput(e, SettingsBox);
        }
    }
}
