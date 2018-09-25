using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public class SOConnectionAndCredentials
    {
        public Connection Connection { get; set; }
        public string Ticket { get; set; }
        public int AssociateId { get; set; }
    }
}
