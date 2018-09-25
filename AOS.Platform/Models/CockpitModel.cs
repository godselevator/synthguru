using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AOS.Platform.Models
{
    public class CockpitModel
    {
        public string CurrentAccountName { get; set; }
        public string CurrentAccountRole { get; set; }
        public string CurrentConnectionStatusIcon { get; set; }
        public string CurrentConnectionStatus { get; set; }
        public string CurrentAccountOwner { get; set; }
        public string CurrentAccountOwnerEmail { get; set; }
    }
}