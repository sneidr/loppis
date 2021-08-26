using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace loppis.Views
{
    /// <summary>
    /// Interaction logic for SellerView.xaml
    /// </summary>
    public partial class SellerView : Window
    {
        public SellerView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            if (dialog.ShowDialog() == true)
            {
                foreach (var line in File.ReadAllLines(dialog.FileName))
                {
                    listbox1.Items.Add(line);
                }
            }
        }
    }
}
