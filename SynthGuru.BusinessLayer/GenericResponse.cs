using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthGuru.BusinessLayer
{
    public class GenericResponse
    {
        public bool IsOK { get; set; }
        public string ErrorMsg { get; set; }

        public GenericResponse()
        {
            IsOK = true;
            ErrorMsg = string.Empty;
        }
    }

    public class GenericResponse<T>
    {
        public bool IsOK { get; set; }
        public string ErrorMsg { get; set; }
        public T ReturnObj { get; set; }

        public GenericResponse()
        {
            IsOK = true;
            ErrorMsg = string.Empty;
        }
    }
}
