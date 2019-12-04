using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_WallysWonderfulWorldOfWalldressings
{
    public class Customer
    {
        
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }


        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public int GetPhoneNumberAsInt()
        {
            return int.Parse(PhoneNumber);
        }

        public override bool Equals(object obj)
        {
            bool Result = false;

            if(this.FirstName == (obj as Customer).FirstName)
            {
                if(this.LastName == (obj as Customer).LastName)
                {
                    if(this.PhoneNumber == (obj as Customer).PhoneNumber)
                    {
                        Result = true;
                    }
                }
            }

            return Result;
        }
    }
}
