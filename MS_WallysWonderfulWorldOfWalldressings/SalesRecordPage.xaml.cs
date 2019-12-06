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
    /// Interaction logic for SalesRecordPage.xaml
    /// </summary>
    public partial class SalesRecordPage : Page
    {

        private SqlConnectionHandler MyConnectionHandler;

        private List<OrderLine> OrderLines;
        private Customer CurrentCustomer;
        private string BranchName;
        private int BranchID;
        private decimal OrderFinalCost;

        public SalesRecordPage(List<OrderLine> IncommingOrderLines, Customer IncommingCustomer, string IncommingBranchName, int IncommingBranchID)
        {
            InitializeComponent();
            MyConnectionHandler = new SqlConnectionHandler();
            OrderLines = IncommingOrderLines;
            CurrentCustomer = IncommingCustomer;
            BranchName = IncommingBranchName;
            BranchID = IncommingBranchID;


            ProductsToPurchaseListBox.Items.Clear();

            decimal Subtotal = 0;

            int Counter = 1;

            foreach (OrderLine OL in IncommingOrderLines)
            {
                Subtotal += OL.ExtendedPrice;
                string OrderLineInStringForm = $"{Counter.ToString()}.)\t{OL.Product.ProductName} ({OL.Product.ProductType}) x{OL.QuantityOrdered.ToString()} ${OL.Product.sPrice} = ${OL.ExtendedPrice.ToString("N2")}";
                ProductsToPurchaseListBox.Items.Add(OrderLineInStringForm);
                Counter++;
            }

            decimal TaxPortion = Subtotal * (decimal)0.13;

            decimal FinalCost = TaxPortion + Subtotal;

            OrderFinalCost = FinalCost;

            SubtotalextBlock.Text = "$" + Subtotal.ToString("N2");
            HSTTextBlock.Text = "$" + TaxPortion.ToString("N2");
            SaleTotalTextBlock.Text = "$" + FinalCost.ToString("N2");

            IntroTextBlock.Text = $"Thank you {CurrentCustomer.FirstName } {CurrentCustomer.LastName}, for shopping at Wally's Wonderful World of Wallcoverings {BranchName} location on {DateTime.Now.ToShortDateString()}.";

            OrderNumberTextBlock.Text = (MyConnectionHandler.DetermineLatestOrderID() + 1).ToString();
        }

        private void MarkPaidButton_Click(object sender, RoutedEventArgs e)
        {
            bool AddNewOrderResult = MyConnectionHandler.AddNewOrder(CurrentCustomer.CustomerID, BranchID, OrderFinalCost, OrderLines);
            if (AddNewOrderResult == true)
            {

                TextDecoration MyUnderline = new TextDecoration();

                TextDecorationCollection MyCollection = new TextDecorationCollection();

                MyCollection.Add(MyUnderline);
                PaidStatusTextBlock.TextDecorations = MyCollection;

                PaidStatusTextBlock.Text = "Paid - Thank you!\"";
                PaidStatusTextBlock.FontSize = 16;
                MarkPaidButton.IsEnabled = false;
                CancelOrderButton.Content = "Return to Sales";
            }
            else
            {
                
                MessageBox.Show("Error! Unable to successfully place order into database!");
            }
        }

        private void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
            MainFrame.Navigate(new SalesScreenPage());
        }
    }
}
