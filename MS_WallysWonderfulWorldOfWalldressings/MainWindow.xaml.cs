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
                    tbErrorMessages.Foreground = Brushes.Red;
                    tbErrorMessages.Text = "Unable to Add New Customer. First name fails validation; Ensure it is letters only.";
                    NewCustomerFirstNameBorder.BorderBrush = Brushes.Red;
                    NewCustomerFirstNameBorder.BorderThickness = new Thickness(3);
                }
            }
            else
            {
                tbErrorMessages.Foreground = Brushes.Red;
                tbErrorMessages.Text = "Unable to Add New Customer. First name fails validation; Ensure it is letters only.";
                NewCustomerFirstNameBorder.BorderBrush = Brushes.Red;
                NewCustomerFirstNameBorder.BorderThickness = new Thickness(3);
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
                    tbErrorMessages.Foreground = Brushes.Red;
                    tbErrorMessages.Text = "Unable to Add New Customer. Last name fails validation; Ensure it is letters only.";
                    NewCustomerLastNameBorder.BorderBrush = Brushes.Red;
                    NewCustomerLastNameBorder.BorderThickness = new Thickness(3);
                }
            }
            else
            {
                tbErrorMessages.Foreground = Brushes.Red;
                tbErrorMessages.Text = "Unable to Add New Customer. Last name length must be greater than 0.";
                NewCustomerLastNameBorder.BorderBrush = Brushes.Red;
                NewCustomerLastNameBorder.BorderThickness = new Thickness(3);
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
                    tbErrorMessages.Foreground = Brushes.Red;
                    tbErrorMessages.Text = "Unable to Add New Customer. Phone number fails validation; Ensure it is numbers only.";
                    NewCustomerPhoneNumberBorder.BorderBrush = Brushes.Red;
                    NewCustomerPhoneNumberBorder.BorderThickness = new Thickness(3);
                }
            }
            else
            {
                tbErrorMessages.Foreground = Brushes.Red;
                tbErrorMessages.Text = "Unable to Add New Customer. Phone number length must be greater than 0.";
                NewCustomerPhoneNumberBorder. BorderBrush = Brushes.Red;
                NewCustomerPhoneNumberBorder.BorderThickness = new Thickness(3);
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

        }

        private void ViewOrderHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            OrderHistory oh = new OrderHistory();
            oh.Show();

            this.Close();
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
    }
}
