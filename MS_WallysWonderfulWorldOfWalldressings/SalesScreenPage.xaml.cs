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
    /// Interaction logic for SalesScreenPage.xaml
    /// </summary>
    public partial class SalesScreenPage : Page
    {
        private SqlConnectionHandler MyConnectionHandler;

        private decimal FinalOrderCost = 0;

        public SalesScreenPage()
        {
            InitializeComponent();

            MyConnectionHandler = new SqlConnectionHandler();

            UpdateProductView();
        }

        private void UpdateProductView()
        {
            ProductsViewListBox.ItemsSource = MyConnectionHandler.GetAllProducts();
        }

        private void ProductQuantityIncrementButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ValueOfQuantityBox = int.Parse(ProductQuantityTextBox.Text);

                ValueOfQuantityBox++;

                var ErrorBlock = (TextBlock)Application.Current.MainWindow.FindName("tbErrorMessages");

                ProductQuantityTextBox.Text = ValueOfQuantityBox.ToString();

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                {
                    ErrorBlock.Foreground = Brushes.Transparent;
                    ErrorBlock.Text = "";
                }));

                ClearErrorBorderFromProductQuantityBox();

            }
            catch (ArgumentNullException)
            {
                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true );
            }
            catch (FormatException)
            {
                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true);
            }
            catch (OverflowException)
            {
                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true);
            }

        }


        private void ProductQuantityDecrementButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ValueOfQuantityBox = int.Parse(ProductQuantityTextBox.Text);
                if (ValueOfQuantityBox > 0)
                {
                    ValueOfQuantityBox--;

                    ProductQuantityTextBox.Text = ValueOfQuantityBox.ToString();

                    var ErrorBlock = (TextBlock)Application.Current.MainWindow.FindName("tbErrorMessages");

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                    {
                        ErrorBlock.Foreground = Brushes.Transparent;
                        ErrorBlock.Text = "";
                    }));

                    ClearErrorBorderFromProductQuantityBox();

                }
                else
                {
                    SalesScreenDisplayErrorMessage("Error. Unable to decrement product quantity box below 0.", true);
                }
            }
            catch (ArgumentNullException)
            {
                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true);
            }
            catch (FormatException)
            {
                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true);
            }
            catch (OverflowException)
            {
                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true);
            }
        }
        
        

        private void AddItemToOrderButton_Click(object sender, RoutedEventArgs e)
        {
            int Quantity = int.Parse(ProductQuantityTextBox.Text);

            bool FoundDuplicate = false;
            bool ErrorEncountered = false;

            OrderLine OrderLineToRemove = new OrderLine();

            if (Quantity > 0)
            {
                if (ProductsViewListBox.SelectedItem != null)
                {
                    Product ChosenItem = ProductsViewListBox.SelectedItem as Product;

                    if(int.Parse(ChosenItem.StockCount) >= Quantity)
                    {
                        foreach (OrderLine OL in OrderDataGrid.Items)
                        {
                            if (OL.Product.ProductSKU == ChosenItem.ProductSKU)
                            {
                                // Check to see if the proposed amount would cause the stock to go into a negative value.
                                if((OL.QuantityOrdered + Quantity) > int.Parse(OL.Product.StockCount))
                                {
                                    SalesScreenDisplayErrorMessage($"Error. StockCount remaining for item is less than requested quantity.", true);
                                    ErrorEncountered = true;
                                    break; 
                                }

                                // Duplicate entry, do not add but instead update the quantity count.
                                OrderLineToRemove = OL;
                                Quantity += OL.QuantityOrdered;

                                FoundDuplicate = true;
                                break;
                            }
                        }

                        if (FoundDuplicate == true)
                        {
                            OrderDataGrid.Items.Remove(OrderLineToRemove);
                        }
                        if(ErrorEncountered == false)
                        {
                            try
                            {
                                OrderLine MyOrderLine = new OrderLine(ChosenItem, Quantity);

                                OrderDataGrid.Items.Add(MyOrderLine);

                                ProductQuantityTextBox.Text = "0";
                            }
                            catch (ArgumentNullException)
                            {
                                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true);
                            }
                            catch (FormatException)
                            {
                                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true);
                            }
                            catch (OverflowException)
                            {
                                SalesScreenDisplayErrorMessage("Error. Unable to convert quantity into a number, please make sure only numbers entered into quantity box.", true);
                            }

                            // Trigger totals to be calculated.
                            CalculateCosts();
                        }
                    }
                    else
                    {
                        SalesScreenDisplayErrorMessage($"Error. StockCount remaining for item is less than requested quantity. Suggest reducing quantity to {ChosenItem.StockCount} or less.", true);
                    }
                }
            }
            else
            {
                SalesScreenDisplayErrorMessage("Error. Product quantity can not be 0.", true);
            }
        }

        private void ClearErrorBorderFromProductQuantityBox()
        {
            ProductQuantityBorder.BorderThickness = new Thickness(0);
        }


        private void SalesScreenDisplayErrorMessage(string ErrorMessage, bool HighLightQuantityBorderRed)
        {

            var ErrorBlock = (TextBlock)Application.Current.MainWindow.FindName("tbErrorMessages");

            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                ErrorBlock.Foreground = Brushes.Red;
                ErrorBlock.Text = ErrorMessage;
            }));

            if (HighLightQuantityBorderRed == true)
            {
                ProductQuantityBorder.BorderBrush = Brushes.Red;
                ProductQuantityBorder.BorderThickness = new Thickness(3);
            }
        }


        private void CalculateCosts()
        {
            decimal Subtotal = 0;

            foreach (OrderLine OL in OrderDataGrid.Items)
            {
                Subtotal += OL.ExtendedPrice;
            }

            decimal TaxPortion = Subtotal * (decimal)0.13;

            decimal FinalCost = TaxPortion + Subtotal;

            OrderPreviewSubtotal.Text = "$" + Subtotal.ToString("N2");
            OrderPreviewTaxes.Text = "$" + TaxPortion.ToString("N2");
            OrderPreviewFinalTotal.Text = "$" + FinalCost.ToString("N2");

            FinalOrderCost = FinalCost;
        }


        private void ResetOrderPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            OrderPreviewFinalTotal.Text = "";
            OrderPreviewTaxes.Text = "";
            OrderPreviewSubtotal.Text = "";
            OrderDataGrid.Items.Clear();
        }


        private void RemoveItemFromOrderButton_Click(object sender, RoutedEventArgs e)
        {
            int Quantity = int.Parse(ProductQuantityTextBox.Text);

            OrderLine OrderLineToRemove = new OrderLine();

            if (Quantity > 0)
            {
                if (ProductsViewListBox.SelectedItem != null)
                {
                    Product ChosenItem = ProductsViewListBox.SelectedItem as Product;

                    foreach (OrderLine OL in OrderDataGrid.Items)
                    {
                        if (OL.Product.ProductSKU == ChosenItem.ProductSKU)
                        {
                            // Duplicate entry, update the quantity count.

                            if (OL.QuantityOrdered <= Quantity)
                            {
                                // If the quantity ordered is less then the quantity requested to remove the result will be less than 0 and thus the entry should be removed from the list.
                                OrderLineToRemove = OL;
                                ProductQuantityTextBox.Text = "0";
                                break;
                            }
                            else
                            {
                                OrderLineToRemove = OL;
                                Quantity = OL.QuantityOrdered - Quantity;

                                try
                                {
                                    OrderLine MyOrderLine = new OrderLine(ChosenItem, Quantity);
                                    OrderDataGrid.Items.Add(MyOrderLine);
                                    ProductQuantityTextBox.Text = "0";
                                }
                                catch (ArgumentNullException)
                                {
                                    SalesScreenDisplayErrorMessage("Error. Unable to convert Product Quantity into a number, please make sure to enter only numbers into the quantity box.", true);
                                }
                                catch (FormatException)
                                {
                                    SalesScreenDisplayErrorMessage("Error. Unable to convert Product Quantity into a number, please make sure to enter only numbers into the quantity box.", true);
                                }
                                catch (OverflowException)
                                {
                                    SalesScreenDisplayErrorMessage("Error. Unable to convert Product Quantity into a number, please make sure to enter only numbers into the quantity box.", true);
                                }
                                break;
                            }
                        }
                    }

                    OrderDataGrid.Items.Remove(OrderLineToRemove);

                    // Trigger totals to be calculated.
                    CalculateCosts();
                }
            }
            else
            {
                SalesScreenDisplayErrorMessage("Unable to remove 0 units. Provide a number greater than 0 to remove.", true);
            }
        }


        private void AcceptOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var CustomerListBox = (ListBox)Application.Current.MainWindow.FindName("CustomerListBox");

            if (CustomerListBox.SelectedItem != null)
            {
                int OrderCustomerID = DetermineCustomerID();
                int OrderBranchID = DetermineBranchID();
                decimal FinalPrice = FinalOrderCost;

                List<OrderLine> OrderLines = new List<OrderLine>();

                foreach (OrderLine OL in OrderDataGrid.Items)
                {
                    OrderLines.Add(OL);
                }

                Customer CurrentlySelectedCustomer = CustomerListBox.SelectedItem as Customer;

                if (CurrentlySelectedCustomer != null)
                {
                    string BranchName = BranchLocationComboBox.Text;
                    if(BranchName != null)
                    {
                        SalesRecordPage SRP = new SalesRecordPage(OrderLines, CurrentlySelectedCustomer, BranchName, (BranchLocationComboBox.SelectedIndex +1));

                        var MainFrame = (Frame)Application.Current.MainWindow.FindName("MainFrame");
                        MainFrame.Navigate(SRP);
                    }
                    else
                    {
                        SalesScreenDisplayErrorMessage("Unable to place order - Branch Location is not set.", false);
                    }
                }
                else
                {
                    SalesScreenDisplayErrorMessage("Unable to select customer from CustomerListBox.SelectedItem while attempting to build order object.", false);
                }
            }
            else
            {
                SalesScreenDisplayErrorMessage("Unable to generate order - No customer selected in Customer Navigator.", false);
            }
        }


        private int DetermineCustomerID()
        {
            var CustomerListBox = (ListBox)Application.Current.MainWindow.FindName("CustomerListBox");

            Customer SelectedCustomer = CustomerListBox.SelectedItem as Customer;

            return SelectedCustomer.CustomerID;
        }

        private int DetermineBranchID()
        {
            return (BranchLocationComboBox.SelectedIndex + 1);
        }
    }
}
