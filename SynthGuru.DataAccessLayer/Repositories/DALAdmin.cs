using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthGuru.DataAccessLayer.Repositories
{
    public class DALAdmin
    {
        public void ResetDatabase()
        {
            using (var context = new SynthGuruEntities())
            {
                // Delete SynthModels
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.SynthModel");
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.Manufacturer");
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Manufacturer', RESEED, 0)");
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('SynthModel', RESEED, 0)");
            }
        }
    }
}
