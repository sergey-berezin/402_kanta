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
using Wpf;
using WpfApp;

namespace Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private PlotModel plotModel;
        OXYPlot oxyPlotModel;
        public ViewData data;
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
                Stop_button.IsEnabled = true;
                MessageBox.Show("Algorytm started!");
                data.ProgramStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Termination(object sender, RoutedEventArgs e)
        {
            data.ProgramCancel();
            Show_button.IsEnabled = true;
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

        private void Show(object sender, RoutedEventArgs e)
        {
            try
            {
                oxyPlotModel = new OXYPlot(data);
                plot.Model = oxyPlotModel.plot;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in Show_button\n" + ex.Message);
            }
        }
    }
}
