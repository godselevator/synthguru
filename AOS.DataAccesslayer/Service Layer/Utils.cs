using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.DataAccessLayer
{
    public static class Utils
    {
        public static string GetDBConnectionString()
        {
            using (var context = new AOSEntities())
            {
                return context.Database.Connection.ConnectionString;
            }
        }
    }
}
