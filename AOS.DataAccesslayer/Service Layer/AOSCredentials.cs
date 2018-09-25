using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.DataAccessLayer
{
    public interface IAOSCredentials
    {
        bool Success { get; set; }
        bool AuthenticationSucceeded { get; set; }
        string ErrorMsg { get; set; }

        SoPrincipal75.SoCredentials Credentials75 { get; set; }
        SoPrincipal75.SoTimeZone TimeZone75 { get; set; }
        SoPrincipal75.SoPrincipalCarrier ResponseCarrier75 { get; set; }
        SoPrincipal75.SoExtraInfo ExtraInfo75 { get; set; }

        SoPrincipal82.SoCredentials Credentials82 { get; set; }
        SoPrincipal82.SoTimeZone TimeZone82 { get; set; }
        SoPrincipal82.SoPrincipalCarrier ResponseCarrier82 { get; set; }
        SoPrincipal82.SoExtraInfo ExtraInfo82 { get; set; }

    }

    public abstract class AOSCredentialsBase : IAOSCredentials
    {
        public virtual bool Success { get; set; }
        public virtual bool AuthenticationSucceeded { get; set; }
        public virtual string ErrorMsg { get; set; }

        public virtual SoPrincipal75.SoCredentials Credentials75 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual SoPrincipal75.SoTimeZone TimeZone75 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual SoPrincipal75.SoPrincipalCarrier ResponseCarrier75 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual SoPrincipal75.SoExtraInfo ExtraInfo75 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public virtual SoPrincipal82.SoCredentials Credentials82 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual SoPrincipal82.SoTimeZone TimeZone82 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual SoPrincipal82.SoPrincipalCarrier ResponseCarrier82 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual SoPrincipal82.SoExtraInfo ExtraInfo82 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
    }

    public class AOSCred75 : AOSCredentialsBase
    {
        public override bool Success { get; set; }
        public override bool AuthenticationSucceeded { get; set; }
        public override string ErrorMsg { get; set; }

        public override SoPrincipal75.SoCredentials Credentials75 { get; set; }
        public override SoPrincipal75.SoTimeZone TimeZone75 { get; set; }
        public override SoPrincipal75.SoPrincipalCarrier ResponseCarrier75 { get; set; }
        public override SoPrincipal75.SoExtraInfo ExtraInfo75 { get; set; }
    }

    public class AOSCred82 : AOSCredentialsBase
    {
        public override bool Success { get; set; }
        public override bool AuthenticationSucceeded { get; set; }
        public override string ErrorMsg { get; set; }

        public override SoPrincipal82.SoCredentials Credentials82 { get; set; }
        public override SoPrincipal82.SoTimeZone TimeZone82 { get; set; }
        public override SoPrincipal82.SoPrincipalCarrier ResponseCarrier82 { get; set; }
        public override SoPrincipal82.SoExtraInfo ExtraInfo82 { get; set; }
    }
}
