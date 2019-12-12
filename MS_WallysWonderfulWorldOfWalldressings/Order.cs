using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    public class Order
    {
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

       public string CustomerName { get; set; }

        public int BranchID { get; set; }

        public string BranchName { get; set; }

        public string OrderDate { get; set; }

        public string OrderStatus { get; set; }

        // Final sale price, what the customer must pay.
        public string sPrice { get; set; }

        public DateTime OrderRefundDate { get; set; }
    }
}
