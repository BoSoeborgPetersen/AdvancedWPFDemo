using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _02350AdvancedDemo.ViewModel
{
    public class DialogViews
    {
        private static OpenFileDialog openDialog = new OpenFileDialog() { Title = "Open Diagram", Filter = "XML Document (.xml)|*.xml", DefaultExt = "xml", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CheckFileExists = true };
        private static SaveFileDialog saveDialog = new SaveFileDialog() { Title = "Save Diagram", Filter = "XML Document (.xml)|*.xml", DefaultExt = "xml", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };

        public bool ShowNew() => 
            MessageBox.Show("Are you sure (bla bla)?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

        public string ShowOpen() => openDialog.ShowDialog() == true ? openDialog.FileName : null;

        public string ShowSave() => saveDialog.ShowDialog() == true ? saveDialog.FileName : null;
    }
}
