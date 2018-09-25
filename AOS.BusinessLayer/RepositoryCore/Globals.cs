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
        IList<Globals> GetAllGlobals();
        Globals GetGlobalsById(int Id);
        Globals GetGlobalsEnvironmentByAppId(int AppID);
        Globals GetGlobalsByEnvironmentType(string Environment);
        void AddGlobals(params Globals[] Globals);
        void UpdateGlobals(params Globals[] Globals);
        void RemoveGlobals(params Globals[] Globals);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Globals> GetAllGlobals()
        {
            return _GlobalsRepository.GetAll();
        }

        public Globals GetGlobalsById(int Id)
        {
            return _GlobalsRepository.GetSingle(d => d.Id.Equals(Id));
        }

        public Globals GetGlobalsEnvironmentByAppId(int AppID)
        {
            // Get App
            var currApp = GetAppById(AppID);

            if (currApp == null)
                return null;

            return _GlobalsRepository.GetSingle(d => d.Id.Equals(currApp.GlobalSettingsId));
        }

        public Globals GetGlobalsByEnvironmentType(string Environment)
        {
            return _GlobalsRepository.GetSingle(d => d.Environment.ToLower().Equals(Environment.ToLower()));
        }

        public void AddGlobals(params Globals[] Globals)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in Globals)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _GlobalsRepository.Add(Globals);
        }

        public void UpdateGlobals(params Globals[] Globals)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in Globals)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _GlobalsRepository.Update(Globals);
        }

        public void RemoveGlobals(params Globals[] Globals)
        {
            /* Validation and error handling omitted */
            _GlobalsRepository.Remove(Globals);
        }
    }
}
