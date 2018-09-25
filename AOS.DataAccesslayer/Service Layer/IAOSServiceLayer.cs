using AOS.DomainModel;
using System.Collections.Generic;

namespace AOS.DataAccessLayer
{
    public interface IAOSServiceLayer
    {
        TestServiceConnectionResponse TestConnection(string url, string userName, string password);
        IAOSCredentials SOGetCredentials(string userName, string password);
        List<Selection> SOFindSelections(IAOSCredentials credentials, string searchStr);
        List<Contact> SOGetAllContacts(IAOSCredentials credentials);
        List<Contact> SOGetContactsBySelectionId(IAOSCredentials credentials, string selectionId);
        List<Selection> SOGetAllSelections(IAOSCredentials credentials);
        List<SelMember> SOGetSelectionMembersBySelectionId(IAOSCredentials credentials, int selectionId);
        bool GetPrefDescById(IAOSCredentials credentials, int prefDescId);
        int GetPreferenceBySectionAndKey(IAOSCredentials credentials, string section, string key);
        int SOSavePrefDesc(IAOSCredentials credentials, string section, string key, string name);
        int SOGetAccountIdInPreference(IAOSCredentials credentials, int userPrefId);
        int SOSaveAccountIdInPreference(IAOSCredentials credentials, int prefDescId, int accountId);
        int SOSaveContact(IAOSCredentials credentials, string name);
    }
}
