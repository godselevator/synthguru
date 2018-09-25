using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.DataAccessLayer
{
    public class TestServiceConnectionResponse
    {
        public TestServiceConnectionResponse()
        {
            NetServerVersion = "";
            DatabaseVersion = "";
            SerialNumber = "";
            DatabaseType = "";
            CompanyName = "";
            CompanyId = 0;
            WsVersion = "";
            Version = "";
            Error = "";
            Build = "";
            Url = "";
            TimeZoneId = 0;
            TimeZoneLocationCode = "";
            Ticket = string.Empty;
            AssociateId = 0;
        }

        public string Url { get; set; }
        public string Build { get; set; }
        public string Error { get; set; }
        public string Version { get; set; }
        public string NetServerVersion { get; set; }
        public string WsVersion { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public string SerialNumber { get; set; }
        public string DatabaseType { get; set; }
        public string DatabaseVersion { get; set; }
        public string TimeZoneLocationCode { get; set; }
        public int TimeZoneId { get; set; }
        public string Ticket { get; set; }
        public int AssociateId { get; set; }
    }

    public enum SOServiceVersion
    {
        Unknown,
        SO6,
        SO70,
        SO71,
        SO73,
        SO73SSL,
        SO75,
        SO75SSL,
        SO80,
        SO80SSL,
        SO81,
        SO81SSL,
        SO82,
        SO82SSL
    }

}
