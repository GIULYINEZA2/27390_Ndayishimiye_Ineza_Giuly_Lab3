using System;
using System.Windows.Forms;
using LabTestSystem.Views;

namespace LabTestSystem;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}