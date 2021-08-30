using loppis.ViewModels;
using System.Windows;
using System.Windows.Controls;

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

        private void items_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {



        }

        private void items_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                for (int i = 0; i < ((ListBox)sender).Items.Count; i++)
                {
                    if (item == ((ListBox)sender).ItemContainerGenerator.ContainerFromIndex(i))
                    {
                        // ListBox item clicked - do some cool things here
                        if (((SalesViewModel)DataContext).UndoCommand.CanExecute(i))
                        {
                            ((SalesViewModel)DataContext).UndoCommand.Execute(i);
                        }
                    }
                }
            }
        }
    }
}
