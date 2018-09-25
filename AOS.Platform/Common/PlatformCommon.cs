using AOS.BusinessLayer;
using AOS.DomainModel;
using AOS.Platform.Models;
using AOS.SAMLTokenHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AOS.WebAPIPlugins;
using SendGrid.Helpers.Mail;
using SendGrid;
using AOS.MailUtils;

namespace AOS.Platform.Common
{
    public class PlatformCommon
    {
        public static string GetIPAddress()
        {
            string resp = null;

            HttpContext context = HttpContext.Current;

            if (context == null)
                return null;
            else
            {
                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        resp = addresses[0];
                    }
                }
                else
                    resp = context.Request.ServerVariables["REMOTE_ADDR"];
            }

            return resp;
        }

        public static byte[] GetPlaceholderImage()
        {
            var dir = HttpContext.Current.Server.MapPath(VirtualPathUtility.ToAbsolute("~/Content/Images"));
            var path = Path.Combine(dir, "placehold.jpg");
            var file = File.ReadAllBytes(path);

            return file;
        }

        public static AOSResponse TestSOEndpoint(Connection conn)
        {
            var resp = new AOSResponse();
            resp.ErrorMsg = "Endpoint valid";

            //var testConnection = new TestConnection(conn);

            //ConnectionStatusObj connStatus = testConnection.GetConnectionStatus();

            //resp.IsOK = (connStatus.Status == ConnectionStatus.ConnectionValid) ? true : false;
            //resp.ErrorMsg = connStatus.ErrorMessage;

            return resp;
        }

        public static AOSCertResponse GetCertificate(bool isDevelopment)
        {
            var resp = new AOSCertResponse();
            resp.IsOK = true;

            var storeLocation = (isDevelopment) ? StoreLocation.LocalMachine : StoreLocation.CurrentUser;

            var keyStore = new X509Store(StoreName.My, storeLocation);

            keyStore.Open(OpenFlags.ReadOnly);
            X509Certificate2 cert =
                  keyStore.Certificates
                    .OfType<X509Certificate2>()
                    .FirstOrDefault(c => c.Thumbprint ==
                    "FD59BA88FFEA123F549FD19D217BECE12EE9E267");

            if (cert == null)
            {
                resp.IsOK = false;
                resp.ReturnMsg = "Certificate not found. Returning list of located certificates";
                resp.Certificates = keyStore.Certificates;
            }

            resp.ReturnedCert = cert;
            return resp;
        }

        public static bool IsValidEmail(string email)
        {
            var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

            return !string.IsNullOrEmpty(email) && r.IsMatch(email);
        }

        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static string GetIFrameUrl(string inputUrl, string appInfoController)
        {
            var offset = inputUrl.IndexOf("Account");
            var xxx = inputUrl.Remove(offset);
            var rootUrl = xxx + appInfoController;

            return rootUrl;
        }

        public static AOSResponse SendJsonDataToElasticIO(string inputJson, string webHookURL)
        {
            AOSResponse resp = new AOSResponse();

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webHookURL);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //string json = "{\"PersonId\":\"1\"," + "\"Name\":\"Anders Andersen\"," + "\"Email\":\"anders@andersen.dk\"}";

                    streamWriter.Write(inputJson);
                    //streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                resp.ErrorMsg = ex.Message;
                resp.IsOK = false;
            }

            return resp;
        }

        public static AspNetUsers GetUser(string userId)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            return businessLayer.GetUserById(userId);
        }

        public static AOSSOOnlineSAMLResponse UnpackSAMLToken(string token, bool isDevelopment)
        {
            var resp = new AOSSOOnlineSAMLResponse();

            // Get info from SAML token
            try
            {
                // Get certificate
                var certResp = GetCertificate(isDevelopment);

                // Validate data from SAMl extraction
                if (!certResp.IsOK)
                {
                    resp.IsOK = false;
                    resp.ReturnMsg = certResp.ReturnMsg + certResp.Certificates.ToString();
                    return resp;
                }

                // Get data from token
                SamlTokenHandler samlTokHandl = new SamlTokenHandler();
                SOOnlineCreateUser createUser = samlTokHandl.getUserFromSAMLToken(token, certResp.ReturnedCert);
                resp.CreateUser = createUser;
            }
            catch (Exception e)
            {
                var errMsg = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                resp.IsOK = false;
                resp.ReturnMsg = "UnpackSAMLToken function failed. Exception: " + errMsg;

                return resp;
            }

            return resp;
        }

        public static AOSResponse ValidateSAMLToken(SOOnlineCreateUser createUser)
        {
            var resp = new AOSResponse();

            var businessLayer = new BusinessLayer.BusinessLayer();

            if (createUser == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "getUserFromSAMLToken call returned null";
                return resp;
            }

            if (createUser.FirstName.ToLower().Contains("object"))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "No data returned from getUserFromSAMLToken call. Check if token has expired";
                return resp;
            }

            // Firstname must be filled
            if (String.IsNullOrEmpty(createUser.FirstName))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Firstname not filled";
                return resp;
            }

            // Lastname must be filled
            if (String.IsNullOrEmpty(createUser.LastName))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Lastname not filled";
                return resp;
            }

            // Email must be filled
            if (String.IsNullOrEmpty(createUser.EMail))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Email not filled";
                return resp;
            }

            // Email must be valid
            if (!PlatformCommon.IsValidEmail(createUser.EMail))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Email not valid (" + createUser.EMail + ")";
                return resp;
            }

            // AppId must be filled
            if (createUser.AOSAppId <= 0)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "AppId not filled";
                return resp;
            }

            // AppId must exist
            var currApp = businessLayer.GetAppById(createUser.AOSAppId);
            if (currApp == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "No app with appId " + createUser.AOSAppId.ToString() + " could be found";
                return resp;
            }

            // ConnectionId must be filled
            if (createUser.ConnectionId <= 0)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "ConnectionId not filled";
                return resp;
            }

            // ConnectionId must exist
            if (businessLayer.GetConnectionById(createUser.ConnectionId) == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "No connection with connectionId " + createUser.ConnectionId.ToString() + " could be found";
                return resp;
            }

            // CustId must be filled
            if (String.IsNullOrEmpty(createUser.CustId))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "CustId not filled";
                return resp;
            }

            return resp;
        }

        public static AOSResponse ValidatePayload(AutoRegisterPayload payload)
        {
            var resp = new AOSResponse();

            var businessLayer = new BusinessLayer.BusinessLayer();

            if (payload == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Payload is null";
                return resp;
            }

            if (payload.PersonFirstName.ToLower().Contains("object"))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "No data returned from GetUserFromSAMLToken call. Check if token has expired";
                return resp;
            }

            // Firstname must be filled
            if (String.IsNullOrEmpty(payload.PersonFirstName))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Firstname not filled";
                return resp;
            }

            // Lastname must be filled
            if (String.IsNullOrEmpty(payload.PersonLastName))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Lastname not filled";
                return resp;
            }

            // Email must be filled
            if (String.IsNullOrEmpty(payload.PersonEmail))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Email not filled";
                return resp;
            }

            // Email must be valid
            if (!PlatformCommon.IsValidEmail(payload.PersonEmail))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "Email not valid (" + payload.PersonEmail + ")";
                return resp;
            }

            // AppId must be filled
            if (payload.AOSAppId <= 0)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "AppId not filled";
                return resp;
            }

            // AppId must exist
            var currApp = businessLayer.GetAppById(payload.AOSAppId);
            if (currApp == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "No app with appId " + payload.AOSAppId.ToString() + " could be found";
                return resp;
            }

            // ConnectionId must be filled
            if (payload.AOSConnectionId <= 0)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "ConnectionId not filled";
                return resp;
            }

            // ConnectionId must exist
            if (businessLayer.GetConnectionById(payload.AOSConnectionId) == null)
            {
                resp.IsOK = false;
                resp.ErrorMsg = "No connection with connectionId " + payload.AOSConnectionId.ToString() + " could be found";
                return resp;
            }

            // CustId must be filled
            if (String.IsNullOrEmpty(payload.CustId))
            {
                resp.IsOK = false;
                resp.ErrorMsg = "CustId not filled";
                return resp;
            }

            return resp;
        }

        public static AOSResponse CreateUserFromSAMLToken(SOOnlineCreateUser createUser)
        {
            var resp = new AOSResponse();


            return resp;
        }

        public static AspNetRoles GetCurrentRoleForUser(string userId)
        {
            var currentAccountId = GetCurrentAccountForUser(userId).AccountID;
            var businessLayer = new BusinessLayer.BusinessLayer(userId);

            var currentURA = businessLayer.GetUserRoleAccountByUserIdAndAccountId(userId, currentAccountId);

            var currentRole = businessLayer.GetRoleById(currentURA.RoleID);

            return currentRole;
        }

        public static bool HasRole(ApplicationUser currentUser, UserRole role)
        {
            var resp = false;

            var currentRole = GetCurrentRoleForUser(currentUser.Id);

            if (role == UserRole.Administrator)
            {
                if (currentRole.Name.ToLower() == UserRole.Administrator.Name.ToLower() || currentRole.Name.ToLower() == UserRole.SystemUser.Name.ToLower())
                    resp = true;
            }
            else
                resp = (currentRole.Name.ToLower() == role.Name.ToLower());

            return resp;
        }

        public static Connection GetCurrentConnection(string userId, Account account)
        {
            if (account.ConnectionID == null)
                return null;

            var businessLayer = new BusinessLayer.BusinessLayer(userId);
            var conn = businessLayer.GetConnectionById(account.ConnectionID);

            return conn;
        }

        public static Account GetCurrentAccountForUser(string userId)
        {
            var businessLayer = new BusinessLayer.BusinessLayer(userId);
            var userRoleAccounts = businessLayer.GetUserRoleAccountByUserId(userId);

            var currentAccountId = 0;
            var currentRoleId = string.Empty;

            // Add accounts to user accounts object
            foreach (var uac in userRoleAccounts)
            {
                if (uac.ActiveAccount == true)
                {
                    currentAccountId = uac.AccountID;
                    currentRoleId = uac.RoleID;
                }
            }

            if (currentAccountId != 0)
                return businessLayer.GetAccountById(currentAccountId);
            else
                return null;
        }

        //public static void SendAssignAccountNotification(ApplicationUser applier, Account account, bool newAccount)
        //{
        //    // Check notification settings
        //    var businessLayer = new BusinessLayer.BusinessLayer();
        //    bool notify = businessLayer.GetNotificationByCode(EventCode.ACCCREA.ToString()).NotificationEnabled;

        //    if (!notify)
        //        return;

        //    var message = string.Empty;

        //    if (newAccount)
        //        message = string.Format("New user <strong>{0}</strong> just created new account <strong>{1}</strong><br /><br />", applier.FirstNameLastName, account.Name);
        //    else
        //        message = string.Format("New user <strong>{0}</strong> just applied for membership of existing account <strong>{1}</strong><br /><br />", applier.FirstNameLastName, account.Name);

        //    message += "Best regards<br />";
        //    message += "<strong>The Adwiza Online Services Team</strong>";

        //    PlatformCommon.SendMailGeneric("aos@adwiza.com", "aosaccount@adwiza.com", "Adwiza Online Services - New account created", message, MailServiceProvider.UseSendGrid);
        //}

        public static void SendMembershipGrantedNotification(ApplicationUser applier, ApplicationUser owner, Account account)
        {
            // Check notification settings
            var businessLayer = new BusinessLayer.BusinessLayer();
            bool notify = businessLayer.GetNotificationByCode(EventCode.OWNERGRANTEDACCESS.ToString()).SendEmail;

            if (!notify)
                return;

            var message = string.Empty;

            message = string.Format("Account owner <strong>{0}</strong> just granted membership of account <strong>{1}</strong> to user <strong>{2}</strong><br /><br />", owner.FirstNameLastName, account.Name, applier.FirstNameLastName);
            message += "Best regards<br />";
            message += "<strong>The Adwiza Online Services Team</strong>";

            SendMailGeneric("aos@adwiza.com", "aosaccount@adwiza.com", "Adwiza Online Services - New account created", message, MailServiceProvider.UseSendGrid);
        }

        //public static void SendGrantAccessEmailToAccountOwner(ApplicationUser applier, ApplicationUser owner, Account account, string callbackUrl)
        //{
        //    var message = string.Format("Hello <strong>{0}</strong><br /><br />", owner.FirstNameLastName) +
        //                  string.Format("The registered Adwiza Online Services user <strong>{0}</strong> has applied for membership of account <strong>{1}</strong><br /><br />", applier.FirstNameLastName, account.Name) +
        //                  "Since you are the owner of this account, you must grant access for this user. The user will be assigned to your account as a normal user without administration rights<br /><br />" +
        //                  string.Format("Click <a href=\"{0}\">here</a> to grant access<br /><br />", callbackUrl);

        //    message += "Best regards<br />";
        //    message += "<strong>The Adwiza Online Services Team</strong>";

        //    PlatformCommon.SendMailGeneric("aos@adwiza.com", owner.Email, "Adwiza Online Services - Grant account membership", message, MailServiceProvider.UseSendGrid);
        //}

        //public static void SendGrantAccessEmailToApplier(ApplicationUser applier, ApplicationUser owner, Account account)
        //{
        //    var message = string.Format("Hello <strong>{0}</strong><br /><br />", applier.FirstNameLastName) +
        //                  string.Format("You have applied for membership of account <strong>{0}</strong><br /><br />", account.Name) +
        //                  string.Format("The account owner <strong>{0}</strong> has now been contacted. You will be notified by email when you have been granted access<br /><br />", owner.FirstNameLastName);
        //    message += "Best regards<br />";
        //    message += "<strong>The Adwiza Online Services Team</strong>";

        //    PlatformCommon.SendMailGeneric("aos@adwiza.com", applier.Email, "Adwiza Online Services - Apply for account membership", message, MailServiceProvider.UseSendGrid);
        //}

        public static void SendGrantAccessConfirmedEmailToApplier(ApplicationUser applier, ApplicationUser owner, Account account, string callbackUrl)
        {
            var ownerName = (owner.FirstNameLastName == null) ? owner.UserName : owner.FirstNameLastName;
            var applierName = (applier.FirstNameLastName == null) ? applier.UserName : applier.FirstNameLastName;

            var message = string.Format("Hello <strong>{0}</strong><br /><br />", applierName) +
                          string.Format("The account owner <strong>{0}</strong> has granted you access to account <strong>{1}</strong><br /><br />", ownerName, account.Name) +
                          string.Format("Click <a href=\"{0}\">here</a> to login to Adwiza Online Services and start using your account<br /><br />", callbackUrl);
            message += "Best regards<br />";
            message += "<strong>The Adwiza Online Services Team</strong>";

            SendMailGeneric("aos@adwiza.com", applier.Email, "Adwiza Online Services - Account membership succesfully granted", message, MailServiceProvider.UseSendGrid);
        }

        public static void SendMailGeneric(string sender, string receiver, string subject, string message, MailServiceProvider provider)
        {
            try
            {
                // Retrieve the API key from the environment variables. See the project README for more info about setting this up.
                var apiKey = "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";

                var client = new SendGridClient(apiKey);

                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(sender, "Adwiza Online Services"),
                    Subject = subject,
                    PlainTextContent = message,
                    HtmlContent = message
                };
                msg.AddTo(new EmailAddress(receiver, "Receiver..."));

                client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class IntegrityCheckReturnResponse
    {
        public bool IsOK { get; set; }
        public string ReturnMsg { get; set; }
        public List<string> OrphanAccountOwnersList { get; set; }
        public List<IdAndName> AccountsWithNoConnectionList { get; set; }
        public List<IdAndName> OrphanConnectionsList { get; set; }
        public List<string> OrphanUsersList { get; set; }
    }

    public class IdAndName
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ConnectionStatusReturnResponse
    {
        public bool IsOK { get; set; }
        public string ReturnMsg { get; set; }
        public List<AOSConnectionStatus> ConnectionStatus { get; set; }
    }

    public class AOSConnectionStatus
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public int ConnectionId { get; set; }
        public string ConnectionURL { get; set; }
        public string ConnectionType { get; set; }
        public string StatusSeverity { get; set; }
        public string ConnectionStatusText { get; set; }

        public AOSConnectionStatus()
        {
            AccountId = 0;
            AccountName = string.Empty;
            ConnectionId = 0;
            ConnectionType = string.Empty;
            StatusSeverity = "Success";
            ConnectionStatusText = "Up and running";
        }
    }

    public class RemoveAccountReturnResponse
    {
        public bool IsOK { get; set; }
        public string ReturnMsg { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AccountStats { get; set; }
        public string ConnectionStats { get; set; }
        public string UserStats { get; set; }
        public string AddressBookStats { get; set; }
        public string BisNodeInfoStats { get; set; }
        public string BisNodeManagerStats { get; set; }
        public string ConverterStats { get; set; }
        public string ElasticIOStats { get; set; }
        public string MatchPersonStats { get; set; }
        public string ReferralStats { get; set; }
        public string RelationWiseStats { get; set; }
        public string RWAppointmentTaskStats { get; set; }
        public string RWAppointmentTriggerStats { get; set; }
        public string RWChosenVariableStats { get; set; }
        public string RWSaleTriggerStats { get; set; }
        public string RWTriggerStats { get; set; }
        public string SignicatStats { get; set; }
        public string SignicatAccountSignLanguageStats { get; set; }
        public string SignicatAccountSignMethodStats { get; set; }
        public string SignicatDocumentTemplateStats { get; set; }
        public string SignicatEmailStats { get; set; }
        public string SignicatLoggingStats { get; set; }
        public string SignicatSecureFormStats { get; set; }
        public string PDFManagerStats { get; set; }
        public string ZapierStats { get; set; }
        public string DashboardStats { get; set; }
        public string AppSystemUserTokenStats { get; set; }
        public string AccountAppAssociatesStats { get; set; }
        public string AccountAppStats { get; set; }
        public string UserRoleStats { get; set; }
        public string UserRoleAccountStats { get; set; }
    }

    public class TempApp
    {
        public string AppCode { get; set; }
        public bool Installed { get; set; }
        public bool Trial { get; set; }
        public bool Activated { get; set; }
    }

    public class SelObj
    {
        public List<SelMember> SelMembers { get; set; }
    }

    public class NotFoundModel : HandleErrorInfo
    {
        public NotFoundModel(Exception exception, string controllerName, string actionName)
            : base(exception, controllerName, actionName)
        {
        }

        public string RequestedUrl { get; set; }
        public string ReferrerUrl { get; set; }
    }

    public class AOSCertResponse
    {
        public bool IsOK { get; set; }
        public string ReturnMsg { get; set; }
        public X509Certificate2Collection Certificates { get; set; }
        public X509Certificate2 ReturnedCert { get; set; }
    }
}