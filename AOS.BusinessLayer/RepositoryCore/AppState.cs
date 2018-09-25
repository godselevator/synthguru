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
        IList<AppState> GetAllAppStates();
        AppState GetAppStateById(int AppStateId);
        void AddAppState(params AppState[] AppStates);
        void UpdateAppState(params AppState[] AppStates);
        void RemoveAppState(params AppState[] AppStates);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<AppState> GetAllAppStates()
        {
            return _AppStateRepository.GetAll();
        }

        public AppState GetAppStateById(int AppStateId)
        {
            return _AppStateRepository.GetSingle(d => d.AppStateID.Equals(AppStateId));
        }

        public void AddAppState(params AppState[] AppStates)
        {
            _AppStateRepository.Add(AppStates);
        }

        public void UpdateAppState(params AppState[] AppStates)
        {
            _AppStateRepository.Update(AppStates);
        }

        public void RemoveAppState(params AppState[] AppStates)
        {
            /* Validation and error handling omitted */
            _AppStateRepository.Remove(AppStates);
        }
    }
}
