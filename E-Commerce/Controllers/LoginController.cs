using E_Commerce.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            Session[Common.SESSION_FNAME] = null;
            Session[Common.SESSION_ISADMIN] = null;
            Session[Common.SESSION_LNAME] = null;
            Session[Common.SESSION_USERID] = null;
            return View("Login");
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        public ActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveUserDetails(RegisterViewModel registerView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new Models.DEMO_ECOMMERCEEntities())
                    {
                        var Email = (from cus in db.USERS_PROFILE where cus.USR_EMAILADDRESS == registerView.EmailAddress select cus.USR_PASSWORD).Any();
                        if (Email)
                        {
                            ModelState.AddModelError(string.Empty, "Email Address already exists");
                            ViewBag.Message = "Email Address already exists";
                            return View("Register");
                        }

                        USERS_PROFILE userProfile = new USERS_PROFILE();
                        userProfile.USR_FNAME = registerView.FirstName;
                        userProfile.USR_LNAME = registerView.LastName;
                        userProfile.USR_EMAILADDRESS = registerView.EmailAddress;
                        userProfile.USR_PASSWORD = Encrypt(registerView.Password);
                        userProfile.USR_PHONENO = registerView.MobileNo;
                        userProfile.CREATEDAT = DateTime.Now;
                        userProfile.USR_ISADMIN = registerView.IsAdmin;

                        db.USERS_PROFILE.Add(userProfile);
                        db.SaveChanges();
                    }
                }
                else
                {
                    return View("Register");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
               // ModelState.Clear();
            }
            return View("Login");
        }

        public string Encrypt(string str)
        {
            string EncrptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        [HttpPost]
        public ActionResult LoginIndex(LoginViewModel user)
        {
            try
            {
                string password = user.Password;
                string userName = user.EmailAddress;
                if (ModelState.IsValid)
                {
                    if (IsValid(userName, password))
                    {
                        return RedirectToAction("Index", "Product");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid UserName or Password");
                    }
                }
                return View("Login");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //ModelState.Clear();
            }
        }

        private bool IsValid(string userName, string password)
        {
            bool IsValid = false;
            using (var db = new Models.DEMO_ECOMMERCEEntities())
            {
                string encryptPassword = Encrypt(password);
                RegisterViewModel rg = new RegisterViewModel();
                var result = (from user in db.USERS_PROFILE
                              where user.USR_EMAILADDRESS.ToUpper() == userName.Trim().ToUpper()
                              && user.USR_PASSWORD == encryptPassword
                              select new{
                                  UserId = user.USR_ID,
                                  FirstName = user.USR_FNAME,
                                  LastName = user.USR_LNAME,
                                  IsAdmin = user.USR_ISADMIN
                              }).FirstOrDefault();


                if (result !=null)
                {
                    Session[Common.SESSION_USERID] = result.UserId;
                    Session[Common.SESSION_FNAME] = result.FirstName;
                    Session[Common.SESSION_LNAME] = result.LastName;
                    Session[Common.SESSION_ISADMIN] = result.IsAdmin;
                    IsValid = true;
                }
            }
            return IsValid;
        }

        public string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            string DecryptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
    }
}