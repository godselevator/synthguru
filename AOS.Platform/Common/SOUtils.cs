using AOS.DomainModel;
using AOS.WebAPIPlugins;
using SuperOffice;
using SuperOffice.Security.Principal;
using SuperOffice.Configuration;
using SuperOffice.SuperID.Client.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Web;
using SuperOffice.SuperID.Contracts.SystemUser.V1;
using System.Security.Claims;
using System.Threading;
using SuperOffice.CRM.Services;
using SuperOffice.Data;
using SuperOffice.CRM.ArchiveLists;

namespace AOS.Platform
{
    public class SOUtils
    {
        public BusinessLayer.BusinessLayer AOSBLL { get; set; }

        public string SysUser { get; set; }
        public string SysPassword { get; set; }
        public string Endpoint { get; set; }

        public SOUtils(string sysUser, string sysPassword, string endpoint)
        {
            SysUser = sysUser;
            SysPassword = sysPassword;
            Endpoint = endpoint;

            AOSBLL = new BusinessLayer.BusinessLayer();
        }

        public GenericResponse LogInToSO()
        {
            var resp = new GenericResponse();

            try
            {
                // Set SuperOffice Remote Base URL
                ConfigFile.Services.RemoteBaseURL = Endpoint;

                // We have to use an applicationtoken. We'll use Online Signature's
                var currApp = AOSBLL.GetAppByCode("SIGN");
                if (currApp == null)
                {
                    resp.IsOK = false;
                    resp.ErrorMsg = "App record not found - AppCode: SIGN";

                    return resp;
                }

                ConfigFile.Services.ApplicationToken = currApp.ApplicationToken;

                SoContext.CloseCurrentSession();

                SoDatabaseContext.DatabaseContext context = SoDatabaseContext.EnterDatabaseContext("context" + Guid.NewGuid().ToString());

                SoSession.Authenticate(SysUser, SysPassword);
            }
            catch (Exception ex)
            {
                var innerMsg = (ex.InnerException != null) ? ex.InnerException.Message : string.Empty;

                resp.IsOK = false;
                resp.ErrorMsg = $"LogInToSO - Exception: {ex.Message}, Inner exception: {innerMsg}";
            }

            return resp;
        }

        public SoSystemInfo GetSOInfo()
        {
            return SoSystemInfo.GetCurrent();
        }

        public int GetAccountIdInPreference(int userPrefId)
        {
            var resp = 0;

            using (var agent = new PreferenceAgent())
            {
                var pref = agent.GetPreference(userPrefId);

                if (pref == null)
                    return resp;

                if (pref.RawValue != null) // Found
                {
                    var accountId = 0;
                    var parseOK = int.TryParse(pref.RawValue, out accountId);

                    if (parseOK)
                        resp = accountId;
                    else
                        resp = -1;
                }
            }

            return resp;
        }


        public int GetPrefDescId(int id)
        {
            var resp = 0;

            using (var agent = new PreferenceAgent())
            {
                var pref = agent.GetPreference(id);
                if (pref == null)
                {
                    return resp;
                }

                var rawValue = pref.RawValue;

                if (rawValue != null) // Found
                {
                    var accountId = 0;
                    var parseOK = int.TryParse(rawValue, out accountId);

                    if (parseOK)
                    {
                        resp = accountId;
                    }
                    else
                    {
                        resp = -1;
                    }
                }
                else
                {
                    resp = 0; // Not found            }
                }
            }

            return resp;
        }

        public int GetPreferenceBySectionAndKey(string section, string key)
        {
            var resp = 0;

            using (var agent = new PreferenceAgent())
            {
                var pref = agent.GetPreferences(new PreferenceSpec[] { new PreferenceSpec { Section = section, Key = key } });

                if (pref == null || pref.Length == 0) // Not found
                    return resp;

                if (pref[0].RawValue == null || pref[0].RawValue == string.Empty) // Not found
                    return resp;
                else
                {
                    var accountId = 0;
                    var parseOK = int.TryParse(pref[0].RawValue, out accountId);

                    if (parseOK)
                        resp = accountId;
                    else
                        resp = -1;
                }
            }

            return resp;
        }

        public int SavePreference(string section, string key, string name)
        {
            var resp = 0;

            using (var agent = new PreferenceAgent())
            {
                var newEntry = new PreferenceDescription
                {
                    Key = key,
                    Section = section,
                    Name = name,
                    ValueType = (key == ".") ? PrefDescValueType.Unknown : PrefDescValueType.Number,
                    MaxLevel = PreferenceLevel.SystemWide,
                    SysMaxLevel = PreferenceLevel.SystemWide,
                    AccessFlags = (PrefDescAccessFlags)7
                };

                var pref = agent.SavePreferenceDescription(newEntry);
                if (pref == null)
                {
                    return resp;
                }
            }

            return resp;
        }

        public int SavePreferenceEntity(int prefDescId, int accountId)
        {
            var resp = 0;

            using (var agent = new PreferenceAgent())
            {
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

                var pref = agent.SavePreferenceEntity(newObject, true);

                if (pref == null)
                {
                    return resp;
                }

                resp = pref.UserPreferenceId;
            }

            return resp;
        }

        public List<Selection> FindSelections(string searchStr)
        {
            var resp = new List<Selection>();

            try
            {
                List<ArchiveRestrictionInfo> listRestrictions = new List<ArchiveRestrictionInfo>();

                listRestrictions.Add(new ArchiveRestrictionInfo()
                {
                    Name = "targetTableNumber",
                    Operator = "oneOf",
                    Values = new string[] { "5" },
                    IsActive = true
                });

                var restrictions = listRestrictions.ToArray();

                string[] columns = { "SelectionId", "Name", "visibleFor", "targetTableNumber", "type", "kind" };

                ArchiveOrderByInfo[] orderby = new ArchiveOrderByInfo[1];
                orderby[0] = new ArchiveOrderByInfo();
                orderby[0].Direction = OrderBySortType.ASC;
                orderby[0].Name = "SelectionId";

                using (var agent = new FindAgent())
                {
                    var list = agent.FindFromRestrictionsColumnsOrderBy(restrictions, "FindSelection", columns, orderby, 500, 0);

                    if (list == null)
                        return resp;

                    foreach (var archiveRow in list.ArchiveRows)
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

                        resp.Add(new Selection
                        {
                            SelectionId = SelectionId,
                            Name = Name,
                        });
                    }
                }

            }
            catch (Exception)
            {
                return resp;
            }

            return resp;
        }

    }
}
