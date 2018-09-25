using SynthGuru.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynthGuru.Web.Models
{
    public class SynthViewModel
    {
        public IList<SynthModel> SynthList { get; set; }
    }
}