using System.Windows;

namespace WpfApp
{
    public partial class NameWindow : Window
    {
        public string ExperimentName { get; private set; }

        public NameWindow(string DefaultName)
        {
            InitializeComponent();

            NameTextBox.Text = DefaultName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ExperimentName = NameTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
