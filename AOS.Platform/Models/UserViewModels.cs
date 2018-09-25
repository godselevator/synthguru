using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AOS.Platform.Models
{
    public class UserHomeViewModel
    {
        public IList<AppExtended> MyAppsExtended { get; set; }
        public IList<App> AvailableApps { get; set; }
        public bool EndpointAlive { get; set; }
        public string EndpointStatusText { get; set; }
        public bool IsNormalUser { get; set; }
        public bool IsSOOnlineAccount { get; set; }
    }

    public class TermsOfServiceViewModel
    {
        public string TermsOfServiceText { get; set; }
    }
}