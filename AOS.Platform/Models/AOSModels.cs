using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AOS.Platform.Models
{
    public class UserPropertiesModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public ApplicationUser User { get; set; }
        public Account CurrentAccount { get; set; }
        public string CurrentAccountName { get; set; }
        public string CurrentAccountRole { get; set; }
        public string CurrentConnectionStatus { get; set; }
        public string CurrentConnectionStatusIcon { get; set; }
        public string CurrentAccountOwnerName { get; set; }
        public string CurrentAccountOwnerEmail { get; set; }
        public IList<Account> AllAccounts { get; set; }
        public IList<Account> Accounts { get; set; }
        public IList<AccountApp> AccountApps { get; set; }
        public IList<App> AllApps { get; set; }
        public IList<App> MyApps { get; set; }
        public IList<AppExtended> MyAppsExtended { get; set; }
        public IList<App> AvailableApps { get; set; }
        public ChangePasswordViewModel ChangePasswordModel { get; set; }
        public List<Selection> Selections { get; set; }
        public int SelectedSelection { get; set; }
        public string WebHookURL { get; set; }
        public List<SelMember> SelectionMembers { get; set; }
        public List<UserAdminInfo> UserAdminInfoList { get; set; }
        public IList<AspNetRoles> Roles { get; set; }

        [Url]
        public string EndpointUrl { get; set; }
        [DataType(DataType.Password)]
        public string EndpointPwd { get; set; }

        public Connection Endpoint { get; set; }
        public string Ticket { get; set; }

        public AppClass AppAdmin { get; set; }
    }

    public class AppClass
    {
        public string AppIntroHeader { get; set; }
        public string AppIntroText { get; set; }
        public string AppIFrameHeader { get; set; }
        public string AppAdminURL { get; set; }
        public bool IsOK { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class UserAdminInfo
    {
        public bool UserEnabled { get; set; }
        public bool UserLockedOut { get; set; }
        public int UserLockedOutFailedCount { get; set; }
        public string AccountName { get; set; }
        public string UserName { get; set; }
        public string CurrentRole { get; set; }
        public string CurrentRoleId { get; set; }
        public DateTime UserCreated { get; set; }
        public DateTime LastLogin { get; set; }
        public string UserId { get; set; }
    }

}