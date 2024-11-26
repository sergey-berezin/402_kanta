using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp;
using Newtonsoft.Json;
using System.IO;

namespace Wpf
{
    public partial class MainWindow : Window
    {

        public ViewData data;
        private CancellationTokenSource cts;

        private String openedExperimentName = "...";

        private String Reestr = "F:\\Projects\\Practicum_2024-25\\Save_data\\Reestr.json";

        private String files_root = "F:\\Projects\\Practicum_2024-25\\Save_data";

        List<Experiment> experiments;
        public MainWindow()
        {
            InitializeComponent();
            data = new ViewData();
            DataContext = data;
            MessageBox.Show("Rules:\n" + "If new experiment needed, choose parameters in panel, then click 'Data commit', then 'Start'\n" + "If loaded experiment, then click 'Start'");

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
            data.loaded = false;
        }

        public Experiment? LoadExperimentData(string experimentName)
        {

            if (!File.Exists(Reestr)) return null;

            var experiments = JsonConvert.DeserializeObject<List<Experiment>>(File.ReadAllText(Reestr));
            var selectedExperiment = experiments.FirstOrDefault(exp => exp.Name == experimentName);

            return selectedExperiment;
        }

        public List<Experiment> LoadAllExperiments()
        {
            if (!File.Exists(Reestr))
            {
                return new List<Experiment>();
            };

            var experiments = JsonConvert.DeserializeObject<List<Experiment>>(File.ReadAllText(Reestr))!;

            return experiments;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Start_button.IsEnabled = true;
            Stop_button.IsEnabled = false;

            var experimentName = "";
            var nameInputDialog = new NameWindow(openedExperimentName);
            if (nameInputDialog.ShowDialog() == true)
            {
                experimentName = nameInputDialog.ExperimentName;
                if (LoadExperimentData(experimentName) != null)
                {
                    MessageBox.Show($"Existing experiment would be rewritten");
                }
            }
            else
            {
                MessageBox.Show($"New experiment will be created");
                return;
            }

            List<Experiment> experiments;
            if (File.Exists(Reestr))
            {
                experiments = JsonConvert.DeserializeObject<List<Experiment>>(File.ReadAllText(Reestr));
            }
            else
            {
                experiments = new List<Experiment>();
            }

            var solverFileName = $"{files_root}\\{experimentName}.json";
            var handler = new SafeJsonFileHandler<Algorytm_data>(solverFileName);
            handler.SaveData(data.data_to_json);

            Experiment? newExper = LoadExperimentData(experimentName);
            if (newExper != null) return;

            newExper = new Experiment(experimentName, solverFileName);
            experiments.Add(new Experiment(experimentName, solverFileName));

            var experhandler = new SafeJsonFileHandler<List<Experiment>>(Reestr);
            experhandler.SaveData(experiments);

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            List<Experiment> experiments = LoadAllExperiments();
            if (experiments.Count == 0)
            {
                MessageBox.Show($"No experiments are created yet.");
                return;
            }

            SelectionWindow selectionWindow = new SelectionWindow(experiments, Reestr);

            if (selectionWindow.ShowDialog() == false) return;

            Experiment selectedExperiment = selectionWindow.SelectedExperiment;

            if (!data.LoadJson(selectedExperiment.FilePath))
            {
                MessageBox.Show($"Failed to load experiment.");
                return;
            };

            openedExperimentName = selectedExperiment.Name;

            Start_button.IsEnabled = true;

        }
    }
}