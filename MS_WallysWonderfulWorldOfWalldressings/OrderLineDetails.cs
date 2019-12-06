using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    public class OrderLineDetails
    {
        public int OrderLineID { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }

        public string ProductCost { get; set; }

        public int QuantityOrdered { get; set; }

        public string TotalCost { get; set; }
    }
}
