using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AOS.DomainModel;
using AOS.DataAccessLayer;

namespace AOS.DataAccessLayer
{

    #region *** Repository interfaces ***

    /// <summary>
    /// Account
    /// </summary>
    public interface IAccountRepository : IGenericDataRepository<Account>
    {
    }

    /// <summary>
    /// AccountApp
    /// </summary>
    public interface IAccountAppRepository : IGenericDataRepository<AccountApp>
    {
    }

    /// <summary>
    /// AddressBook
    /// </summary>
    public interface IAddressBookRepository : IGenericDataRepository<Addressbook>
    {
    }

    /// <summary>
    /// AOSLog
    /// </summary>
    public interface ILogRepository : IGenericDataRepository<Log>
    {

    }

    /// <summary>
    /// App
    /// </summary>
    public interface IAppRepository : IGenericDataRepository<App>
    {
    }

    /// <summary>
    /// AppState
    /// </summary>
    public interface IAppStateRepository : IGenericDataRepository<AppState>
    {
    }

    /// <summary>
    /// AspNetRoles
    /// </summary>
    public interface IRoleRepository : IGenericDataRepository<AspNetRoles>
    {
    }

    /// <summary>
    /// AspNetUsers
    /// </summary>
    public interface IUserRepository : IGenericDataRepository<AspNetUsers>
    {
    }

    /// <summary>
    /// BisNodeInfo
    /// </summary>
    public interface IBisNodeInfoRepository : IGenericDataRepository<BisNodeInfo>
    {
    }

    /// <summary>
    /// BisNodeManager
    /// </summary>
    public interface IBisNodeManagerRepository : IGenericDataRepository<BisnodeManager>
    {
    }

    /// <summary>
    /// Connnection
    /// </summary>
    public interface IConnectionRepository : IGenericDataRepository<Connection>
    {
    }

    /// <summary>
    /// Converter
    /// </summary>
    public interface IConverterRepository : IGenericDataRepository<Converter>
    {
    }

    /// <summary>
    /// ElasticIO
    /// </summary>
    public interface IElasticIORepository : IGenericDataRepository<ElasticIO>
    {
    }

    /// <summary>
    /// Language
    /// </summary>
    public interface ILanguageRepository : IGenericDataRepository<Language>
    {
    }

    /// <summary>
    /// LogType
    /// </summary>
    public interface ILogGroupRepository : IGenericDataRepository<LogGroup>
    {
    }

    /// <summary>
    /// MatchPerson
    /// </summary>
    public interface IMatchPersonRepository : IGenericDataRepository<MatchPerson>
    {
    }

    /// <summary>
    /// Referral
    /// </summary>
    public interface IReferralRepository : IGenericDataRepository<Referral>
    {
    }

    /// <summary>
    /// RelationWise
    /// </summary>
    public interface IRelationWiseRepository : IGenericDataRepository<RelationWise>
    {
    }

    /// <summary>
    /// RWAppointmentStatus
    /// </summary>
    public interface IRWAppointmentStatusRepository : IGenericDataRepository<RWAppointmentStatus>
    {
    }

    /// <summary>
    /// RWAppointmentTask
    /// </summary>
    public interface IRWAppointmentTaskRepository : IGenericDataRepository<RWAppointmentTask>
    {
    }

    /// <summary>
    /// RWAppointmentTrigger
    /// </summary>
    public interface IRWAppointmentTriggerRepository : IGenericDataRepository<RWAppointmentTrigger>
    {
    }

    /// <summary>
    /// RWChosenVariable
    /// </summary>
    public interface IRWChosenVariableRepository : IGenericDataRepository<RWChosenVariable>
    {
    }

    /// <summary>
    /// RWSaleStatus
    /// </summary>
    public interface IRWSaleStatusRepository : IGenericDataRepository<RWSaleStatus>
    {
    }

    /// <summary>
    /// RWSaleTrigger
    /// </summary>
    public interface IRWSaleTriggerRepository : IGenericDataRepository<RWSaleTrigger>
    {
    }

    /// <summary>
    /// RWSOVariable
    /// </summary>
    public interface IRWSOVariableRepository : IGenericDataRepository<RWSOVariable>
    {
    }

    /// <summary>
    /// RWTrigger
    /// </summary>
    public interface IRWTriggerRepository : IGenericDataRepository<RWTrigger>
    {
    }

    /// <summary>
    /// Signicat
    /// </summary>
    public interface ISignicatRepository : IGenericDataRepository<Signicat>
    {
    }

    /// <summary>
    /// SignicatAccountSignLanguage
    /// </summary>
    public interface ISignicatAccountSignLanguageRepository : IGenericDataRepository<SignicatAccountSignLanguage>
    {
    }

    /// <summary>
    /// SignicatAccountSignLanguage
    /// </summary>
    public interface ISignicatAccountSignMethodRepository : IGenericDataRepository<SignicatAccountSignMethod>
    {
    }

    /// <summary>
    /// SignicatDocumentTemplate
    /// </summary>
    public interface ISignicatDocumentTemplateRepository : IGenericDataRepository<SignicatDocumentTemplate>
    {
    }

    /// <summary>
    /// SignicatEmail
    /// </summary>
    public interface ISignicatEmailRepository : IGenericDataRepository<SignicatEmail>
    {
    }

    /// <summary>
    /// SignicatLanguage
    /// </summary>
    public interface ISignicatLanguageRepository : IGenericDataRepository<SignicatLanguage>
    {
    }

    /// <summary>
    /// SignicatLogging
    /// </summary>
    public interface ISignicatLoggingRepository : IGenericDataRepository<SignicatLogging>
    {
    }

    /// SignicatSignMethod
    /// </summary>
    public interface ISignicatSignMethodRepository : IGenericDataRepository<SignicatSignMethod>
    {
    }

    /// SignicatStatus
    /// </summary>
    public interface ISignicatStatusRepository : IGenericDataRepository<SignicatStatus>
    {
    }

    /// <summary>
    /// UserRole
    /// </summary>
    public interface IUserRoleRepository : IGenericDataRepository<AspNetUserRoles>
    {
    }

    /// <summary>
    /// UserRoleAccount
    /// </summary>
    public interface IUserRoleAccountRepository : IGenericDataRepository<UserRoleAccount>
    {
    }

    /// <summary>
    /// Globals
    /// </summary>
    public interface IGlobalsRepository : IGenericDataRepository<Globals>
    {
    }

    /// <summary>
    /// Mail
    /// </summary>
    public interface IMailRepository : IGenericDataRepository<Mail>
    {
    }

    /// <summary>
    /// Notification
    /// </summary>
    public interface INotificationRepository : IGenericDataRepository<Notification>
    {
    }

    /// <summary>
    /// Text
    /// </summary>
    public interface ITextRepository : IGenericDataRepository<Text>
    {
    }

    /// <summary>
    /// Country
    /// </summary>
    public interface ICountryRepository : IGenericDataRepository<Country>
    {
    }

    /// <summary>
    /// TextType
    /// </summary>
    public interface ITextTypeRepository : IGenericDataRepository<TextType>
    {
    }

    /// <summary>
    /// AppIcons
    /// </summary>
    public interface IAppIconsRepository : IGenericDataRepository<AppIcons>
    {
    }

    /// <summary>
    /// PDF Manager
    /// </summary>
    public interface IPDFManagerRepository : IGenericDataRepository<PDFManager>
    {
    }

    /// <summary>
    /// SyncContact
    /// </summary>
    public interface ISyncContactRepository : IGenericDataRepository<SyncContact>
    {
    }

    /// <summary>
    /// SyncPerson
    /// </summary>
    public interface ISyncPersonRepository : IGenericDataRepository<SyncPerson>
    {
    }

    /// <summary>
    /// SyncSale
    /// </summary>
    public interface ISyncSaleRepository : IGenericDataRepository<SyncSale>
    {
    }

    /// <summary>
    /// SystemUserToken
    /// </summary>
    public interface IAppSystemUserTokenRepository : IGenericDataRepository<AppSystemUserToken>
    {
    }

    /// <summary>
    /// AppPrivateKey
    /// </summary>
    public interface IAppPrivateKeyRepository : IGenericDataRepository<AppPrivateKey>
    {
    }

    /// <summary>
    /// Zapier
    /// </summary>
    public interface IZapierRepository : IGenericDataRepository<Zapier>
    {
    }

    /// <summary>
    /// Dashboard
    /// </summary>
    public interface IDashboardRepository : IGenericDataRepository<Dashboard>
    {
    }

    /// <summary>
    /// AccountAppAssociate
    /// </summary>
    public interface IAccountAppAssociateRepository : IGenericDataRepository<AccountAppAssociate>
    {
    }

    /// <summary>
    /// Session
    /// </summary>
    public interface ISessionRepository : IGenericDataRepository<Session>
    {
    }

    /// <summary>
    /// SOInclude
    /// </summary>
    public interface ISOIncludeRepository : IGenericDataRepository<SOInclude>
    {
    }

    /// <summary>
    /// PBXDial
    /// </summary>
    public interface IPBXDialRepository : IGenericDataRepository<PBXDial>
    {
    }

    /// <summary>
    /// SignicatSecureForm
    /// </summary>
    public interface ISignicatSecureFormRepository : IGenericDataRepository<SignicatSecureForm>
    {
    }

    /// <summary>
    /// VisibleIn
    /// </summary>
    public interface IVisibleInRepository : IGenericDataRepository<VisibleIn>
    {
    }

    /// <summary>
    /// MirroringLogType
    /// </summary>
    public interface IMirroringLogTypeRepository : IGenericDataRepository<MirroringLogType>
    {
    }

    /// <summary>
    /// MirroringStatus
    /// </summary>
    public interface IMirroringStatusRepository : IGenericDataRepository<MirroringStatus>
    {
    }

    /// <summary>
    /// MirroringLog
    /// </summary>
    public interface IMirroringLogRepository : IGenericDataRepository<MirroringLog>
    {
    }

    /// <summary>
    /// MirroringLogLevel
    /// </summary>
    public interface IMirroringLogLevelRepository : IGenericDataRepository<MirroringLogLevel>
    {
    }

    /// <summary>
    /// APIKey
    /// </summary>
    public interface IAPIKeyRepository : IGenericDataRepository<APIKey>
    {
    }

    /// <summary>
    /// Certificate
    /// </summary>
    public interface ISOCertificateRepository : IGenericDataRepository<SOCertificate>
    {
    }

    /// <summary>
    /// DataSync
    /// </summary>
    public interface IDataSyncRepository : IGenericDataRepository<DataSync>
    {
    }

    /// <summary>
    /// InstallInfo
    /// </summary>
    public interface IInstallInfoRepository : IGenericDataRepository<InstallInfo>
    {
    }

    /// <summary>
    /// MachineName
    /// </summary>
    public interface IMachineNameRepository : IGenericDataRepository<MachineName>
    {
    }

    /// <summary>
    /// SignicatEventStatus
    /// </summary>
    public interface ISignicatEventStatusRepository : IGenericDataRepository<SignicatEventStatus>
    {
    }

    /// <summary>
    /// DocArcActivity
    /// </summary>
    public interface IDocArcActivityRepository : IGenericDataRepository<DocArcActivity>
    {
    }

    /// <summary>
    /// SignicatEventLog
    /// </summary>
    public interface ISignicatEventLogRepository : IGenericDataRepository<SignicatEventLog>
    {
    }

    /// <summary>
    /// SignicatEventType
    /// </summary>
    public interface ISignicatEventTypeRepository : IGenericDataRepository<SignicatEventType>
    {
    }

    /// <summary>
    /// SignicatEventOrigin
    /// </summary>
    public interface ISignicatEventOriginRepository : IGenericDataRepository<SignicatEventOrigin>
    {
    }

    /// <summary>
    /// AppOnlineProvisioning
    /// </summary>
    public interface IAppOnlineProvisioningRepository : IGenericDataRepository<AppOnlineProvisioning>
    {
    }

    /// <summary>
    /// DocArcTemplate
    /// </summary>
    public interface IDocArcTemplateRepository : IGenericDataRepository<DocArcTemplate>
    {
    }

    /// <summary>
    /// DocArcAssociation
    /// </summary>
    public interface IDocArcAssociationRepository : IGenericDataRepository<DocArcAssociation>
    {
    }

    /// <summary>
    /// DocArcAdministrators
    /// </summary>
    public interface IDocArcAdministratorsRepository : IGenericDataRepository<DocArcAdministrators>
    {
    }

    /// <summary>
    /// LOISLicenseCompany
    /// </summary>
    public interface ILOISLicenseCompanyRepository : IGenericDataRepository<LOISLicenseCompany>
    {
    }

    /// <summary>
    /// LOISLicenseCompanyKeys
    /// </summary>
    public interface ILOISLicenseCompanyKeysRepository : IGenericDataRepository<LOISLicenseCompanyKeys>
    {
    }

    #endregion

    #region *** Repository classes ***

    /// <summary>
    /// Account
    /// </summary>
    public class AccountRepository : GenericDataRepository<Account>, IAccountRepository
    {
    }

    /// <summary>
    /// AccountApp
    /// </summary>
    public class AccountAppRepository : GenericDataRepository<AccountApp>, IAccountAppRepository
    {
    }

    /// <summary>
    /// AddressBook
    /// </summary>
    public class AddressBookRepository : GenericDataRepository<Addressbook>, IAddressBookRepository
    {
    }

    /// <summary>
    /// AOSLog
    /// </summary>
    public class LogRepository : GenericDataRepository<Log>, ILogRepository
    {
    }

    /// <summary>
    /// App
    /// </summary>
    public class AppRepository : GenericDataRepository<App>, IAppRepository
    {
    }

    /// <summary>
    /// AppState
    /// </summary>
    public class AppStateRepository : GenericDataRepository<AppState>, IAppStateRepository
    {
    }

    /// <summary>
    /// AspNetRoles
    /// </summary>
    public class RoleRepository : GenericDataRepository<AspNetRoles>, IRoleRepository
    {
    }

    /// <summary>
    /// AspNetUsers
    /// </summary>
    public class UserRepository : GenericDataRepository<AspNetUsers>, IUserRepository
    {
    }

    /// <summary>
    /// BisNodeInfo
    /// </summary>
    public class BisNodeInfoRepository : GenericDataRepository<BisNodeInfo>, IBisNodeInfoRepository
    {
    }

    /// <summary>
    /// BisNodeManager
    /// </summary>
    public class BisNodeManagerRepository : GenericDataRepository<BisnodeManager>, IBisNodeManagerRepository
    {
    }

    /// <summary>
    /// Connection
    /// </summary>
    public class ConnectionRepository : GenericDataRepository<Connection>, IConnectionRepository
    {
    }

    /// <summary>
    /// Converter
    /// </summary>
    public class ConverterRepository : GenericDataRepository<Converter>, IConverterRepository
    {
    }

    /// <summary>
    /// ElasticIO
    /// </summary>
    public class ElasticIORepository : GenericDataRepository<ElasticIO>, IElasticIORepository
    {
    }

    /// <summary>
    /// Language
    /// </summary>
    public class LanguageRepository : GenericDataRepository<Language>, ILanguageRepository
    {
    }

    /// <summary>
    /// Language
    /// </summary>
    public class LogGroupRepository : GenericDataRepository<LogGroup>, ILogGroupRepository
    {
    }

    /// <summary>
    /// MatchPerson
    /// </summary>
    public class MatchPersonRepository : GenericDataRepository<MatchPerson>, IMatchPersonRepository
    {
    }

    /// <summary>
    /// Referral
    /// </summary>
    public class ReferralRepository : GenericDataRepository<Referral>, IReferralRepository
    {
    }

    /// <summary>
    /// RelationWise
    /// </summary>
    public class RelationWiseRepository : GenericDataRepository<RelationWise>, IRelationWiseRepository
    {
    }

    /// <summary>
    /// RWAppointmentStatus
    /// </summary>
    public class RWAppointmentStatusRepository : GenericDataRepository<RWAppointmentStatus>, IRWAppointmentStatusRepository
    {
    }

    /// <summary>
    /// RWAppointmentTask
    /// </summary>
    public class RWAppointmentTaskRepository : GenericDataRepository<RWAppointmentTask>, IRWAppointmentTaskRepository
    {
    }

    /// <summary>
    /// RWAppointmentTrigger
    /// </summary>
    public class RWAppointmentTriggerRepository : GenericDataRepository<RWAppointmentTrigger>, IRWAppointmentTriggerRepository
    {
    }

    /// <summary>
    /// RWChosenVariable
    /// </summary>
    public class RWChosenVariableRepository : GenericDataRepository<RWChosenVariable>, IRWChosenVariableRepository
    {
    }

    /// <summary>
    /// RWSaleStatus
    /// </summary>
    public class RWSaleStatusRepository : GenericDataRepository<RWSaleStatus>, IRWSaleStatusRepository
    {
    }

    /// <summary>
    /// RWSaleTrigger
    /// </summary>
    public class RWSaleTriggerRepository : GenericDataRepository<RWSaleTrigger>, IRWSaleTriggerRepository
    {
    }

    /// <summary>
    /// RWSOVariable
    /// </summary>
    public class RWSOVariableRepository : GenericDataRepository<RWSOVariable>, IRWSOVariableRepository
    {
    }

    /// <summary>
    /// RWTrigger
    /// </summary>
    public class RWTriggerRepository : GenericDataRepository<RWTrigger>, IRWTriggerRepository
    {
    }

    /// <summary>
    /// Signicat
    /// </summary>
    public class SignicatRepository : GenericDataRepository<Signicat>, ISignicatRepository
    {
    }

    /// <summary>
    /// SignicatAccountSignLanguage
    /// </summary>
    public class SignicatAccountSignLanguageRepository : GenericDataRepository<SignicatAccountSignLanguage>, ISignicatAccountSignLanguageRepository
    {
    }

    /// <summary>
    /// SignicatAccountSignMethod
    /// </summary>
    public class SignicatAccountSignMethodRepository : GenericDataRepository<SignicatAccountSignMethod>, ISignicatAccountSignMethodRepository
    {
    }

    /// <summary>
    /// SignicatDocumentTemplate
    /// </summary>
    public class SignicatDocumentTemplateRepository : GenericDataRepository<SignicatDocumentTemplate>, ISignicatDocumentTemplateRepository
    {
    }

    /// <summary>
    /// SignicatEmail
    /// </summary>
    public class SignicatEmailRepository : GenericDataRepository<SignicatEmail>, ISignicatEmailRepository
    {
    }

    /// <summary>
    /// SignicatLanguage
    /// </summary>
    public class SignicatLanguageRepository : GenericDataRepository<SignicatLanguage>, ISignicatLanguageRepository
    {
    }

    /// <summary>
    /// SignicatLogging
    /// </summary>
    public class SignicatLoggingRepository : GenericDataRepository<SignicatLogging>, ISignicatLoggingRepository
    {
    }

    /// <summary>
    /// SignicatSignMethod
    /// </summary>
    public class SignicatSignMethodRepository : GenericDataRepository<SignicatSignMethod>, ISignicatSignMethodRepository
    {
    }

    /// <summary>
    /// SignicatStatus
    /// </summary>
    public class SignicatStatusRepository : GenericDataRepository<SignicatStatus>, ISignicatStatusRepository
    {
    }

    /// <summary>
    /// UserRoleAccount
    /// </summary>
    public class UserRoleRepository : GenericDataRepository<AspNetUserRoles>, IUserRoleRepository
    {
    }

    /// <summary>
    /// UserRoleAccount
    /// </summary>
    public class UserRoleAccountRepository : GenericDataRepository<UserRoleAccount>, IUserRoleAccountRepository
    {
    }

    /// <summary>
    /// Globals
    /// </summary>
    public class GlobalsRepository : GenericDataRepository<Globals>, IGlobalsRepository
    {
    }

    /// <summary>
    /// Mail
    /// </summary>
    public class MailRepository : GenericDataRepository<Mail>, IMailRepository
    {
    }

    /// <summary>
    /// Notification
    /// </summary>
    public class NotificationRepository : GenericDataRepository<Notification>, INotificationRepository
    {
    }

    /// <summary>
    /// Text
    /// </summary>
    public class TextRepository : GenericDataRepository<Text>, ITextRepository
    {
    }

    /// <summary>
    /// Country
    /// </summary>
    public class CountryRepository : GenericDataRepository<Country>, ICountryRepository
    {
    }

    /// <summary>
    /// TextType
    /// </summary>
    public class TextTypeRepository : GenericDataRepository<TextType>, ITextTypeRepository
    {
    }

    /// <summary>
    /// AppIcons
    /// </summary>
    public class AppIconsRepository : GenericDataRepository<AppIcons>, IAppIconsRepository
    {
    }

    /// <summary>
    /// AppIcons
    /// </summary>
    public class PDFManagerRepository : GenericDataRepository<PDFManager>, IPDFManagerRepository
    {
    }

    /// <summary>
    /// SyncContact
    /// </summary>
    public class SyncContactRepository : GenericDataRepository<SyncContact>, ISyncContactRepository
    {
    }

    /// <summary>
    /// SyncPerson
    /// </summary>
    public class SyncPersonRepository : GenericDataRepository<SyncPerson>, ISyncPersonRepository
    {
    }

    /// <summary>
    /// SyncSale
    /// </summary>
    public class SyncSaleRepository : GenericDataRepository<SyncSale>, ISyncSaleRepository
    {
    }

    /// <summary>
    /// SystemUserToken
    /// </summary>
    public class AppSystemUserTokenRepository : GenericDataRepository<AppSystemUserToken>, IAppSystemUserTokenRepository
    {
    }

    /// <summary>
    /// AppPrivateKey
    /// </summary>
    public class AppPrivateKeyRepository : GenericDataRepository<AppPrivateKey>, IAppPrivateKeyRepository
    {
    }

    /// <summary>
    /// Zapier
    /// </summary>
    public class ZapierRepository : GenericDataRepository<Zapier>, IZapierRepository
    {
    }

    /// <summary>
    /// Dashboard
    /// </summary>
    public class DashboardRepository : GenericDataRepository<Dashboard>, IDashboardRepository
    {
    }

    /// <summary>
    /// AccountAppAssociate
    /// </summary>
    public class AccountAppAssociateRepository : GenericDataRepository<AccountAppAssociate>, IAccountAppAssociateRepository
    {
    }

    /// <summary>
    /// Session
    /// </summary>
    public class SessionRepository : GenericDataRepository<Session>, ISessionRepository
    {
    }

    /// <summary>
    /// SOInclude
    /// </summary>
    public class SOIncludeRepository : GenericDataRepository<SOInclude>, ISOIncludeRepository
    {
    }

    /// <summary>
    /// PBXDial
    /// </summary>
    public class PBXDialRepository : GenericDataRepository<PBXDial>, IPBXDialRepository
    {
    }

    /// <summary>
    /// SignicatSecureForm
    /// </summary>
    public class SignicatSecureFormRepository : GenericDataRepository<SignicatSecureForm>, ISignicatSecureFormRepository
    {
    }

    /// <summary>
    /// VisibleIn
    /// </summary>
    public class VisibleInRepository : GenericDataRepository<VisibleIn>, IVisibleInRepository
    {
    }

    /// <summary>
    /// MirroringLogType
    /// </summary>
    public class MirroringLogTypeRepository : GenericDataRepository<MirroringLogType>, IMirroringLogTypeRepository
    {
    }

    /// <summary>
    /// MirroringLogType
    /// </summary>
    public class MirroringStatusRepository : GenericDataRepository<MirroringStatus>, IMirroringStatusRepository
    {
    }

    /// <summary>
    /// MirroringLog
    /// </summary>
    public class MirroringLogRepository : GenericDataRepository<MirroringLog>, IMirroringLogRepository
    {
    }

    /// <summary>
    /// MirroringLogLevel
    /// </summary>
    public class MirroringLogLevelRepository : GenericDataRepository<MirroringLogLevel>, IMirroringLogLevelRepository
    {
    }

    /// <summary>
    /// APIKey
    /// </summary>
    public class APIKeyRepository : GenericDataRepository<APIKey>, IAPIKeyRepository
    {
    }

    /// <summary>
    /// SOCertificate
    /// </summary>
    public class SOCertificateRepository : GenericDataRepository<SOCertificate>, ISOCertificateRepository
    {
    }

    /// <summary>
    /// DataSync
    /// </summary>
    public class DataSyncRepository : GenericDataRepository<DataSync>, IDataSyncRepository
    {
    }

    /// <summary>
    /// InstallInfo
    /// </summary>
    public class InstallInfoRepository : GenericDataRepository<InstallInfo>, IInstallInfoRepository
    {
    }

    /// <summary>
    /// MachineName
    /// </summary>
    public class MachineNameRepository : GenericDataRepository<MachineName>, IMachineNameRepository
    {
    }

    /// <summary>
    /// SignicatEventStatus
    /// </summary>
    public class SignicatEventStatusRepository : GenericDataRepository<SignicatEventStatus>, ISignicatEventStatusRepository
    {
    }

    /// <summary>
    /// DocArcActivity
    /// </summary>
    public class DocArcActivityRepository : GenericDataRepository<DocArcActivity>, IDocArcActivityRepository
    {
    }

    /// <summary>
    /// SignicatEventLog
    /// </summary>
    public class SignicatEventLogRepository : GenericDataRepository<SignicatEventLog>, ISignicatEventLogRepository
    {
    }

    /// <summary>
    /// SignicatEventType
    /// </summary>
    public class SignicatEventTypeRepository : GenericDataRepository<SignicatEventType>, ISignicatEventTypeRepository
    {
    }

    /// <summary>
    /// SignicatEventOrigin
    /// </summary>
    public class SignicatEventOriginRepository : GenericDataRepository<SignicatEventOrigin>, ISignicatEventOriginRepository
    {
    }

    /// <summary>
    /// AppOnlineProvisioning
    /// </summary>
    public class AppOnlineProvisioningRepository : GenericDataRepository<AppOnlineProvisioning>, IAppOnlineProvisioningRepository
    {
    }

    /// <summary>
    /// DocArcAssociation
    /// </summary>
    public class DocArcAssociationRepository : GenericDataRepository<DocArcAssociation>, IDocArcAssociationRepository
    {
    }

    /// <summary>
    /// DocArcTemplate
    /// </summary>
    public class DocArcTemplateRepository : GenericDataRepository<DocArcTemplate>, IDocArcTemplateRepository
    {
    }

    /// <summary>
    /// DocArcAdministrators
    /// </summary>
    public class DocArcAdministratorsRepository : GenericDataRepository<DocArcAdministrators>, IDocArcAdministratorsRepository
    {
    }

    /// <summary>
    /// LOISLicenseCompany
    /// </summary>
    public class LOISLicenseCompanyRepository : GenericDataRepository<LOISLicenseCompany>, ILOISLicenseCompanyRepository
    {
    }

    /// <summary>
    /// LOISLicenseCompanyKeys
    /// </summary>
    public class LOISLicenseCompanyKeysRepository : GenericDataRepository<LOISLicenseCompanyKeys>, ILOISLicenseCompanyKeysRepository
    {
    }

    #endregion
}
