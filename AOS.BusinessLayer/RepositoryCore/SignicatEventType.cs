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
        IList<SignicatEventType> GetAllSignicatEventTypes();
        void AddSignicatEventType(params SignicatEventType[] SignicatEventTypes);
        void RemoveSignicatEventType(params SignicatEventType[] SignicatEventTypes);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatEventType> GetAllSignicatEventTypes()
        {
            return _SignicatEventTypeRepository.GetAll();
        }

        public void AddSignicatEventType(params SignicatEventType[] SignicatEventTypes)
        {
            _SignicatEventTypeRepository.Add(SignicatEventTypes);
        }

        public void RemoveSignicatEventType(params SignicatEventType[] SignicatEventTypes)
        {
            _SignicatEventTypeRepository.Remove(SignicatEventTypes);
        }
    }
}
