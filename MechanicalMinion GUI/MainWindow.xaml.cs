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
      
            // Event handlers
            changeDirectory.Click += (o, e) => ChangeDirectory();
            fileList.SelectionChanged += SelectionChangedEvent;
            this.Loaded += (o, e) => fileList.Focus();

            // Prepare the buttons
            opt1.Click += (o, e) => ExecuteOpt1();
            opt2.Click += (o, e) => ExecuteOpt2();
            opt3.Click += (o, e) => ExecuteOpt3();

            // Set colours
            opt1.Background = Brushes.AliceBlue;
            opt2.Background = Brushes.AliceBlue;
            opt3.Background = Brushes.AliceBlue;

            // Focus handlers
            opt1.GotFocus += (o, e) => opt1.Background = Brushes.LightSkyBlue;
            opt2.GotFocus += (o, e) => opt2.Background = Brushes.LightSkyBlue;
            opt3.GotFocus += (o, e) => opt3.Background = Brushes.LightSkyBlue;
            opt1.LostFocus += (o, e) => opt1.Background = Brushes.AliceBlue;
            opt2.LostFocus += (o, e) => opt2.Background = Brushes.AliceBlue; 
            opt3.LostFocus += (o, e) => opt3.Background = Brushes.AliceBlue;                
            
            // Populate the listview automatically
            UpdateLatestFiles(directory);
        }

        // Fired when fileList selection is changed
        private void SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtons();
        }

        /// Populates the file list with the most recently modified files
        private void UpdateLatestFiles(string directory)
        {
            // Retrieve the most recently modified files
            FileInfo[] files = Retrieval.GetAllFiles(directory);
            var latestFiles = (from f in files
                               orderby f.LastWriteTime descending
                               select f).ToArray();
           
            fileList.SelectionChanged -= SelectionChangedEvent; // Prevent event notification for null items.
            fileList.Items.Clear();
            fileList.SelectionChanged += SelectionChangedEvent; // Reattach the event handler

            // Add the retrieved files to the fileList
            int length = (latestFiles.Length >= 5) ? 5 : latestFiles.Length; // Don't add more than five files
            for (int i = 0; i < length; i++)
                fileList.Items.Add(latestFiles[i]);
            if (fileList.HasItems)
                fileList.SelectedItem = fileList.Items[0];
        }

        /// Opens a filebrowserdialog prompting the user to select a new default directory.
        private void ChangeDirectory()
        {
            // Open a browser dialog and check to see if the user selected a new folder
            var folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowser.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            // Save the new folder to settings
            directory = folderBrowser.SelectedPath;
            Properties.Settings.Default.defaultDirectory = directory;
            Properties.Settings.Default.Save();
            UpdateLatestFiles(directory);
        }

        // Updates the content on the buttons depending on the selected file
        private void UpdateButtons()
        {
            if (IsExtractable((FileInfo)fileList.SelectedItem))
            {
                opt1.Content = "Extract";
                opt2.Content = "Put away";
                opt3.Content = "Delete";
            }
            else
            {
                opt1.Content = "Open";
                opt2.Content = "Put away";
                opt3.Content = "Delete";
            }
        }

        // Returns a bool indicating whether the file extension of the given file is extractable.
        private bool IsExtractable(FileInfo file)
        {
            switch (file.Extension.ToLower())
            {
                // Extractable
                case (".7z"):
                case (".7zip"):
                case (".zip"):
                case (".rar"):
                    return true;

                // Other
                default:
                    return false;
            }
        }

        // Extract or Open
        private void ExecuteOpt1()
        {
            if (!fileList.HasItems || fileList.SelectedItem == null)
            {
                MessageBox.Show("Please select an item.", "Error", MessageBoxButton.OK);
                return;
            }

            var file = (FileInfo)fileList.SelectedItem;

            if (IsExtractable(file)) // Extract compressed files
            {
                try
                {
                    string extractDirectory = file.FullName.Remove(file.FullName.Length - 4);
                    Actions.Extract(file.FullName, extractDirectory, true);
                    Actions.Open(extractDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK);
                }
            }
            else // Open other programs
            {
                try { Actions.Open(file.FullName); }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK); }
            }
           
            UpdateLatestFiles(directory);
            UpdateButtons();
        }
        
        // Putaway
        private void ExecuteOpt2()
        {

        }

        // Delete
        private void ExecuteOpt3()
        {
            if (!fileList.HasItems || fileList.SelectedItem == null)
            {
                MessageBox.Show("Please select an item.", "Error", MessageBoxButton.OK);
                return;
            }

            try
            {
                var file = (FileInfo)fileList.SelectedItem;
                string fileName = file.FullName;
                fileList.Items.Remove(file);
                Actions.Delete(fileName);
                while (File.Exists(fileName)) // Wait for file to be deleted
                    System.Threading.Thread.Sleep(20);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK);
            }
            UpdateLatestFiles(directory);
            UpdateButtons();
        }

        /// Focus to buttons when user presses the enter key
        private void fileListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            opt1.Focus();
            e.Handled = true;
        }
    }
}
