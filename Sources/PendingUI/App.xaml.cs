#region

using System;
using System.Windows;

#endregion

namespace Waveface
{
    public partial class App : Application
    {
        public static String[] Args;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Args = e.Args;

            if(Args.Length > 0)
            {
                
            }
            else
            {
                Shutdown();
            }
        }
    }
}