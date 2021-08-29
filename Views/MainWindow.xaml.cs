using loppis.ViewModels;
using System.Windows;
namespace loppis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            DataContext = new SalesViewModel();
        }
    }
}
