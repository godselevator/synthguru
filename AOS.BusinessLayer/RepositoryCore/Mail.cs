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
        IList<Mail> GetAllMails();
        Mail GetMailById(int Id);
        Mail GetMailByTextId(int TextId);
        void AddMail(params Mail[] Mail);
        void UpdateMail(params Mail[] Mail);
        void RemoveMail(params Mail[] Mail);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Mail> GetAllMails()
        {
            return _MailRepository.GetAll();
        }

        public Mail GetMailById(int Id)
        {
            return _MailRepository.GetSingle(d => d.MailID.Equals(Id));
        }

        public Mail GetMailByTextId(int TextId)
        {
            return _MailRepository.GetSingle(d => d.TextID.Equals(TextId));
        }

        public void AddMail(params Mail[] Mail)
        {
            /* Validation and error handling omitted */
            _MailRepository.Add(Mail);
        }

        public void UpdateMail(params Mail[] Mail)
        {
            /* Validation and error handling omitted */
            _MailRepository.Update(Mail);
        }

        public void RemoveMail(params Mail[] Mail)
        {
            /* Validation and error handling omitted */
            _MailRepository.Remove(Mail);
        }
    }
}
