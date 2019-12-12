using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    public class SqlConnectionHandler
    {
        MySqlCommand MyCommand;
        MySqlDataReader MyReader;


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
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                throw new Exception(e.Message);
            }

            Logger.LogStatus($"Successfully retrieved Customer list containing {CustomerList.Count} customers");

            return CustomerList;
        }

        public List<Product> GetAllProducts()
        {
            List<Product> ProductList = new List<Product>();

            string SqlCommand = @"SELECT ProductSKU, ProductName, wPrice, ProductType, Stock FROM MSWally.Product_T";

            try
            {
                using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
                    MySqlConn.Open();

                    MyReader = MyCommand.ExecuteReader();
                    if (MyReader.HasRows == true)
                    {
                        while (MyReader.Read())
                        {
                            // Get a temp Product object and fill its values with the values read in from the MySqlDataReader .
                            Product tmp = new Product();
                            tmp.ProductSKU = MyReader.GetInt16(0);
                            tmp.ProductName = MyReader.GetString(1);
                            tmp.sPrice = (MyReader.GetDecimal(2) * (decimal)1.40).ToString("N2");     // Here is where Wally's Profit Margin comes into play - we are adjusting the wPrice by the required 1.40x. 
                            tmp.ProductType = MyReader.GetString(3);
                            tmp.StockCount = MyReader.GetInt16(4).ToString();

                            // Add a copy of tmp to the customer list, to be returned as a collection of Customers.
                            ProductList.Add(tmp);
                        }

                    }
                    else
                    {
                        Logger.LogError(@"Unable to find any Products in the database - MyReader returned 0 rows when asked to retrieve a list of Products.");
                        throw new Exception("Unable to find any Products in the database - MyReader returned 0 rows when asked to retrieve a list of Products.");
                    }

                    MySqlConn.Close();

                }

                //using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
                //{
                //    SqlCommand = @"SELECT ProductBundleID, BundleName, BundlePrice, BundleStock FROM MSWally.ProductBundle_T";
                //    MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
                //    MySqlConn.Open();
                //    MyReader = MyCommand.ExecuteReader();
                //    if (MyReader.HasRows == true)
                //    {
                //        while (MyReader.Read())
                //        {
                //            // Get a temp Product object and fill its values with the values read in from the MySqlDataReader .
                //            Product tmp = new Product();
                //            tmp.ProductSKU = MyReader.GetInt16(0);
                //            tmp.ProductName = MyReader.GetString(1);
                //            tmp.sPrice = (MyReader.GetDecimal(2) * (decimal)1.40).ToString("N2");        // Here is where Wally's Profit Margin comes into play - we are adjusting the wPrice by the required 1.40x. 
                //            tmp.StockCount = MyReader.GetInt16(3).ToString();
                //            tmp.ProductType = "bundle";

                //            // Add a copy of tmp to the customer list, to be returned as a collection of Customers.
                //            ProductList.Add(tmp);
                //        }
                //    }
                //    else
                //    {
                //        Logger.LogError(@"Unable to find any ProductBundles in the database - MyReader returned 0 rows when asked to retrieve a list of ProductBundle.");
                //        throw new Exception("Unable to find any ProductBundles in the database - MyReader returned 0 rows when asked to retrieve a list of ProductBundle.");
                //    }

                //    MySqlConn.Close();
                //}
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                throw new Exception(e.Message);
            }

            Logger.LogStatus($"Successfully retrieved Product list containing {ProductList.Count} products");

            return ProductList;
        }

        public List<Order> GetAllOrders()
        {
            List<Order> OrderList = new List<Order>();

            string SqlCommand = @"SELECT OrderID, Order_T.CustomerID, CONCAT(Customer_T.FirstName, ' ', Customer_T.LastName), Order_T.BranchID, Branch_T.BranchName, OrderDate, OrderStatus, sPrice, OrderRefundDate
                                                    FROM Order_T 
                                                    INNER JOIN Customer_T ON Customer_T.CustomerID = Order_T.CustomerID
                                                    INNER JOIN Branch_T ON Branch_T.BranchID = Order_T.BranchID
                                                    ORDER BY Order_T.OrderID;";

            try
            {
                using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
                    MySqlConn.Open();

                    MyReader = MyCommand.ExecuteReader();
                    if (MyReader.HasRows == true)
                    {
                        while (MyReader.Read())
                        {
                            // Get a temp Order object and fill its values with the values read in from the MySqlDataReader .
                            Order tmp = new Order();
                            tmp.OrderID = MyReader.GetInt16(0);
                            tmp.CustomerID = MyReader.GetInt16(1);
                            tmp.CustomerName = MyReader.GetString(2);
                            tmp.BranchID = MyReader.GetInt16(3);
                            tmp.BranchName = MyReader.GetString(4);    
                            tmp.OrderDate = MyReader.GetDateTime(5).ToShortDateString();
                            tmp.OrderStatus = MyReader.GetString(6);
                            tmp.sPrice = MyReader.GetDecimal(7).ToString();

                            if( tmp.OrderStatus == "RFND")
                            {
                                tmp.OrderRefundDate = MyReader.GetDateTime(8);
                            }

                            // Add a copy of tmp to the order list, to be returned as a collection of Orders.
                            OrderList.Add(tmp);
                        }

                    }
                    else
                    {
                        Logger.LogError(@"Unable to find any Orders in the database - MyReader returned 0 rows when asked to retrieve a list of Orders.");
                        throw new Exception("Unable to find any Orders in the database - MyReader returned 0 rows when asked to retrieve a list of Orders.");
                    }

                    MySqlConn.Close();

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                throw new Exception(e.Message);
            }

            Logger.LogStatus($"Successfully retrieved Product list containing {OrderList.Count} products");

            return OrderList;
        }

        public Order GetSingleOrder(int OrderID)
        {
            Order TmpOrder = new Order();

            string SqlCommand = @"SELECT Customer_T.CustomerID, CONCAT(Customer_T.FirstName, ' ', Customer_T.LastName), Branch_T.BranchName, Order_T.OrderDate, Order_T.OrderStatus, Order_T.sPrice, Order_T.OrderRefundDate
                                                    FROM Order_T
                                                    INNER JOIN Customer_T ON Customer_T.CustomerID = Order_T.CustomerID
                                                    INNER JOIN Branch_T ON Branch_T.BranchID = Order_T.BranchID
                                                    WHERE Order_T.OrderID = "+ OrderID.ToString()+ ";";

            try
            {
                using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
                    MySqlConn.Open();

                    MyReader = MyCommand.ExecuteReader();
                    if (MyReader.HasRows == true)
                    {
                        while (MyReader.Read())
                        {
                            // Get a temp Order object and fill its values with the values read in from the MySqlDataReader .
                            TmpOrder = new Order();
                            TmpOrder.CustomerID = MyReader.GetInt16(0);
                            TmpOrder.CustomerName = MyReader.GetString(1);
                            TmpOrder.BranchName = MyReader.GetString(2);
                            TmpOrder.OrderDate = MyReader.GetDateTime(3).ToShortDateString();
                            TmpOrder.OrderStatus = MyReader.GetString(4);
                            TmpOrder.sPrice = MyReader.GetDecimal(5).ToString("N2");

                            if (TmpOrder.OrderStatus == "RFND")
                            {
                                TmpOrder.OrderRefundDate = MyReader.GetDateTime(6);
                            }
                        }
                    }
                    else
                    {
                        Logger.LogError($"Unable to find specific Order in the database - MyReader returned 0 rows when asked to retrieve OrderID: {OrderID.ToString()}");
                        throw new Exception($"Unable to find specific Order in the database - MyReader returned 0 rows when asked to retrieve OrderID: {OrderID.ToString()}");
                    }

                    MySqlConn.Close();

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                throw new Exception(e.Message);
            }

            Logger.LogStatus($"Successfully retrieved OrderID{OrderID.ToString()}.");

            return TmpOrder;
        }

        public List<OrderLineDetails> GetOrderLineDetails(int OrderID)
        {
            List<OrderLineDetails> MyOrderLineDetails = new List<OrderLineDetails>();

            string SqlCommand = @"SELECT OrderLineID, Product_T.ProductName, Product_T.ProductType, Product_T.wPrice, QuantityOrdered
                                                    FROM OrderLine_T
                                                    INNER JOIN Product_T ON Product_T.ProductSKU = OrderLine_T.ProductSKU
                                                    WHERE OrderLine_T.OrderID = "+ OrderID.ToString() + ";";

            try
            {
                using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
                    MySqlConn.Open();

                    MyReader = MyCommand.ExecuteReader();
                    if (MyReader.HasRows == true)
                    {
                        while (MyReader.Read())
                        {
                            // Get a temp Order object and fill its values with the values read in from the MySqlDataReader .
                            OrderLineDetails tmp = new OrderLineDetails();
                            tmp.OrderLineID = MyReader.GetInt16(0);
                            tmp.ProductName = MyReader.GetString(1);
                            tmp.ProductType = MyReader.GetString(2);
                            tmp.ProductCost = (MyReader.GetDecimal(3) * (decimal) 1.40).ToString("N2");
                            tmp.QuantityOrdered = MyReader.GetInt16(4);

                            tmp.TotalCost = ((MyReader.GetDecimal(3) * (decimal)1.40) * tmp.QuantityOrdered).ToString("N2");

                            // Add a copy of tmp to the order list, to be returned as a collection of Orders.
                            MyOrderLineDetails.Add(tmp);
                        }

                    }
                    else
                    {
                        Logger.LogError(@"Unable to find OrderLines matching OrderID" +OrderID.ToString() + " in the database - MyReader returned 0 rows when asked to retrieve a list of OrderLines matching this OrderId.");
                        throw new Exception("Unable to find OrderLines matching OrderID" + OrderID.ToString() + " in the database - MyReader returned 0 rows when asked to retrieve a list of OrderLines matching this OrderId.");
                    }

                    MySqlConn.Close();

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                throw new Exception(e.Message);
            }

            Logger.LogStatus($"Successfully retrieved a list of OrderLines matching OrderID:{OrderID.ToString()} ");

            return MyOrderLineDetails;
        }

        //public List<Product> GetStockLevels()
        //{
        //    List<Product> StockLevels = new List<Product>();

        //    string SqlCommand = @"SELECT * FROM MSWally.Product_T";

        //    try
        //    {
        //        using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
        //        {
        //            MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
        //            MySqlConn.Open();

        //            MyReader = MyCommand.ExecuteReader();
        //            if (MyReader.HasRows == true)
        //            {
        //                while (MyReader.Read())
        //                {
        //                    // Get a temp Product object and fill its values with the values read in from the MySqlDataReader .
        //                    Product tmp = new Product();
        //                    tmp.ProductName = MyReader.GetString(0);
        //                    tmp.StockCount = MyReader.GetInt16(1).ToString();

        //                    // Add a copy of tmp to the customer list, to be returned as a collection of Customers.
        //                    StockLevels.Add(tmp);
        //                }

        //            }
        //            else
        //            {
        //                Logger.LogError(@"Unable to find any Products with stock in the database - MyReader returned 0 rows when asked to retrieve a list of Products with stock.");
        //                throw new Exception(@"Unable to find any Products with stock in the database - MyReader returned 0 rows when asked to retrieve a list of Products with stock.");
        //            }

        //            MySqlConn.Close();

        //        }
                
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.LogError(e.Message);
        //        throw new Exception(e.Message);
        //    }

        //    Logger.LogStatus($"Successfully retrieved Stock List containing for all products");

        //    return StockLevels;
        //}
        

        public bool AddNewCustomer(Customer NewCustomer)
        {
            bool Result = false;

            string SqlCommand = $"" +
                $"INSERT INTO MSWally.Customer_T (FirstName, LastName, PhoneNumber)" +
                $"VALUES ('{NewCustomer.FirstName}', '{NewCustomer.LastName}', '{NewCustomer.PhoneNumber}')";

            Logger.LogStatus($"Attempting to insert customer {NewCustomer.ToString()} into database.");

            try
            {
                using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
                    MySqlConn.Open();

                    int NumberRowsAffected = MyCommand.ExecuteNonQuery();

                    if (NumberRowsAffected >= 1)
                    {
                        // I am using this as a check to see if the command worked.
                        Result = true;
                        Logger.LogStatus($"Successfully added Customer : {NewCustomer.FirstName + " " + NewCustomer.LastName + " " + NewCustomer.PhoneNumber} to database.");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Exception caught while attemping to insert new Customer. Details: {e.Message}");
            }

            return Result;
        }

        /*
        **	Method Name:	        AddNewOrder()
        **	Parameters:		
        **	Return Values:	
        **	Description:	        This method uses a method of parameterized MySql Statement that was insipired by a post I found on StackOverflow. Thanks to Thorsten Dittmar. https://stackoverflow.com/questions/1032495/insert-datetime-value-in-sql-database-with-c-sharp/1032519, Retrieved December 5, 2019 By Matthew Schatz.
        */
        public bool AddNewOrder(int CustomerID, int BranchID, decimal sPrice, List<OrderLine> NewOrderLines)
        {
            bool Result = false;

            DateTime dt = DateTime.Now;
            string SqlCommand = "" +
               $"INSERT INTO MSWally.Order_T (CustomerID, BranchID, OrderDate, OrderStatus, sPrice) " +
               $"VALUES ('{CustomerID.ToString()}', '{BranchID.ToString()}', '{dt.ToString("yyyy-MM-dd")}', 'PAID', {sPrice.ToString("N2")})";

            Logger.LogStatus("Attempting to insert new order into database.");

            try
            {
                using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
                    MySqlConn.Open();

                    int NumberRowsAffected = MyCommand.ExecuteNonQuery();

                    if (NumberRowsAffected >= 1)
                    {
                        // I am using this as a check to see if the command worked.
                        Logger.LogStatus($"Successfully added order to database.");
                    }
                }

                int LatestOrderID = DetermineLatestOrderID();

                int NewOrderID = LatestOrderID;
                int OrderLineCounter = 1;

                // Updating the OrderLine table in the data base.
                foreach (OrderLine OL in NewOrderLines)
                {
                    SqlCommand = "" +
                       $"INSERT INTO MSWally.OrderLine_T (OrderID, OrderLineID, ProductSKU, QuantityOrdered) " +
                       $"VALUES ({NewOrderID.ToString()}, {OrderLineCounter.ToString()}, {OL.Product.ProductSKU.ToString()}, {OL.QuantityOrdered.ToString()});";

                    OrderLineCounter++;

                    using (MySqlConnection MySqlConn2 = new MySqlConnection(Properties.Settings.Default.connStr))
                    {
                        MyCommand = new MySqlCommand(SqlCommand, MySqlConn2);
                        MySqlConn2.Open();

                        int NumberRowsAffected2 = MyCommand.ExecuteNonQuery();

                        if (NumberRowsAffected2 >= 1)
                        {
                            // I am using this as a check to see if the command worked.
                            Result = true;
                            Logger.LogStatus($"Successfully added OrderLine[{OrderLineCounter.ToString()}] to database.");
                        }
                    }

                }

                // Now going through and updating stock levels in product table.
                foreach (OrderLine OL in NewOrderLines)
                {
                    SqlCommand = "" +
                        $"UPDATE MSWally.Product_T " +
                        $"SET Stock = Stock - {OL.QuantityOrdered.ToString()} " +
                        $"WHERE ProductSKU = {OL.Product.ProductSKU.ToString()};";

                    using (MySqlConnection MySqlConn2 = new MySqlConnection(Properties.Settings.Default.connStr))
                    {
                        MyCommand = new MySqlCommand(SqlCommand, MySqlConn2);
                        MySqlConn2.Open();

                        int NumberRowsAffected2 = MyCommand.ExecuteNonQuery();

                        if (NumberRowsAffected2 >= 1)
                        {
                            // I am using this as a check to see if the command worked.
                            Result = true;
                            Logger.LogStatus($"Successfully updated Stock level for {OL.Product.ProductName}. Stock decreased by {OL.QuantityOrdered.ToString()}");
                        }
                        else
                        {
                            Logger.LogStatus($"No stock records were updated in Products table");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Result = false;
                Logger.LogError($"Exception caught while attemping to insert new Order. Details: {e.Message}");
            }

            return Result;
        }

        public int DetermineLatestOrderID()
        {
            int Result = 0;

            string SqlCommand = @"SELECT MAX(OrderID) FROM MSWally.Order_T;";
            try
            {
                using (MySqlConnection MySqlConn = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConn);
                    MySqlConn.Open();

                    MyReader = MyCommand.ExecuteReader();
                    if (MyReader.HasRows == true)
                    {
                        while (MyReader.Read())
                        {
                            // Get a temp Product object and fill its values with the values read in from the MySqlDataReader .
                            Result = MyReader.GetInt16(0);
                        }
                    }
                    else
                    {
                        Logger.LogError(@"Unable to find any orderIDs in the database - MyReader returned 0 rows when asked to retrieve MAX(OrderID).");
                        throw new Exception("Unable to find any orderIDs in the database - MyReader returned 0 rows when asked to retrieve MAX(OrderID)");
                    }
                }
            }

            catch (Exception e)
            {
                Logger.LogError(e.Message);
                throw new Exception(e.Message);
            }

            Logger.LogStatus($"Succesfully determined latest OrderID to be {Result.ToString()}");
            return Result;

        }


        public void RefundAnOrder(int OrderID)
        {
            string SqlCommand;

            List<OrderLineDetails> MyOrderLineDetails = GetOrderLineDetails(OrderID);

            // Restoring the stock amounts from a refunded order.
            foreach (OrderLineDetails OLD in MyOrderLineDetails)
            {
                SqlCommand = "" +
                    $"UPDATE MSWally.Product_T " +
                    $"SET Stock = Stock + {OLD.QuantityOrdered.ToString()} " +
                    $"WHERE ProductName = '{OLD.ProductName}';";

                using (MySqlConnection MySqlConn2 = new MySqlConnection(Properties.Settings.Default.connStr))
                {
                    MyCommand = new MySqlCommand(SqlCommand, MySqlConn2);
                    MySqlConn2.Open();

                    int NumberRowsAffected2 = MyCommand.ExecuteNonQuery();

                    if (NumberRowsAffected2 >= 1)
                    {
                        // I am using this as a check to see if the command worked.
                        Logger.LogStatus($"Successfully updated Stock level for {OLD.ProductName}. Stock increased by {OLD.QuantityOrdered.ToString()}");
                    }
                    else
                    {
                        Logger.LogStatus($"No stock records were updated in Products table");
                    }
                }
            }

            // Adjusting the order status to RFND (refund).
            SqlCommand = "" +
                    $"UPDATE MSWally.Order_T " +
                    $"SET OrderStatus = 'RFND',  OrderRefundDate = '{DateTime.Now.ToString("yyyy-MM-dd")}' " +
                    $"WHERE OrderID = {OrderID.ToString()};";

            using (MySqlConnection MySqlConn2 = new MySqlConnection(Properties.Settings.Default.connStr))
            {
                MyCommand = new MySqlCommand(SqlCommand, MySqlConn2);
                MySqlConn2.Open();

                int NumberRowsAffected2 = MyCommand.ExecuteNonQuery();

                if (NumberRowsAffected2 >= 1)
                {
                    // I am using this as a check to see if the command worked.
                    Logger.LogStatus($"Successfully updated OrderStatus for OrderID: {OrderID.ToString()}. Now set to RFND.");
                }
                else
                {
                    Logger.LogStatus($"Unable to update OrderStatus for Order[{OrderID.ToString()}].");
                }

            }
        }
    }
}
