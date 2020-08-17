using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using Login.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace LoginApp.Controllers
{
    public class LoginController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        void connectionString()
        {
            con.ConnectionString = @"Data Source=DESKTOP-3633F0G\LAST; database=LoginDB; integrated security = SSPI";
        }

        bool check(string username)
        {
            connectionString();
            string checkQuery = "SELECT * FROM Users WHERE Username= @0";
            com = new SqlCommand(checkQuery, con);
            com.Parameters.AddWithValue("@0", username);
            con.Open();
            SqlDataReader rdr= com.ExecuteReader();
            if(rdr.HasRows)
            {
                con.Close();
                return false;
            }
            else
            {
                con.Close();
                return true;
            }

        }
        [HttpPost]
        public JsonResult Insert(User user)
        {
            string msg = string.Empty;
            connectionString();
            string username = user.Username;
            string password = user.Password;
            string confirmPassword = user.ConfirmPassword;
            if(username.Length<6)
            {
                msg = "Username is too short";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            if (password.Length < 6)
            {
                msg = "Password is too short";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            if(password != confirmPassword)
            {
                msg = "'Confirm Password' and 'Password' do not match.";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            string hashedPassword = Encrypt.Hash(password);
            if(check(username)==true)
            {
                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@0,(@1))";
                com = new SqlCommand(insertQuery, con);
                com.Parameters.AddWithValue("@0", username);
                com.Parameters.AddWithValue("@1", hashedPassword);
                con.Open();
                com.ExecuteNonQuery();
                con.Close();
                msg = "Account successfully created";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                msg = "You already have an account";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Authentication(User user)
        {  
            connectionString();
            string username = user.Username;
            string password = user.Password;
            string hashedPassword = Encrypt.Hash(password);
            string selectQuery = "SELECT * FROM Users WHERE Username = @0 AND Password=@1";
            com = new SqlCommand(selectQuery, con);
            com.Parameters.AddWithValue("@0", username);
            com.Parameters.AddWithValue("@1", hashedPassword);
            con.Open();
            SqlDataReader rdr = com.ExecuteReader();
            if (rdr.Read())
            {
                con.Close();
                Session["UserID"] = user.UserID;
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                con.Close();
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}