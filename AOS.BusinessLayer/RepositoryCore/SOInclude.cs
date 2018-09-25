using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SOInclude> GetAllSOIncludes();
        IList<SOInclude> GetSOIncludeByTableName(string TableName);
        void AddSOInclude(params SOInclude[] SOIncludes);
        void RemoveSOInclude(params SOInclude[] SOIncludes);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SOInclude> GetAllSOIncludes()
        {
            return _SOIncludeRepository.GetAll();
        }

        public IList<SOInclude> GetSOIncludeByTableName(string TableName)
        {
            return _SOIncludeRepository.GetList(d => d.TableName.ToLower().Equals(TableName.ToLower()));
        }

        public void AddSOInclude(params SOInclude[] SOIncludes)
        {
            /* Validation and error handling omitted */

            _SOIncludeRepository.Add(SOIncludes);
        }

        public void RemoveSOInclude(params SOInclude[] SOIncludes)
        {
            /* Validation and error handling omitted */
            _SOIncludeRepository.Remove(SOIncludes);
        }
    }
}
