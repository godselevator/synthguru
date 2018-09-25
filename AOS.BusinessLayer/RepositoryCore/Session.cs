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
        Session GetSessionByAppId(int AppId);
        void AddSession(params Session[] Sessions);
        void RemoveSession(params Session[] Sessions);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public Session GetSessionByAppId(int AppId)
        {
            return _SessionRepository.GetSingle(d => d.AppID.Equals(AppId));
        }

        public void AddSession(params Session[] Sessions)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in Sessions)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SessionRepository.Add(Sessions);
        }

        public void RemoveSession(params Session[] Sessions)
        {
            /* Validation and error handling omitted */
            _SessionRepository.Remove(Sessions);
        }
    }
}
