using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    /// <summary>
    /// Interaction logic for OrderHistoryPage.xaml
    /// </summary>
    public partial class OrderHistoryPage : Page
    {

        private SqlConnectionHandler MyConnectionHandler;

        public OrderHistoryPage()
        {
            InitializeComponent();

            MyConnectionHandler = new SqlConnectionHandler();

            PopulateOrderHistory();
        }

        private void PopulateOrderHistory()
        {
            OrderHistoryDataGrid.ItemsSource = MyConnectionHandler.GetAllOrders();
        }

        private void OrderHistoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if(OrderHistoryDataGrid.SelectedItem.ToString() == "MS_WallysWonderfulWorldOfWalldressings.Order")
            {
                Order TmpOrder = OrderHistoryDataGrid.SelectedItem as Order;

                CurrentlySelectedOrderTextBox.Text = TmpOrder.OrderID.ToString();
            }
        }

        private void ViewOrderDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");

            Order TmpOrder = OrderHistoryDataGrid.SelectedItem as Order;

            if(TmpOrder != null)
            {
                OrderDetailsPage ODP = new OrderDetailsPage(TmpOrder.OrderID);

                MainFrame.Navigate(ODP);
            }

           
        }

        private void OrderHistoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");

            Order TmpOrder = OrderHistoryDataGrid.SelectedItem as Order;

            if (TmpOrder != null)
            {
                OrderDetailsPage ODP = new OrderDetailsPage(TmpOrder.OrderID);

                MainFrame.Navigate(ODP);
            }
        }
    }
}
