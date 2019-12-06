using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    public class OrderLine
    {                
        public int QuantityOrdered { get; set; }

        public decimal ExtendedPrice { get; set; }

        public Product Product { get; set; }

        public OrderLine(Product IncommingProduct, int IncommingQuantityOrdered)
        {
            QuantityOrdered = IncommingQuantityOrdered;
            Product = IncommingProduct;
            ExtendedPrice = Decimal.Parse(IncommingProduct.sPrice) * QuantityOrdered;
        }
        public OrderLine()
        {

        }
    }
}
