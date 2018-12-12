using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace ConversionApp
{
    /// <summary>
    /// Interaction logic for ConsoleOutput.xaml
    /// </summary>
    public partial class ConsoleOutput : Window
    {
        public ConsoleOutput(String cmdArgs)
        {
            InitializeComponent();

            Process convert = new Process();
            convert.StartInfo.FileName = "Resources\\AccessConversion.exe";
            convert.StartInfo.Arguments = cmdArgs;

            convert.StartInfo.RedirectStandardError = true;
            convert.StartInfo.RedirectStandardOutput = true;
            convert.StartInfo.UseShellExecute = false;
            convert.StartInfo.CreateNoWindow = true;

            convert.OutputDataReceived += DataHandler;
            convert.ErrorDataReceived += DataHandler;
            
            convert.EnableRaisingEvents = true;
            convert.Exited += LastThing;

            try
            {
                convert.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Conversion process failed to start: " + ex.Message);
                throw new OperationCanceledException();
            }

            ConsoleBox.Text += ("Starting conversion... " + Environment.NewLine);

            convert.BeginOutputReadLine();
            convert.BeginErrorReadLine();
        }

        private void DataHandler(Object sender, DataReceivedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                object[] args = { sender, e };
                Dispatcher.Invoke(new DataReceivedEventHandler(DataHandler), args);
            }
            else if (!string.IsNullOrEmpty(e.Data))
            {
                
                ConsoleBox.Text += (" > " + e.Data + Environment.NewLine);
                ConsoleBox.Focus();
                ConsoleBox.CaretIndex = ConsoleBox.Text.Length;
                ConsoleBox.ScrollToEnd();
            }
        }

        private void LastThing(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                object[] args = { sender, e };
                Dispatcher.Invoke(new EventHandler(LastThing), args);
            }
            else
            {
                ConsoleBox.Text += "Conversion complete!" + Environment.NewLine;
                ConsoleBox.Focus();
                ConsoleBox.CaretIndex = ConsoleBox.Text.Length;
                ConsoleBox.ScrollToEnd();
                DoneButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog browser = new SaveFileDialog
            {
                ValidateNames = true,
                AddExtension = true,
                CheckPathExists = true,
                OverwritePrompt = true,
                FileName = "output.log",

                DefaultExt = ".log",
                Filter = "Console Log (.log)|*.log"
            };
            DialogResult result = browser.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    System.IO.File.WriteAllText(browser.FileName, ConsoleBox.Text);
                }
                catch (UnauthorizedAccessException ex)
                {
                    System.Windows.MessageBox.Show("Could not write to file: " + ex.Message);
                }
            }
        }
    }
}