using Inspector.Services.Interfaces;
using Microsoft.Win32;
using System.Windows;

namespace Inspector.Services
{
    internal class DialogService : IDialogService
    {
        public string FileName { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog _openFileDialog = new OpenFileDialog();
            if(_openFileDialog.ShowDialog() == true) 
            { 
                FileName = _openFileDialog.FileName; 
                return true; 
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                FileName = saveFileDialog.FileName;
                return true;
            }

            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Уведомление" ,MessageBoxButton.OK);
        }
    }
}
