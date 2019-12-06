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
    /// Interaction logic for OrderDetailsPage.xaml
    /// </summary>
    public partial class OrderDetailsPage : Page
    {
        private int OrderID;
        private Order TargetOrder;
        private SqlConnectionHandler MyConnectionHandler;
        public OrderDetailsPage(int IncommingOrderID)
        {
            InitializeComponent();
            MyConnectionHandler = new SqlConnectionHandler();
            OrderID = IncommingOrderID;
            PopulateOrderDetails();

            OrderLinesForOrderLabel.Content = $"Order Lines for OrderID {OrderID.ToString()}:";
        }

        private void PopulateOrderDetails()
        {
            TargetOrder = MyConnectionHandler.GetSingleOrder(OrderID);

            CustomerIDTextBlock.Text = TargetOrder.CustomerID.ToString();
            CustomerNameTextBlock.Text = TargetOrder.CustomerName;

            
            OrderedIDTextBlock.Text = OrderID.ToString();
            OrderedAtBranchTextBlock.Text = TargetOrder.BranchName;
            OrderTotalPriceTextBlock.Text = "$" + TargetOrder.sPrice.ToString();

            OrderDateTextBlock.Text = TargetOrder.OrderDate.ToShortDateString();
            OrderStatusTextBlock.Text = TargetOrder.OrderStatus;
            
            if(TargetOrder.OrderStatus == "RFND")
            {
                OrderRefundDateTextBlock.Text = TargetOrder.OrderRefundDate.ToLongDateString();
                RefundOrderButton.IsEnabled = false;
            }
            else
            {
                OrderRefundDateTextBlock.Text = "N/A";
            }
            OrderLinesDataGrid.ItemsSource = MyConnectionHandler.GetOrderLineDetails(OrderID);
        }

        private void ReturnToOrderHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");

            OrderHistoryPage OHP = new OrderHistoryPage();

            MainFrame.Navigate(OHP);
        }

        private void RefundOrderButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(Application.Current.MainWindow, "Refund this account?", "Refund Confirmation", System.Windows.MessageBoxButton.YesNo);

            if(messageBoxResult == MessageBoxResult.Yes)
            {
                MyConnectionHandler.RefundAnOrder(OrderID);

                MessageBox.Show(Application.Current.MainWindow,  "Refund successfully applied.");

                PopulateOrderDetails();

            }
        }
    }
}
