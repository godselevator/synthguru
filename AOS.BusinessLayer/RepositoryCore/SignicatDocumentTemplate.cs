using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<SignicatDocumentTemplate> GetAllSignicatDocumentTemplates();
        SignicatDocumentTemplate GetSignicatDocumentTemplateById(int id);
        IList<SignicatDocumentTemplate> GetSignicatDocumentTemplateByAccountId(int? accountId);
        IList<SignicatDocumentTemplate> GetSignicatDocumentTemplateBySODocTemplateIDList(params int[] SODocTemplateIDList);
        void AddSignicatDocumentTemplate(params SignicatDocumentTemplate[] SignicatDocumentTemplates);
        void UpdateSignicatDocumentTemplate(params SignicatDocumentTemplate[] SignicatDocumentTemplates);
        void RemoveSignicatDocumentTemplate(params SignicatDocumentTemplate[] SignicatDocumentTemplates);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<SignicatDocumentTemplate> GetAllSignicatDocumentTemplates()
        {
            return _SignicatDocumentTemplateRepository.GetAll();
        }

        public SignicatDocumentTemplate GetSignicatDocumentTemplateById(int id)
        {
            return _SignicatDocumentTemplateRepository.GetSingle(d => d.SignicatDocumentTemplateID.Equals(id));
        }

        public IList<SignicatDocumentTemplate> GetSignicatDocumentTemplateByAccountId(int? accountId)
        {
            return _SignicatDocumentTemplateRepository.GetList(d => d.AccountID.Equals(accountId));
        }

        public IList<SignicatDocumentTemplate> GetSignicatDocumentTemplateBySODocTemplateIDList(params int[] SODocTemplateIDList)
        {
            return _SignicatDocumentTemplateRepository.GetList(d => SODocTemplateIDList.Contains(d.SODocTemplateID));
        }

        public void AddSignicatDocumentTemplate(params SignicatDocumentTemplate[] SignicatDocumentTemplates)
        {
            /* Validation and error handling omitted */

            // Set createdate and createuser
            foreach (var item in SignicatDocumentTemplates)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _SignicatDocumentTemplateRepository.Add(SignicatDocumentTemplates);
        }

        public void UpdateSignicatDocumentTemplate(params SignicatDocumentTemplate[] SignicatDocumentTemplates)
        {
            /* Validation and error handling omitted */
            foreach (var item in SignicatDocumentTemplates)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _SignicatDocumentTemplateRepository.Update(SignicatDocumentTemplates);
        }

        public void RemoveSignicatDocumentTemplate(params SignicatDocumentTemplate[] SignicatDocumentTemplates)
        {
            /* Validation and error handling omitted */
            _SignicatDocumentTemplateRepository.Remove(SignicatDocumentTemplates);
        }
    }
}
