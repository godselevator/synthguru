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
        IList<VisibleIn> GetAllVisibleIns();
        VisibleIn GetVisibleInById(int visibleInId);
        VisibleIn GetVisibleInByKey(int visibleInKey);
        IList<VisibleIn> GetVisibleInBySOVersion(int soVersion);
        void AddVisibleIn(params VisibleIn[] VisibleIns);
        void UpdateVisibleIn(params VisibleIn[] VisibleIns);
        void RemoveVisibleIn(params VisibleIn[] VisibleIns);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<VisibleIn> GetAllVisibleIns()
        {
            return _VisibleInRepository.GetAll();
        }

        public VisibleIn GetVisibleInById(int visibleInId)
        {
            return _VisibleInRepository.GetSingle(d => d.VisibleInID.Equals(visibleInId));
        }

        public VisibleIn GetVisibleInByKey(int visibleInKey)
        {
            return _VisibleInRepository.GetSingle(d => d.VisibleInKey.Equals(visibleInKey));
        }

        public IList<VisibleIn> GetVisibleInBySOVersion(int soVersion)
        {
            return _VisibleInRepository.GetList(d => d.SOVersion.Equals(soVersion));
        }

        public void AddVisibleIn(params VisibleIn[] VisibleIns)
        {
            /* Validation and error handling omitted */
            _VisibleInRepository.Add(VisibleIns);
        }

        public void UpdateVisibleIn(params VisibleIn[] VisibleIns)
        {
            /* Validation and error handling omitted */
            _VisibleInRepository.Update(VisibleIns);
        }

        public void RemoveVisibleIn(params VisibleIn[] VisibleIns)
        {
            /* Validation and error handling omitted */
            _VisibleInRepository.Remove(VisibleIns);
        }
    }
}
