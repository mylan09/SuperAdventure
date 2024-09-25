using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperAdventure
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            StartScreen startScreen = new StartScreen();
            startScreen.StartPosition = FormStartPosition.CenterScreen;
            
            Application.Run(startScreen);
            
        }
    }
}
