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
        IList<DocArcAssociation> GetAllDocArcAssociations();
        IList<DocArcAssociation> GetDocArcAssociationsByAccountId(int accountId);
        DocArcAssociation GetDocArcAssociationById(int id);
        void AddDocArcAssociation(params DocArcAssociation[] DocArcAssociations);
        void UpdateDocArcAssociation(params DocArcAssociation[] DocArcAssociations);
        void RemoveDocArcAssociation(params DocArcAssociation[] DocArcAssociations);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<DocArcAssociation> GetAllDocArcAssociations()
        {
            return _DocArcAssociationRepository.GetAll();
        }

        public IList<DocArcAssociation> GetDocArcAssociationsByAccountId(int accountId)
        {
            return _DocArcAssociationRepository.GetList(d => d.AccountId.Equals(accountId));
        }

        public DocArcAssociation GetDocArcAssociationById(int id)
        {
            return _DocArcAssociationRepository.GetSingle(d => d.AssociationId.Equals(id));
        }

        public void AddDocArcAssociation(params DocArcAssociation[] DocArcAssociations)
        {
            /* Validation and error handling omitted */
            _DocArcAssociationRepository.Add(DocArcAssociations);
        }

        public void UpdateDocArcAssociation(params DocArcAssociation[] DocArcAssociations)
        {
            /* Validation and error handling omitted */
            _DocArcAssociationRepository.Update(DocArcAssociations);
        }

        public void RemoveDocArcAssociation(params DocArcAssociation[] DocArcAssociations)
        {
            /* Validation and error handling omitted */
            _DocArcAssociationRepository.Remove(DocArcAssociations);
        }
    }
}
