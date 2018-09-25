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
        IList<PDFManager> GetAllPDFManagers();
        PDFManager GetPDFManagerById(int PDFManagerId);
        PDFManager GetPDFManagerByAccountId(int accountId);
        PDFManager GetPDFManagerByAccountIDAndAppID(int accountID, int appID);
        void AddPDFManager(params PDFManager[] PDFManagers);
        void UpdatePDFManager(params PDFManager[] PDFManagers);
        void RemovePDFManager(params PDFManager[] PDFManagers);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<PDFManager> GetAllPDFManagers()
        {
            return _PDFManagerRepository.GetAll();
        }

        public PDFManager GetPDFManagerById(int PDFManagerId)
        {
            return _PDFManagerRepository.GetSingle(d => d.PDFManagerID.Equals(PDFManagerId));
        }

        public PDFManager GetPDFManagerByAccountId(int accountId)
        {
            return _PDFManagerRepository.GetSingle(d => d.AccountID.Equals(accountId));
        }

        public PDFManager GetPDFManagerByAccountIDAndAppID(int accountID, int appID)
        {
            return _PDFManagerRepository.GetSingle(d => d.AccountID.Equals(accountID) && d.AppID.Equals(appID));
        }

        public void AddPDFManager(params PDFManager[] PDFManagers)
        {
            /* Validation and error handling omitted */

            // Fill createdate and createuser fields
            foreach (var item in PDFManagers)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            _PDFManagerRepository.Add(PDFManagers);
        }

        public void UpdatePDFManager(params PDFManager[] PDFManagers)
        {
            /* Validation and error handling omitted */

            // Fill updatedate and updateuser fields
            foreach (var item in PDFManagers)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _PDFManagerRepository.Update(PDFManagers);
        }

        public void RemovePDFManager(params PDFManager[] PDFManagers)
        {
            /* Validation and error handling omitted */
            _PDFManagerRepository.Remove(PDFManagers);
        }
    }
}
