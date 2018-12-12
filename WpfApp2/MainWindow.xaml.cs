using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ConversionApp
{

    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String physDump = PhysicalDumpLocationBox.Text;
            String logDump = LogicalDumpLocationBox.Text;
            String userName = UsernameBox.Text;

            String passWord = PasswordBox.Password;
            String apiIP = APIIPBox.Text;
            String instance = InstanceBox.Text;

            String subInst = SubInstanceBox.Text;
            String foldRoot = RootFolderBox.Text;
            String foldSub = SubFolderBox.Text;
            
            if ((physDump == "") && (logDump == ""))
            {
                MessageBox.Show("Must specifiy dump file.");
                return;
            }

            if ((userName == "") || (passWord == ""))
            {
                MessageBox.Show("Enter a username and password.");
                return;
            }

            if (instance == "")
            {
                MessageBox.Show("Enter an instance name.");
                return;
            }

            if (foldRoot == "" && foldSub != "")
            {
                MessageBox.Show("Can't have sub folder without a root folder.");
                return;
            }

            StringBuilder result = new StringBuilder();

            if (physDump != "")
            {
                result.Append("-devicedump \"" + physDump + "\"");
            }

            if (logDump != "")
            {
                result.Append(" -personneldump \"" + logDump + "\"");
            }

            result.Append(" -username " + userName + " -password " + passWord);

            if (apiIP != "")
            {
                result.Append(" -apiserver " + apiIP);
            }

            result.Append(" -instance " + instance);

            if (subInst != "")
            {
                result.Append(" -subinstance " + subInst);
            }

            if (foldRoot != "")
            {
                result.Append(" -rootfolder " + foldRoot);
            }

            if (foldSub != "" && subInst == "")
            {
                result.Append(" -subfolder " + foldSub);
            }
            else if (foldSub != "" && subInst != "")
            {
                MessageBox.Show("Can't have a sub folder, if a sub instance is provided.");
                return;
            }

            ConsoleOutput cOut;
            try
            {
                cOut = new ConsoleOutput(result.ToString());
            } catch (OperationCanceledException)
            {
                MessageBox.Show("AccessConversion.exe failed to start.");
                return;
            }
            
            cOut.ShowDialog();
            return;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browser1 = new OpenFileDialog
            {
                ValidateNames = true,
                Multiselect = false,

                DefaultExt = ".dmp",
                Filter = "Continuum Dump (.dmp)|*.dmp"
            };

            bool? diaBool = browser1.ShowDialog(this);
            
            if (diaBool == null || diaBool == false)
            {
                return;
            }

            PhysicalDumpLocationBox.Text = browser1.FileName;
            return;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browser2 = new OpenFileDialog
            {
                ValidateNames = true,
                Multiselect = false,

                DefaultExt = ".dmp",
                Filter = "Continuum Dump (.dmp)|*.dmp"
            };

            bool? diaBool = browser2.ShowDialog(this);

            if (diaBool == null || diaBool == false)
                return;

            LogicalDumpLocationBox.Text = browser2.FileName;
            return;
        }
    }
}