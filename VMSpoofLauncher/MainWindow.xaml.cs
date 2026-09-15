using System.Windows;

namespace VMSpoofLauncher
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.StartSpoofing();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.StopSpoofing();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RefreshProcessList();
        }
    }
}
