using AOS.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AOS.Platform.Common
{
    public static class AOSCommon
    {
        public static void CreateCookie(CookieType cookieType, int accountId)
        {
            var cookieTokenPrefix = (cookieType == CookieType.AOSCookie) ? "aoscookietoken" : "iframecookietoken";
            var cookieName = (cookieType == CookieType.AOSCookie) ? "aoscookie" : "iframecookie";

            // Encrypt, Base64 encode and URL ENcode cookie value
            var cookieValue = Utils.ProtectServicePassword($"{cookieTokenPrefix}_{accountId}");
            var cookieValueBase64 = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(cookieValue));
            var cookieValueBase64URLEncoded = HttpUtility.UrlEncode(cookieValueBase64);

            // Setup cookie if 
            HttpCookie cookie = new HttpCookie(cookieName);

            // Cookie must be readable for all subdomains under online.adwiza.com hence the "." wildcard
            cookie.Domain = ".online.adwiza.com";
            cookie.Value = cookieValueBase64URLEncoded;
            cookie.HttpOnly = true;
            cookie.Secure = true;
            cookie.Expires = DateTime.Now.AddYears(50); // For a cookie to effectively never expire

            // Add the cookie.
            HttpContext.Current.Response.Cookies.Add(cookie);


        }
    }
}