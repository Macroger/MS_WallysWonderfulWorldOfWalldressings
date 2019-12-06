using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    public class Product
    {
        public int ProductSKU { get; set; }

        public string ProductName { get; set; }

        public string sPrice { get; set; }

        public string ProductType { get; set; }

        public string StockCount { get; set; }

        public override string ToString()
        {
            return ProductName;
        }
    }
}
