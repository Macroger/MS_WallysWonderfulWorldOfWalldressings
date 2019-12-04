using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    public  class SqlConnectionHandler
    {
        MySqlCommand MyCommand;
        MySqlDataReader MyReader;
        // Properties.Settings.Default.connStr

        public List<Customer> GetAllCustomers()
        {
            List<Customer> CustomerList = new List<Customer>();

            string SqlCommand = @"SELECT * FROM Customer_T;";

            try
            {
                Logger.LogStatus("Attempting to retieve Customer list from MsWally database.");

                using (MySqlConnection MySqlConnection = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConnection);
                    MySqlConnection.Open();

                    MyReader = MyCommand.ExecuteReader();

                    if (MyReader.HasRows == true)
                    {
                        while (MyReader.Read())
                        {
                            // Get a temp Customer object and fill its values with the values read in from the MySqlDataReader .
                            Customer tmp = new Customer();
                            tmp.CustomerID = MyReader.GetInt16(0);
                            tmp.FirstName = MyReader.GetString(1);
                            tmp.LastName = MyReader.GetString(2);
                            tmp.PhoneNumber = MyReader.GetString(3);

                            // Add a copy of tmp to the customer list, to be returned as a collection of Customers.
                            CustomerList.Add(tmp);
                        }
                    }
                    else
                    {
                        Logger.LogError(@"Unable to find any customers in the database - MyReader returned 0 rows when asked to retrieve a list of Customers.");
                        throw new Exception("Unable to find any customers in the database - MyReader returned 0 rows when asked to retrieve a list of Customers.");
                    }
                }
            }
            catch(Exception e)
            {
                Logger.LogError(e.Message);
                throw new Exception(e.Message);
            }

            Logger.LogStatus($"Successfully retrieved Customer list containing {CustomerList.Count} customers");

            return CustomerList;
        }

        public bool AddNewCustomer(Customer NewCustomer)
        {
            bool Result = false;

            string SqlCommand = $"" +
                $"INSERT INTO MSWally.Customer_T (FirstName, LastName, PhoneNumber)" +
                $"VALUES ('{NewCustomer.FirstName}', '{NewCustomer.LastName}', '{NewCustomer.PhoneNumber}')";

            Logger.LogStatus("Attempting to insert new customer into database.");

            try
            {
                using (MySqlConnection MySqlConnection = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConnection);
                    MySqlConnection.Open();

                    int NumberRowsAffected = MyCommand.ExecuteNonQuery();

                    if(NumberRowsAffected >= 1)
                    {
                        // I am using this as a check to see if the command worked.
                        Result = true;
                        Logger.LogStatus($"Successfully added new Customer : {NewCustomer.FirstName + " " + NewCustomer.LastName + " " + NewCustomer.PhoneNumber} .");
                    }
                }
            }
            catch(Exception e)
            {
                Logger.LogError($"Exception caught while attemping to insert new Customer. Details: {e.Message}");
            }

            return Result;
        }

      

    }
}
