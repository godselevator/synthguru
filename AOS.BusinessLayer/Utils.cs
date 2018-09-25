using Adwiza.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public static class Utils
    {
        public static string EncryptKey = "Adwiza Security - Adwiza Online Services";

        public static string GenerateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            //int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            return new string(chars);
        }

        /// <summary>
        /// Parses the URL of a service call
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The Person Id that his encoded in the hash</returns>
        public static int ParseHashUrl(string url)
        {
            try
            {
                string code = url;

                if (url.StartsWith("http"))
                {
                    int pos = url.Split('/').Length - 1;
                    code = url.Split('/')[pos];
                }

                RijndaelEnhanced encryptQuery = new RijndaelEnhanced(EncryptKey);

                // let's append the Hash ending
                code = code.EndsWith("==") ? code : code + "==";

                // Decode
                int number = 0;
                int.TryParse(encryptQuery.Decrypt(code), out number);

                return number;
            }
            catch (Exception)
            {

            }
            return 0;
        }
        /// <summary>
        /// Generates a Hashed from the passed EntityId, suitable for encoding the service call
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="skipSpecialChars"></param>
        /// <returns></returns>
        public static string GenerateHash(decimal entityId, bool skipSpecialChars = false)
        {
            try
            {
                RijndaelEnhanced encryptQuery = new RijndaelEnhanced(EncryptKey);
                string hash = encryptQuery.Encrypt(entityId.ToString());

                if (!skipSpecialChars)
                    while (hash.Contains("/") || hash.Contains(" ") || hash.Contains("+"))
                        hash = encryptQuery.Encrypt(entityId.ToString());

                // let's remove the Hash ending
                hash = hash.Replace("=", "");
                return hash;
            }
            catch (Exception)
            {
            }

            return "";
        }

        /// <summary>
        /// Generates a Hashed form the input, suitable for storing password
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ProtectServicePassword(string str, bool removeSpecialCharsForUrlUse = false)
        {
            try
            {
                RijndaelEnhanced encryptQuery = new RijndaelEnhanced(EncryptKey);
                string hash = encryptQuery.Encrypt(str);

                if (removeSpecialCharsForUrlUse)
                    while (hash.Contains("+") || hash.Contains("/"))
                    {
                        // let's remove the Hash ending
                        hash = encryptQuery.Encrypt(str).TrimEnd('=').Replace("==", "");
                    }

                return hash.Replace("=", "");
            }
            catch (Exception)
            {
            }

            return "";
        }
        /// <summary>
        /// Parses a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnprotectServicePassword(string str)
        {
            RijndaelEnhanced encryptQuery = new RijndaelEnhanced(EncryptKey);

            try
            {
                return encryptQuery.Decrypt(str);
            }
            catch (Exception ex)
            {
                // append one more '='
                str += "=";

                try
                {
                    return encryptQuery.Decrypt(str);
                }
                catch (Exception ex2)
                {
                    str += "=";

                    return encryptQuery.Decrypt(str);
                }
            }
        }

        /// <summary>
        /// Hash Password using BCrypt
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }
        /// <summary>
        /// Verify Password using BCrypt
        /// </summary>
        /// <param name="plainPassword"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }

        public static string WriteDate(DateTime dt, string separator)
        {
            // Min Value
            if (dt == DateTime.Parse(System.Data.SqlTypes.SqlDateTime.MinValue.ToString()))
                return "Never";

            // Recent Date
            DateTime now = DateTime.UtcNow;
            string date = "";

            if (dt.Date == now.Date)
                date = "Today";
            else if (dt.Date == now.AddDays(-1).Date)
                date = "Yesterday";
            else
                date = dt.Date.ToString("dd-MM-yyyy");

            return String.Concat(date, separator, dt.ToLongTimeString());
        }

        public static bool IsNumeric(string item)
        {
            int n;
            bool isNumeric = int.TryParse(item, out n);

            return isNumeric;
        }

    }
}
