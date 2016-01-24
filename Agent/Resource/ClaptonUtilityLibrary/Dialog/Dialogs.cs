using System.Windows.Forms;

namespace Clapton.Dialog
{
    public static class Dialogs
    {
        public static bool ConfirmationDialog(string text, string title, ConfirmDialogDefaultButton defaultButton)
        {
            return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, (MessageBoxDefaultButton)defaultButton) == DialogResult.Yes ? true : false;
        }
        public static void WarningDialog(string text) { WarningDialog(text, "Warning!"); }
        public static void WarningDialog(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public static void ErrorDialog(string text) { ErrorDialog(text, "Error"); }
        public static void ErrorDialog(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }

    public enum ConfirmDialogDefaultButton
    {
        Yes = 0,
        No = 256,
    }
}
