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
        IList<DocArcTemplate> GetAllDocArcTemplates();
        IList<DocArcTemplate> GetDocArcTemplatesByAccountId(int accountId);
        DocArcTemplate GetDocArcTemplateById(int id);
        void AddDocArcTemplate(params DocArcTemplate[] DocArcTemplates);
        void UpdateDocArcTemplate(params DocArcTemplate[] DocArcTemplates);
        void RemoveDocArcTemplate(params DocArcTemplate[] DocArcTemplates);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<DocArcTemplate> GetAllDocArcTemplates()
        {
            return _DocArcTemplateRepository.GetAll();
        }

        public IList<DocArcTemplate> GetDocArcTemplatesByAccountId(int accountId)
        {
            return _DocArcTemplateRepository.GetList(d => d.AccountId.Equals(accountId));
        }

        public DocArcTemplate GetDocArcTemplateById(int id)
        {
            return _DocArcTemplateRepository.GetSingle(d => d.TemplateId.Equals(id));
        }

        public void AddDocArcTemplate(params DocArcTemplate[] DocArcTemplates)
        {
            /* Validation and error handling omitted */
            _DocArcTemplateRepository.Add(DocArcTemplates);
        }

        public void UpdateDocArcTemplate(params DocArcTemplate[] DocArcTemplates)
        {
            /* Validation and error handling omitted */
            _DocArcTemplateRepository.Update(DocArcTemplates);
        }

        public void RemoveDocArcTemplate(params DocArcTemplate[] DocArcTemplates)
        {
            /* Validation and error handling omitted */
            _DocArcTemplateRepository.Remove(DocArcTemplates);
        }
    }
}
