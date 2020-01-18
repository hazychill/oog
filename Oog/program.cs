using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Oog {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
          AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
            MessageBox.Show(e.ExceptionObject.ToString());
          };
          try {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
          }
          catch (Exception e) {
            MessageBox.Show(e.ToString());
          }
        }
    }
}