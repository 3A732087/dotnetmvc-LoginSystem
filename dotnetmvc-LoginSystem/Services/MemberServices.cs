using dotnetmvc_LoginSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace dotnetmvc_LoginSystem.Services
{
    public class MemberServices
    {
        dbQuanMembersSystemEntities db = new dbQuanMembersSystemEntities();

        public Dictionary<string, string> HashPassword(string status, string account, string password)
        {
            string salt;
            if (status == "Register")
            {
                //產生鹽
                salt = Guid.NewGuid().ToString("N");
            }
            else
            {
                var memberData = db.Members.Where(m => m.Account == account).FirstOrDefault();
                salt = memberData.Salt;
            }

            string saltAndPassword = string.Concat(password, salt);
            SHA256CryptoServiceProvider sha256Hash = new SHA256CryptoServiceProvider();
            byte[] passwordData = Encoding.Default.GetBytes(saltAndPassword);
            byte[] hashData = sha256Hash.ComputeHash(passwordData);
            string hashResult = Convert.ToBase64String(hashData);

            Dictionary<string, string> hashDataList = new Dictionary<string, string>();
            hashDataList.Add("salt", salt);
            hashDataList.Add("password", hashResult);

            return hashDataList;
        }

    }
}