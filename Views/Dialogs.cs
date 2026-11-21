using System;
using System.Windows.Forms;
using LabTestSystem.Models;

namespace LabTestSystem.Views;

public static class Dialogs
{
    public static void Info(string message) =>
        MessageBox.Show(message, "Lab Test System", MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static void Warn(string message) =>
        MessageBox.Show(message, "Please check", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    // One place that turns any exception into a friendly message
    public static void Error(Exception ex)
    {
        if (ex is InvalidStateTransitionException)
            MessageBox.Show(ex.Message, "Invalid status change", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        else if (ex is UncertifiedSectionException)
            MessageBox.Show(ex.Message, "Not certified", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        else if (ex is ArgumentException)
            MessageBox.Show(ex.Message, "Please check", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        else
            MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}