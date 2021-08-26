using loppis.ViewModels;
using System.Windows;

namespace loppis.Views
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new SalesViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sellerView = new SellerView();
            sellerView.DataContext = new SellersViewModel((SalesViewModel)this.DataContext);
            sellerView.Show();
        }
    }
}
