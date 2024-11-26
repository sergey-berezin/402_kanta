using System.Windows;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp
{
    public partial class SelectionWindow : Window
    {
        public Experiment SelectedExperiment { get; private set; }
        private List<Experiment> _experiments;
        private string _Reestr;

        public SelectionWindow(List<Experiment> experiments, string Reestr)
        {
            InitializeComponent();
            _experiments = experiments;
            ExperimentListBox.ItemsSource = _experiments;
            _Reestr = Reestr;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedExperiment = (Experiment)ExperimentListBox.SelectedItem;
            if (SelectedExperiment != null)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Select an experiment.");
                return;
            }
            MessageBox.Show($"{SelectedExperiment.Name} experiment selected");
        }   

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedExperiment = (Experiment)ExperimentListBox.SelectedItem;
            if (selectedExperiment != null)
            {

                try
                {
                    var experiments = JsonConvert.DeserializeObject<List<Experiment>>(File.ReadAllText(_Reestr));
                    var clearedExperiments = experiments.RemoveAll(exp => exp.Name == selectedExperiment!.Name);

                    var handler = new SafeJsonFileHandler<List<Experiment>>(_Reestr);
                    handler.SaveData(experiments);

                    File.Delete(selectedExperiment.FilePath);

                    _experiments.Remove(selectedExperiment);
                    ExperimentListBox.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Delete failed.");
                    return;
                }
                MessageBox.Show($"{selectedExperiment.Name} experiment deleted");
            }
            else
            {
                MessageBox.Show("Select an experiment.");
            }
        }
    }
}
