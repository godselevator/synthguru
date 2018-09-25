using AOS.DataAccessLayer;
using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        SOCallReturn<IAOSCredentials> SOGetCredentials();
        SOCallReturn<SOConnectionAndCredentials> SOGetInfo(string endpointURI, string sysUser, string sysPassword);
        SOCallReturn<List<Contact>> SOGetAllContacts();
        SOCallReturn<List<Contact>> SOGetContactsBySelectionId(string selectionId);
        SOCallReturn<List<Selection>> SOGetAllSelections();
        SOCallReturn<List<SelMember>> SOGetSelectionMembersBySelectionId(int selectionId);
        SOCallReturn<int> SOSavePrefDesc(string section, string key, string name);
        SOCallReturn<int> SOGetAccountIdInPreference(int userPrefId);
        SOCallReturn<int> SOCheckPreference();
        SOCallReturn<int> SOSaveAccountIdInPreference(int prefDescId, int accountId);
    }

    public partial class BusinessLayer : IBusinessLayer
    {

        public bool IsEndpointAlive(string url)
        {
            return this.ServiceLayer != null;
        }

        public SOCallReturn<IAOSCredentials> SOGetCredentials()
        {
            var returnVal = new SOCallReturn<IAOSCredentials>();

            if (this.ServiceLayer == null)
            {
                returnVal.ReturnValue = null;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            var credentials = this.ServiceLayer.SOGetCredentials(this.SOSysUser, this.SOSysPassword);

            returnVal.ReturnValue = credentials;

            return returnVal;
        }

        public SOCallReturn<SOConnectionAndCredentials> SOGetInfo(string endpointURI, string sysUser, string sysPassword)
        {
            var returnVal = new SOCallReturn<SOConnectionAndCredentials>();

            if (this.ServiceLayer == null)
            {
                returnVal.ReturnValue = null;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            if (this.SOCredentials.AuthenticationSucceeded)
            {
                var endpointInfo = this.ServiceLayer.TestConnection(endpointURI, sysUser, sysPassword);

                if (endpointInfo.Error != string.Empty)
                {
                    returnVal.ReturnValue = null;
                    returnVal.ErrorMsg = endpointInfo.Error;
                    return returnVal;
                }

                var conn = new Connection();

                var slVersion = this.ServiceLayer.GetType();

                conn.CompanyID = endpointInfo.CompanyId;
                conn.CompanyName = endpointInfo.CompanyName;
                conn.CompanySerial = endpointInfo.SerialNumber;
                conn.NetServer = endpointInfo.NetServerVersion;

                var strA = endpointInfo.NetServerVersion.ToLower().Replace("superoffice netserver ", "");
                var strB = strA.Substring(0, 3).Replace(".", "");

                conn.WSVersion = Convert.ToInt32(strB);

                // Strip alphanumeric characters from buildno.
                var validBuild = Regex.Replace(endpointInfo.Build, "[^0-9.]", "");
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                validBuild = rgx.Replace(validBuild, "");

                if (validBuild.Length > 4)
                    validBuild = validBuild.Substring(0, 4);

                conn.WSBuild = Convert.ToInt32(validBuild);
                conn.WSHttps = endpointInfo.Url.ToLower().Contains("https");

                var connAndTicket = new SOConnectionAndCredentials();
                connAndTicket.Connection = conn;
                connAndTicket.Ticket = endpointInfo.Ticket;

                returnVal.ReturnValue = connAndTicket;
                returnVal.ErrorMsg = endpointInfo.Error;

            }
            else
            {
                returnVal.ReturnValue = null;
                returnVal.ErrorMsg = "Endpoint connection could not be established. Check URL and credentials";
                return returnVal;
            }

            return returnVal;
        }

        public SOCallReturn<List<Selection>> SOFindSelections(string selName)
        {
            var returnVal = new SOCallReturn<List<Selection>>();

            if (this.ServiceLayer == null)
            {
                returnVal.ReturnValue = null;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Find selection
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.SOFindSelections(this.SOCredentials, selName);

            return returnVal;
        }

        public SOCallReturn<List<Contact>> SOGetAllContacts()
        {
            var returnVal = new SOCallReturn<List<Contact>>();

            if (this.ServiceLayer == null)
            {
                returnVal.ReturnValue = null;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Get all selections
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.SOGetAllContacts(this.SOCredentials);

            return returnVal;
        }

        public SOCallReturn<List<Contact>> SOGetContactsBySelectionId(string selectionId)
        {
            var returnVal = new SOCallReturn<List<Contact>>();

            if (this.ServiceLayer == null)
            {
                returnVal.ReturnValue = null;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Get all selections
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.SOGetContactsBySelectionId(this.SOCredentials, selectionId);

            return returnVal;
        }

        public SOCallReturn<List<Selection>> SOGetAllSelections()
        {
            var returnVal = new SOCallReturn<List<Selection>>();

            if (this.ServiceLayer == null)
            {
                returnVal.ReturnValue = null;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Get all selections
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.SOGetAllSelections(this.SOCredentials);

            return returnVal;
        }

        public SOCallReturn<List<SelMember>> SOGetSelectionMembersBySelectionId(int selectionId)
        {
            var returnVal = new SOCallReturn<List<SelMember>>();

            if (this.ServiceLayer == null)
            {
                returnVal.ReturnValue = null;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            var credentials = this.ServiceLayer.SOGetCredentials(this.SOSysUser, this.SOSysPassword);

            if (!credentials.AuthenticationSucceeded)
            {
                returnVal.ReturnValue = null;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            var selectionMembers = this.ServiceLayer.SOGetSelectionMembersBySelectionId(credentials, selectionId);

            returnVal.ReturnValue = selectionMembers;

            return returnVal;
        }

        public SOCallReturn<bool> SOGetPrefDescById(int prefDescId)
        {
            var returnVal = new SOCallReturn<bool>();
            returnVal.ReturnValue = false;

            if (this.ServiceLayer == null)
            {
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            if (!this.SOCredentials.AuthenticationSucceeded)
            {
                returnVal.ReturnValue = false;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Get PrefDesc
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.GetPrefDescById(this.SOCredentials, prefDescId);

            return returnVal;
        }

        public SOCallReturn<int> SOSavePrefDesc(string section, string key, string name)
        {
            var returnVal = new SOCallReturn<int>();
            returnVal.ReturnValue = 0;

            if (this.ServiceLayer == null)
            {
                returnVal.ReturnValue = 0;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            if (!this.SOCredentials.AuthenticationSucceeded)
            {
                returnVal.ReturnValue = -1;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Save prefdesc entry and get PrefDescId
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.SOSavePrefDesc(this.SOCredentials, section, key, name);

            return returnVal;
        }

        public SOCallReturn<int> SOGetAccountIdInPreference(int userPrefId)
        {
            var returnVal = new SOCallReturn<int>();
            returnVal.ReturnValue = 0;

            if (this.ServiceLayer == null)
            {
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            if (!this.SOCredentials.AuthenticationSucceeded)
            {
                returnVal.ReturnValue = -1;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Get Preference
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.SOGetAccountIdInPreference(this.SOCredentials, userPrefId);

            return returnVal;
        }

        public SOCallReturn<int> SOCheckPreference()
        {
            var returnVal = new SOCallReturn<int>();
            returnVal.ReturnValue = 0;

            if (this.ServiceLayer == null)
            {
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            if (!this.SOCredentials.AuthenticationSucceeded)
            {
                returnVal.ReturnValue = -1;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Get Preference
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.GetPreferenceBySectionAndKey(this.SOCredentials, "Adwiza.OnlineServices", "AccountId");

            return returnVal;
        }

        public SOCallReturn<int> SOSaveAccountIdInPreference(int prefDescId, int accountId)
        {
            var returnVal = new SOCallReturn<int>();
            returnVal.ReturnValue = 0;

            if (this.ServiceLayer == null)
            {
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            if (!this.SOCredentials.AuthenticationSucceeded)
            {
                returnVal.ReturnValue = -1;
                returnVal.IsOK = false;
                returnVal.ErrorMsg = "Endpoint connection could not be established";
                return returnVal;
            }

            // Get Preference
            if (this.SOCredentials.AuthenticationSucceeded)
                returnVal.ReturnValue = this.ServiceLayer.SOSaveAccountIdInPreference(this.SOCredentials, prefDescId, accountId);

            return returnVal;
        }

    }
}
