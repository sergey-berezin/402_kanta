using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Algorytm;
using OxyPlot;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ViewData data;
        private CancellationTokenSource cts;
        public MainWindow()
        {
            InitializeComponent();
            data = new ViewData();
            DataContext = data;
            CommandBinding cmb1 = new CommandBinding();
            cmb1.Command = ApplicationCommands.Save;
            cmb1.CanExecute += CanSave_DataCommandHandler;
            cmb1.Executed += Save_DataCommandHandler;
        }

        private void Algorytm_Start(object sender, RoutedEventArgs e)
        {
            try
            {
                cts = new CancellationTokenSource();
                Stop_button.IsEnabled = true;
                MessageBox.Show("Algorytm starting...");
                Task task = Task.Factory.StartNew(i => {
                    data.Process(cts.Token);
                }, TaskCreationOptions.LongRunning, cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Starting error", ex.Message);
            }
        }

        private void Termination(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            cts.Dispose();
        }

        private void CanSave_DataCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            {
                e.CanExecute = (Validation.GetHasError(SelectionTextBox) || Validation.GetHasError(CitiescountTextBox)
                  || Validation.GetHasError(MutationrateTextBox) || Validation.GetHasError(PopulationTextBox));
                e.CanExecute = !e.CanExecute;
            }
        }

        private void Save_DataCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Start_button.IsEnabled = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.Filter = "Json files (.json)|*.json";
            saveFileDialog.CreatePrompt = true;
            if (saveFileDialog.ShowDialog() == false)
            {
                MessageBox.Show("Saving Error");
                return;
            }
            else
            {
                string FilePath = saveFileDialog.FileName;
                saveFileDialog.DefaultExt = ".json";
                saveFileDialog.Filter = "Json files (.json)|*.json";

                try
                {
                    string backup = "F:\\Projects\\Practicum_2024-25\\Save_data\\main_data.json";
                    data.SaveJson(FilePath, backup);
                    MessageBox.Show("You saved in: " + FilePath + "; Backup: " + backup);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "Json files (.json)|*.json";
            if (openFileDialog.ShowDialog() == false)
            {
                MessageBox.Show("Loading Error");
                return;
            }
            else
            {
                string FilePath = openFileDialog.FileName;
                try
                {
                    data.LoadJson(FilePath);
                    MessageBox.Show("You loaded from: " + FilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }
    }
}