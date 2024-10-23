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
				MessageBox.Show(ex.Message);
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
	}
}