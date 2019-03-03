using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradeKerala2017.Models;
using System.Data;
using System.Net.Mail;


namespace TradeKerala2017.Controllers
{
    public class VendorController : Controller
    {
        DbTradekeralaEntities1 db = new DbTradekeralaEntities1();
        // GET: Vendor
        public ActionResult Index()
        {
            try
            {
                if (Session["Vendor"] != null)
                {
                    string name = Session["Vendor"].ToString();

                    ViewBag.Profile = db.tbl_Vendor.Where(i => i.Email == name).FirstOrDefault();
                    int Id = db.tbl_Vendor.Where(i => i.Email == name).FirstOrDefault().Vendor_id;
                    ViewBag.Product = db.tblProduct.Where(i => i.Vendor == Id && i.Hide == "Show").ToList();
                }
                else
                {
                    return Redirect("~/Vendor/Login");
                }
                
            }
            catch (Exception ex) { }

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Login(FormCollection collect)
        {
            string Username = collect["txtUsername"];
            string Password = collect["txtPassword"];
            try
            {
                int Count = db.tblLogins.Where(i => i.username == Username && i.password == Password).Count();

                if (Count > 0)
                {
                    string type = db.tblLogins.Where(i => i.username == Username && i.password == Password).FirstOrDefault().type;
                    if (type == "Vendor")
                    {

                        tblLogin Log = db.tblLogins.Where(i => i.username == Username && i.password == Password).FirstOrDefault();
                        Session["Vendor"] = Log.username;
                        Log.lastlogin = DateTime.Now;
                        Log.status = Convert.ToString("true");
                        db.SaveChanges();
                        
                        return Redirect("~/Vendor/Index");
                    }

                    else
                    {
                        ViewBag.Message = "Invalid username/password";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid username/password";
                    return View();
                }
            }
            catch
            {
                ViewBag.Message = "Invalid username/password";
                return View();
            }

        }
        public ActionResult Logout()
        {
            try
            {
                string username = Session["Vendor"].ToString();
                tblLogin tblLogin = db.tblLogins.Where(i => i.username == username).FirstOrDefault();
                tblLogin.status = Convert.ToString(" false");
                db.SaveChanges();

                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
            }
            catch
            {

            }
            return Redirect("~/Vendor/Login");
        }

        public ActionResult EditVendor()
        {
            try
            {
                string name = Session["Vendor"].ToString();
                ViewBag.EditVendor = db.tbl_Vendor.Where(i => i.Email == name).FirstOrDefault();

            }
            catch { }
            return View();
        }

        [HttpPost]
        public ActionResult EditVendor(FormCollection collect)
        {

            int id = Convert.ToInt32(collect["HiddenId"]);
            try
            {


                tbl_Vendor vnd = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();
                vnd.Address = collect["VendorAddress"];
                vnd.Mobile = Convert.ToInt64(collect["MobileNumber"]);
                vnd.Pincode = collect["PinCode"];
                vnd.state = collect["State"];
                vnd.Pro_Info = collect["ProductInfo"];
                vnd.LandLine = Convert.ToInt64(collect["LandLine"]);
                db.SaveChanges();

                ViewBag.Message = "Vendor Details Updated Successfully!!";


            }
            catch
            {
                ViewBag.Message = "Vendor Details Not Updated Successfully!!";
            }

            return Redirect("~/Vendor/Index?Msg=" + ViewBag.Message);
        }
        public ActionResult Orderview()
        {
            try
            {
                string name = Session["Vendor"].ToString();
                tbl_Vendor vd = db.tbl_Vendor.Where(i => i.Email == name).FirstOrDefault();
                int id = vd.Vendor_id;
                ViewBag.order = db.tblOrders.Where(i => i.Vendor == id).ToList();
            }
            catch
            {

            }
                return View();
        }

        public ActionResult  updatestatus(int id)
        {
            try
            {
                ViewBag.EditOffer = db.tblOrders.Where(i => i.Orderno == id).FirstOrDefault();

            }
            catch
            {
            }
            return View();
        }

        [HttpPost]

        public ActionResult updatestatus(FormCollection collect)
        {
            try
            {
                int id =Convert.ToInt32(collect["Id"]);
                tblOrders tb = db.tblOrders.Where(i => i.Orderno == id).FirstOrDefault();
                tb.status = collect["status"];
                db.SaveChanges();

                ViewBag.Message = "Status Updated Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Status NOT Updated!!";
            }
            return Redirect("~/Vendor/orderview?Msg=" + ViewBag.Message);
        }

        public ActionResult Printorder(int id)
        {
            try
            {
                tblOrders tb = db.tblOrders.Where(i => i.Orderno == id).FirstOrDefault();
                ViewBag.Order = db.tblOrders.Where(i => i.Orderno == id).FirstOrDefault();
                int invoice_id = Convert.ToInt32(tb.Invoice_Id);
                ViewBag.invoice_number = invoice_id;
                int Address = Convert.ToInt16(tb.Address);
                ViewBag.address = db.tb_address.Where(i => i.ID == Address).FirstOrDefault();
            }
            catch
            {
            }
            return View();
        }
    }
}