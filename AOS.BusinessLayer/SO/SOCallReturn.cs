using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public class SOCallReturn<T>
    {
        public T ReturnValue { get; set; }
        public bool IsOK { get; set; }
        public string ErrorMsg { get; set; }

        public SOCallReturn()
        {
            IsOK = true;
        }
    }
}
