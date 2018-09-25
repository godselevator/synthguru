using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.DomainModel
{
    public class Customer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
    }

    public class Contact
    {
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Country {get; set; }
        public string Number {get; set; }
        public string Code {get; set; }
        public string AssociateId {get; set; }
        public string Business {get; set; }
        public string Category {get; set; }
        public string Stop {get; set; }
        public string NoMail {get; set; }
        public string Orgnr {get; set; }
        public string PostAddressline1 {get; set; }
        public string PostAddressline2 {get; set; }
        public string PostAddressline3 {get; set; }
        public string PostAddresscity {get; set; }
        public string PostAddresszip {get; set; }
        public string PostAddressstate {get; set; }
        public string PostAddresscountry {get; set; }
        public string StreetAddressline1 {get; set; }
        public string StreetAddressline2 {get; set; }
        public string StreetAddressline3 {get; set; }
        public string StreetAddresscity {get; set; }
        public string StreetAddresszip {get; set; }
        public string StreetAddressstate {get; set; }
        public string StreetAddresscountry {get; set; }
        public string Fax {get; set; }
        public string Phone {get; set; }
        public string Phone2 {get; set; }
        public string Phone3 {get; set; }
        public string Email {get; set; }
        public string Website {get; set; }
    }

    public class Selection
    {
        public int SelectionId { get; set; }
        public string Name { get; set; }
    }

    public class SelMember
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
