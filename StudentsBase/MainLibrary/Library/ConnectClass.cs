using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Library
{
    public class ConnectClass
    {
        public static void ConnectWProgram()
        {
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Plugins\"))
            {
                string assemblyName = file.Remove(0, file.LastIndexOf(@"\") + 1);
                assemblyName = assemblyName.Remove(assemblyName.Length - 4, 4);
                if (assemblyName != "Library")
                {
                    var pluginAssembly = Assembly.Load(assemblyName);

                    try
                   {
                        var pluginType = pluginAssembly.GetTypes().Where(t => typeof(MyPlugin).IsAssignableFrom(t)).Single();
                        MyPlugin plugin = (MyPlugin)pluginType.GetConstructor(new Type[0]).Invoke(null);
                        plugin.startPlugin();
                    }
                    catch
                   {
                        MessageBox.Show(assemblyName + " do not realize interface");
                   }
                }
            }
        }
    }
}
