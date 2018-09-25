using AOS.DataAccessLayer.Contact75;
using AOS.DataAccessLayer.Find75;
using AOS.DataAccessLayer.Preference75;
using AOS.DataAccessLayer.SoPrincipal75;
using AOS.DataAccessLayer.User75;
using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AOS.DataAccessLayer
{
    public class AOSServiceLayer75 : IAOSServiceLayer
    {
        private bool _HttpsEnabled;
        public bool HttpsEnabled
        {
            get { return _HttpsEnabled; }
            set { _HttpsEnabled = value; }
        }

        private string _Endpoint;
        public string Endpoint
        {
            get { return _Endpoint; }
            set { _Endpoint = value; }
        }

        private string _SysUser;
        public string SysUser
        {
            get { return _SysUser; }
            set { _SysUser = value; }
        }

        private string _SysPassword;
        public string SysPassword
        {
            get { return _SysPassword; }
            set { _SysPassword = value; }
        }

        private AOSCredentialsBase _Credentials;
        public AOSCredentialsBase Credentials
        {
            get { return _Credentials; }
            set { _Credentials = value; }
        }

        // Constructor 1
        public AOSServiceLayer75(string endpoint)
        {
            Endpoint = endpoint;
        }

        // Constructor 2
        public AOSServiceLayer75(string endpoint, bool httpsEnabled)
        {
            Endpoint = endpoint;
            HttpsEnabled = httpsEnabled;
        }

        public TestServiceConnectionResponse TestConnection(string url, string userName, string password)
        {
            TestServiceConnectionResponse resp = new TestServiceConnectionResponse();

            resp.Url = url.TrimEnd('/');

            const string CONNECTION_ERROR_CREDENTIALS = "Connection was made to a <strong>SuperOffice 7</strong> service, but the credentials seems invalid.";

            bool success = false;

            SoPrincipal75.SoTimeZone tzone;
            SoPrincipal75.SoSystemInfoCarrier response;
            SoPrincipal75.SoExtraInfo extraInfo;

            // Service Endpoint
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_SoPrincipal75SSL" : "BasicHttpBinding_SoPrincipal75";
            SoPrincipal75.SoPrincipalClient client = new SoPrincipal75.SoPrincipalClient(bindingName);

            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(url.TrimEnd('/') + "/SoPrincipal.svc");

            // Service Call
            var credentials = SOGetCredentials(userName, password);

            if (credentials.AuthenticationSucceeded)
            {
                client.GetSystemInfo(string.Empty, out extraInfo, out success, out tzone, out response);

                if (success)
                {
                    resp.CompanyName = response.CompanyName;
                    resp.CompanyId = response.CompanyId;
                    resp.DatabaseType = response.DatabaseType;
                    resp.DatabaseVersion = response.DatabaseVersion.ToString();
                    resp.SerialNumber = response.License.SerialNr;
                    resp.Version = response.License.LicenseVersion;
                    resp.NetServerVersion = response.Description;
                    resp.Build = response.BuildLabel;
                    resp.Ticket = credentials.Credentials75.Ticket;
                }
            }
            else
            {
                resp.Error = CONNECTION_ERROR_CREDENTIALS;
            }

            client.Close();

            return resp;
        }

        public IAOSCredentials SOGetCredentials(string userName, string password)
        {
            // Service call
            try
            {
                bool success = false;
                bool authenticationSuceeded = false;

                // Service Header
                var bindingName = (HttpsEnabled) ? "BasicHttpBinding_SoPrincipal75SSL" : "BasicHttpBinding_SoPrincipal75";

                SoPrincipal75.SoPrincipalClient credClient = new SoPrincipal75.SoPrincipalClient(bindingName);
                SoPrincipal75.SoPrincipalCarrier responseCarrier = null;
                SoPrincipal75.SoCredentials credentials = null;
                SoPrincipal75.SoTimeZone timezone = null;
                SoPrincipal75.SoExtraInfo extraInfo = null;

                // Service Endpoint
                var url = String.Concat(Endpoint.TrimEnd('/'), "/SoPrincipal.svc");

                credClient.Endpoint.Address = new EndpointAddress(url);

                var exceptionInfo = credClient.AuthenticateUsernamePassword(string.Empty, userName, password, out extraInfo, out success, out timezone, out responseCarrier, out credentials, out authenticationSuceeded);

                // Return AOSCredentials object
                var returnVal = new AOSCred75()
                {
                    AuthenticationSucceeded = authenticationSuceeded,
                    Success = success,
                    ErrorMsg = "Ok",
                    Credentials75 = credentials,
                    ResponseCarrier75 = responseCarrier,
                    TimeZone75 = timezone
                };

                return returnVal;
            }
            catch (Exception ex)
            {
                var msg = string.Empty;

                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;

                return new AOSCred75()
                {
                    AuthenticationSucceeded = false,
                    Success = false,
                    ErrorMsg = msg,
                    Credentials75 = null,
                    ResponseCarrier75 = null,
                    TimeZone75 = null
                };
            }
        }

        public List<AOS.DomainModel.Contact> SOGetAllContacts(IAOSCredentials credentials)
        {
            List<AOS.DomainModel.Contact> returnVal = new List<AOS.DomainModel.Contact>();

            // Service header
            Find75.FindResults findList = null;

            Find75.SoTimeZone tzone = new Find75.SoTimeZone();
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Find75SSL" : "BasicHttpBinding_Find75";

            Find75.FindClient findClient = new FindClient(bindingName);
            Find75.SoCredentials cred = new Find75.SoCredentials();
            Find75.SoExtraInfo extraInfo = new Find75.SoExtraInfo();

            bool success;

            // Service Endpoint
            findClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint.TrimEnd('/') + "/Find.svc");

            // Restrictions
            Find75.ArchiveRestrictionInfo[] restrictions;
            List<Find75.ArchiveRestrictionInfo> listRestrictions = new List<Find75.ArchiveRestrictionInfo>();

            listRestrictions.Add(new Find75.ArchiveRestrictionInfo()
            {
                Name = "getAllRows",
                Operator = "set",
                Values = new string[] { "true" },
                IsActive = true
            });

            restrictions = listRestrictions.ToArray();

            List<string> columnsList = new List<string>();
            columnsList.Add("contactId");
            columnsList.Add("name");
            columnsList.Add("department");
            columnsList.Add("country");
            columnsList.Add("number");
            columnsList.Add("code");
            columnsList.Add("associateId");
            columnsList.Add("business");
            columnsList.Add("category");
            columnsList.Add("stop");
            columnsList.Add("contactNoMail");
            columnsList.Add("orgnr");
            columnsList.Add("postAddress/line1");
            columnsList.Add("postAddress/line2");
            columnsList.Add("postAddress/line3");
            columnsList.Add("postAddress/city");
            columnsList.Add("postAddress/zip");
            columnsList.Add("postAddress/state");
            columnsList.Add("postAddress/country");
            columnsList.Add("streetAddress/line1");
            columnsList.Add("streetAddress/line2");
            columnsList.Add("streetAddress/line3");
            columnsList.Add("streetAddress/city");
            columnsList.Add("streetAddress/zip");
            columnsList.Add("streetAddress/state");
            columnsList.Add("streetAddress/country");
            columnsList.Add("contactFax/formattedNumber");
            columnsList.Add("phone/formattedNumber");
            columnsList.Add("contactPhone/formattedNumber");
            columnsList.Add("searchPhone/formattedNumber");
            columnsList.Add("email/emailAddress");
            columnsList.Add("url/URLAddress");

            Find75.ArchiveOrderByInfo[] orderby = new Find75.ArchiveOrderByInfo[1];
            orderby[0] = new Find75.ArchiveOrderByInfo();
            orderby[0].Direction = Find75.OrderBySortType.ASC;
            orderby[0].Name = "contactId";

            cred.ExtensionData = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.ExtensionData;
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                tzone.ExtensionData = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = (credentials.TimeZone75 == null) ? 0 : credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            }

            // Service Call
            findClient.FindFromRestrictionsColumnsOrderBy(string.Empty, cred, ref tzone, restrictions, "FindContact", columnsList.ToArray(), orderby, 500, 0, out extraInfo, out success, out findList);

            if (success)
            {
                foreach (var archiveRow in findList.ArchiveRows)
                {
                    var contactRow = new AOS.DomainModel.Contact();

                    foreach (var v in archiveRow.ColumnData)
                    {
                        try
                        {
                            switch (v.Key.ToLower())
                            {
                                case "contactid":
                                    string s = v.Value.DisplayValue.TrimEnd(']'); // [I:3
                                    s = s.Split(':')[1]; // 3
                                    contactRow.ContactId = Convert.ToInt32(s);
                                    break;
                                case "name":
                                    contactRow.Name = v.Value.DisplayValue;
                                    break;
                                case "department":
                                    contactRow.Department = v.Value.DisplayValue;
                                    break;
                                case "country":
                                    contactRow.Country = v.Value.DisplayValue;
                                    break;
                                case "number":
                                    contactRow.Number = v.Value.DisplayValue;
                                    break;
                                case "code":
                                    contactRow.Code = v.Value.DisplayValue;
                                    break;
                                case "associateid":
                                    contactRow.AssociateId = v.Value.DisplayValue;
                                    break;
                                case "business":
                                    contactRow.Business = v.Value.DisplayValue;
                                    break;
                                case "category":
                                    contactRow.Category = v.Value.DisplayValue;
                                    break;
                                case "stop":
                                    string t = v.Value.DisplayValue.TrimEnd(']'); // [I:3
                                    t = t.Split(':')[1]; // 3
                                    contactRow.Stop = t;
                                    break;
                                case "contactnomail":
                                    contactRow.NoMail = v.Value.DisplayValue;
                                    break;
                                case "orgnr":
                                    contactRow.Orgnr = v.Value.DisplayValue;
                                    break;
                                case "postaddress/line1":
                                    contactRow.PostAddressline1 = v.Value.DisplayValue;
                                    break;
                                case "postaddress/line2":
                                    contactRow.PostAddressline2 = v.Value.DisplayValue;
                                    break;
                                case "postaddress/line3":
                                    contactRow.PostAddressline3 = v.Value.DisplayValue;
                                    break;
                                case "postaddress/city":
                                    contactRow.PostAddresscity = v.Value.DisplayValue;
                                    break;
                                case "postaddress/zip":
                                    contactRow.PostAddresszip = v.Value.DisplayValue;
                                    break;
                                case "postaddress/state":
                                    contactRow.PostAddressstate = v.Value.DisplayValue;
                                    break;
                                case "postaddress/country":
                                    contactRow.PostAddresscountry = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/line1":
                                    contactRow.StreetAddressline1 = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/line2":
                                    contactRow.StreetAddressline2 = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/line3":
                                    contactRow.StreetAddressline3 = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/city":
                                    contactRow.StreetAddresscity = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/zip":
                                    contactRow.StreetAddresszip = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/state":
                                    contactRow.StreetAddressstate = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/country":
                                    contactRow.StreetAddresscountry = v.Value.DisplayValue;
                                    break;
                                case "contactfax/formattednumber":
                                    contactRow.Fax = v.Value.DisplayValue;
                                    break;
                                case "phone/formattednumber":
                                    contactRow.Phone = v.Value.DisplayValue;
                                    break;
                                case "contactphone/formattednumber":
                                    contactRow.Phone2 = v.Value.DisplayValue;
                                    break;
                                case "searchphone/formattednumber":
                                    contactRow.Phone3 = v.Value.DisplayValue;
                                    break;
                                case "email/emailaddress":
                                    contactRow.Email = v.Value.DisplayValue;
                                    break;
                                case "url/urladdress":
                                    contactRow.Website = v.Value.DisplayValue;
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception e)
                        {

                        }

                    }

                    returnVal.Add(contactRow);
                }
            }

            if (returnVal.Count > 0)
            {
                returnVal = returnVal.OrderBy(x => x.Name).ToList();
            }

            return returnVal;
        }

        public List<AOS.DomainModel.Contact> SOGetContactsBySelectionId(IAOSCredentials credentials, string selectionId)
        {
            List<AOS.DomainModel.Contact> returnVal = new List<AOS.DomainModel.Contact>();

            // Service header
            Find75.FindResults findList = null;

            Find75.SoTimeZone tzone = new Find75.SoTimeZone();
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Find75SSL" : "BasicHttpBinding_Find75";

            Find75.FindClient findClient = new FindClient(bindingName);
            Find75.SoCredentials cred = new Find75.SoCredentials();
            Find75.SoExtraInfo extraInfo = new Find75.SoExtraInfo();

            bool success;

            // Service Endpoint
            findClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint.TrimEnd('/') + "/Find.svc");

            // Restrictions
            Find75.ArchiveRestrictionInfo[] restrictions;
            List<Find75.ArchiveRestrictionInfo> listRestrictions = new List<Find75.ArchiveRestrictionInfo>();

            listRestrictions.Add(new Find75.ArchiveRestrictionInfo()
            {
                Name = "selectionId",
                Operator = "equals",
                Values = new string[] { selectionId },
                IsActive = true
            });

            restrictions = listRestrictions.ToArray();

            List<string> columnsList = new List<string>();
            columnsList.Add("contactId");
            columnsList.Add("name");
            columnsList.Add("department");
            columnsList.Add("country");
            columnsList.Add("number");
            columnsList.Add("code");
            columnsList.Add("associateId");
            columnsList.Add("business");
            columnsList.Add("category");
            columnsList.Add("stop");
            columnsList.Add("contactNoMail");
            columnsList.Add("orgnr");
            columnsList.Add("postAddress/line1");
            columnsList.Add("postAddress/line2");
            columnsList.Add("postAddress/line3");
            columnsList.Add("postAddress/city");
            columnsList.Add("postAddress/zip");
            columnsList.Add("postAddress/state");
            columnsList.Add("postAddress/country");
            columnsList.Add("streetAddress/line1");
            columnsList.Add("streetAddress/line2");
            columnsList.Add("streetAddress/line3");
            columnsList.Add("streetAddress/city");
            columnsList.Add("streetAddress/zip");
            columnsList.Add("streetAddress/state");
            columnsList.Add("streetAddress/country");
            columnsList.Add("contactFax/formattedNumber");
            columnsList.Add("phone/formattedNumber");
            columnsList.Add("contactPhone/formattedNumber");
            columnsList.Add("searchPhone/formattedNumber");
            columnsList.Add("email/emailAddress");
            columnsList.Add("url/URLAddress");

            Find75.ArchiveOrderByInfo[] orderby = new Find75.ArchiveOrderByInfo[1];
            orderby[0] = new Find75.ArchiveOrderByInfo();
            orderby[0].Direction = Find75.OrderBySortType.ASC;
            orderby[0].Name = "contactId";

            cred.ExtensionData = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.ExtensionData;
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                tzone.ExtensionData = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = (credentials.TimeZone75 == null) ? 0 : credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            }

            // Service Call
            findClient.FindFromRestrictionsColumnsOrderBy(string.Empty, cred, ref tzone, restrictions, "FindContact", columnsList.ToArray(), orderby, 500, 0, out extraInfo, out success, out findList);

            if (success)
            {
                foreach (var archiveRow in findList.ArchiveRows)
                {
                    var contactRow = new AOS.DomainModel.Contact();

                    foreach (var v in archiveRow.ColumnData)
                    {
                        try
                        {
                            switch (v.Key.ToLower())
                            {
                                case "contactid":
                                    string s = v.Value.DisplayValue.TrimEnd(']'); // [I:3
                                    s = s.Split(':')[1]; // 3
                                    contactRow.ContactId = Convert.ToInt32(s);
                                    break;
                                case "name":
                                    contactRow.Name = v.Value.DisplayValue;
                                    break;
                                case "department":
                                    contactRow.Department = v.Value.DisplayValue;
                                    break;
                                case "country":
                                    contactRow.Country = v.Value.DisplayValue;
                                    break;
                                case "number":
                                    contactRow.Number = v.Value.DisplayValue;
                                    break;
                                case "code":
                                    contactRow.Code = v.Value.DisplayValue;
                                    break;
                                case "associateid":
                                    contactRow.AssociateId = v.Value.DisplayValue;
                                    break;
                                case "business":
                                    contactRow.Business = v.Value.DisplayValue;
                                    break;
                                case "category":
                                    contactRow.Category = v.Value.DisplayValue;
                                    break;
                                case "stop":
                                    string t = v.Value.DisplayValue.TrimEnd(']'); // [I:3
                                    t = t.Split(':')[1]; // 3
                                    contactRow.Stop = t;
                                    break;
                                case "contactnomail":
                                    contactRow.NoMail = v.Value.DisplayValue;
                                    break;
                                case "orgnr":
                                    contactRow.Orgnr = v.Value.DisplayValue;
                                    break;
                                case "postaddress/line1":
                                    contactRow.PostAddressline1 = v.Value.DisplayValue;
                                    break;
                                case "postaddress/line2":
                                    contactRow.PostAddressline2 = v.Value.DisplayValue;
                                    break;
                                case "postaddress/line3":
                                    contactRow.PostAddressline3 = v.Value.DisplayValue;
                                    break;
                                case "postaddress/city":
                                    contactRow.PostAddresscity = v.Value.DisplayValue;
                                    break;
                                case "postaddress/zip":
                                    contactRow.PostAddresszip = v.Value.DisplayValue;
                                    break;
                                case "postaddress/state":
                                    contactRow.PostAddressstate = v.Value.DisplayValue;
                                    break;
                                case "postaddress/country":
                                    contactRow.PostAddresscountry = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/line1":
                                    contactRow.StreetAddressline1 = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/line2":
                                    contactRow.StreetAddressline2 = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/line3":
                                    contactRow.StreetAddressline3 = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/city":
                                    contactRow.StreetAddresscity = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/zip":
                                    contactRow.StreetAddresszip = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/state":
                                    contactRow.StreetAddressstate = v.Value.DisplayValue;
                                    break;
                                case "streetaddress/country":
                                    contactRow.StreetAddresscountry = v.Value.DisplayValue;
                                    break;
                                case "contactfax/formattednumber":
                                    contactRow.Fax = v.Value.DisplayValue;
                                    break;
                                case "phone/formattednumber":
                                    contactRow.Phone = v.Value.DisplayValue;
                                    break;
                                case "contactphone/formattednumber":
                                    contactRow.Phone2 = v.Value.DisplayValue;
                                    break;
                                case "searchphone/formattednumber":
                                    contactRow.Phone3 = v.Value.DisplayValue;
                                    break;
                                case "email/emailaddress":
                                    contactRow.Email = v.Value.DisplayValue;
                                    break;
                                case "url/urladdress":
                                    contactRow.Website = v.Value.DisplayValue;
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception e)
                        {

                        }

                    }

                    returnVal.Add(contactRow);
                }
            }

            if (returnVal.Count > 0)
            {
                returnVal = returnVal.OrderBy(x => x.Name).ToList();
            }

            return returnVal;
        }

        public List<Selection> SOGetAllSelections(IAOSCredentials credentials)
        {
            List<Selection> returnVal = new List<Selection>();

            // Service header
            Find75.FindResults findList = null;

            Find75.SoTimeZone tzone = new Find75.SoTimeZone();
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Find75SSL" : "BasicHttpBinding_Find75";

            Find75.FindClient findClient = new FindClient(bindingName);
            Find75.SoCredentials cred = new Find75.SoCredentials();
            Find75.SoExtraInfo extraInfo = new Find75.SoExtraInfo();

            bool success;

            // Service Endpoint
            findClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint.TrimEnd('/') + "/Find.svc");

            // Restrictions
            Find75.ArchiveRestrictionInfo[] restrictions;
            List<Find75.ArchiveRestrictionInfo> listRestrictions = new List<Find75.ArchiveRestrictionInfo>();

            listRestrictions.Add(new Find75.ArchiveRestrictionInfo()
            {
                Name = "targetTableNumber",
                Operator = "oneOf",
                Values = new string[] { "5" },
                IsActive = true
            });

            restrictions = listRestrictions.ToArray();

            string[] columns = { "SelectionId", "Name", "visibleFor", "targetTableNumber", "type", "kind" };

            Find75.ArchiveOrderByInfo[] orderby = new Find75.ArchiveOrderByInfo[1];
            orderby[0] = new Find75.ArchiveOrderByInfo();
            orderby[0].Direction = Find75.OrderBySortType.ASC;
            orderby[0].Name = "SelectionId";

            cred.ExtensionData = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.ExtensionData;
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                tzone.ExtensionData = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = (credentials.TimeZone75 == null) ? 0 : credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            }

            // Service Call
            findClient.FindFromRestrictionsColumnsOrderBy(string.Empty, cred, ref tzone, restrictions, "FindSelection", columns, orderby, 500, 0, out extraInfo, out success, out findList);

            if (success)
            {
                foreach (var archiveRow in findList.ArchiveRows)
                {
                    int SelectionId = 0;
                    string Name = string.Empty;
                    string VisibleFor = string.Empty;
                    string TargetTableNumber = string.Empty;
                    string Type = string.Empty;
                    string Kind = string.Empty;

                    foreach (var v in archiveRow.ColumnData)
                    {
                        switch (v.Key.ToLower())
                        {
                            case "selectionid":
                                string s = v.Value.DisplayValue.TrimEnd(']'); // [I:3
                                s = s.Split(':')[1]; // 3
                                SelectionId = Convert.ToInt32(s);
                                break;
                            case "name":
                                Name = v.Value.DisplayValue;
                                break;
                            case "visiblefor":
                                VisibleFor = v.Value.DisplayValue;
                                break;
                            case "targettablenumber":
                                TargetTableNumber = v.Value.DisplayValue;
                                break;
                            case "type":
                                Type = v.Value.DisplayValue;
                                break;
                            case "kind":
                                Kind = v.Value.DisplayValue;
                                break;
                            default:
                                break;
                        }
                    }

                    returnVal.Add(new Selection
                    {
                        SelectionId = SelectionId,
                        Name = Name,
                    });
                }
            }

            if (returnVal.Count > 0)
            {
                returnVal = returnVal.OrderBy(x => x.Name).ToList();
            }

            return returnVal;
        }

        public List<Selection> SOFindSelections(IAOSCredentials credentials, string searchStr)
        {
            List<Selection> returnVal = new List<Selection>();

            // Service header
            Find75.FindResults findList = null;

            Find75.SoTimeZone tzone = new Find75.SoTimeZone();
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Find75SSL" : "BasicHttpBinding_Find75";

            Find75.FindClient findClient = new FindClient(bindingName);
            Find75.SoCredentials cred = new Find75.SoCredentials();
            Find75.SoExtraInfo extraInfo = new Find75.SoExtraInfo();

            bool success;

            // Service Endpoint
            findClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint.TrimEnd('/') + "/Find.svc");

            // Restrictions
            Find75.ArchiveRestrictionInfo[] restrictions;
            List<Find75.ArchiveRestrictionInfo> listRestrictions = new List<Find75.ArchiveRestrictionInfo>();

            listRestrictions.Add(new Find75.ArchiveRestrictionInfo()
            {
                Name = "targetTableNumber",
                Operator = "oneOf",
                Values = new string[] { "5" },
                IsActive = true
            });

            restrictions = listRestrictions.ToArray();

            string[] columns = { "SelectionId", "Name", "visibleFor", "targetTableNumber", "type", "kind" };

            Find75.ArchiveOrderByInfo[] orderby = new Find75.ArchiveOrderByInfo[1];
            orderby[0] = new Find75.ArchiveOrderByInfo();
            orderby[0].Direction = Find75.OrderBySortType.ASC;
            orderby[0].Name = "SelectionId";

            cred.ExtensionData = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.ExtensionData;
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                tzone.ExtensionData = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = (credentials.TimeZone75 == null) ? 0 : credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75 == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            }

            // Service Call
            findClient.FindFromRestrictionsColumnsOrderBy(string.Empty, cred, ref tzone, restrictions, "FindSelection", columns, orderby, 500, 0, out extraInfo, out success, out findList);

            if (success)
            {
                foreach (var archiveRow in findList.ArchiveRows)
                {
                    int SelectionId = 0;
                    string Name = string.Empty;
                    string VisibleFor = string.Empty;
                    string TargetTableNumber = string.Empty;
                    string Type = string.Empty;
                    string Kind = string.Empty;

                    foreach (var v in archiveRow.ColumnData)
                    {
                        switch (v.Key.ToLower())
                        {
                            case "selectionid":
                                string s = v.Value.DisplayValue.TrimEnd(']'); // [I:3
                                s = s.Split(':')[1]; // 3
                                SelectionId = Convert.ToInt32(s);
                                break;
                            case "name":
                                Name = v.Value.DisplayValue;
                                break;
                            case "visiblefor":
                                VisibleFor = v.Value.DisplayValue;
                                break;
                            case "targettablenumber":
                                TargetTableNumber = v.Value.DisplayValue;
                                break;
                            case "type":
                                Type = v.Value.DisplayValue;
                                break;
                            case "kind":
                                Kind = v.Value.DisplayValue;
                                break;
                            default:
                                break;
                        }
                    }

                    returnVal.Add(new Selection
                    {
                        SelectionId = SelectionId,
                        Name = Name,
                    });
                }
            }

            if (returnVal.Count > 0)
            {
                returnVal = returnVal.OrderBy(x => x.Name).ToList();
            }

            // Filter the list based on search string
            var rVal = new List<Selection>();

            foreach (var item in returnVal)
            {
                if (item.Name.ToLower().Contains(searchStr))
                    rVal.Add(item);
            }

            return rVal;
        }

        public List<SelMember> SOGetSelectionMembersBySelectionId(IAOSCredentials credentials, int selectionId)
        {
            Archive75.ArchiveListItem[] response = null;
            Archive75.SoTimeZone tzone = new Archive75.SoTimeZone();
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Archive75SSL" : "BasicHttpBinding_Archive75";
            Archive75.ArchiveClient archiveClient = new Archive75.ArchiveClient(bindingName);
            Archive75.SoCredentials cred = new Archive75.SoCredentials();
            Archive75.SoExtraInfo extraInfo = new Archive75.SoExtraInfo();

            bool success = false;

            List<SelMember> returnVal = new List<SelMember>();

            // Restrictions
            Archive75.ArchiveRestrictionInfo[] restrictions = new Archive75.ArchiveRestrictionInfo[1];
            restrictions[0] = new Archive75.ArchiveRestrictionInfo();
            restrictions[0].Name = "selectionId";
            restrictions[0].Operator = "equals";
            restrictions[0].Values = new string[] { selectionId.ToString() };
            restrictions[0].IsActive = true;

            Archive75.ArchiveOrderByInfo[] orderby = new Archive75.ArchiveOrderByInfo[1];
            orderby[0] = new Archive75.ArchiveOrderByInfo();
            orderby[0].Direction = Archive75.OrderBySortType.DESC;
            orderby[0].Name = "fullName";

            string[] columns = { "selectionId", "personId", "contactId", "fullName", "personEmail/emailAddress" };
            string[] entities = { "staticPerson", "dynamicContact" };

            // Service Endpoint
            archiveClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint.TrimEnd('/') + "/Archive.svc");

            if (credentials.AuthenticationSucceeded)
            {
                cred.ExtensionData = credentials.Credentials75.ExtensionData;
                cred.Ticket = credentials.Credentials75.Ticket;

                if (credentials.TimeZone75 != null)
                {
                    tzone.ExtensionData = credentials.TimeZone75.ExtensionData;
                    tzone.SoTimeZoneId = credentials.TimeZone75.SoTimeZoneId;
                    tzone.SoTimeZoneLocationCode = credentials.TimeZone75.SoTimeZoneLocationCode;
                }

                // Service Call
                archiveClient.GetArchiveListByColumns(string.Empty, cred, ref tzone, "ContactSelection", columns, orderby, restrictions, entities, 0, int.MaxValue, out extraInfo, out success, out response);

                if (success)
                {
                    foreach (var r in response)
                    {
                        int PersId = 0;
                        int ContcId = 0;
                        string PersName = string.Empty;
                        string PersEmail = string.Empty;

                        foreach (var d in r.ColumnData)
                        {
                            switch (d.Key)
                            {
                                case "personId":
                                    string p = d.Value.DisplayValue.TrimEnd(']');
                                    p = p.Split(':')[1];
                                    PersId = int.Parse(p);
                                    break;
                                case "contactId":
                                    string c = d.Value.DisplayValue.TrimEnd(']');
                                    c = c.Split(':')[1];
                                    ContcId = int.Parse(c);
                                    break;
                                case "fullName":
                                    PersName = d.Value.DisplayValue;
                                    break;
                                case "personEmail/emailAddress":
                                    if (d.Value != null)
                                        PersEmail = d.Value.DisplayValue;
                                    break;
                                default:
                                    break;
                            }
                        }

                        // Add only if name is filled
                        if (!string.IsNullOrEmpty(PersName))
                        {
                            returnVal.Add(new SelMember
                            {
                                PersonId = PersId,
                                Name = PersName,
                                Email = PersEmail
                            });
                        }
                    }
                }
            }

            return returnVal;
        }

        public User SOGetUserByPersonId(IAOSCredentials credentials, int personId)
        {
            bool success = false;

            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_User75SSL" : "BasicHttpBinding_User75";
            User1Client uClient = new User1Client(bindingName);

            User[] response = null;
            User currentUser = null;
            User75.SoTimeZone tzone = new User75.SoTimeZone();
            User75.SoCredentials cred = new User75.SoCredentials();
            User75.SoExtraInfo extraInfo = new User75.SoExtraInfo();

            uClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint + "/User.svc");

            cred.ExtensionData = (credentials.Credentials75.ExtensionData == null) ? null : credentials.Credentials75.ExtensionData;
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                tzone.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75.SoTimeZoneLocationCode == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            }

            uClient.GetUserFromPersonId(string.Empty, cred, ref tzone, personId, out extraInfo, out success, out response);

            if (success)
                currentUser = response[0];

            return currentUser;
        }

        public int GetPreferenceBySectionAndKey(IAOSCredentials credentials, string section, string key)
        {
            var returnVal = 0;

            // Setup bindingname depending on http/https
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Preference75SSL" : "BasicHttpBinding_Preference75";

            // Setup properties prior to webservice call
            Preference75.SoTimeZone tzone = new Preference75.SoTimeZone();
            Preference75.Preference1Client client = new Preference75.Preference1Client(bindingName);
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint + "/Preference.svc");
            Preference75.SoExtraInfo extraInfo = new Preference75.SoExtraInfo();
            Preference75.SoCredentials cred = new Preference75.SoCredentials();
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                credentials.Credentials75.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75.SoTimeZoneLocationCode == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            };

            // Define succes and response variables
            bool success = false;
            Preference75.Preference[] response = null;

            // Set up specifications
            var listOfSpecifications = new List<PreferenceSpec>();

            var prefSpec = new PreferenceSpec()
            {
                Key = key, // "AccountId"
                Section = section//"Adwiza.OnlineServices"
            };

            listOfSpecifications.Add(prefSpec);

            // Get object
            var exceptionInfo = client.GetPreferences(string.Empty, cred, ref tzone, listOfSpecifications.ToArray(), out extraInfo, out success, out response);

            // Did we find anything ?
            if (success)
            {
                if (response == null) // Not found
                    returnVal = 0;
                else
                {
                    if (response[0].RawValue == null || response[0].RawValue == string.Empty) // Not found
                        returnVal = 0;
                    else
                    {
                        var accountId = 0;
                        var parseOK = int.TryParse(response[0].RawValue, out accountId);

                        if (parseOK)
                            returnVal = accountId;
                        else
                            returnVal = -1;
                    }
                }
            }

            return returnVal;
        }

        public bool GetPrefDescById(IAOSCredentials credentials, int prefDescId)
        {
            var returnVal = false;

            // Setup bindingname depending on http/https
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Preference75SSL" : "BasicHttpBinding_Preference75";

            // Setup properties prior to webservice call
            Preference75.SoTimeZone tzone = new Preference75.SoTimeZone();
            Preference75.Preference1Client client = new Preference75.Preference1Client(bindingName);
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint + "/Preference.svc");
            Preference75.SoExtraInfo extraInfo = new Preference75.SoExtraInfo();
            Preference75.SoCredentials cred = new Preference75.SoCredentials();
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                credentials.Credentials75.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75.SoTimeZoneLocationCode == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            };

            // Define succes and response variables
            bool success = false;
            Preference75.PreferenceDescription response = null;

            // Get object
            var exceptionInfo = client.GetPreferenceDescription(string.Empty, cred, ref tzone, prefDescId, out extraInfo, out success, out response);

            if (success)
                returnVal = true;

            return returnVal;
        }

        public int SOSavePrefDesc(IAOSCredentials credentials, string section, string key, string name)
        {
            var returnVal = 0;

            // Setup bindingname depending on http/https
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Preference75SSL" : "BasicHttpBinding_Preference75";

            // Setup properties prior to webservice call
            Preference75.SoTimeZone tzone = new Preference75.SoTimeZone();
            Preference75.Preference1Client client = new Preference75.Preference1Client(bindingName);
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint + "/Preference.svc");
            Preference75.SoExtraInfo extraInfo = new Preference75.SoExtraInfo();
            Preference75.SoCredentials cred = new Preference75.SoCredentials();
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                credentials.Credentials75.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75.SoTimeZoneLocationCode == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            };

            // Define succes and response variables
            bool success = false;
            Preference75.PreferenceDescription response = null;

            // Create object to be inserted
            var newObject = new PreferenceDescription()
            {
                Key = key,
                Section = section,
                Name = name,
                ValueType = (key == ".") ? PrefDescValueType.Unknown : PrefDescValueType.Number,
                MaxLevel = PreferenceLevel.SystemWide,
                SysMaxLevel = PreferenceLevel.SystemWide,
                AccessFlags = (PrefDescAccessFlags)7
            };

            // Save object
            var exceptionInfo = client.SavePreferenceDescription(string.Empty, cred, ref tzone, newObject, out extraInfo, out success, out response);

            if (success)
                returnVal = response.PrefDescId; // Return PrefDescId

            return returnVal;
        }

        public int SOGetAccountIdInPreference(IAOSCredentials credentials, int userPrefId)
        {
            var returnVal = 0;

            // Setup bindingname depending on http/https
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Preference75SSL" : "BasicHttpBinding_Preference75";

            // Setup properties prior to webservice call
            Preference75.SoTimeZone tzone = new Preference75.SoTimeZone();
            Preference75.Preference1Client client = new Preference75.Preference1Client(bindingName);
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint + "/Preference.svc");
            Preference75.SoExtraInfo extraInfo = new Preference75.SoExtraInfo();
            Preference75.SoCredentials cred = new Preference75.SoCredentials();
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                credentials.Credentials75.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75.SoTimeZoneLocationCode == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            };

            // Define succes and response variables
            bool success = false;
            Preference response = null;

            // Get object
            var exceptionInfo = client.GetPreference(string.Empty, cred, ref tzone, userPrefId, out extraInfo, out success, out response);

            // Was it saved correctly ?
            if (success)
            {
                if (response == null) // Not found
                    returnVal = 0;
                else
                {
                    if (response.RawValue != null) // Found
                    {
                        var accountId = 0;
                        var parseOK = int.TryParse(response.RawValue, out accountId);

                        if (parseOK)
                            returnVal = accountId;
                        else
                            returnVal = -1;
                    }
                    else
                        returnVal = 0; // Not found
                }
            }

            return returnVal;
        }

        public int SOSaveAccountIdInPreference(IAOSCredentials credentials, int prefDescId, int accountId)
        {
            var returnVal = 0;

            // Setup bindingname depending on http/https
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Preference75SSL" : "BasicHttpBinding_Preference75";

            // Setup properties prior to webservice call
            Preference75.SoTimeZone tzone = new Preference75.SoTimeZone();
            Preference75.Preference1Client client = new Preference75.Preference1Client(bindingName);
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint + "/Preference.svc");
            Preference75.SoExtraInfo extraInfo = new Preference75.SoExtraInfo();
            Preference75.SoCredentials cred = new Preference75.SoCredentials();
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                credentials.Credentials75.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75.SoTimeZoneLocationCode == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            };

            // Define succes and response variables
            bool success = false;
            Preference response = null;

            // Set up specifications
            var prefSpec = new PreferenceSpec()
            {
                Key = "AccountId",
                Section = "Adwiza.OnlineServices",
            };

            // Create new object
            var newObject = new Preference()
            {
                DisplayTooltip = "Account Id",
                DisplayValue = accountId.ToString(),
                Level = PreferenceLevel.SystemWide,
                Specification = prefSpec,
                PrefDescId = prefDescId,
                UserPreferenceId = 0,
                RawValue = accountId.ToString(),
            };

            // Save account id
            var exceptionInfo = client.SavePreferenceEntity(string.Empty, cred, ref tzone, newObject, true, out extraInfo, out success, out response);

            if (success)
                returnVal = response.UserPreferenceId;

            return returnVal;
        }

        /// <summary>
        /// Save a new contact into SO
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int SOSaveContact(IAOSCredentials credentials, string name)
        {
            var returnVal = 0;

            // Setup bindingname depending on http/https
            var bindingName = (HttpsEnabled) ? "BasicHttpBinding_Contact75SSL" : "BasicHttpBinding_Contact75";

            // Setup properties prior to webservice call
            Contact75.SoTimeZone tzone = new Contact75.SoTimeZone();
            Contact75.Contact1Client client = new Contact75.Contact1Client(bindingName);
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Endpoint + "/Contact.svc");
            Contact75.SoExtraInfo extraInfo = new Contact75.SoExtraInfo();
            Contact75.SoCredentials cred = new Contact75.SoCredentials();
            cred.Ticket = credentials.Credentials75.Ticket;

            if (credentials.TimeZone75 != null)
            {
                credentials.Credentials75.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.ExtensionData = (credentials.TimeZone75.ExtensionData == null) ? null : credentials.TimeZone75.ExtensionData;
                tzone.SoTimeZoneId = credentials.TimeZone75.SoTimeZoneId;
                tzone.SoTimeZoneLocationCode = (credentials.TimeZone75.SoTimeZoneLocationCode == null) ? null : credentials.TimeZone75.SoTimeZoneLocationCode;
            };

            // Define succes and response variables
            bool success = false;
            ContactEntity newContactEntity;

            // Create new default contact entity
            var exceptionInfo = client.CreateDefaultContactEntity(string.Empty, cred, ref tzone, out extraInfo, out success, out newContactEntity);

            if (!success)
                return -1;

            // Update contact entity with values from input parameter
            newContactEntity.Name = name;

            // Define succes and response variables
            bool success2 = false;
            ContactEntity newContactEntity2;

            var exceptionInfo2 = client.SaveContactEntity(string.Empty, cred, ref tzone, newContactEntity, out extraInfo, out success2, out newContactEntity2);

            if (!success2)
                return -1;

            return 1;
        }
    }
}
