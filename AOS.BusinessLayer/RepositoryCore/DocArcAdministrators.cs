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
        IList<DocArcAdministrators> GetAllDocArcAdministrators();
        IList<DocArcAdministrators> GetDocArcAdministratorsByAccountId(int accountId);
        DocArcAdministrators GetDocArcAdministratorsById(int id);
        void AddDocArcAdministrators(params DocArcAdministrators[] DocArcAdministrators);
        void UpdateDocArcAdministrators(params DocArcAdministrators[] DocArcAdministrators);
        void RemoveDocArcAdministrators(params DocArcAdministrators[] DocArcAdministrators);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<DocArcAdministrators> GetAllDocArcAdministrators()
        {
            return _DocArcAdministratorsRepository.GetAll();
        }

        public IList<DocArcAdministrators> GetDocArcAdministratorsByAccountId(int accountId)
        {
            return _DocArcAdministratorsRepository.GetList(d => d.AccountId.Equals(accountId));
        }

        public DocArcAdministrators GetDocArcAdministratorsById(int id)
        {
            return _DocArcAdministratorsRepository.GetSingle(d => d.AdministratorId.Equals(id));
        }

        public void AddDocArcAdministrators(params DocArcAdministrators[] DocArcAdministrators)
        {
            /* Validation and error handling omitted */
            _DocArcAdministratorsRepository.Add(DocArcAdministrators);
        }

        public void UpdateDocArcAdministrators(params DocArcAdministrators[] DocArcAdministrators)
        {
            /* Validation and error handling omitted */
            _DocArcAdministratorsRepository.Update(DocArcAdministrators);
        }

        public void RemoveDocArcAdministrators(params DocArcAdministrators[] DocArcAdministrators)
        {
            /* Validation and error handling omitted */
            _DocArcAdministratorsRepository.Remove(DocArcAdministrators);
        }
    }
}
