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
        IList<PBXDial> GetAllPBXDials();
        PBXDial GetPBXDialByAppId(int AppId);
        void AddPBXDial(params PBXDial[] PBXDials);
        void UpdatePBXDial(params PBXDial[] PBXDials);
        void RemovePBXDial(params PBXDial[] PBXDials);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<PBXDial> GetAllPBXDials()
        {
            return _PBXDialRepository.GetAll();
        }

        public PBXDial GetPBXDialByAppId(int AppId)
        {
            return _PBXDialRepository.GetSingle(d => d.AppID.Equals(AppId));
        }

        public void AddPBXDial(params PBXDial[] PBXDials)
        {
            /* Validation and error handling omitted */
            _PBXDialRepository.Add(PBXDials);
        }

        public void UpdatePBXDial(params PBXDial[] PBXDials)
        {
            /* Validation and error handling omitted */
            _PBXDialRepository.Update(PBXDials);
        }

        public void RemovePBXDial(params PBXDial[] PBXDials)
        {
            /* Validation and error handling omitted */
            _PBXDialRepository.Remove(PBXDials);
        }
    }
}
