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
        IList<SignicatEventOrigin> GetAllSignicatEventOrigins();
        void AddSignicatEventOrigin(params SignicatEventOrigin[] SignicatEventOrigins);
        void RemoveSignicatEventOrigin(params SignicatEventOrigin[] SignicatEventOrigins);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatEventOrigin> GetAllSignicatEventOrigins()
        {
            return _SignicatEventOriginRepository.GetAll();
        }

        public void AddSignicatEventOrigin(params SignicatEventOrigin[] SignicatEventOrigins)
        {
            _SignicatEventOriginRepository.Add(SignicatEventOrigins);
        }

        public void RemoveSignicatEventOrigin(params SignicatEventOrigin[] SignicatEventOrigins)
        {
            _SignicatEventOriginRepository.Remove(SignicatEventOrigins);
        }
    }
}
