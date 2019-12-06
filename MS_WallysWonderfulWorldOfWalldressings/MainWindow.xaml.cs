using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnectionHandler MyConnectionHandler;
        public MainWindow()
        {
            InitializeComponent();
            MyConnectionHandler = new SqlConnectionHandler();

            UpdateCustomerView();

            MainFrame.Source = new Uri("SalesScreenPage.xaml", UriKind.Relative);
        }

        private void AddNewCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            bool FinalValidationPasses = true;
            bool FirstNamePassesValidation = false;
            bool LastNamePassesValidation = false;
            bool PhoneNumberPassesValidation = false;

            if(NewCustomerFirstNameTextBox.Text.Length > 0)
            {
                if(ValidateNewCustomerName(NewCustomerFirstNameTextBox.Text) == true)
                {
                    // Validation passes.
                    FirstNamePassesValidation = true;
                }
                else
                {
                    DisplayErrorMessageOnMainWindow("Unable to Add New Customer. First name fails validation; Ensure it is letters only.", NewCustomerFirstNameBorder);
                }
            }
            else
            {
                DisplayErrorMessageOnMainWindow("Unable to Add New Customer. First name fails validation; Ensure it is letters only.", NewCustomerFirstNameBorder);
            }

            if(NewCustomerLastNameTextBox.Text.Length > 0)
            {
                if(ValidateNewCustomerName(NewCustomerLastNameTextBox.Text) == true)
                {
                    // Validation passes.
                    LastNamePassesValidation = true;
                }
                else
                {
                    DisplayErrorMessageOnMainWindow("Unable to Add New Customer. Last name fails validation; Ensure it is letters only.", NewCustomerLastNameBorder);
                }
            }
            else
            {
                DisplayErrorMessageOnMainWindow("Unable to Add New Customer. Last name length must be greater than 0.", NewCustomerLastNameBorder);
            }

            if(NewCustomerPhoneNumberTextBox.Text.Length > 0)
            {
                if(ValidateNewCustomerPhoneNumber(NewCustomerPhoneNumberTextBox.Text))
                {
                    // Validation passes.
                    PhoneNumberPassesValidation = true;
                }
                else
                {
                    DisplayErrorMessageOnMainWindow("Unable to Add New Customer. Phone number fails validation; Ensure it is numbers only.", NewCustomerPhoneNumberBorder);
                }
            }
            else
            {
                DisplayErrorMessageOnMainWindow("Unable to Add New Customer. Phone number length must be greater than 0.", NewCustomerPhoneNumberBorder);
            }

            if(FirstNamePassesValidation == true && LastNamePassesValidation == true && PhoneNumberPassesValidation == true)
            {
                // Create a new Customer object out of the assembled data.
                Customer NewCustomer = new Customer()
                {
                    FirstName = NewCustomerFirstNameTextBox.Text,
                    LastName = NewCustomerLastNameTextBox.Text,
                    PhoneNumber = AdjustPhoneNumberBeforeInserting(NewCustomerPhoneNumberTextBox.Text)
                };

                foreach(Customer cstmr in CustomerListBox.ItemsSource)
                {
                    if(cstmr.Equals(NewCustomer))
                    {
                        // Duplicate entry found - prevent it from being added to the database
                        FinalValidationPasses = false;
                        break;
                    }
                }
                if(FinalValidationPasses == true)
                {
                    // All fields validated - send the new customer to the database.
                    if (MyConnectionHandler.AddNewCustomer(NewCustomer) == true)
                    {
                        UpdateCustomerView();
                        tbErrorMessages.Foreground = Brushes.Green;
                        tbErrorMessages.Text = "Successfully added new customer.";
                    }
                    else
                    {
                        tbErrorMessages.Foreground = Brushes.Red;
                        tbErrorMessages.Text = "Unable to Add New Customer. Add new customer operation failed, check logs for more details.";
                    }
                }
                else
                {
                    tbErrorMessages.Foreground = Brushes.Red;
                    tbErrorMessages.Text = "Unable to Add New Customer. Duplicate entry detected.";
                }
            }
        }


        private string AdjustPhoneNumberBeforeInserting(string PhoneNumberToAdjust)
        {
            string AdjustedPhoneNumber = String.Empty;

            AdjustedPhoneNumber = PhoneNumberToAdjust.Insert(6, "-");

            AdjustedPhoneNumber = AdjustedPhoneNumber.Insert(3, "-");

            return AdjustedPhoneNumber;
        }


        private void ViewInventoryLevelsButton_Click(object sender, RoutedEventArgs e)
        {
            Object PageBeingShownInFrame = MainFrame.Content.GetType();

            // Here is where the logic is built to differentiate how the system handles clicking a customer name depending on the currently loaded frame page.
            if (PageBeingShownInFrame.ToString() == "MS_WallysWonderfulWorldOfWalldressings.InventoryLevelsPage")
            {
                DisplayErrorMessageOnMainWindow("Unable to switch to Inventory Levels Page - already on it.", null);
            }
            else
            {
                MainFrame.Source = new Uri("InventoryLevelsPage.xaml", UriKind.Relative);
            }
        }


        private void ViewOrderHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            Object PageBeingShownInFrame = MainFrame.Content.GetType();

            // Here is where the logic is built to differentiate how the system handles clicking a customer name depending on the currently loaded frame page.
            if (PageBeingShownInFrame.ToString() == "MS_WallysWonderfulWorldOfWalldressings.OrderHistoryPage")
            {
                DisplayErrorMessageOnMainWindow("Unable to switch to Order History Page - already on Order History Page.", null);
            }
            else
            {
                MainFrame.Source = new Uri("OrderHistoryPage.xaml", UriKind.Relative);
            }
        }


        private void UpdateCustomerView()
        {
            CustomerListBox.ItemsSource = MyConnectionHandler.GetAllCustomers();
        }


        private bool ValidateNewCustomerPhoneNumber(string PhoneNumberString)
        {
            bool Result = false;

            if(String.IsNullOrWhiteSpace( PhoneNumberString) == false)
            {
                string RegexPattern = @"[0-9]{10}";
                Regex rx = new Regex(RegexPattern);

                if(rx.IsMatch(PhoneNumberString) == true)
                {
                    Result = true;
                }
            }

            return Result;
        }


        private bool ValidateNewCustomerName(string NameToValidate)
        {
            bool Result = false;

            string RegexPattern = @"[A-Za-z]{1,16}";
            Regex rx = new Regex(RegexPattern);

            if(rx.IsMatch(NameToValidate) == true)
            {
                // Name passes validation
                Result = true;
            }

            return Result;
        }


        private void NewCustomerLastNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbErrorMessages.Text.Length > 0 && tbErrorMessages.Foreground == Brushes.Red)
            {
                tbErrorMessages.Text = "";
                NewCustomerPhoneNumberTextBox.Text = "";
                NewCustomerLastNameTextBox.Text = "";
                NewCustomerFirstNameTextBox.Text = "";
                NewCustomerLastNameBorder.BorderThickness = new Thickness(0);
            }
        }


        private void NewCustomerPhoneNumberTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbErrorMessages.Text.Length > 0 && tbErrorMessages.Foreground == Brushes.Red)
            {
                NewCustomerPhoneNumberTextBox.Text = "";
                NewCustomerLastNameTextBox.Text = "";
                NewCustomerFirstNameTextBox.Text = "";
                tbErrorMessages.Text = "";
                NewCustomerPhoneNumberBorder.BorderThickness = new Thickness(0);
            }
        }


        private void NewCustomerFirstNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbErrorMessages.Text.Length > 0 && tbErrorMessages.Foreground == Brushes.Red)
            {
                tbErrorMessages.Text = "";
                NewCustomerPhoneNumberTextBox.Text = "";
                NewCustomerLastNameTextBox.Text = "";
                NewCustomerFirstNameTextBox.Text = "";
                NewCustomerFirstNameBorder.BorderThickness = new Thickness(0);
            }
        }


        private void DisplayErrorMessageOnMainWindow(string ErrorMessage, Border BorderToHighlightRed)
        {
            tbErrorMessages.Foreground = Brushes.Red;
            tbErrorMessages.Text = ErrorMessage;

            if(BorderToHighlightRed != null)
            {
                BorderToHighlightRed.BorderBrush = Brushes.Red;
                BorderToHighlightRed.BorderThickness = new Thickness(3);
            }
        }


        private void CustomerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox EventOriginator = sender as ListBox;

            if (EventOriginator != null)
            {
                Customer EventOriginatorItem = EventOriginator.SelectedItem as Customer;

                if (EventOriginatorItem != null)
                {
                    Object PageBeingShownInFrame = MainFrame.Content.GetType();

                    // Here is where the logic is built to differentiate how the system handles clicking a customer name depending on the currently loaded frame page.
                    if (PageBeingShownInFrame.ToString() == "MS_WallysWonderfulWorldOfWalldressings.SalesScreenPage")
                    {
                        //MessageBox.Show(PageBeingShownInFrame.ToString());

                        Page SalesScreenPage = MainFrame.Content as Page;

                        TextBlock SalesScreenCustomerFirstNameTextBox = (TextBlock)SalesScreenPage.FindName("SalesScreenCustomerFirstNameTextBox");
                        TextBlock SalesScreenCustomerLastNameTextBox = (TextBlock)SalesScreenPage.FindName("SalesScreenCustomerLastNameTextBox");
                        TextBlock SalesScreenCustomerPhoneNumberTextBox = (TextBlock)SalesScreenPage.FindName("SalesScreenCustomerPhoneNumberTextBox");
                        TextBlock CustomerSelectedTextBlock = (TextBlock)SalesScreenPage.FindName("CustomerSelectedTextBlock");

                        SalesScreenCustomerFirstNameTextBox.Text = EventOriginatorItem.FirstName;
                        SalesScreenCustomerLastNameTextBox.Text = EventOriginatorItem.LastName;
                        SalesScreenCustomerPhoneNumberTextBox.Text = EventOriginatorItem.PhoneNumber;

                        CustomerSelectedTextBlock.Text = EventOriginatorItem.FirstName + " " + EventOriginatorItem.LastName;
                    }
                }
            }
        }


        private void SearchCustomersButton_Click(object sender, RoutedEventArgs e)
        {
            bool IsValidName = ValidateNewCustomerName(CustomerSearchBox.Text);

            bool IsValidPhoneNumber = ValidateNewCustomerPhoneNumber(CustomerSearchBox.Text);

            if(IsValidPhoneNumber == true )
            {
                string AdjustedPhoneNumber = AdjustPhoneNumberBeforeInserting(CustomerSearchBox.Text);
                foreach (Customer cst in CustomerListBox.Items)
                {
                    if(AdjustedPhoneNumber == cst.PhoneNumber)
                    {
                        // Match found - highlight the selected customer in the customer navigator and move credentials over to the Sales Screen.
                        CustomerListBox.SelectedItem = cst;
                    }
                }
            }
            else if( IsValidName == true)
            {
                foreach (Customer cst in CustomerListBox.Items)
                {
                    if (CustomerSearchBox.Text == cst.LastName)
                    {
                        // Match found - highlight the selected customer in the customer navigator and move credentials over to the Sales Screen.
                        CustomerListBox.SelectedItem = cst;
                    }
                }
            }
            else
            {
                DisplayErrorMessageOnMainWindow("Unable to find customers matching search string.", null);
            }
        }

        private void ViewSalesScreenButton_Click(object sender, RoutedEventArgs e)
        {
            Object PageBeingShownInFrame = MainFrame.Content.GetType();

            // Here is where the logic is built to differentiate how the system handles clicking a customer name depending on the currently loaded frame page.
            if (PageBeingShownInFrame.ToString() == "MS_WallysWonderfulWorldOfWalldressings.SalesScreenPage")
            {
                DisplayErrorMessageOnMainWindow("Unable to switch to Sales Screen - already on Sales Screen.", null);
            }
            else
            {
                MainFrame.Source = new Uri("SalesScreenPage.xaml", UriKind.Relative);
            }
           
        }
    }

}
