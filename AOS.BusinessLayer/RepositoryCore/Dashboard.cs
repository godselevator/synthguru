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
        IList<Dashboard> GetAllDashboards();
        Dashboard GetDashboardById(int DashboardId);
        Dashboard GetDashboardByAccountId(int accountId);
        Dashboard GetDashboardByAPIKey(string APIKey);
        void AddDashboard(params Dashboard[] Dashboards);
        void UpdateDashboard(params Dashboard[] Dashboards);
        void RemoveDashboard(params Dashboard[] Dashboards);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Dashboard> GetAllDashboards()
        {
            return _DashboardRepository.GetAll();
        }

        public Dashboard GetDashboardById(int DashboardId)
        {
            return _DashboardRepository.GetSingle(d => d.DashboardID.Equals(DashboardId));
        }

        public Dashboard GetDashboardByAccountId(int accountId)
        {
            return _DashboardRepository.GetSingle(d => d.AccountID.Equals(accountId));
        }

        public Dashboard GetDashboardByAPIKey(string APIKey)
        {
            return _DashboardRepository.GetSingle(d => d.APIKey.Equals(APIKey));
        }

        public void AddDashboard(params Dashboard[] Dashboards)
        {
            /* Validation and error handling omitted */
            _DashboardRepository.Add(Dashboards);
        }

        public void UpdateDashboard(params Dashboard[] Dashboards)
        {
            /* Validation and error handling omitted */
            _DashboardRepository.Update(Dashboards);
        }

        public void RemoveDashboard(params Dashboard[] Dashboards)
        {
            /* Validation and error handling omitted */
            _DashboardRepository.Remove(Dashboards);
        }
    }
}
