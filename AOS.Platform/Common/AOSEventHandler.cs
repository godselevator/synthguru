using AOS.BusinessLayer;
using AOS.DomainModel;
using AOS.Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AOS.Platform.Common
{
    public class AOSEventHandler
    {
        public static void SendEmail(Mail mail, string targetEmail, params string[] parameters)
        {
            var businessLayer = new BusinessLayer.BusinessLayer();

            var subject = mail.Subject;

            // Get mail template
            var mailTemplate = businessLayer.GetTextByTypeId(5);

            // Get mail text and substitute text placeholders with parameter values
            var text = businessLayer.GetTextById(mail.TextID);
            var emailText = text.Text1;

            // Replace placeholders in email text with parameter values
            for (int i = 0; i < parameters.Length; i++)
			{
                var offset = i + 1;
                emailText = emailText.Replace("{" + offset.ToString() + "}", parameters[i]);
			}

            // Append signature
            emailText += "<br /><br />Best regards<br /><strong>The Adwiza Online Services Team</strong>";

            // Incorporate emailtext into mail template
            var finalText = mailTemplate[0].Text1.Replace("{1}", emailText);

            PlatformCommon.SendMailGeneric("aos@adwiza.com", targetEmail, subject, finalText, MailServiceProvider.UseSendGrid);
        }

        public static void WriteToLog(EventCode eventCode, NotificationInfo notificationInfo, List<MailInfo> mailInfoList, SendMail sendMail)
        {
            var businessLayer = new BusinessLayer.BusinessLayer();

            // Get notification entry
            var notification = businessLayer.GetNotificationByCode(eventCode.ToString());

            // Get event text from Text table
            var text = businessLayer.GetTextById((int)notification.TextID);
            var eventText = text.Text1;

            // Create new log entry
            var logEntry = new Log()
            {
                LogGroupID = notification.LogGroupID,
                TextID = notification.TextID,
                UserID = notificationInfo.UserId,
                AccountID = notificationInfo.AccountId,
                AppID = notificationInfo.AppId,
                IPAddress = notificationInfo.IPAddress,
                LogDate = DateTime.Now
            };

            businessLayer.AddLog(logEntry);

            // Check if we should send notification mail also
            if (notification.SendEmail)
            {
                // Get log meta-data info
                var logUser = (String.IsNullOrEmpty(notificationInfo.UserId)) ? "n/a" : businessLayer.GetUserById(notificationInfo.UserId).Email;
                var logAccount = (notificationInfo.AccountId > 0) ? businessLayer.GetAccountById(notificationInfo.AccountId).Name : "n/a";
                var logAccountId = (notificationInfo.AccountId > 0) ? notificationInfo.AccountId.ToString() : "n/a";
                var logApp = (notificationInfo.AppId > 0) ? businessLayer.GetAppById(notificationInfo.AppId).Name : "n/a";
                var logIP = (String.IsNullOrEmpty(notificationInfo.IPAddress)) ? "n/a" : notificationInfo.IPAddress;

                // Get globals and retrieve notification email address
                var targetMail = businessLayer.GetAllGlobals()[0].NotificationEmailAddress;
                //var targetMail = "mok@adwiza.com"; // NB! DELETE THIS

                var subject = text.Text1;
                var emailText = string.Format("Log data:<br /><br />Email: <strong>{0}</strong><br />Account: <strong>{1}</strong><br />Account ID: <strong>{2}</strong><br />App name: <strong>{3}</strong><br />IP-address: <strong>{4}</strong><br /><br />", logUser, logAccount, logAccountId, logApp, logIP);
                emailText += ("You are receiving this email because the email notification setting is enabled for this particular event.<br />You can change this in the Notification Settings in the System User Dashboard");

                // Append signature
                emailText += "<br /><br />Best regards<br /><strong>The Adwiza Online Services Team</strong>";

                PlatformCommon.SendMailGeneric("aos@adwiza.com", targetMail, subject, emailText, MailServiceProvider.UseSendGrid);
            }

            // Check if noMail switch is true. If se, then don't generate any mails despite MailID1 and MailID2 indicates values 
            if (sendMail == SendMail.NoMails)
                return;

            // Check which emails to send
            if (notification.MailID1 > 0)
            {
                if (mailInfoList.Count > 0)
                {
                    var mail1 = businessLayer.GetMailById(notification.MailID1);
                    SendEmail(mail1, mailInfoList[0].TargetEmail, mailInfoList[0].Parameters);
                }
            }

            if (notification.MailID2 > 0)
            {
                if (mailInfoList.Count > 0)
                {
                    var mail2 = businessLayer.GetMailById(notification.MailID2);
                    SendEmail(mail2, mailInfoList[1].TargetEmail, mailInfoList[1].Parameters);
                }
            }
        }

    }

    public enum SendMail
    {
        Default,
        NoMails
    }

    public enum EventCode
    {
        USERCREATED	= 1, // OK - Test OK					
        USERCONFIRMEDEMAIL = 2, // OK - Test OK
        ACCOUNTCREATEDBYUSER = 3, // OK - Test OK
        USERAPPLIEDFORMEMBERSHIP = 4, // OK - Test OK
        OWNERGRANTEDACCESS = 5, // OK - Test OK
        USERANDACCOUNTAUTOCREATEDBYAOS = 6, // Implement
        BUYAPPCOMPLETED = 7, // OK - Test OK
        TRYAPPCOMPLETED = 8, // OK - Test OK
        ANONYMOUSUSERCLICKEDAPP = 9, // OK - Test OK
        ANONYMOUSUSERBUYAPP = 10, // OK - Test OK
        ANONYMOUSUSERTRYAPP = 11, // OK - Test OK
        APPTRIALPERIODSTARTED = 12, // OK - Test OK
        APPTRIALPERIODEXPIRING = 13, // Implement via Web Job
        APPTRIALPERIODEXPIRED = 14, // Implement via Web Job
        APPUNINSTALLED = 15, // OK
        USERCREATEDBYOWNER = 16, // OK - Test OK
        USERASSIGNEDBYOWNER	= 17, // OK - Test OK
        RESENDEMAILCONFIRMATION = 18, // OK
        USERFORGOTPASSWORD = 19, // OK - Test OK
        NOTIFYUSERONPREMBETA = 20, //
        NOTIFYUSERSOONLINEBETA = 21, //
        NEWUSERCREATEDBYAOS = 22,
        NEWACCOUNTCREATEDBYAOS = 23,
        TRIALVERSIONCHANGEDTOFULLVERSION = 24
    }
    
    public class MailInfo
    {
        public string TargetEmail { get; set; }
        public string[] Parameters { get; set; }
    }

    public class NotificationInfo
    {
        public string UserId { get; set; }
        public int AccountId { get; set; }
        public int AppId { get; set; }
        public string IPAddress { get; set; }
    }
}