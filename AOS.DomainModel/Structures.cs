//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Timers;

//namespace AOS.DomainModel.Structures
//{
//    public class SoDocument
//    {
//        public SoDocument()
//        {
//            this.AssociateName = string.Empty;
//            this.AssociateLogin = string.Empty;
//            this.AssociateId = 0;
//            this.Attention = string.Empty;
//            this.ContactId = 0;
//            this.ContactName = string.Empty;
//            this.Description = string.Empty;
//            this.DocumentId = 0;
//            this.DocumentTemplate = string.Empty;
//            this.DocumentTemplateId = 0;
//            this.Header = string.Empty;
//            this.IsPublished = false;
//            this.Filename = string.Empty;
//            this.PersonName = string.Empty;
//            this.FirstName = string.Empty;
//            this.LastName = string.Empty;
//            this.PersonEmail = string.Empty;
//            this.PersonId = 0;
//            this.ProjectId = 0;
//            this.ProjectName = string.Empty;
//            this.YourRef = string.Empty;
//            this.Date = new DateTime();
//            this.UpdateDate = new DateTime();
//            this.PhysicalDocument = new byte[0];
//            this.SignicatStatus = SignDocumentStatus.ToBeSigned;
//            this.SignicatRequestId = string.Empty;
//            this.SignicatSendtDate = new DateTime();
//            this.SignicatDocumentId = string.Empty;
//            this.SignicatTaskId = string.Empty;
//            this.ConvertedToPDF = false;
//        }

//        public string AssociateName { get; set; }
//        public string AssociateLogin { get; set; }
//        public int AssociateId { get; set; }
//        public string Attention { get; set; }
//        public int ContactId { get; set; }
//        public string ContactName { get; set; }
//        public string Description { get; set; }
//        public int DocumentId { get; set; }
//        public string DocumentTemplate { get; set; }
//        public int DocumentTemplateId { get; set; }
//        public string Header { get; set; }
//        public bool IsPublished { get; set; }
//        public string Filename { get; set; }
//        public string PersonName { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string PersonEmail { get; set; }
//        public int PersonId { get; set; }
//        public int ProjectId { get; set; }
//        public string ProjectName { get; set; }
//        public string YourRef { get; set; }
//        public DateTime Date { get; set; }
//        public DateTime UpdateDate { get; set; }
//        public byte[] PhysicalDocument { get; set; }
//        public SignDocumentStatus SignicatStatus { get; set; }
//        public string SignicatRequestId { get; set; }
//        public DateTime SignicatSendtDate { get; set; }
//        public string SignicatDocumentId { get; set; }
//        public string SignicatTaskId { get; set; }
//        public bool ConvertedToPDF { get; set; }
//        public string Extension
//        {
//            get
//            {
//                return Path.GetExtension(Filename).Substring(1).ToLower();
//            }
//        }

//        public bool CanBeConverted
//        {
//            get
//            {

//                if (this.Extension.Equals("doc") ||
//                    this.Extension.Equals("xls") ||
//                    this.Extension.Equals("docx") ||
//                    this.Extension.Equals("xlsx"))
//                    return true;

//                return false;
//            }
//        }
//    }


//    public class SoDocumentTemplate
//    {
//        public SoDocumentTemplate()
//        {
//            this.DefaultOref = string.Empty;
//            this.DocumentTemplateId = 0;
//            this.Filename = string.Empty;
//            this.Name = string.Empty;
//        }

//        public string DefaultOref { get; set; }
//        public SoDocumentTemplateDirection Direction { get; set; }
//        public int DocumentTemplateId { get; set; }
//        public string Filename { get; set; }
//        public string Name { get; set; }
//        public SoDocumtentType Type { get; set; }
//    }


//    public enum SoDocumentTemplateDirection
//    {
//        Outgoing,
//        Incoming,
//        SaintAll,
//        Unknown
//    }

//    public enum SoDocumtentType
//    {
//        Document,
//        EMail,
//        Fax
//    }

//    public class SoAppointment
//    {
//        public SoAppointment() { }

//        public string VisibleFor { get; set; }
//        public int AppointmentId { get; set; }
//        public string TaskName { get; set; }
//        public string PersonFirstname { get; set; }
//        public string PersonLastname { get; set; }
//        public string ContactName { get; set; }
//        public string ContactPhones { get; set; }
//        public string ContactPostalAddress { get; set; }

//        public bool AllDayEvent { get; set; }
//        public bool FreeBusy { get; set; }
//        public bool HasAlarm { get; set; }

//        public DateTime EventStart { get; set; }
//        public DateTime EventEnd { get; set; }

//        public string Location { get; set; }
//        public string AssignedBy { get; set; }
//        public string Type { get; set; }

//    }

//    public class SoCompany
//    {
//        public SoCompany()
//        {
//            this.AssociateId = 0;
//            this.AssociateName = string.Empty;
//            this.ContactId = 0;
//            this.employees = new List<SoEmployee>();
//            this.Name = string.Empty;
//            this.Department = string.Empty;
//            this.Stop = false;
//            this.OrgNr = string.Empty;
//            this.Business = string.Empty;
//            this.VisitAddress = new SoAddress();
//            this.PostalAddress = new SoAddress();
//            this.Category = string.Empty;
//            this.OurContact = new SoEmployee();
//            this.Exits = false;
//            this.Number2 = string.Empty;
//        }


//        public int AssociateId { get; set; }
//        public string AssociateName { get; set; }

//        public int ContactId { get; set; }
//        public string Name { get; set; }
//        public string Department { get; set; }

//        public bool Stop { get; set; }
//        public string OrgNr { get; set; }
//        public string Business { get; set; }

//        public SoAddress VisitAddress { get; set; }
//        public SoAddress PostalAddress { get; set; }

//        public List<SoPhoneNumber> Phones { get; set; }
//        public List<SoEmailAddress> Emails { get; set; }
//        public List<SoUrlAddress> Webpages { get; set; }

//        private List<SoEmployee> employees;
//        public List<SoEmployee> Employees
//        {
//            get
//            {
//                return this.employees.OrderBy(x => x.Rank).ToList();
//            }
//            set { this.employees = value; }
//        }

//        public string Category { get; set; }

//        public SoEmployee OurContact { get; set; }
//        public bool Exits { get; set; }
//        public string Number2 { get; set; }
//    }
//    public class SoEmployee
//    {
//        public SoEmployee() { this.InvitationStatus = CalendarInvitationStatus.None; }

//        public int PersonId { get; set; }
//        public int AssociateId { get; set; }
//        public int ContactId { get; set; }

//        public bool IsAssociate { get { return this.AssociateId > 0; } }
//        public bool IsRetired { get; set; }

//        public string Firstname { get; set; }
//        public string Lastname { get; set; }
//        public string Name { get; set; }
//        public string Username { get; set; }
//        public string Title { get; set; }
//        public SoGenderType Gender { get; set; }
//        public string ContactName { get; set; }
//        public string ContactDepartment { get; set; }
//        public string ContactOrgNr { get; set; }

//        public byte[] Image { get; set; }

//        public SoAddress Address { get; set; }
//        public List<SoPhoneNumber> Phones { get; set; }
//        public List<SoEmailAddress> Emails { get; set; }
//        public List<SoUrlAddress> Webpages { get; set; }

//        public bool DoesMatch { get; set; }
//        public decimal AOSConnectionId { get; set; }

//        public string DialInPrefix { get; set; }

//        // to use when it's an Appointment Participant
//        public CalendarInvitationStatus InvitationStatus { get; set; }

//        public string Salutation { get; set; }
//        public short Rank { get; set; }
//        public string Thumbnail
//        {
//            get
//            {
//                string r = "person-";

//                if (this.Gender == SoGenderType.FEMALE)
//                    r += "female-";
//                else
//                    r += "male-";

//                if (this.IsRetired)
//                    r += "inactive.png";
//                else
//                    r += "active.png";


//                return r;
//            }
//        }
//    }

//    public class SoCategory
//    {
//        public SoCategory() { }

//        public int CategoryId { get; set; }
//        public string Name { get; set; }
//        public string ToolTip { get; set; }
//    }

//    public class SoAddress
//    {
//        public SoAddress() { this.Country = new SoCountry(); }

//        public string Street { get; set; }
//        public string ZipCode { get; set; }
//        public string City { get; set; }
//        public SoCountry Country { get; set; }

//        public string GoogleMapsAddress
//        {
//            get
//            {
//                string r = "http://maps.google.com/?q=";

//                if (!String.IsNullOrWhiteSpace(this.Street) && this.Street.Length > 0)
//                {
//                    short count = 0;
//                    string[] a = this.Street.Split(' ');
//                    foreach (string s in a)
//                    {
//                        decimal d = 0;
//                        if (decimal.TryParse(s, out d))
//                            count++;

//                        r += String.Concat(s, " ");

//                        if (count > 0)
//                            break;
//                    }

//                }

//                if (!String.IsNullOrWhiteSpace(this.City) && this.City.Length > 0)
//                    r += String.Concat(this.City.ToUpper(), " ");

//                if (this.Country != null && !String.IsNullOrWhiteSpace(this.Country.EnglishName) && this.Country.EnglishName.Length > 0)
//                    r += this.Country.EnglishName;

//                return r;
//            }
//        }

//        public double Latitude { get; set; }
//        public double Longitude { get; set; }
//    }
//    public class SoPhoneNumber
//    {
//        public SoPhoneNumber() { }

//        public string Number { get; set; }
//        public SoPhoneNumberType Type { get; set; }
//    }
//    public class SoEmailAddress
//    {
//        public SoEmailAddress() { }

//        public string Email { get; set; }
//        public string Description { get; set; }
//        public short Rank { get; set; }
//    }
//    public class SoUrlAddress
//    {
//        public SoUrlAddress() { }

//        public string Description { get; set; }

//        public string _url = "";
//        public string Url
//        {
//            get
//            {
//                string r = "";
//                if (this._url != null && this._url.Length > 0)
//                {
//                    if (!this._url.StartsWith("http"))
//                        r = String.Concat("http://", this._url.Trim());
//                    else
//                        r = this._url.Trim();
//                }
//                return r;
//            }
//            set { this._url = value; }
//        }
//        public short Rank { get; set; }
//        public SoUrlType Type { get; set; }
//    }
//    public class SoCountry
//    {
//        public SoCountry() { }

//        public int CountryId { get; set; }
//        public int CurrencyId { get; set; }
//        public string EnglishName { get; set; }
//        public string DialInPrefix { get; set; }
//        public string Iso2Name { get; set; }
//        public string Iso3Name { get; set; }
//    }

//    public class SoECPContact
//    {
//        public SoECPContact()
//        {
//        }

//        public int AssociateId { get; set; }
//        public string Business { get; set; }
//        public string Category { get; set; }
//        public string City { get; set; }
//        public int ContactId { get; set; }
//        public int CountryId { get; set; }
//        public string Country { get; set; }
//        public string Department { get; set; }

//        public string DirectPhone { get; set; }
//        public string Email { get; set; }
//        public string EmaiName { get; set; }

//        public string FormattedAddress { get; set; }
//        public string FullName { get; set; }
//        public bool IsOwnerContact { get; set; }
//        public string Kananame { get; set; }
//        public string Name { get; set; }
//        public string OrgNr { get; set; }
//        public string URL { get; set; }
//        public string URLName { get; set; }
//        public bool Exist { get; set; }
//    }

//    public enum SoPhoneNumberType
//    {
//        Fax, Direct, Phone, Mobile, Private, Unknown
//    }
//    public enum SoGenderType
//    {
//        UNKNOWN = 0, MALE = 2, FEMALE = 1
//    }
//    public enum SoAddressType
//    {
//        VisitAddress, PostalAddress, PersonAddress
//    }
//    public enum SoUrlType
//    {
//        Webpage = 0
//    }
//    public enum SoPhoneTypeIdx
//    {
//        Contact = 0, Person = 1
//    }

//    public class SoCultureInfo
//    {
//        public SoCultureInfo()
//        {
//            this.CountryId = 0;
//            this.CountryName = string.Empty;
//            this.CountryAbrv = string.Empty;
//            this.LanguageInfoId = 0;
//            this.LanguageInfoName = string.Empty;
//            this.CultureInfoAbrv = string.Empty;
//        }

//        public int CountryId { get; set; }
//        public string CountryName { get; set; }
//        public string CountryAbrv { get; set; }
//        public int LanguageInfoId { get; set; }
//        public string LanguageInfoName { get; set; }
//        public string CultureInfoAbrv { get; set; }
//    }


//    public class SoUserDefinedFieldInfo
//    {
//        public SoUserDefinedFieldInfo()
//        {
//            this.ColumnId = 0;
//            this.FieldDefault = string.Empty;
//            this.FieldHeight = 0;
//            this.FieldLabel = string.Empty;
//            this.FieldLeft = 0;
//            this.FieldTop = 0;
//            this.FieldWidth = 0;
//            this.FormatMask = string.Empty;
//            this.HasBeenPublished = false;
//            this.HideLabel = false;
//            this.IsIndexed = false;
//            this.IsMandatory = false;
//            this.IsReadOnly = false;
//            this.LabelHeight = 0;
//            this.LabelLeft = 0;
//            this.LabelTop = 0;
//            this.LabelWidth = 0;
//            this.LastVersionId = 0;
//            this.ListTableId = 0;
//            this.Page1LineNo = 0;
//            this.ProgId = string.Empty;
//            this.ShortLabel = string.Empty;
//            this.TabOrder = 0;
//            this.TemplateVariableName = string.Empty;
//            this.TextLength = 0;
//            this.Tooltip = string.Empty;
//            this.UDefFieldId = 0;
//            this.UdefIdentity = 0;
//            this.UDListDefinitionId = 0;
//            this.Version = 0;
//        }

//        public int ColumnId { get; set; }
//        public string FieldDefault { get; set; }
//        public short FieldHeight { get; set; }
//        public string FieldLabel { get; set; }
//        public short FieldLeft { get; set; }
//        public short FieldTop { get; set; }
//        public SoUDefFieldType FieldType { get; set; }
//        public short FieldWidth { get; set; }
//        public string FormatMask { get; set; }
//        public bool HasBeenPublished { get; set; }
//        public bool HideLabel { get; set; }
//        public bool IsIndexed { get; set; }
//        public bool IsMandatory { get; set; }
//        public bool IsReadOnly { get; set; }
//        public SoUdefJustification Justification { get; set; }
//        public short LabelHeight { get; set; }
//        public short LabelLeft { get; set; }
//        public short LabelTop { get; set; }
//        public short LabelWidth { get; set; }
//        public int LastVersionId { get; set; }
//        public short ListTableId { get; set; }
//        public short Page1LineNo { get; set; }
//        public string ProgId { get; set; }
//        public string ShortLabel { get; set; }
//        public short TabOrder { get; set; }
//        public string TemplateVariableName { get; set; }
//        public short TextLength { get; set; }
//        public string Tooltip { get; set; }
//        public SoUDefType Type { get; set; }
//        public int UDefFieldId { get; set; }
//        public int UdefIdentity { get; set; }
//        public int UDListDefinitionId { get; set; }
//        public short Version { get; set; }
//    }

//    public class SoMODListItem
//    {
//        public SoMODListItem()
//        {
//            this.MDOId = 0;
//            this.Name = string.Empty;
//            this.Rank = 0;
//            this.StyleHint = string.Empty;
//            this.Type = string.Empty;
//        }

//        public int MDOId { get; set; }
//        public string Name { get; set; }
//        public int Rank { get; set; }
//        public string StyleHint { get; set; }
//        public string Type { get; set; }
//    }



//    public enum SoUDefFieldType
//    {
//        Checkbox, List, Date, Decimal, LongText, Number, ShortText, UnlimitedDate
//    }

//    public enum SoUdefJustification
//    {
//        Center, Default, Left, Right
//    }

//    public enum SoUDefType
//    {
//        Appointment, Contact, Document, Invalid, None, Person, Project, Sale, Temp
//    }

//    public class AvailableClientServices
//    {
//        public AvailableClientServices() { }
//        /*
//        public IEnumerable<OS_Services> AvailableServices { get; set; }
//        public IEnumerable<OS_ClientServices> ClientServices { get; set; }
//        public OS_SuperOfficeDetails SuperOfficeDetails { get; set; }
//        */
//    }

//    [Serializable]
//    public class CalendarAppointment
//    {
//        public CalendarAppointment()
//        {
//            this.AppointmentId = 0;

//            this.VisibleFor = CalendarVisibilityType.All;
//            this.Type = CalendarAppointmentType.inDiary;
//            this.TaskType = CalendarTaskType.Appointment;

//            this.AssignmentStatus = CalendarAssignmentStatus.Assigning;
//            this.InvitationStatus = CalendarInvitationStatus.Accepted;
//            this.ActivityStatus = CalendarActivityStatus.NotStarted;

//            this.Registered = DateTime.UtcNow;
//            this.Updated = DateTime.UtcNow;
//            this.DoBy = DateTime.UtcNow;
//            this.EndDate = DateTime.UtcNow;

//            this.IsPrivate = false;
//            this.IsBusy = false;
//            this.IsAllDayEvent = false;

//            this.HasAlarm = false;
//            this.AlarmLeadTime = new TimeSpan();

//            this.ProjectName = "";
//            this.ProjectId = 0;

//            this.AlldayEvent = 0;
//            this.Location = "";
//            this.AppointmentText = "";

//            this.AssignedByName = "";
//            this.TaskName = "";

//            this.PersonDetails = new SoEmployee();
//            this.PersonFirstname = "";
//            this.PersonLastname = "";
//            this.PersonId = 0;

//            this.ContactId = 0;
//            this.ContactName = "";
//            this.ContactPhones = new List<SoPhoneNumber>();
//            this.ContactPostalAddress = new SoAddress();

//            this.Owner = new SoEmployee();
//            this.Participants = new List<SoEmployee>();
//        }

//        public int AppointmentId { get; set; }

//        public CalendarVisibilityType VisibleFor { get; set; }
//        public CalendarAppointmentType Type { get; set; }
//        public CalendarTaskType TaskType { get; set; }

//        public CalendarAssignmentStatus AssignmentStatus { get; set; }
//        public CalendarInvitationStatus InvitationStatus { get; set; }
//        public CalendarActivityStatus ActivityStatus { get; set; }
//        public CalendarBookingType BookingType { get; set; }

//        public DateTime Registered { get; set; }
//        public DateTime Updated { get; set; }
//        public DateTime DoBy { get; set; }
//        public DateTime EndDate { get; set; }

//        public bool IsPrivate { get; set; }
//        public bool IsBusy { get; set; }
//        public bool IsAllDayEvent { get; set; }
//        public bool IsCompleted { get { return this.ActivityStatus == CalendarActivityStatus.Completed; } }

//        public bool HasAlarm { get; set; }
//        public TimeSpan AlarmLeadTime { get; set; }

//        public string ProjectName { get; set; }
//        public int ProjectId { get; set; }

//        public int AlldayEvent { get; set; }
//        public string Location { get; set; }
//        public string AppointmentText { get; set; }

//        public SoEmployee Owner { get; set; }
//        public List<SoEmployee> Participants { get; set; }
//        public string AssignedByName { get; set; }
//        public string TaskName { get; set; }

//        public SoEmployee PersonDetails { get; set; }
//        public string PersonFirstname { get; set; }
//        public string PersonLastname { get; set; }
//        public int PersonId { get; set; }

//        public int ContactId { get; set; }
//        public string ContactName { get; set; }
//        public List<SoPhoneNumber> ContactPhones { get; set; }
//        public SoAddress ContactPostalAddress { get; set; }
//    }

//    public enum CalendarTaskType
//    {
//        Appointment, Document, Email, Fax, MailMergeDraft, MailMergeFinal, Phone, Report, SaintAll, ToDo, Unknown

//        //Service.SoWcfAppointment.TaskType.Appointment
//        //Service.SoWcfAppointment.TaskType.Document
//        //Service.SoWcfAppointment.TaskType.Email
//        //Service.SoWcfAppointment.TaskType.Fax
//        //Service.SoWcfAppointment.TaskType.MailMergeDraft
//        //Service.SoWcfAppointment.TaskType.MailMergeFinal
//        //Service.SoWcfAppointment.TaskType.Phone
//        //Service.SoWcfAppointment.TaskType.Report
//        //Service.SoWcfAppointment.TaskType.SaintAll
//        //Service.SoWcfAppointment.TaskType.ToDo
//        //Service.SoWcfAppointment.TaskType.Unknown
//    }
//    public enum CalendarInvitationStatus
//    {
//        Accepted, Cancelled, Declined, Hidden, Invitation, Moved, MovedSeen, None, Seen, Unknown

//        //Service.SoWcfAppointment.InvitationStatus.Accepted
//        //Service.SoWcfAppointment.InvitationStatus.Cancelled
//        //Service.SoWcfAppointment.InvitationStatus.Declined
//        //Service.SoWcfAppointment.InvitationStatus.Hidden
//        //Service.SoWcfAppointment.InvitationStatus.Invitation
//        //Service.SoWcfAppointment.InvitationStatus.Moved
//        //Service.SoWcfAppointment.InvitationStatus.MovedSeen
//        //Service.SoWcfAppointment.InvitationStatus.None
//        //Service.SoWcfAppointment.InvitationStatus.Seen
//        //Service.SoWcfAppointment.InvitationStatus.Unknown
//    }
//    public enum CalendarActivityStatus
//    {
//        Completed, NotStarted, Started, Unknown

//        //SoWcfAppointment.ActivityStatus.Unknown
//        //SoWcfAppointment.ActivityStatus.Started
//        //SoWcfAppointment.ActivityStatus.NotStarted
//        //SoWcfAppointment.ActivityStatus.Completed

//    }
//    public enum CalendarBookingType
//    {
//        None, Owner, Participant, Unknown

//        //SoWcfAppointment.BookingType.Unknown 
//        //SoWcfAppointment.BookingType.Participant 
//        //SoWcfAppointment.BookingType.Owner 
//        //SoWcfAppointment.BookingType.None 

//    }

//    public enum CalendarAssignmentStatus
//    {
//        Assigning, Declined, None, Seen, Unknown

//        //Service.SoWcfAppointment.AssignmentStatus.Assigning
//        //Service.SoWcfAppointment.AssignmentStatus.Declined
//        //Service.SoWcfAppointment.AssignmentStatus.None
//        //Service.SoWcfAppointment.AssignmentStatus.Seen
//        //Service.SoWcfAppointment.AssignmentStatus.Unknown
//    }

//    public enum CalendarVisibilityType
//    {
//        All, Associate, Group

//        //SuperOffice.Data.Visibility.All
//        //SuperOffice.Data.Visibility.Associate
//        //SuperOffice.Data.Visibility.Group
//    }
//    public enum CalendarAppointmentType
//    {
//        BookingForChecklist, BookingForDiary, Document, inChecklist, inDiary, MergeDraft, MergeFinal, Note, SavedReport, Unknown

//        //SuperOffice.Data.AppointmentType.BookingForChecklist
//        //SuperOffice.Data.AppointmentType.BookingForDiary
//        //SuperOffice.Data.AppointmentType.Document
//        //SuperOffice.Data.AppointmentType.inChecklist
//        //SuperOffice.Data.AppointmentType.inDiary
//        //SuperOffice.Data.AppointmentType.MergeDraft
//        //SuperOffice.Data.AppointmentType.MergeFinal
//        //SuperOffice.Data.AppointmentType.Note
//        //SuperOffice.Data.AppointmentType.SavedReport
//        //SuperOffice.Data.AppointmentType.Unknown
//    }

//    public class RegisterResponse
//    {
//        public RegisterResponse()
//        {
//            this.Error = "";
//            this.Email = "";
//            this.Message = "";
//        }

//        public string Message { get; set; }
//        public string Error { get; set; }
//        public string Email { get; set; }
//    }

//    public class LogOnResponse
//    {
//        public LogOnResponse()
//        {
//            this.Message = "";
//            this.Error = "";
//            this.Url = "";
//        }

//        public string Message { get; set; }
//        public string Error { get; set; }
//        public string Url { get; set; }
//    }

//    public class ContactResponse
//    {
//        public ContactResponse()
//        {
//            this.Message = "";
//            this.Error = "";
//        }

//        public string Message { get; set; }
//        public string Error { get; set; }
//    }

//    public class TestServiceConnectionResponse
//    {
//        public TestServiceConnectionResponse()
//        {
//            this.NetServerVersion = "";
//            this.DatabaseVersion = "";
//            this.SerialNumber = "";
//            this.DatabaseType = "";
//            this.CompanyName = "";
//            this.CompanyId = 0;
//            this.WsVersion = "";
//            this.Version = "";
//            this.Error = "";
//            this.Build = "";
//            this.Url = "";

//            this.TimeZoneId = 0;
//            this.TimeZoneLocationCode = "";
//        }

//        public string Url { get; set; }
//        public string Build { get; set; }
//        public string Error { get; set; }
//        public string Version { get; set; }
//        public string NetServerVersion { get; set; }
//        public string WsVersion { get; set; }
//        public string CompanyName { get; set; }
//        public int CompanyId { get; set; }
//        public string SerialNumber { get; set; }
//        public string DatabaseType { get; set; }
//        public string DatabaseVersion { get; set; }
//        public string TimeZoneLocationCode { get; set; }

//        public int TimeZoneId { get; set; }
//    }


//    public class UserMembershipHelper
//    {
//        public UserMembershipHelper() { }

//        public decimal UserId { get; set; }
//        public decimal ClientId { get; set; }
//        public int RoleId { get; set; }

//        public string UserName { get; set; }
//    }


//    public class CreateExtAppResponse
//    {
//        public CreateExtAppResponse()
//        {
//            this.ExtAppId = 0;
//            this.Name = string.Empty;
//            this.ToolTip = string.Empty;
//            this.WindowName = string.Empty;
//            this.AppType = 0;
//            this.Deleted = 0;
//            this.Parameters = string.Empty;
//            this.Workdir = string.Empty;
//            this.UpdatedCount = 0;
//            this.WaitToFinish = 0;
//            this.Url1 = string.Empty;
//            this.Url2 = string.Empty;
//            this.Filename = string.Empty;
//            this.Path = string.Empty;
//            this.Icon = 0;
//            this.Rank = 0;

//            this.ExtAppGroupLinkResponse = new CreateExtAppGroupLinkRespone();

//            this.Registrered = new DateTime();
//            this.RegisteredAssociateId = 0;
//            this.Updated = new DateTime();
//            this.UpdatedAssociateId = 0;
//        }

//        public int ExtAppId { get; set; }
//        public string Name { get; set; }
//        public string ToolTip { get; set; }
//        public string WindowName { get; set; }
//        public short AppType { get; set; }
//        public string Parameters { get; set; }
//        public short Deleted { get; set; }
//        public string Workdir { get; set; }
//        public short UpdatedCount { get; set; }
//        public short WaitToFinish { get; set; }
//        public string Url1 { get; set; }
//        public string Url2 { get; set; }
//        public string Filename { get; set; }
//        public string Path { get; set; }
//        public short Icon { get; set; }
//        public short Rank { get; set; }

//        public DateTime Registrered { get; set; }
//        public int RegisteredAssociateId { get; set; }
//        public DateTime Updated { get; set; }
//        public int UpdatedAssociateId { get; set; }

//        public CreateExtAppGroupLinkRespone ExtAppGroupLinkResponse { get; set; }
//    }

//    public class CreateExtAppGroupLinkRespone
//    {
//        public CreateExtAppGroupLinkRespone()
//        {
//            this.ExtappgrouplinkId = 0;
//            this.GroupId = 0;
//            this.Registered = new DateTime();
//            this.RegisteredAssociateId = 0;
//            this.Updated = new DateTime();
//            this.UpdatedAssociateId = 0;
//            this.UpdatedCount = 0;
//        }

//        public int ExtappgrouplinkId { get; set; }
//        public int GroupId { get; set; }
//        public DateTime Registered { get; set; }
//        public int RegisteredAssociateId { get; set; }
//        public DateTime Updated { get; set; }
//        public int UpdatedAssociateId { get; set; }
//        public short UpdatedCount { get; set; }
//    }

//    public class ActivateRWStatus
//    {
//        public ActivateRWStatus()
//        {
//            this.ActivateRWCode = 0;
//            this.ActivateRWMessage = string.Empty;
//        }

//        public int ActivateRWCode { get; set; }
//        public string ActivateRWMessage { get; set; }
//    }

//    public class RWSalesStakeHolder
//    {
//        public RWSalesStakeHolder()
//        {
//            this.PersonId = 0;
//            this.ContactId = 0;
//            this.ContactorgNr = 0;
//            this.PersonName = string.Empty;
//            this.PersonEmail = string.Empty;
//            this.ContactName = string.Empty;
//            this.SaleId = 0;
//            this.SaleName = string.Empty;
//        }

//        public int PersonId { get; set; }
//        public int ContactId { get; set; }
//        public int ContactorgNr { get; set; }
//        public string PersonName { get; set; }
//        public string PersonEmail { get; set; }
//        public string ContactName { get; set; }
//        public int SaleId { get; set; }
//        public string SaleName { get; set; }
//    }

//    public class RWAppointmentParticipant
//    {
//        public RWAppointmentParticipant()
//        {
//            this.PersonId = 0;
//            this.ContactId = 0;
//            this.ContactorgNr = 0;
//            this.PersonName = string.Empty;
//            this.PersonEmail = string.Empty;
//            this.ContactName = string.Empty;
//            this.AppointId = 0;
//            this.AppointDescription = string.Empty;
//        }

//        public int PersonId { get; set; }
//        public int ContactId { get; set; }
//        public int ContactorgNr { get; set; }
//        public string PersonName { get; set; }
//        public string PersonEmail { get; set; }
//        public string ContactName { get; set; }
//        public int AppointId { get; set; }
//        public string AppointDescription { get; set; }

//    }


//    public class RWSelection
//    {
//        public RWSelection()
//        {
//            this.PersonId = 0;
//            this.ContactId = 0;
//            this.PersonName = string.Empty;
//            this.PersonEmail = string.Empty;
//            this.ContactName = string.Empty;
//            this.SelectionId = 0;
//            this.SelectionName = string.Empty;
//            this.MemberCount = 0;
//        }

//        public int PersonId { get; set; }
//        public int ContactId { get; set; }
//        public string PersonName { get; set; }
//        public string PersonEmail { get; set; }
//        public string ContactName { get; set; }
//        public int SelectionId { get; set; }
//        public string SelectionName { get; set; }
//        public uint MemberCount { get; set; }
//    }

//    public class RWSendRequestResponse
//    {
//        public RWSendRequestResponse()
//        {
//            this.ResponseCode = 0;
//            this.ResponseMessage = string.Empty;
//            this.PersonId = 0;
//            this.ContactId = 0;
//            this.SaleId = 0;
//            this.AppointmentId = 0;
//            this.PersonName = string.Empty;
//            this.ContactName = string.Empty;
//        }

//        public int ResponseCode { get; set; }
//        public string ResponseMessage { get; set; }
//        public int PersonId { get; set; }
//        public int ContactId { get; set; }
//        public int SaleId { get; set; }
//        public int AppointmentId { get; set; }
//        public string PersonName { get; set; }
//        public string ContactName { get; set; }
//    }

//    public class RWActivateTriggerResponse
//    {
//        public RWActivateTriggerResponse()
//        {
//            this.ActivateCode = 0;
//            this.ActivateMessage = string.Empty;
//        }

//        public int ActivateCode { get; set; }
//        public string ActivateMessage { get; set; }
//        public Timer TimerObject { get; set; }
//    }

//    public class RWAppointmentTask
//    {
//        public RWAppointmentTask()
//        {
//            this.AppointmentTaskId = 0;
//            this.AppointmentTaskName = string.Empty;
//        }

//        public int AppointmentTaskId { get; set; }
//        public string AppointmentTaskName { get; set; }
//    }

//    public class RWSoTemplateVariable
//    {
//        public RWSoTemplateVariable()
//        {
//            this.VariableCode = string.Empty;
//            this.VariableName = string.Empty;
//            this.SoVariableId = string.Empty;
//            this.ChoosenVariableId = string.Empty;
//            this.ContactId = string.Empty;
//            this.SelectionId = string.Empty;
//            this.SaleId = string.Empty;
//            this.AppointmentId = string.Empty;
//        }

//        public string VariableCode { get; set; }
//        public string VariableName { get; set; }
//        public string SoVariableId { get; set; }
//        public string ChoosenVariableId { get; set; }
//        public string ContactId { get; set; }
//        public string SelectionId { get; set; }
//        public string SaleId { get; set; }
//        public string AppointmentId { get; set; }
//    }

//    public class RWCheck
//    {
//        public RWCheck()
//        {
//            this.RWId = string.Empty;
//            this.ClientId = string.Empty;
//            this.EnableSale = false;
//            this.EnableAppointment = false;
//        }

//        public string RWId { get; set; }
//        public string ClientId { get; set; }
//        public bool EnableSale { get; set; }
//        public bool EnableAppointment { get; set; }
//    }


//    public class SignicatSubject
//    {
//        public SignicatSubject()
//        {
//            this.Id = string.Empty;
//            this.FirstName = string.Empty;
//            this.LastName = string.Empty;
//            this.NationalId = string.Empty;
//        }

//        public string Id { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string NationalId { get; set; }
//    }

//    public class SignicatDocumentRequest
//    {
//        public SignicatDocumentRequest()
//        {
//            this.RequestVersion = string.Empty;
//            this.RequestService = string.Empty;
//            this.RequestProfile = string.Empty;
//            this.RequestPassword = string.Empty;
//            this.RequestLanguage = string.Empty;
//            this.RequestOntaskcomplete = string.Empty;
//            this.RequestOntaskcancel = string.Empty;
//            this.RequestClientReference = string.Empty;
//            this.RequestRedirectUrl = string.Empty;
//            this.RequestMultible = false;
//            this.IsProd = false;
//            this.ThumbPrint = string.Empty;
//        }

//        public string RequestVersion { get; set; }
//        public string RequestService { get; set; }
//        public string RequestProfile { get; set; }
//        public string RequestPassword { get; set; }
//        public string RequestLanguage { get; set; }
//        public string RequestOntaskcomplete { get; set; }
//        public string RequestOntaskcancel { get; set; }
//        public string RequestClientReference { get; set; }
//        public string RequestRedirectUrl { get; set; }
//        public bool RequestMultible { get; set; }
//        public bool IsProd { get; set; }
//        public string WebServiceEndPoint { get; set; }
//        public string ThumbPrint { get; set; }
//        public SignicatSubject Subject { get; set; }
//        public SignicatTask[] TaskArray { get; set; }
//    }

//    public class SignicatTask
//    {
//        public SignicatTask()
//        {
//            this.Id = string.Empty;
//            this.Authentication = new SignicatAuthentication();
//            this.Ontaskcomplete = string.Empty;
//            this.Ontaskcancel = string.Empty;
//        }

//        public string Id { get; set; }
//        public string Ontaskcomplete { get; set; }
//        public string Ontaskcancel { get; set; }
//        public SignicatAuthentication Authentication { get; set; }
//        public SignicatSignature[] SignatureArray { get; set; }
//        public SignicatDocumentaction[] DocumentationArray { get; set; }
//        public SignicatNotification[] NotificationArray { get; set; }
//        public SignicatSubject Subject { get; set; }
//    }

//    public class SignicatAuthentication
//    {
//        public SignicatAuthentication()
//        {
//            this.Artifact = false;
//            this.ArtifactSpecified = false;
//        }

//        public bool Artifact { get; set; }
//        public bool ArtifactSpecified { get; set; }
//        public string[] Method { get; set; }
//    }

//    public class SignicatSignature
//    {
//        public SignicatSignature() { }

//        public string[] MethodArray { get; set; }
//    }

//    public class SignicatDocumentaction
//    {
//        public SignicatDocumentaction()
//        {
//            this.Document = new SignicatDocument();
//            this.DocumentactionType = string.Empty;
//        }

//        public SignicatDocument Document { get; set; }
//        public string DocumentactionType { get; set; }
//    }

//    public class SignicatDocument
//    {
//        public SignicatDocument()
//        {
//            this.Id = string.Empty;
//            this.Description = string.Empty;
//            this.Mimetype = string.Empty;
//            this.Source = string.Empty;
//        }

//        public string Id { get; set; }
//        public string Description { get; set; }
//        public string Mimetype { get; set; }
//        public byte[] Data { get; set; }
//        public string Source { get; set; }
//    }

//    public class SignicatNotification
//    {
//        public SignicatNotification()
//        {
//            this.RecipientEmail = string.Empty;
//            this.Header = string.Empty;
//            this.Message = string.Empty;
//            this.Body = string.Empty;
//            this.Greetings = string.Empty;
//            this.SenderEmail = string.Empty;
//            this.Company = string.Empty;
//            this.SenderName = string.Empty;
//        }

//        public string RecipientEmail { get; set; }
//        public string Header { get; set; }
//        public string Message { get; set; }
//        public string Body { get; set; }
//        public string Greetings { get; set; }
//        public string SenderEmail { get; set; }
//        public string Company { get; set; }
//        public string SenderName { get; set; }
//    }

//    public class ActivateSignicatStatus
//    {
//        public ActivateSignicatStatus()
//        {
//            this.ActivateSigCode = 0;
//            this.ActivateSigMessage = string.Empty;
//        }

//        public int ActivateSigCode { get; set; }
//        public string ActivateSigMessage { get; set; }
//    }

//    public class SaveSignicatObject
//    {

//        public SaveSignicatObject()
//        {
//            this.AccountId = 0;
//            this.UserId = 0;
//            this.SignicatVersion = 0;
//            this.SignicatService = string.Empty;
//            this.SignicatPassword = string.Empty;
//            this.SignicatTaskId = string.Empty;
//            this.SoWebPanelFrontId = 0;
//            this.SoWebPanelBackId = 0;
//            this.SoStatusSentUdefId = 0;
//            this.SoStatusCompletedUdefId = 0;
//            this.SoStatusProcessedUdefId = 0;
//            this.SoStatusExpiredUdefId = 0;
//            this.SoStatusRejectedUdefId = 0;
//            this.SoStatusToBeSignUdefId = 0;
//            this.SignWebServiceUrl = string.Empty;
//            this.SignMultibleSign = false;
//            this.Enabled = false;
//            this.UpdateTime = new DateTime();
//            this.LastActivityTime = new DateTime();
//        }

//        public int? AccountId { get; set; }
//        public decimal UserId { get; set; }
//        public int SignicatVersion { get; set; }
//        public string SignicatService { get; set; }
//        public string SignicatPassword { get; set; }
//        public string SignicatTaskId { get; set; }
//        public int SoWebPanelFrontId { get; set; }
//        public int SoWebPanelBackId { get; set; }
//        public int SoStatusSentUdefId { get; set; }
//        public int SoStatusCompletedUdefId { get; set; }
//        public int SoStatusProcessedUdefId { get; set; }
//        public int SoStatusExpiredUdefId { get; set; }
//        public int SoStatusRejectedUdefId { get; set; }
//        public int SoStatusToBeSignUdefId { get; set; }
//        public bool SignMultibleSign { get; set; }
//        public string SignWebServiceUrl { get; set; }
//        public bool Enabled { get; set; }
//        public DateTime UpdateTime { get; set; }
//        public DateTime LastActivityTime { get; set; }
//    }

//    public class CreateWebPanelStatus
//    {
//        public CreateWebPanelStatus()
//        {
//            this.CreateSuccess = false;
//            this.SoWebPanelId = 0;
//            this.ErrorMessage = string.Empty;
//        }

//        public bool CreateSuccess { get; set; }
//        public int SoWebPanelId { get; set; }
//        public string ErrorMessage { get; set; }
//    }


//    public class CreateWebPanelObj
//    {
//        public CreateWebPanelObj()
//        {
//            this.PanelName = string.Empty;
//            this.WindowName = string.Empty;
//            this.Rank = 0;
//            this.EndPointUrl = string.Empty;
//            this.EncryptClientId = string.Empty;
//            this.SignicatUrl = string.Empty;
//        }

//        public string PanelName { get; set; }
//        public string WindowName { get; set; }
//        public int Rank { get; set; }
//        public string EndPointUrl { get; set; }
//        public string EncryptClientId { get; set; }
//        public string SignicatUrl { get; set; }
//        public WebPanelNavigation Navigation { get; set; }
//    }

//    public enum WebPanelNavigation
//    {
//        ContactArchive, BrowserPanel
//    }

//    public class SignCheck
//    {
//        public SignCheck()
//        {
//            this.SignicatId = string.Empty;
//            this.ClientId = string.Empty;
//            this.EnableSignicat = false;
//        }

//        public string SignicatId { get; set; }
//        public string ClientId { get; set; }
//        public bool EnableSignicat { get; set; }
//    }

//    public enum SignDocumentStatus
//    {
//        ToBeSigned, Processed, Signed, Sent, Expired, Rejected,
//    }

//    public class SignSendRequestStatus
//    {
//        public SignSendRequestStatus()
//        {
//            this.SucceesCode = 0;
//            this.RedirectUrl = string.Empty;
//            this.ErrorMessage = string.Empty;
//            this.SingRequestId = string.Empty;
//        }

//        public int SucceesCode { get; set; }
//        public string RedirectUrl { get; set; }
//        public string ErrorMessage { get; set; }
//        public string SingRequestId { get; set; }
//    }


//    public class SignStatusOverView
//    {
//        public SignStatusOverView()
//        {
//            this.Status = string.Empty;
//            this.Sender = string.Empty;
//            this.SenderAssId = 0;
//            this.Title = string.Empty;
//            this.DocumentId = string.Empty;
//            this.Receiver = string.Empty;
//            this.Company = string.Empty;
//            this.Type = string.Empty;
//            this.SentDate = string.Empty;
//            this.LastModifiedDate = string.Empty;
//            this.SignRequestId = string.Empty;
//        }

//        public string Status { get; set; }
//        public string Sender { get; set; }
//        public int SenderAssId { get; set; }
//        public string Title { get; set; }
//        public string DocumentId { get; set; }
//        public string Receiver { get; set; }
//        public string Company { get; set; }
//        public string Type { get; set; }
//        public string SentDate { get; set; }
//        public string LastModifiedDate { get; set; }
//        public string SignRequestId { get; set; }
//    }

//    public class SignDocumentOverView
//    {

//        public SignDocumentOverView()
//        {
//            this.CreateDate = string.Empty;
//            this.SignicatSendtDate = string.Empty;
//            this.UpdateDate = string.Empty;
//            this.DocumentTemplate = string.Empty;
//            this.Description = string.Empty;
//            this.PersonName = string.Empty;
//            this.DocumentStatus = string.Empty;
//            this.DocumentId = string.Empty;
//            this.FirstName = string.Empty;
//            this.LastName = string.Empty;
//            this.PersonEmail = string.Empty;
//        }

//        public string CreateDate { get; set; }
//        public string SignicatSendtDate { get; set; }
//        public string UpdateDate { get; set; }
//        public string DocumentTemplate { get; set; }
//        public string Description { get; set; }
//        public string PersonName { get; set; }
//        public string DocumentStatus { get; set; }
//        public string DocumentId { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string PersonEmail { get; set; }
//    }


//    public class SignicatCreateRequest
//    {

//        public SignicatCreateRequest()
//        {
//            this.DocDescript = string.Empty;
//            this.RecieFirstName = string.Empty;
//            this.RecieLastName = string.Empty;
//            this.RecieEmail = string.Empty;
//            this.Valid = false;
//            this.MoPhone = string.Empty;
//            this.Lang = string.Empty;
//            this.Meth = string.Empty;
//            this.NotiMessage = string.Empty;
//            this.NotiHeader = string.Empty;
//            this.NotiBody = string.Empty;
//            this.SoDocuId = string.Empty;
//            this.AosClientUrl = string.Empty;
//            this.CompleteRedirectUrl = string.Empty;
//            this.CancelRedirectUrl = string.Empty;
//            this.SenderPersonName = string.Empty;
//            this.SenderCompanyName = string.Empty;
//            this.SenderEmail = string.Empty;
//            this.SenderGreetings = string.Empty;
//        }

//        public string DocDescript { get; set; }
//        public string RecieFirstName { get; set; }
//        public string RecieLastName { get; set; }
//        public string RecieEmail { get; set; }
//        public bool Valid { get; set; }
//        public string MoPhone { get; set; }
//        public string Lang { get; set; }
//        public SignMimeType MType { get; set; }
//        public string Meth { get; set; }
//        public string NotiHeader { get; set; }
//        public string NotiMessage { get; set; }
//        public string NotiBody { get; set; }
//        public string SoDocuId { get; set; }
//        public string AosClientUrl { get; set; }
//        public string CompleteRedirectUrl { get; set; }
//        public string CancelRedirectUrl { get; set; }
//        public string SenderPersonName { get; set; }
//        public string SenderCompanyName { get; set; }
//        public string SenderSoUserName { get; set; }
//        public string SenderEmail { get; set; }
//        public string SenderGreetings { get; set; }
//    }

//    public enum SignMimeType
//    {
//        Pdf, Office
//    }

//    public class SignicatLogggingObj
//    {
//        public SignicatLogggingObj()
//        {
//            this.AOSAcountId = 0;
//            this.SOContactId = 0;
//            this.SODocumentId = 0;
//            this.SignMeth = string.Empty;
//            this.DocTypeExtension = string.Empty;
//            this.SOReceiverEmail = string.Empty;
//            this.SOSenderUserName = string.Empty;
//        }

//        public decimal AOSAcountId { get; set; }
//        public int SOContactId { get; set; }
//        public int SODocumentId { get; set; }
//        public string SignMeth { get; set; }
//        public string DocTypeExtension { get; set; }
//        public string SOReceiverEmail { get; set; }
//        public string SOSenderUserName { get; set; }

//    }

//    public class CreatePhysicalDocumentStatus
//    {
//        public CreatePhysicalDocumentStatus()
//        {
//            this.CreateStatusCode = 0;
//            this.SoDocumentId = 0;
//            this.SoDocumentName = string.Empty;
//            this.ErrorMessage = string.Empty;
//            this.SignReqId = string.Empty;
//        }

//        public int CreateStatusCode { get; set; }
//        public int SoDocumentId { get; set; }
//        public string SoDocumentName { get; set; }
//        public string ErrorMessage { get; set; }
//        public string SignReqId { get; set; }
//    }

//    public class CreatePhysicalDocumentObj
//    {
//        public CreatePhysicalDocumentObj()
//        {
//            this.EndPointUrl = string.Empty;
//            this.PhysicalDoc = new byte[0];
//            this.SoDocumentId = 0;
//            this.SoContactId = 0;
//            this.SoDocumentExtension = string.Empty;
//            this.NamePrefix = string.Empty;
//            this.SignRequestId = string.Empty;
//            this.SignTaskId = string.Empty;
//            this.SignDocId = string.Empty;
//            this.SignService = string.Empty;
//            this.SignPassword = string.Empty;
//            this.SoStatusSentUdefId = 0;
//            this.SoStatusCompletedUdefId = 0;
//            this.SoStatusProcessedUdefId = 0;
//            this.SoStatusExpiredUdefId = 0;
//            this.SoStatusRejectedUdefId = 0;
//            this.SoStatusSignUdefId = 0;
//            this.SignicatRedirectUrl = string.Empty;
//            this.WebServiceEndPoint = string.Empty;
//            this.IsProd = false;
//            this.ThumbPrint = string.Empty;
//        }

//        public string EndPointUrl { get; set; }
//        public byte[] PhysicalDoc { get; set; }
//        public int SoDocumentId { get; set; }
//        public int SoContactId { get; set; }
//        public string SoDocumentExtension { get; set; }
//        public string NamePrefix { get; set; }
//        public string SignRequestId { get; set; }
//        public SignDocumentStatus SignStatus { get; set; }
//        public string StatusUdefKey { get; set; }
//        public string SignTaskId { get; set; }
//        public string SignDocId { get; set; }
//        public string SignService { get; set; }
//        public string SignPassword { get; set; }
//        public string SignicatRedirectUrl { get; set; }
//        public string WebServiceEndPoint { get; set; }
//        public bool IsProd { get; set; }
//        public string ThumbPrint { get; set; }
//        public int SoStatusSentUdefId { get; set; }
//        public int SoStatusCompletedUdefId { get; set; }
//        public int SoStatusProcessedUdefId { get; set; }
//        public int SoStatusExpiredUdefId { get; set; }
//        public int SoStatusRejectedUdefId { get; set; }
//        public int SoStatusSignUdefId { get; set; }
//    }


//    public class PdfConvertObjtInput
//    {
//        public PdfConvertObjtInput()
//        {
//            this.PhysDoc = new byte[0];
//            this.SoAssociateName = string.Empty;
//            this.SoSerialNo = string.Empty;
//            this.FileName = string.Empty;
//            this.SoAssociateId = 0;
//        }

//        public byte[] PhysDoc { get; set; }
//        public string FileName { get; set; }
//        public string SoAssociateName { get; set; }
//        public int SoAssociateId { get; set; }
//        public string SoSerialNo { get; set; }
//    }

//    public class PdfConvertObjtOutput
//    {
//        public PdfConvertObjtOutput()
//        {
//            this.PhysPdfDoc = new byte[0];
//            this.StatusCode = 0;
//            this.ErrorMessage = string.Empty;
//        }

//        public byte[] PhysPdfDoc { get; set; }
//        public int StatusCode { get; set; }
//        public string ErrorMessage { get; set; }
//    }

//    public class PdfConvertStatus
//    {
//        public PdfConvertStatus()
//        {
//            this.ErrorMessage = string.Empty;
//            this.SucceesCode = 0;
//        }

//        public string ErrorMessage { get; set; }
//        public int SucceesCode { get; set; }
//    }

//    public class CreateListStatus
//    {
//        public CreateListStatus()
//        {
//            this.CreateSuccess = false;
//            this.UdListDefinitionId = 0;
//            this.ErrorMessage = string.Empty;
//        }

//        public bool CreateSuccess { get; set; }
//        public int UdListDefinitionId { get; set; }
//        public string ErrorMessage { get; set; }
//    }

//    public class CreateListObj
//    {
//        public CreateListObj()
//        {
//            this.Deleted = false;
//            this.Id = 0;
//            this.InUseByUserDefinedFields = false;
//            this.IsCustomList = false;
//            this.IsMDOList = false;
//            this.ListType = string.Empty;
//            this.Name = string.Empty;
//            this.Rank = 0;
//            this.Tooltip = string.Empty;
//        }

//        public bool Deleted { get; set; }
//        public int Id { get; set; }
//        public bool InUseByUserDefinedFields { get; set; }
//        public bool IsCustomList { get; set; }
//        public bool IsMDOList { get; set; }
//        public string ListType { get; set; }
//        public string Name { get; set; }
//        public int Rank { get; set; }
//        public string Tooltip { get; set; }
//        public bool UseGroupsAndHeadings { get; set; }
//    }

//    public class CreateListItemObj
//    {
//        public CreateListItemObj()
//        {
//            this.Deleted = false;
//            this.Id = 0;
//            this.Name = string.Empty;
//            this.Rank = 0;
//            this.Tooltip = string.Empty;
//            this.UDListDefinitionId = 0;
//        }

//        public bool Deleted { get; set; }
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public int Rank { get; set; }
//        public string Tooltip { get; set; }
//        public int UDListDefinitionId { get; set; }
//    }

//    public class CreateListItemStatus
//    {
//        public CreateListItemStatus()
//        {
//            this.CreateSuccess = false;
//            this.UdListId = 0;
//            this.UdListName = string.Empty;
//            this.ErrorMessage = string.Empty;
//        }

//        public bool CreateSuccess { get; set; }
//        public int UdListId { get; set; }
//        public string UdListName { get; set; }
//        public string ErrorMessage { get; set; }
//    }

//    public class CreatePersonStatus
//    {
//        public CreatePersonStatus()
//        {
//            this.ErrorMessage = string.Empty;
//            this.SucceesCode = 0;
//        }

//        public string ErrorMessage { get; set; }
//        public int SucceesCode { get; set; }
//    }

//    public class SignWebServiceParameters
//    {
//        public SignWebServiceParameters()
//        {
//            this.ServiceName = string.Empty;
//            this.ServicePassWord = string.Empty;
//            this.ServiceEndPoint = string.Empty;
//            this.IsProdService = false;
//            this.ThumbPrint = string.Empty;
//        }

//        public string ServiceName { get; set; }
//        public string ServicePassWord { get; set; }
//        public string ServiceEndPoint { get; set; }
//        public bool IsProdService { get; set; }
//        public string ThumbPrint { get; set; }
//    }


//    public class AOSSaveSuccessStatus
//    {
//        public AOSSaveSuccessStatus()
//        {
//            this.ErrorMessage = string.Empty;
//            this.SaveSuccess = false;
//        }

//        public string ErrorMessage { get; set; }
//        public bool SaveSuccess { get; set; }
//    }

//}
