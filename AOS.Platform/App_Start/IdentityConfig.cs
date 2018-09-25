using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using AOS.Platform.Models;
using SendGrid;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using Twilio;
using System.Net.Mail;
using Twilio.Clients;

namespace AOS.Platform
{
    public enum MailServiceProvider
    {
        UseSmtp,
        UseSendGrid
    }

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            // NB! Remove this code as soon as SendGrid problems has been solved !!!
            try
            {
                MailMessage mail = new MailMessage("aos@adwiza.com", message.Destination);
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "smtp.adwiza.com";
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception e)
            {
            }

            //await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {
            // NB! Remove this code as soon as SendGrid problems has been solved !!!
            try
            {
                MailMessage mail = new MailMessage("aos@adwiza.com", message.Destination);
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "smtp.adwiza.com";
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception e)
            {
            }
            //// Create the email object first, then add the properties.
            //var myMessage = new SendGridMessage();

            //// Add the message properties.
            //myMessage.From = new MailAddress("aos@adwiza.com", "Adwiza Online Services");

            //myMessage.AddTo(message.Destination);

            //myMessage.Subject = message.Subject;

            ////Add the HTML and Text bodies
            //myMessage.Text = message.Body;
            //myMessage.Html = message.Body;

            ////var credentials = new NetworkCredential(username, pswd);
            //var credentials = new NetworkCredential(ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);

            //// Create an Web transport for sending email.
            //var transportWeb = new Web(credentials);

            //// Send the email.
            //transportWeb.Deliver(myMessage);

            //try
            //{
            //    // Send the email.
            //    if (transportWeb != null)
            //    {
            //        await transportWeb.DeliverAsync(myMessage);
            //    }
            //    else
            //    {
            //        Trace.TraceError("Failed to create Web transport.");
            //        await Task.FromResult(0);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Trace.TraceError(ex.Message + " SendGrid probably not configured correctly.");
            //}

            //// *************************************************** //

            ////var myMessage = new SendGridMessage();

            ////myMessage.AddTo(message.Destination);
            ////myMessage.From = new System.Net.Mail.MailAddress("aos@adwiza.com", "Adwiza Online Services");
            ////myMessage.Subject = message.Subject;
            ////myMessage.Text = message.Body;
            ////myMessage.Html = message.Body;

            ////// Create a Web transport for sending email.
            ////var transportWeb = new Web(credentials);

            ////try
            ////{
            ////    // Send the email.
            ////    if (transportWeb != null)
            ////    {
            ////        await transportWeb.DeliverAsync(myMessage);
            ////    }
            ////    else
            ////    {
            ////        Trace.TraceError("Failed to create Web transport.");
            ////        await Task.FromResult(0);
            ////    }
            ////}
            ////catch (Exception ex)
            ////{
            ////    Trace.TraceError(ex.Message + " SendGrid probably not configured correctly.");
            ////}
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            //manager.PasswordValidator = new PasswordValidator
            //{
            //    RequiredLength = 6,
            //    RequireNonLetterOrDigit = true,
            //    RequireDigit = true,
            //    RequireLowercase = true,
            //    RequireUppercase = true,
            //};

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
