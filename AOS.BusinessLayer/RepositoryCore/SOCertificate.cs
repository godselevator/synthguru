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
        SOCertificate GetSOCertificate();
        SOCertificate GetSOCertificateByEnvironment(string environment);
        void AddSOCertificate(params SOCertificate[] SOCertificates);
        void UpdateSOCertificate(params SOCertificate[] SOCertificates);
        void RemoveSOCertificate(params SOCertificate[] SOCertificates);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public SOCertificate GetSOCertificate()
        {
            var list = _SOCertificateRepository.GetAll();

            return (list.Count == 0) ? null : list[0];
        }

        public SOCertificate GetSOCertificateByEnvironment(string environment)
        {
            return _SOCertificateRepository.GetSingle(d => d.Environment.ToUpper().Equals(environment.ToUpper()));
        }

        public void AddSOCertificate(params SOCertificate[] SOCertificates)
        {
            /* Validation and error handling omitted */
            _SOCertificateRepository.Add(SOCertificates);
        }

        public void UpdateSOCertificate(params SOCertificate[] SOCertificates)
        {
            /* Validation and error handling omitted */
            _SOCertificateRepository.Update(SOCertificates);
        }

        public void RemoveSOCertificate(params SOCertificate[] SOCertificates)
        {
            /* Validation and error handling omitted */
            _SOCertificateRepository.Remove(SOCertificates);
        }
    }
}
