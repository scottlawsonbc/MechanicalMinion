using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MechanicalMinion;

namespace MechanicalMinion_GUI
{
    public partial class MainWindow : Window
    {
        string directory = Properties.Settings.Default.defaultDirectory;

        public MainWindow()
        {
            InitializeComponent();
            changeDirectory.Click += (o, e) => ChangeDirectory();
            fileList.SelectionChanged += (o, e) => UpdateButton(o, e);
            
            UpdateLatestFiles(directory);
        }

        private void UpdateLatestFiles(string directory)
        {
            FileInfo[] files = Retrieval.GetAllFiles(directory);
            var latestFiles = (from f in files
                               orderby f.LastWriteTime descending
                               select f).ToArray();
            int length = (latestFiles.Length >= 5) ? 5 : latestFiles.Length;
            fileList.Items.Clear();
            for (int i = 0; i < length; i++)
                fileList.Items.Add(latestFiles[i]);
            if (fileList.HasItems)
                fileList.SelectedItem = fileList.Items[0];
        }

        private void ChangeDirectory()
        {
            var folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowser.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            directory = folderBrowser.SelectedPath;
            Properties.Settings.Default.defaultDirectory = directory;
            Properties.Settings.Default.Save();
            UpdateLatestFiles(directory);
        }

        private void UpdateButton(object sender, EventArgs e)
        {
            switch (((FileInfo)fileList.SelectedItem).Extension.ToLower())
            {
                // Extractable
                case (".7z"):
                case (".7zip"):
                case (".zip"):
                case (".rar"):
                    opt1.Content = "Extract";
                    opt2.Content = "Open";
                    opt3.Content = "Delete";
                    break;
                // Other
                default:
                    opt1.Content = "Open";
                    opt2.Content = "Put away";
                    opt3.Content = "Delete";
                    break;
            }
        }

    }
}
