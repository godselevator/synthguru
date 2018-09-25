using AOS.DomainModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AOS.Platform.Models
{
    public class AdministratorHomeViewModel
    {
        [Display(Name = "Polling enabled")]
        public bool EndpointPollingEnabled { get; set; }

        [Range(0, 30000)]
        [Display(Name = "Polling interval (ms)")]
        public int EndpointPollingInterval { get; set; }

        public NotificationAndText[] NotificationList { get; set; }

        [Required]
        [Display(Name = "Notification email")]
        [EmailAddress]
        public string NotificationEmail { get; set; }
        public string DBEnvironment { get; set; }
    }

    public class PersonalSettingsViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstNameLastName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class GrantAccessViewModel
    {
        public string GrantedUser { get; set; }
        public string GrantedAccount { get; set; }
        public string ErrorMessage { get; set; }

        public GrantAccessViewModel()
        {
            GrantedUser = "";
            GrantedAccount = "";
            ErrorMessage = "";
        }
    }

    public class CockpitSmallModel
    {
        public string CurrentConnectionStatusIcon { get; set; }
        public string CurrentConnectionStatus { get; set; }
        public int StatusAsInt { get; set; }
    }

    public class AccountWithConnType
    {
        public Account Account { get; set; }
        public string ConnectionType { get; set; }
    }

    public class CockpitViewModel
    {
        public string CurrentUserId { get; set; }
        public int CurrentAccountId { get; set; }
        public string CurrentAccountName { get; set; }
        public string CurrentAccountRole { get; set; }
        public string CurrentConnectionStatusIcon { get; set; }
        public string CurrentConnectionStatus{ get; set; }
        public int StatusAsInt { get; set; }
        public bool IsSOOnline { get; set; }
        public string CurrentAccountOwnerName { get; set; }
        public string CurrentAccountOwnerEmail { get; set; }
        public List<string> Accounts { get; set; }
        public List<Account> AccountsList { get; set; }
        //public List<AccountWithConnType> AccountsList { get; set; }
        public bool EndpointPollingEnabled { get; set; }
        public int EndpointPollingInterval { get; set; }
    }
    
    public class AccountSettingsViewModel
    {
        [Required]
        public string Name { get; set; }
        public string URL { get; set; }
        [Required]
        public string Address { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string Zip { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public bool Enabled { get; set; }
        public bool? Partner { get; set; }
        public string Owner { get; set; }
        public bool IsSOOnline { get; set; }
        public bool IsSystemUser { get; set; }
    }

    public class UserAdministratorViewModel
    {
        public List<AspNetRoles> ListOfRoles { get; set; }
        public List<UserAdminInfo> ListOfUsers { get; set; }
    }

    public class AccountUser
    {
        public string UserID { get; set; }
        public string FirstNameLastName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOwner { get; set; }
    }

    public class AccountUsersViewModel
    {
        public int AccountId { get; set; }
        public List<AccountUser> AccountUsers { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string AssignUserFirstName { get; set; }
        [Display(Name = "Last name")]
        [Required]
        public string AssignUserLastName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string AssignUserEmail { get; set; }
    }


    public class EndpointViewModel
    {
        public int ConnectionId { get; set; }
        [Required]
        public string EndpointURI { get; set; }
        [Required]
        public string SysUser { get; set; }
        [Required]
        public string SysPassword { get; set; }
        public string SOCompanyName { get; set; }
        public string SOVersion { get; set; }
        public string SOSerial { get; set; }
        public string SONetserverVersion { get; set; }
        public int PrefDescId { get; set; }
        public HandshakeStatus HandshakeStatus { get; set; }
        public bool IsSOOnline { get; set; }
    }

    public enum HandshakeStatus
    {
        Ok,
        NotFound,
        Invalid,
        NoMatch,
        NoKey
    }

    public class IFrameHostViewModel
    {
        public AppClass AppInfo { get; set; }
        public bool IsSystemUser { get; set; }
    }

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class AccountModel
    {
        public string UserId { get; set; }
        public int AppID { get; set; }
        public bool IsTrial { get; set; }
        public ExistingAccount ExistingAccount { get; set; }
        public NewAccount NewAccount { get; set; }
        public IList<Account> ExistingAccounts { get; set; }
    }

    public class ExistingAccount
    {
        // Existing or new account - radiobuttons
        [Required]
        [Display(Name = "Existing account")]
        public string ExistingAccountName { get; set; }
        public string ExistingAccountOwnerName { get; set; }
        public string ExistingAccountOwnerEmail { get; set; }
    }

    public class NewAccount 
    {
        [Required]
        [Display(Name = "New account name")]
        public string NewAccountName { get; set; }

        [Required]
        [Display(Name = "New account address")]
        public string NewAccountAddress { get; set; }

        [Display(Name = "New account address 2")]
        public string NewAccountAddress2 { get; set; }

        [Required]
        [Display(Name = "New account postcode")]
        public string NewAccountZip { get; set; }

        [Required]
        [Display(Name = "New account city")]
        public string NewAccountCity { get; set; }

        [Required]
        [Display(Name = "New account country")]
        public string NewAccountCountry { get; set; }

        [Required]
        [Display(Name = "New account publish URL")]
        public string NewAccountPublishURL { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public int AppID { get; set; }
        public string AppName { get; set; }
        public bool IsTrial { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
