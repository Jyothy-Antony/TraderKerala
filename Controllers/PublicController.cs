using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TradeKerala2017.Models;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.IO;
using System.Collections.Specialized;
using CCA.Util;

namespace TradeKerala2017.Controllers
{

    public class PublicController : Controller
    {
        // GET: Public
        public string name;
        public string dupname;
        public string Catword;
        public int invoice_id;
        public string careerid;
        public int q;

        String UID = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
        DbTradekeralaEntities1 db = new DbTradekeralaEntities1();





        [HttpGet]
        public ActionResult comingsoon()
        {

            return View();

        }

        [HttpGet]
        public ActionResult NewIndex()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();

                HttpCookie myCookie = Request.Cookies["MyTestCookie"];
                if (myCookie != null)
                {
                    string Id = myCookie.Value;
                    ViewBag.relatedActive = db.Relatedpro(Id).ToList().Take(1);
                    ViewBag.related = db.Relatedpro(Id).ToList();
                    ViewBag.CookieCount = db.Relatedpro(Id).Count();
                }
                else
                {
                    myCookie = new HttpCookie("MyTestCookie");
                    myCookie.Expires = DateTime.Now.AddMonths(30);
                    ViewBag.CookieCount = 0;
                }

                int r = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (r > 0)
                {
                    ViewBag.recentlog = 1;
                    int recent = db.tblRecentView.Where(i => i.Username == log_uname).Count();
                    if (recent > 0)
                    {
                        ViewBag.Myrecentbag = db.tblRecentView.Where(i => i.Username == log_uname).ToList();
                        ViewBag.recentCount = 1;
                    }
                    else
                    {
                        ViewBag.recentCount = 0;
                    }
                    ViewBag.recent = db.tblRecentView.Take(12).ToList();
                }
                else
                {
                    ViewBag.recentlog = 0;
                    HttpCookie myRecent = Request.Cookies["MyRecentCookie"];
                    if (myRecent != null)
                    {
                        ViewBag.Myrecentbag = myRecent.Value;
                        ViewBag.recentCount = 1;
                    }
                    else
                    {
                        ViewBag.recentCount = 0;
                    }

                }

                HttpCookie model = Request.Cookies["MyModel"];
                if (model == null)
                {
                    model = new HttpCookie("MyModel");
                    model.Value = "1";
                    ViewBag.model = 1;
                    Response.Cookies.Add(model);
                }
                else
                {
                    ViewBag.model = 0;
                }





                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.product = db.tblProdImages.ToList();
                ViewBag.BannerActive = db.tbl_Banner.Where(i => i.Status == "show").ToList().Take(1);
                ViewBag.Banner = db.tbl_Banner.Where(i => i.Status == "show").ToList();
                ViewBag.BrandActive = db.tblBrands.ToList().Take(1);
                ViewBag.BrandList = db.tb_TopBrand.ToList().ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
                ViewBag.TrendingActive = db.Database.SqlQuery<sp_Trending_Result>("exec sp_Trending").ToList().Take(1);
                ViewBag.NewproductsActive = db.Database.SqlQuery<sp_newproducts>("exec sp_newproducts").ToList().Take(1);
                ViewBag.newproducts = db.Database.SqlQuery<sp_newproducts>("exec sp_newproducts").ToList();
                ViewBag.trending = db.Database.SqlQuery<sp_Trending_Result>("exec sp_Trending").ToList();
            }
            catch
            {

            }
            return View();
        }


        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(FormCollection c)
        {
            try
            {
                string user = c["Name"];
                string email = c["Email"];
                int RegCount = db.tblRegistration.Where(i => i.RegEmail == email && i.type != "Guest").Count();
                if (RegCount > 0)
                {
                    ViewBag.Message = "Email address you have given is already associated with traderkerala, Please verify";
                    return Redirect("/Public/Registration?Msg=" + ViewBag.Message);

                }
                tblRegistration reg = new tblRegistration();
                reg.RegName = c["Name"];
                reg.RegEmail = c["Email"];
                reg.RegMobile = c["Mobile"];
                string pword = c["Password"];
                string Cword = c["confirm_Password"];
                if (pword == Cword)
                {
                    reg.RegPassword = c["Password"];
                    reg.status = Convert.ToString("true");
                    db.tblRegistration.Add(reg);
                    int i = db.SaveChanges();

                    if (i > 0)
                    {
                        tblLogin log = new tblLogin();
                        log.username = c["Email"];
                        string pas = c["Password"];
                        log.password = c["Password"];
                        log.lastlogin = Convert.ToDateTime(DateTime.Now);
                        log.status = Convert.ToString("False");
                        log.type = Convert.ToString("user");
                        db.tblLogins.Add(log);
                        db.SaveChanges();


                        string subject = "";
                        string body = "";
                        subject = "Your Email Confirmation for Traderkerala.com";
                        body = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi user</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'>Greetings from Traderkerala.com Per your request, we have successfully completed your registration.Should you need to contact us for any reason, please know that we can give out order information only to the name and e - mail address associated with your account.Thank you again for shopping with us. Please click the following button to complete your Registration</td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 25px 0 0 0;' class='padding-copy'><table border = '0' cellspacing='0' cellpadding='0' class='responsive-table'><tr><td align = 'center' ><a href='http://traderkerala.com/Public/ConfirmMail?mail=" + email + "' style='padding: 10px 16px;text-decoration: none;font-size: 18px;line-height: 1.33;border-radius: 5px;color: #fff;background-color: #428bca;border-color: #357ebd;'> Confirm Registration</a></td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";

                        SendMail(body, email, subject);

                        ViewBag.Message = "Registration completed succesfully; Please check and verify your mail for signin";

                        return Redirect("/Public/login?from=login&Msg=" + ViewBag.Message);

                    }
                }
                else
                {
                    ViewBag.Message = "Passwords are not matching";
                    return Redirect("/Public/Registration?Msg=" + ViewBag.Message);
                }
            }
            catch
            {
                ViewBag.Message = "Registration not completed!! Please Try Again";
                return Redirect("/Public/Registration?Msg=" + ViewBag.Message);
            }

            return Redirect("~/Public/Registration?Msg=" + ViewBag.Message);
        }


        public void SendMail(string Body, string To, string Subject)
        {
            MailMessage MyMailMessage = new MailMessage();

            MyMailMessage.From = new MailAddress("customer@fabstudioz.com");
            MyMailMessage.To.Add(To);
            MyMailMessage.Subject = Subject;
            MyMailMessage.Body = Body;
            MyMailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure |
                        DeliveryNotificationOptions.OnSuccess |
                        DeliveryNotificationOptions.Delay;
            MyMailMessage.IsBodyHtml = true;
            SmtpClient SMTPServer = new SmtpClient("mail.fabstudioz.com");
            SMTPServer.Port = 587;
            SMTPServer.Credentials = new System.Net.NetworkCredential("customer@fabstudioz.com", "F@bweb2016");
            SMTPServer.EnableSsl = false;


            try
            {

                SMTPServer.Send(MyMailMessage);
            }

            catch (Exception)
            {
            }
        }


        public ActionResult ConfirmMail(string mail)
        {
            int Count = db.tblLogins.Where(i => i.username == mail && i.type == "user").Count();
            if (Count > 0)
            {
                tblLogin log = db.tblLogins.Where(i => i.username == mail && i.type == "user").OrderByDescending(i => i.log_Id).FirstOrDefault();
                log.status = "true";
                db.SaveChanges();
            }
            return Redirect("~/Public/login?from=login&name=" + UID);
        }

        [HttpGet]
        public ActionResult login(string from, string name, string pid)
        {
            ViewBag.from = from;
            ViewBag.uname = name;
            ViewBag.pid = pid;
            return View();
        }

        [HttpPost]
        public ActionResult login(FormCollection c)
        {

            string check = c["from"];
            string uname = c["uname"];
            string pid = c["pid"];
            string Username = c["log_Email"];
            string Password = c["log_Password"];
            try
            {


                int Count = db.tblLogins.Where(i => i.username == Username && i.password == Password).Count();

                if (Count > 0)
                {
                    string type = db.tblLogins.Where(i => i.username == Username && i.password == Password).FirstOrDefault().type;
                    if (type == "user")
                    {
                        tblLogin log = db.tblLogins.Where(i => i.username == Username && i.password == Password).FirstOrDefault();
                        Session["username"] = log.username;
                        log.lastlogin = DateTime.Now;
                        db.SaveChanges();
                        if (log.status == "true")
                        {
                            if (check == "login")
                            {
                                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                                {
                                    tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();
                                    ct.user_id = Username;
                                    db.SaveChanges();
                                }
                                return Redirect("~/public/NewIndex");
                            }
                            else if (check == "SignOut")
                            {
                                return Redirect("~/public/NewIndex");
                            }
                            else if (check == "cart")
                            {
                                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                                {
                                    tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();
                                    ct.user_id = Username;
                                    db.SaveChanges();
                                }
                                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == Username).ToList();
                                return Redirect("~/public/cart");
                            }
                            else if (check == "Wishlist")
                            {
                                //tbl_wishlist wish = db.tbl_wishlist.Where(i => i.user_id == uname).FirstOrDefault();
                                //if (wish == null)
                                //{
                                //    List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                                //    foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                                //    {
                                //        tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();
                                //        ct.user_id = Username;
                                //        db.SaveChanges();
                                //    }
                                //    return Redirect("~/public/wishlist");
                                //}
                                //else
                                //{
                                //    tbl_wishlist wsh = new tbl_wishlist();
                                //    List<tbl_wishlist> wshlist = db.tbl_wishlist.Where(i => i.user_id == uname).ToList();
                                //    foreach (TradeKerala2017.Models.tbl_wishlist ct in wshlist)
                                //    {
                                //        tbl_wishlist tblwsh = db.tbl_wishlist.Where(o => o.id == ct.id).FirstOrDefault();
                                //        ct.user_id = Username;
                                //        db.SaveChanges();
                                //    }

                                //    List<tbl_Notfication> notlist = db.tbl_Notfication.Where(i => i.Username == uname).ToList();
                                //    foreach (TradeKerala2017.Models.tbl_Notfication ct in notlist)
                                //    {
                                //        tbl_Notfication noti = db.tbl_Notfication.Where(o => o.Noti_Id == ct.Noti_Id).FirstOrDefault();
                                //        ct.Username = Username;
                                //        db.SaveChanges();
                                //    }
                                //    List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();

                                //    foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                                //    {
                                //        tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();
                                //        ct.user_id = Username;
                                //        db.SaveChanges();
                                //    }
                                //    ViewBag.cart = db.tbl_cart.Where(i => i.user_id == Username).ToList();
                                //    return Redirect("~/public/wishlist");
                                int pro = Convert.ToInt32(pid);
                                int item = db.tbl_wishlist.Where(i => i.prod_id == pro && i.user_id == log.username).Count();
                                if (item < 1)
                                {
                                    tblProduct pd = db.tblProduct.Where(i => i.pid == pro && i.Hide != "Hided").FirstOrDefault();
                                    tblProdImage ma = db.tblProdImages.Where(i => i.Prodid == pro).Take(1).FirstOrDefault();
                                    tbl_Vendor vd = db.tbl_Vendor.Where(i => i.Vendor_id == pd.Vendor).FirstOrDefault();
                                    tbl_wishlist wish = new tbl_wishlist();
                                    wish.user_id = log.username;
                                    wish.prod_id = pro;
                                    wish.cat_id = pd.Catid;
                                    wish.pro_name = pd.ProdName;
                                    wish.pro_price = Convert.ToString(pd.ProdNewPrice);
                                    wish.pro_image = Convert.ToString(ma.id);
                                    wish.pro_qty = Convert.ToString(pd.Qty);
                                    wish.vendor_name = vd.Vendor_Name;
                                    db.tbl_wishlist.Add(wish);
                                    db.SaveChanges();
                                    if (pd.Qty < 1)
                                    {
                                        tbl_Notfication noti = new tbl_Notfication();
                                        noti.Username = log.username;
                                        noti.pro_id = pro;
                                        db.tbl_Notfication.Add(noti);
                                        db.SaveChanges();

                                    }
                                    ViewBag.Message = "product Succesfully Added to Wish List";
                                }
                                else
                                {
                                    ViewBag.Message = "product Already Added to cart";
                                }
                                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                                {
                                    tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();
                                    ct.user_id = Username;
                                    db.SaveChanges();
                                }
                                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == Username).ToList();
                                return Redirect("~/public/wishlist");
                            }

                            else if (check == "Buynow")
                            {
                                tbl_cart TblCart = db.tbl_cart.Where(i => i.user_id == uname).FirstOrDefault();
                                TblCart.user_id = Username;
                                db.SaveChanges();
                                return Redirect("~/public/checkout?uname=" + Username);
                            }
                            else if (check == "UserAccount")
                            {
                                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                                {
                                    tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();
                                    ct.user_id = Username;
                                    db.SaveChanges();
                                }
                                return Redirect("~/public/UserAccount");
                            }
                            else if (check == "UserAddress")
                            {
                                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                                {
                                    tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();
                                    ct.user_id = Username;
                                    db.SaveChanges();
                                }
                                return Redirect("~/public/UserAddress");
                            }

                            else if (check == "Prebooking")
                            {
                                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                                {
                                    tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();
                                    ct.user_id = Username;
                                    db.SaveChanges();
                                }
                                return Redirect("~/public/Prebooking");
                            }
                            else if (check == "Review")
                            {
                                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                                foreach (TradeKerala2017.Models.tbl_cart hf in TblCart)
                                {
                                    tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == hf.id).FirstOrDefault();
                                    hf.user_id = Username;
                                    db.SaveChanges();
                                }
                                return Redirect("~/public/NewProductDetails?id=" + pid);
                            }

                        }
                        else
                        {
                            ViewBag.Message = "Please Confirm Your Mail address and complete registration";
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Invalid username/password";
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid username/password";
                }
            }
            catch
            {
                ViewBag.Message = "Invalid username/password";

            }

            return Redirect("~/public/login?msg=" + ViewBag.message + "&from=login&name=" + uname);
        }


        public ActionResult SignOut()
        {
            try
            {
                string username = Session["username"].ToString();
                int RegCount = db.tblRegistration.Where(i => i.RegEmail == username && i.type == "Guest").Count();
                if (RegCount > 0)
                {
                    tblRegistration reg = db.tblRegistration.Where(i => i.RegEmail == username && i.type == "Guest").FirstOrDefault();
                    db.tblRegistration.Remove(reg);
                    db.SaveChanges();
                }

                //tblLogin tblLogin = db.tblLogins.Where(i => i.username == username).FirstOrDefault();
                //tblLogin.status = Convert.ToString("true");
                //db.SaveChanges();

                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
            }
            catch { }

            return Redirect("~/Public/login?from=SignOut");
        }


        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(FormCollection collect)
        {
            string Username = collect["log_Email"];
            try
            {
                int Count = db.tblLogins.Where(i => i.username == Username).Count();


                if (Count > 0)
                {
                    tblLogin log = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
                    return Redirect("~/public/checkname?name=" + Username);

                }
                else
                {
                    ViewBag.Message = "Invalid Email or Email address doesnt exist";

                }
            }
            catch
            {
                ViewBag.Message = "Invalid Email or Email address doesnt exist";

            }
            return View();
        }

        [HttpGet]
        public ActionResult checkname(string name)
        {
            string email = name;
            ViewBag.email = db.tblLogins.Where(i => i.username == email).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public ActionResult checkname(FormCollection collect)
        {
            string Username = collect["log_email"];
            string name = collect["fullname"];
            string email = Username;
            ViewBag.email = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
            try
            {
                int Count = db.tblRegistration.Where(i => i.RegEmail == Username && i.RegName == name).Count();

                if (Count > 0)
                {
                    tblLogin log = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
                    Random r = new Random();
                    var x = r.Next(0, 1000000);
                    string s = x.ToString("000000");
                    log.password = s;
                    log.status = Convert.ToString("true");
                    db.SaveChanges();
                    string subject = "Traderkerala password assistance";
                    string body = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi user</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'>Greetings from Traderkerala.com Per your request, we are ready to assist you in password changing.Should you need to contact us for any reason, please know that we can give out order information only to the name and e - mail address associated with your account.Thank you again for shopping with us. Please find the following code</td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 25px 0 0 0;' class='padding-copy'><table border = '0' cellspacing='0' cellpadding='0' class='responsive-table'><tr><td align = 'center' >CODE:" + s + "</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";

                    SendMail(body, email, subject);
                    return Redirect("~/public/VerifyPword?name=" + Username);
                }
                else
                {
                    ViewBag.Message = "Invalid Name,Please Retype you currect Full name";
                }
            }
            catch
            {
                ViewBag.Message = "Invalid Name,Please Retype you currect Full name";
            }

            return View();
        }


        [HttpGet]
        public ActionResult VerifyPword(string name)
        {
            string email = name;
            ViewBag.email = db.tblLogins.Where(i => i.username == email).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public ActionResult VerifyPword(FormCollection collect)
        {
            string Username = collect["log_email"];
            string Password = collect["Password"];

            string[] chars = new string[] { " " };
            for (int q = 0; q < chars.Length; q++)
            {
                if (Password.Contains(chars[q]))
                {
                    Password = Password.Replace(chars[q], "");
                }
            }

            ViewBag.email = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
            try
            {
                int Count = db.tblLogins.Where(i => i.username == Username && i.password == Password).Count();

                if (Count > 0)
                {
                    return Redirect("~/public/ChangePword?name=" + Username);
                }
                else
                {
                    ViewBag.Message = "Invalid Pin code";
                }
            }
            catch
            {
                ViewBag.Message = "Invalid Pin code";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ChangePword(string name)
        {
            string email = name;
            ViewBag.email = db.tblLogins.Where(i => i.username == email).FirstOrDefault();
            return View();
        }

        [HttpPost]
        public ActionResult ChangePword(FormCollection collect)
        {
            string Username = collect["log_email"];
            string Password = collect["Password"];
            string confrmPword = collect["confirm_Password"];
            ViewBag.email = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
            try
            {
                if (Password == confrmPword)
                {
                    int Count = db.tblLogins.Where(i => i.username == Username).Count();

                    if (Count > 0)
                    {
                        tblLogin log = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
                        log.password = Password;

                        log.lastlogin = Convert.ToDateTime(DateTime.Now);
                        log.status = Convert.ToString("true");
                        log.type = Convert.ToString("user");
                        int x = db.SaveChanges();
                        if (x > 0)
                        {
                            tblRegistration reg = db.tblRegistration.Where(i => i.RegEmail == Username).FirstOrDefault();
                            reg.RegPassword = Password;
                            db.SaveChanges();
                            Session["username"] = reg.RegEmail;
                            return Redirect("~/public/NewIndex");
                        }
                        else
                        {
                            ViewBag.Message = "Miss matching In password.Please Retype your Password";
                        }

                    }
                }
                else
                {
                    ViewBag.Message = "Miss matching In password.Please Retype your Password";
                }
            }
            catch
            {
                ViewBag.Message = "Miss matching In password.Please Retype your Password";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Directory()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.CatCount = db.tblCategories.Where(i => i.ParentId == 0).Count();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.product = db.tblProdImages.ToList();
                ViewBag.Offerlist = db.tbl_TodaysDeal.ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
            }
            catch
            {

            }

            return View();


        }







        [HttpGet]
        public ActionResult NewProductDetails(int id, string from, string Pid)
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                ViewBag.OfferID = Convert.ToInt32(Pid);
                ViewBag.from = from;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.list = db.tblProduct.Where(i => i.pid == id && i.Hide != "Hided").FirstOrDefault();
                ViewBag.ParCategories = db.tblProduct.Where(i => i.pid == id && i.Hide != "Hided").FirstOrDefault().ParCategories;
                ViewBag.ImageFirst = db.tblProdImages.Where(i => i.Prodid == id).FirstOrDefault();
                ViewBag.image = db.tblProdImages.Where(i => i.Prodid == id).ToList();
                ViewBag.reviews = db.tblreviews.Where(i => i.prodid == id).ToList();
                ViewBag.reviewCount = db.tblreviews.Where(i => i.prodid == id).Count();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();

                tblProduct tp = db.tblProduct.Where(i => i.pid == id && i.Hide != "Hided").FirstOrDefault();
                string pname = tp.ProdName;
                string cname = tp.CatName;
                string prod = tp.ParCategories;
                string nelink = "";
                string[] prodct = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "|" };
                for (int q = 0; q < prodct.Length; q++)
                {
                    if (prod.Contains(prodct[q]))
                    {
                        prod = prod.Replace(prodct[q], "");
                    }
                }
                string[] linkview = prod.Split('~');
                int Lengths = linkview.Length;
                for (int i = Lengths; i > 0; i--)
                {
                    if (nelink == "") {
                        nelink = linkview[i - 1];
                    }
                    else
                    {
                        nelink = nelink + "~" + linkview[i - 1];
                    }
                }
                ViewBag.prolink = nelink;
                ViewBag.recmmndActive = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").OrderByDescending(I => I.AddedDate).ToList().Take(1);
                ViewBag.recmmnd = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").OrderByDescending(I => I.AddedDate).ToList().Take(9);
                ViewBag.recmCount = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").ToList().Count();

                ViewBag.other = db.tblProduct.Where(i => i.ProdName == pname && i.Hide != "Hided").Count();

                int catid = Convert.ToInt32(tp.Catid);
                int pro = id;
                string catids = Convert.ToString(catid);
                ViewBag.size = db.tblProduct.Where(i => i.Catid == catid && i.Hide != "Hided").ToList();
                ViewBag.relatedActive = db.Relatedpro(catids).ToList().Take(1);
                ViewBag.related = db.Relatedpro(catids).ToList();
                ViewBag.relCount = db.Relatedpro(catids).Count();

                HttpCookie myCookie = Request.Cookies["MyTestCookie"];
                if (myCookie == null)
                {
                    myCookie = new HttpCookie("MyTestCookie");
                    myCookie.Value = catids.ToString();
                    myCookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(myCookie);
                }
                else
                {
                    myCookie.Value = catids.ToString();
                    myCookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(myCookie);
                }
                int r = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (r > 0)
                {
                    tblRecentView rec = new tblRecentView();
                    rec.Username = name;
                    rec.pid = Convert.ToInt32(id);
                    db.tblRecentView.Add(rec);
                    db.SaveChanges();
                }
                else
                {
                    HttpCookie myRecent = Request.Cookies["MyRecentCookie"];
                    if (myRecent == null)
                    {
                        myRecent = new HttpCookie("MyRecentCookie");
                        myRecent.Value = id.ToString();
                        myRecent.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(myRecent);
                    }
                    else
                    {
                        string str1 = myRecent.Value;
                        string str2 = "-";
                        string str3 = id.ToString();


                        myRecent.Value = System.String.Concat(str1, str2, str3);
                        myRecent.Expires = DateTime.Now.AddDays(30);
                        Response.Cookies.Add(myRecent);
                    }
                }
                int w = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (w > 0)
                {
                    ViewBag.recentlog = 1;
                    int recent = db.tblRecentView.Where(i => i.Username == name).Count();
                    if (recent > 0)
                    {
                        ViewBag.Myrecentbag = db.tblRecentView.Where(i => i.Username == name).ToList();
                        ViewBag.recentCount = 1;
                    }
                    else
                    {
                        ViewBag.recentCount = 0;
                    }
                    ViewBag.recent = db.tblRecentView.Take(12).ToList();
                }
                else
                {
                    ViewBag.recentlog = 0;
                    HttpCookie myRecent = Request.Cookies["MyRecentCookie"];
                    if (myRecent != null)
                    {
                        ViewBag.Myrecentbag = myRecent.Value;
                        ViewBag.recentCount = 1;
                    }
                    else
                    {
                        ViewBag.recentCount = 0;
                    }

                }
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        [HttpPost]
        public ActionResult NewProductDetails(FormCollection c1)
        {
            int id = Convert.ToInt32(c1["id"]);
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string Username = Session["username"].ToString();
                int log = db.tblRegistration.Where(i => i.RegEmail == Username).Count();
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=Review&name=" + Username + "&pid=" + id);
                }
                else
                {
                    int order = db.tblOrders.Where(i => i.Userid == Username && i.ProId == id).Count();
                    if (order < 1)
                    {
                        ViewBag.Message = "To submit reviews, customers must make a minimum amount of valid debit or credit card purchases. ";
                        return Redirect("~/Public/NewProductDetails?id=" + id + "&Msg=" + ViewBag.Message);
                    }
                    else
                    {
                        string date = System.DateTime.Now.ToShortDateString();
                        string name = c1["Name"];
                        string phone = c1["Telephone"];
                        string reviews = c1["Review"];
                        string star = c1["rating"];
                        if (star == "")
                        {
                            star = 5.ToString();
                        }

                        tblreviews tr = new tblreviews();
                        tr.prodid = id;
                        tr.starValue = Convert.ToInt16(star);
                        tr.date = date;
                        tr.name = name;
                        tr.email = Username;
                        tr.phone = phone;
                        tr.reviews = reviews;
                        tr.type = "User";


                        db.tblreviews.Add(tr);
                        db.SaveChanges();
                        ViewBag.Message = "Review Submitted successfully";
                    }
                }


                ViewBag.OfferID = Convert.ToInt32(c1["Offerid"]);
                ViewBag.list = db.tblProduct.Where(i => i.pid == id && i.Hide != "Hided").FirstOrDefault();
                ViewBag.image = db.tblProdImages.Where(i => i.Prodid == id).ToList();
                ViewBag.reviews = db.tblreviews.Where(i => i.prodid == id).ToList();
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                tblProduct tp = db.tblProduct.Where(i => i.pid == id && i.Hide != "Hided").FirstOrDefault();
                string pname = tp.ProdName;
                ViewBag.other = db.tblProduct.Where(i => i.ProdName == pname && i.Hide != "Hided").Count();
                int catid = Convert.ToInt32(tp.Catid);
                int pro = id;
                string catids = Convert.ToString(catid);
                ViewBag.size = db.tblProduct.Where(i => i.Catid == catid && i.Hide != "Hided").ToList();
                ViewBag.relatedActive = db.Relatedpro(catids).ToList().Take(1);
                ViewBag.related = db.Relatedpro(catids).ToList();
                ViewBag.relCount = db.Relatedpro(catids).Count();
                int w = db.tblRegistration.Where(i => i.RegEmail == Username).Count();
                if (w > 0)
                {
                    ViewBag.recentlog = 1;
                    int recent = db.tblRecentView.Where(i => i.Username == Username).Count();
                    if (recent > 0)
                    {
                        ViewBag.Myrecentbag = db.tblRecentView.Where(i => i.Username == Username).ToList();
                        ViewBag.recentCount = 1;
                    }
                    else
                    {
                        ViewBag.recentCount = 0;
                    }
                    ViewBag.recent = db.tblRecentView.Take(12).ToList();
                }
                else
                {
                    ViewBag.recentlog = 0;
                    HttpCookie myRecent = Request.Cookies["MyRecentCookie"];
                    if (myRecent != null)
                    {
                        ViewBag.Myrecentbag = myRecent.Value;
                        ViewBag.recentCount = 1;
                    }
                    else
                    {
                        ViewBag.recentCount = 0;
                    }

                }
                return Redirect("/Public/NewProductDetails?id=" + id + "&Msg=" + ViewBag.Message);
            }
            catch
            {

            }
            return Redirect("/Public/NewProductDetails?id=" + id);
        }

        [HttpGet]
        public ActionResult Wishlist()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=Wishlist&name=" + name);
                }
                else
                {
                    ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                    ViewBag.WishCount = db.tbl_wishlist.Where(i => i.user_id == name).Count();
                    ViewBag.Wishlist = db.tbl_wishlist.Where(i => i.user_id == name).ToList();
                    ViewBag.product = db.tblProdImages.ToList();
                    int wishCount = ViewBag.WishCount;
                    if (wishCount == 0)
                    {
                        ViewBag.relCount = 0;
                    }
                    else if (wishCount > 0)
                    {
                        tbl_wishlist tp = db.tbl_wishlist.Where(i => i.user_id == name).FirstOrDefault();
                        int catid = Convert.ToInt32(tp.cat_id);
                        string catids = Convert.ToString(catid);
                        ViewBag.relatedActive = db.Relatedpro(catids).ToList().Take(1);
                        ViewBag.related = db.Relatedpro(catids).ToList();
                        ViewBag.relCount = db.Relatedpro(catids).Count();
                    }
                    int w = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                    if (w > 0)
                    {
                        ViewBag.recentlog = 1;
                        int recent = db.tblRecentView.Where(i => i.Username == name).Count();
                        if (recent > 0)
                        {
                            ViewBag.Myrecentbag = db.tblRecentView.Where(i => i.Username == name).ToList();
                            ViewBag.recentCount = 1;
                        }
                        else
                        {
                            ViewBag.recentCount = 0;
                        }
                        ViewBag.recent = db.tblRecentView.Take(12).ToList();
                    }
                    else
                    {
                        ViewBag.recentlog = 0;
                        HttpCookie myRecent = Request.Cookies["MyRecentCookie"];
                        if (myRecent != null)
                        {
                            ViewBag.Myrecentbag = myRecent.Value;
                            ViewBag.recentCount = 1;
                        }
                        else
                        {
                            ViewBag.recentCount = 0;
                        }

                    }
                }
            }
            catch
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult Wishlist(FormCollection collect)
        {
            try
            {
                string pid = Convert.ToString(collect["pro_id"]);
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                int log = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=Wishlist&name=" + name + "&Pid=" + pid);
                }
                else
                {
                    ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                    int prdt = Convert.ToInt16(collect["pro_id"]);
                    int item = db.tbl_wishlist.Where(i => i.prod_id == prdt && i.user_id == name).Count();

                    if (item < 1)
                    {
                        int pro = Convert.ToInt16(collect["qty"]);
                        int qty = Convert.ToInt16(collect["qty"]);
                        tbl_wishlist wish = new tbl_wishlist();
                        wish.user_id = name;
                        wish.prod_id = Convert.ToInt16(collect["pro_id"]);
                        wish.cat_id = Convert.ToInt16(collect["cat_id"]);
                        wish.pro_name = collect["pro_name"];
                        wish.pro_price = collect["pro_price"];
                        wish.pro_image = collect["pro_image"];
                        wish.pro_qty = collect["qty"];
                        wish.vendor_name = collect["vendor"];
                        db.tbl_wishlist.Add(wish);
                        db.SaveChanges();
                        if (qty < 1)
                        {

                            tbl_Notfication noti = new tbl_Notfication();
                            noti.Username = name;
                            noti.pro_id = Convert.ToInt16(collect["pro_id"]);
                            db.tbl_Notfication.Add(noti);
                            db.SaveChanges();
                        }

                        ViewBag.Message = "product Succesfully Added to Wish List";
                    }
                    else
                    {
                        ViewBag.Message = "product Already Added to cart";
                    }
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.product = db.tblProdImages.ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
            }
            catch (Exception ex)
            {

            }
            return Redirect("~/Public/Wishlist?Msg=" + ViewBag.Message);
        }

        [HttpPost]
        public ActionResult DeleteWishlist(FormCollection c)
        {
            int id = Convert.ToInt32(c["Id"]);

            try
            {
                tbl_wishlist wsh = db.tbl_wishlist.Where(i => i.id == id).FirstOrDefault();
                db.tbl_wishlist.Remove(wsh);
                db.SaveChanges();
                ViewBag.Message = "Item is Deleted From Wish List Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Item is Not Deleted From Wish List Successfully!!";
            }

            return Redirect("~/Public/Wishlist?Msg=" + ViewBag.Message);

        }

        [HttpPost]
        public ActionResult Wishlistadd(FormCollection collect)
        {
            try
            {
                tbl_cart crt = new tbl_cart();
                name = Session["username"].ToString();
                int prdt = Convert.ToInt16(collect["pro_id"]);
                int item = db.tbl_cart.Where(i => i.prod_id == prdt && i.user_id == name).Count();

                if (item < 1)
                {
                    crt.user_id = name;
                    crt.prod_id = Convert.ToInt16(collect["pro_id"]);
                    crt.cat_id = Convert.ToInt16(collect["cat_id"]);
                    crt.pro_name = collect["pro_name"];
                    crt.pro_price = collect["pro_price"];
                    crt.pro_image = collect["pro_image"];
                    crt.pro_qty = collect["qty"];
                    crt.sub_total = collect["subtotal"];
                    crt.total = collect["total"];
                    db.tbl_cart.Add(crt);
                    int i = db.SaveChanges();

                    if (i > 0)
                    {
                        int Id = Convert.ToInt32(collect["Id"]);
                        tbl_wishlist wsh = db.tbl_wishlist.Where(j => j.id == Id).FirstOrDefault();
                        db.tbl_wishlist.Remove(wsh);
                        db.SaveChanges();
                    }
                    ViewBag.Message = "product Succesfully Added to cart";
                }
                else
                {
                    tbl_cart ct = db.tbl_cart.Where(j => j.prod_id == prdt && j.user_id == name).FirstOrDefault();
                    int qty = Convert.ToInt16(ct.pro_qty);
                    decimal price = Convert.ToDecimal(ct.pro_price);
                    int newqty = qty + 1;
                    decimal sub = newqty * price;
                    ct.pro_qty = Convert.ToString(newqty);
                    ct.sub_total = Convert.ToString(sub);
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        int Id = Convert.ToInt32(collect["Id"]);
                        tbl_wishlist wsh = db.tbl_wishlist.Where(j => j.id == Id).FirstOrDefault();
                        db.tbl_wishlist.Remove(wsh);
                        db.SaveChanges();
                    }
                    ViewBag.Message = "product Already Added to cart";
                }

                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.product = db.tblProdImages.ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
            }
            catch
            {

            }
            return Redirect("~/Public/Wishlist?Msg=" + ViewBag.Message);

        }
        [HttpGet]
        public ActionResult OfferList(string offer_id, string Discount, string range, string Colour, string Weight, string Size, string Brand, string pageNo)
        {
            try
            {

                string OfferID = "";
                string Wt = "";
                string sz = "";
                string br = "";
                string min = "";
                string max = "";
                string Cvalue = "";
                string diMin = "";
                string diMax = "";
                int Page;
                int RowsPerPage = 20;


                if (String.IsNullOrEmpty(pageNo))
                { Page = 1; }
                else
                { Page = Convert.ToInt32(pageNo); }

                ViewBag.page = Page;


                /*Set OfferId*/
                if (String.IsNullOrEmpty(offer_id))
                {
                    OfferID = Convert.ToString(-1);
                }
                else
                {
                    OfferID = offer_id;
                }

                /*Set Range*/
                if (String.IsNullOrEmpty(range))
                {
                    min = Convert.ToString(-1);
                    max = Convert.ToString(-1);
                }
                else
                {
                    string[] split = range.Split('-');
                    min = split[0];
                    max = split[1];
                }

                /*Set Brand*/
                if (String.IsNullOrEmpty(Brand))
                {
                    br = Convert.ToString(-1);

                }
                else
                {
                    br = Brand;
                }
                /*Set Discount*/
                if (String.IsNullOrEmpty(Discount))
                {
                    diMin = Convert.ToString(-1);
                    diMax = Convert.ToString(-1);
                }
                else
                {
                    string[] Displit = Discount.Split('-');
                    diMin = Displit[0];
                    diMax = Displit[1];
                }
                /*Set colour*/
                if (String.IsNullOrEmpty(Colour))
                {
                    Cvalue = Convert.ToString(-1);
                }
                else
                {
                    Cvalue = "#" + Colour;
                }

                /*Set Weight*/
                if (String.IsNullOrEmpty(Weight))
                {
                    Wt = Convert.ToString(-1);

                }
                else
                {
                    Wt = Weight;
                }

                /*Set Range*/
                if (String.IsNullOrEmpty(Size))
                {
                    sz = Convert.ToString(-1);
                }
                else
                {
                    sz = Size;
                }


                ViewBag.offer_id = offer_id;
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.Offer = db.tbl_specialoffer.ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
                ViewBag.Offerlist = db.sp_Search_Deals(offer_id, diMin, diMax, min, max, Cvalue, Wt, sz, br, Page, RowsPerPage).ToList();
                ViewBag.listCount = db.tbl_specialoffer.Count();

            }
            catch { }
            return View();
        }


        [HttpGet]
        public ActionResult todayDeal(string Offer_Name, string Discount, string range, string Colour, string Weight, string Size, string Brand, string pageNo)
        {
            try
            {
                string OfferName = "";
                string Wt = "";
                string sz = "";
                string min = "";
                string max = "";
                string br = "";
                string Cvalue = "";
                string diMin = "";
                string diMax = "";
                int Page;
                int RowsPerPage = 20;


                if (String.IsNullOrEmpty(pageNo))
                { Page = 1; }
                else
                { Page = Convert.ToInt32(pageNo); }

                ViewBag.page = Page;


                /*Set OfferId*/
                if (String.IsNullOrEmpty(Offer_Name))
                {
                    OfferName = Convert.ToString(-1);
                }
                else
                {
                    OfferName = Offer_Name;
                }

                /*Set Range*/
                if (String.IsNullOrEmpty(range))
                {
                    min = Convert.ToString(-1);
                    max = Convert.ToString(-1);
                }
                else
                {
                    string[] split = range.Split('-');
                    min = split[0];
                    max = split[1];
                }

                /*Set Discount*/
                if (String.IsNullOrEmpty(Discount))
                {
                    diMin = Convert.ToString(-1);
                    diMax = Convert.ToString(-1);
                }
                else
                {
                    string[] Displit = Discount.Split('-');
                    diMin = Displit[0];
                    diMax = Displit[1];
                }
                /*Set colour*/
                if (String.IsNullOrEmpty(Colour))
                {
                    Cvalue = Convert.ToString(-1);
                }
                else
                {
                    Cvalue = "#" + Colour;
                }

                /*Set Brandt*/
                if (String.IsNullOrEmpty(Brand))
                {
                    br = Convert.ToString(-1);

                }
                else
                {
                    br = Brand;
                }

                /*Set Weight*/
                if (String.IsNullOrEmpty(Weight))
                {
                    Wt = Convert.ToString(-1);

                }
                else
                {
                    Wt = Weight;
                }

                /*Set Range*/
                if (String.IsNullOrEmpty(Size))
                {
                    sz = Convert.ToString(-1);
                }
                else
                {
                    sz = Size;
                }

                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.Offer = db.tbl_specialoffer.ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
                ViewBag.Offerlist = db.sp_Search_Today(OfferName, diMin, diMax, min, max, Cvalue, Wt, sz, br, Page, RowsPerPage).ToList();
                ViewBag.listCount = db.tbl_TodaysDeal.Count();
            }
            catch
            {

            }
            return View();
        }

        //[HttpGet]
        //public ActionResult Index()
        //{
        //    if (Session["username"] == null)
        //    {
        //        Session["username"] = UID;

        //    }

        //    ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
        //    ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
        //    ViewBag.product = db.tblProdImages.ToList();
        //    ViewBag.brand = db.tblBrands.ToList();
        //    ViewBag.newproducts = db.Database.SqlQuery<sp_newproducts>("exec sp_newproducts").ToList();
        //    ViewBag.spofferleft = db.tbl_specialoffer.Where(i => i.position == "Left").ToList();
        //    ViewBag.spofferright = db.tbl_specialoffer.Where(i => i.position == "Right").ToList();
        //    ViewBag.Modelcount = 0;


        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Index(FormCollection c)
        //{

        //    ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
        //    ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();

        //    ViewBag.product = db.tblProdImages.ToList();

        //    ViewBag.brand = db.tblBrands.ToList();
        //    ViewBag.newproducts = db.Database.SqlQuery<sp_newproducts>("exec sp_newproducts").ToList();
        //    ViewBag.spofferleft = db.tbl_specialoffer.Where(i => i.position == "Left").ToList();
        //    ViewBag.spofferright = db.tbl_specialoffer.Where(i => i.position == "Right").ToList();

        //    return View();
        //}


        public ActionResult About()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.name = log_uname;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();

            }
            catch
            {

            }
            return View();
        }


        public ActionResult ProductDetails(int id)
        {

            ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
            ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();

            ViewBag.list = db.tblProduct.Where(i => i.pid == id && i.Hide != "Hided").FirstOrDefault();
            ViewBag.image = db.tblProdImages.Where(i => i.Prodid == id).ToList();
            ViewBag.reviews = db.tblreviews.Where(i => i.prodid == id).ToList();

            tblProduct tp = db.tblProduct.Where(i => i.pid == id && i.Hide != "Hided").FirstOrDefault();
            int catid = Convert.ToInt32(tp.Catid);
            string catids = Convert.ToString(catid);
            ViewBag.related = db.Relatedpro(catids).ToList();
            int pro = id;
            ViewBag.size = db.tblProduct.Where(i => i.Catid == catid && i.Hide != "Hided").ToList();


            return View();
        }


        [HttpGet]
        public ActionResult cart()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                List<string> cartList = new List<string>();
                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                {
                    tblStock st = db.tblStocks.Where(i => i.ProductId == ct.prod_id).FirstOrDefault();
                    int stock = Convert.ToInt32(st.Qty);
                    if (stock < 1)
                    {
                        tbl_cart delcart = db.tbl_cart.Where(i => i.id == ct.id).FirstOrDefault();
                        db.tbl_cart.Remove(delcart);
                        db.SaveChanges();
                    }
                    if (!cartList.Contains(ct.prod_id.ToString()))
                    {
                        cartList.Add(ct.prod_id.ToString());
                    }
                    else
                    {
                        tbl_cart delcart = db.tbl_cart.Where(i => i.id == ct.id).FirstOrDefault();
                        db.tbl_cart.Remove(delcart);
                        db.SaveChanges();
                    }

                }

                ViewBag.cname = name;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                int cart = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.cartcount = cart;

                if (cart < 1)
                {
                    return Redirect("~/Public/NewIndex");
                }
                else
                {
                    tbl_cart crt = db.tbl_cart.Where(i => i.user_id == name).OrderByDescending(i => i.id).FirstOrDefault();
                    string cname = db.tblCategories.Where(i => i.Catid == crt.cat_id).FirstOrDefault().CatName;
                    ViewBag.recmmndActive = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").OrderByDescending(I => I.AddedDate).ToList().Take(1);
                    ViewBag.recmmnd = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").OrderByDescending(I => I.AddedDate).ToList().Take(9);
                    ViewBag.recmCount = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").ToList().Count();
                }

                tblRegistration tb = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.product = db.tblProdImages.ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();

              

            }
            catch
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult cart(FormCollection collect)
        {


            try
            {
                tbl_cart crt = new tbl_cart();
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                int prdt = Convert.ToInt16(collect["pro_id"]);
                string pr = Convert.ToString(collect["pro_price"]);
                int item = db.tbl_cart.Where(i => i.prod_id == prdt && i.user_id == name && i.pro_price == pr).Count();
                string from = collect["From"];
                string quantity= collect["qty"];
                if(quantity == "")
                {
                    ViewBag.Message = "Please verify your Quantity";
                    return Redirect("~/Public/NewProductDetails?id=" + prdt + "&msg=" + ViewBag.Message);
                }

                if (item < 1)
                {
                    crt.user_id = name;
                    crt.prod_id = Convert.ToInt16(collect["pro_id"]);
                    crt.cat_id = Convert.ToInt16(collect["cat_id"]);
                    crt.pro_name = collect["pro_name"];
                    crt.pro_price = collect["pro_price"];
                    crt.pro_image = collect["pro_image"];
                    crt.pro_qty = collect["qty"];
                    decimal price = Convert.ToDecimal(collect["pro_price"]);
                    int quanti= Convert.ToInt16(quantity);
                    decimal subtotal = quanti * price;
                    crt.sub_total = subtotal.ToString();
                    crt.total = subtotal.ToString();
                    crt.Type = collect["Type"];
                    db.tbl_cart.Add(crt);
                    db.SaveChanges();
                    ViewBag.Message = "product Succesfully Added to cart";
                    if (from == "search")
                    {
                        string keyword = collect["key"];
                        string Catword = collect["cat"];
                        return Redirect("~/Public/products?keyword=" + keyword + "&Catword=" + Catword + "&msg=" + ViewBag.Message);
                    }
                    else if (from == "SingleView")
                    {
                        return Redirect("~/Public/NewProductDetails?id=" + prdt + "&msg=" + ViewBag.Message);
                    }
                }
                else
                {
                    if (from == "search")
                    {
                        tbl_cart ct = db.tbl_cart.Where(j => j.prod_id == prdt && j.user_id == name && j.pro_price == pr).FirstOrDefault();
                        int qty = Convert.ToInt16(ct.pro_qty);
                        decimal price = Convert.ToDecimal(ct.pro_price);
                        decimal Prdctprice = Convert.ToDecimal(collect["pro_price"]);

                        if (price == Prdctprice)
                        {
                            int newqty = qty + 1;
                            decimal sub = newqty * price;
                            ct.pro_qty = Convert.ToString(newqty);
                            ct.sub_total = Convert.ToString(sub);
                            db.SaveChanges();
                            ViewBag.Message = "product Already Added to cart";
                            string keyword = collect["key"];
                            string Catword = collect["cat"];
                            return Redirect("~/Public/products?keyword=" + keyword + "&Catword=" + Catword + "&msg=" + ViewBag.Message);
                        }
                        else
                        {
                            crt.user_id = name;
                            crt.prod_id = Convert.ToInt16(collect["pro_id"]);
                            crt.cat_id = Convert.ToInt16(collect["cat_id"]);
                            crt.pro_name = collect["pro_name"];
                            crt.pro_price = collect["pro_price"];
                            crt.pro_image = collect["pro_image"];
                            crt.pro_qty = collect["qty"];
                            decimal price1 = Convert.ToDecimal(collect["pro_price"]);
                            int quanti1 = Convert.ToInt16(collect["qty"]);
                            decimal subtotal1 = quanti1 * price1;
                            crt.sub_total = subtotal1.ToString();
                            crt.total = subtotal1.ToString();
                            crt.Type = collect["Type"];
                            db.tbl_cart.Add(crt);
                            db.SaveChanges();
                            ViewBag.Message = "product Succesfully Added to cart";
                            string keyword = collect["key"];
                            string Catword = collect["cat"];
                            return Redirect("~/Public/products?keyword=" + keyword + "&Catword=" + Catword + "&msg=" + ViewBag.Message);
                        }
                    }
                    else if (from == "SingleView")
                    {
                        tbl_cart ct = db.tbl_cart.Where(j => j.prod_id == prdt && j.user_id == name && j.pro_price == pr).FirstOrDefault();
                        int qty = Convert.ToInt16(ct.pro_qty);
                        int Nqty = Convert.ToInt16(collect["qty"]);
                        decimal price = Convert.ToDecimal(ct.pro_price);
                        decimal Prdctprice = Convert.ToDecimal(collect["pro_price"]);
                        int newqty = qty + Nqty;
                        if (newqty > 10)
                        {
                            ViewBag.Message = "product Quantity Overloaded, Please check your cart";
                            return Redirect("~/Public/NewProductDetails?id=" + prdt + "&msg=" + ViewBag.Message);
                        }
                        else
                        {
                            if (price == Prdctprice)
                            {

                                decimal sub = newqty * price;
                                ct.pro_qty = Convert.ToString(newqty);
                                ct.sub_total = Convert.ToString(sub);
                                db.SaveChanges();
                                ViewBag.Message = "product Already Added to cart";
                                return Redirect("~/Public/NewProductDetails?id=" + prdt + "&msg=" + ViewBag.Message);
                            }
                            else
                            {
                                crt.user_id = name;
                                crt.prod_id = Convert.ToInt16(collect["pro_id"]);
                                crt.cat_id = Convert.ToInt16(collect["cat_id"]);
                                crt.pro_name = collect["pro_name"];
                                crt.pro_price = collect["pro_price"];
                                crt.pro_image = collect["pro_image"];
                                crt.pro_qty = collect["qty"];
                                decimal price2 = Convert.ToDecimal(collect["pro_price"]);
                                int quanti2 = Convert.ToInt16(collect["qty"]);
                                decimal subtotal2 = quanti2 * price2;
                                crt.sub_total = subtotal2.ToString();
                                crt.total = subtotal2.ToString();
                                crt.Type = collect["Type"];
                                db.tbl_cart.Add(crt);
                                db.SaveChanges();
                                ViewBag.Message = "product Succesfully Added to cart";
                                return Redirect("~/Public/NewProductDetails?id=" + prdt + "&msg=" + ViewBag.Message);
                            }
                        }
                    }
                }

                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.product = db.tblProdImages.ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
            }
            catch
            {

            }
            return Redirect("~/Public/cart?Msg=" + ViewBag.Message);

        }

        [HttpPost]
        public ActionResult Offercart(FormCollection collect)
        {
            try
            {
                tbl_cart crt = new tbl_cart();
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                int prdt = Convert.ToInt16(collect["pro_id"]);
                string pr = Convert.ToString(collect["pro_price"]);
                int item = db.tbl_cart.Where(i => i.prod_id == prdt && i.user_id == name && i.pro_price == pr).Count();
                string from = collect["From"];
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.product = db.tblProdImages.ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();

                if (item < 1)
                {
                    crt.user_id = name;
                    crt.prod_id = Convert.ToInt16(collect["pro_id"]);
                    crt.cat_id = Convert.ToInt16(collect["cat_id"]);
                    crt.pro_name = collect["pro_name"];
                    crt.pro_price = collect["pro_price"];
                    crt.pro_image = collect["pro_image"];
                    crt.pro_qty = collect["qty"];
                    decimal price = Convert.ToDecimal(collect["pro_price"]);
                    int quanti = Convert.ToInt16(collect["qty"]);
                    decimal subtotal = quanti * price;
                    crt.sub_total = subtotal.ToString();
                    crt.total = subtotal.ToString();
                    crt.Type = collect["Type"];
                    db.tbl_cart.Add(crt);
                    db.SaveChanges();
                    ViewBag.Message = "product Succesfully Added to cart";
                    if (from == "Offer")
                    {
                        string offer = collect["Offer"];
                        return Redirect("~/Public/OfferList?Offer_id=" + offer + "&Msg=" + ViewBag.Message);
                    }
                    else if (from == "Today")
                    {
                        return Redirect("~/Public/todayDeal?Msg=" + ViewBag.Message);
                    }
                }
                else
                {
                    tbl_cart ct = db.tbl_cart.Where(j => j.prod_id == prdt && j.user_id == name).FirstOrDefault();
                    tblProduct pd = db.tblProduct.Where(i => i.pid == prdt && i.Hide != "Hided").FirstOrDefault();
                    int qty = Convert.ToInt16(ct.pro_qty);

                    decimal newprice = Convert.ToDecimal(pd.ProdNewPrice);
                    decimal oldprice = Convert.ToDecimal(ct.sub_total);
                    int newqty = qty + 1;
                        if (newqty > 10)
                        {
                            ViewBag.Message = "product Quantity Overloaded, Please check your cart";
                            return Redirect("~/Public/Cart?msg=" + ViewBag.Message);
                        }
                    decimal sub = oldprice + newprice;
                    ct.pro_qty = Convert.ToString(newqty);
                    ct.sub_total = Convert.ToString(sub);
                    db.SaveChanges();
                    ViewBag.Message = "product Already Added to cart";
                    if (from == "Offer")
                    {
                        string offer = collect["Offer"];
                        return Redirect("~/Public/OfferList?Offer_id=" + offer + "&Msg=" + ViewBag.Message);
                    }
                    else if (from == "Today")
                    {
                        return Redirect("~/Public/todayDeal?Msg=" + ViewBag.Message);
                    }
                }


            }
            catch
            {

            }
            return Redirect("~/Public/cart?Msg=" + ViewBag.Message);

        }

        public string UpdateOfferCart(string id, string qty)
        {
            try
            {
                int pid = Convert.ToInt32(id);
                tbl_cart ct = db.tbl_cart.Where(i => i.id == pid).FirstOrDefault();

                int qtys = Convert.ToInt32(qty);
             
                if (qtys > 10)
                {
                    //tblProduct pd = db.tblProduct.Where(i => i.pid == ct.prod_id && i.Hide != "Hided").FirstOrDefault();

                    //int nqty = qtys - 1;

                    //decimal newprice = Convert.ToDecimal(pd.ProdNewPrice);
                    //decimal oldprice = Convert.ToDecimal(ct.pro_price);

                    //decimal sd = (newprice * nqty) + oldprice;
                    //ct.pro_qty = Convert.ToString(qtys);
                    //ct.sub_total = Convert.ToString(sd);
                    //db.SaveChanges();
                    decimal price = Convert.ToDecimal(ct.pro_price);
                    decimal sub = 1 * price;

                    ct.pro_qty = "1";
                    ct.sub_total = Convert.ToString(sub);
                    db.SaveChanges();

                    ViewBag.Message = "Quantity Updated Successfully";
                    return Redirect("~/Public/cart?Msg=" + ViewBag.Message);
                }
                else
                {
                    tblProduct pd = db.tblProduct.Where(i => i.pid == ct.prod_id && i.Hide != "Hided").FirstOrDefault();
                    tblStock st = db.tblStocks.Where(i => i.ProductId == ct.prod_id).FirstOrDefault();
                    int stock = Convert.ToInt32(st.Qty);
                    if (stock < 10 && qtys > stock)
                    {
                        qtys = stock;
                        int nqty = qtys - 1;
                        decimal newprice = Convert.ToDecimal(pd.ProdNewPrice);
                        decimal oldprice = Convert.ToDecimal(ct.pro_price);

                        decimal sd = (newprice * nqty) + oldprice;
                        ct.pro_qty = Convert.ToString(qtys);
                        ct.sub_total = Convert.ToString(sd);
                        db.SaveChanges();

                        ViewBag.Message = "Quantity Updated Successfully";
                        return Redirect("~/Public/cart?Msg=" + ViewBag.Message);
                    }
                    else
                    {
                        int nqty = qtys - 1;

                        decimal newprice = Convert.ToDecimal(pd.ProdNewPrice);
                        decimal oldprice = Convert.ToDecimal(ct.pro_price);

                        decimal sd = (newprice * nqty) + oldprice;
                        ct.pro_qty = Convert.ToString(qtys);
                        ct.sub_total = Convert.ToString(sd);
                        db.SaveChanges();
                    }
                }


            }
            catch { }
            string status = "Cart Updated succesfully";
            return status;
        }




        public string UpdateCart(string id, string qty)
        {
            try
            {
                int pid = Convert.ToInt32(id);
                int qtys = Convert.ToInt32(qty);
                tbl_cart ct = db.tbl_cart.Where(i => i.id == pid).FirstOrDefault();
                if (qtys>10)
                {
                    tbl_cart ct1 = db.tbl_cart.Where(i => i.id == pid).FirstOrDefault();
                    decimal price1 = Convert.ToDecimal(ct1.pro_price);
                    decimal sub1 = 10 * price1;
                    ct1.pro_qty = "10";
                    ct1.sub_total = Convert.ToString(sub1);
                    db.SaveChanges();
                    ViewBag.Message = "Quantity Updated Successfully";
                    return Redirect("~/Public/cart?Msg=" + ViewBag.Message);
                }
                tblStock st = db.tblStocks.Where(i => i.ProductId == ct.prod_id).FirstOrDefault();
                int stock = Convert.ToInt32(st.Qty);
                if (stock < 10 && qtys > stock)
                {
                    tbl_cart ct2 = db.tbl_cart.Where(i => i.id == pid).FirstOrDefault();
                    decimal price2 = Convert.ToDecimal(ct2.pro_price);
                    decimal sub2 = stock* price2;
                    ct2.pro_qty = stock.ToString();
                    ct2.sub_total = Convert.ToString(sub2);
                    db.SaveChanges();
                    ViewBag.Message = "Quantity Updated Successfully";
                    return Redirect("~/Public/cart?Msg=" + ViewBag.Message);
                }

                
                decimal price = Convert.ToDecimal(ct.pro_price);
                decimal sub = qtys * price;
                ct.pro_qty = qty;
                ct.sub_total = Convert.ToString(sub);
                db.SaveChanges();
            }
            catch { }
            string status = "T";
            return status;
        }



        public ActionResult DeleteCart(int id, string name)
        {

            try
            {
                tbl_cart tb = db.tbl_cart.Where(i => i.id == id).FirstOrDefault();
                db.tbl_cart.Remove(tb);
                db.SaveChanges();
                ViewBag.Message = "Item" + " " + name + " " + "is Deleted From cart Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Item" + " " + name + " " + " is Not Deleted From cart Successfully!!";
            }

            return Redirect("~/Public/cart?Msg=" + ViewBag.Message);
        }

        [HttpPost]
        public ActionResult Buynow(FormCollection collect)
        {
            try
            {
                tbl_cart crt = new tbl_cart();
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string user_name = Session["username"].ToString();
                int log = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                if (log < 1)
                {
                    crt.user_id = user_name;
                    crt.prod_id = Convert.ToInt16(collect["pro_id"]);
                    crt.cat_id = Convert.ToInt16(collect["cat_id"]);
                    crt.pro_name = collect["pro_name"];
                    crt.pro_price = collect["pro_price"];
                    crt.pro_image = collect["pro_image"];
                    crt.pro_qty = collect["qty"];
                    crt.sub_total = collect["subtotal"];
                    crt.total = collect["total"];
                    crt.Type = collect["Type"];
                    db.tbl_cart.Add(crt);
                    db.SaveChanges();
                    return Redirect("~/Public/login?from=Buynow&name=" + user_name);
                }
                else
                {
                    crt.user_id = user_name;
                    crt.prod_id = Convert.ToInt16(collect["pro_id"]);
                    crt.cat_id = Convert.ToInt16(collect["cat_id"]);
                    crt.pro_name = collect["pro_name"];
                    crt.pro_price = collect["pro_price"];
                    crt.pro_image = collect["pro_image"];
                    crt.pro_qty = collect["qty"];
                    crt.sub_total = collect["subtotal"];
                    crt.total = collect["total"];
                    crt.Type = collect["Type"];
                    db.tbl_cart.Add(crt);
                    db.SaveChanges();
                    return Redirect("~/public/checkout?uname=" + user_name);
                }
            }
            catch
            {

            }
            return View();
        }

        public ActionResult OtherOption(string pname)
        {
            try
            {
                ViewBag.pdctDetails = db.tblProduct.Where(i => i.ProdName == pname && i.Hide != "Hided").ToList();
                ViewBag.pdctDetailFirst = db.tblProduct.Where(i => i.ProdName == pname && i.Hide != "Hided").FirstOrDefault();
                string log_uname = Session["username"].ToString();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                tblProduct tp = db.tblProduct.Where(i => i.ProdName == pname && i.Hide != "Hided").FirstOrDefault();
                string cname = tp.CatName;
                ViewBag.recmmndActive = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").OrderByDescending(I => I.AddedDate).ToList().Take(1);
                ViewBag.recmmnd = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").OrderByDescending(I => I.AddedDate).ToList().Take(9);
                ViewBag.recmCount = db.tblProduct.Where(i => i.CatName == cname && i.Hide != "Hided").ToList().Count();
            }
            catch { }
            return View();
        }




        [HttpGet]
        public ActionResult checkout(string uname)
        {
            ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == uname).Count();
            int ue = db.tblRegistration.Where(i => i.RegEmail == uname).Count();
            if (ue > 0)
            {
                ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == uname).FirstOrDefault().RegName;
            }
            int log = db.tblRegistration.Where(i => i.RegEmail == uname).Count();
            ViewBag.cart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
            ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == uname).Count();
            ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == uname).Count();
            ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == uname).ToList();

            int ItemCount = db.tbl_cart.Where(i => i.user_id == uname).Count();

            if (ItemCount < 1)
            {
                return Redirect("~/Public/NewIndex");
            }
            else
            {
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=cart&name=" + uname);

                }
                else
                {
                    ViewBag.cartlist = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                }
            }
            ViewBag.address = db.tb_address.Where(i => i.user_Id == uname).ToList().Take(3);
            ViewBag.addcount = db.tb_address.Where(i => i.user_Id == uname).Count();
            ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
            ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Address(FormCollection collect)
        {
            try
            {
                string user_name = Session["username"].ToString();
                int pin= Convert.ToInt32(collect["Pincode"]);
                int code = db.tbl_Pincode.Where(i => i.Pincode == pin).Count();
                if(code<1)
                {
                    ViewBag.Message = "Pincode You Entered is incorrect";
                    return Redirect("~/public/checkout?uname=" + user_name +"&msg=" + ViewBag.Message);

                }
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == user_name).Count();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).ToList();
                string add1 = collect["Address1"];
                string add2 = collect["Address2"];
                string add3 = collect["Address3"];
                string add4 = collect["city"];
                string address = add1 + " "+ " " + add2 + " " + " " + add3;
                tb_address add = new tb_address();
                add.user_Id = user_name;
                add.fullname = collect["Full_name"];
                add.Phone = Convert.ToInt64(collect["Mobile_Number"]);
                add.Address = address;
                add.pincode = Convert.ToInt64(collect["Pincode"]);
                add.state = collect["state"];
                add.city= collect["city"];
                db.tb_address.Add(add);
                db.SaveChanges();
                ViewBag.address = db.tb_address.Where(i => i.user_Id == user_name);

                int id = db.tb_address.Where(i => i.user_Id == user_name).Max(i => i.ID);
               

                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                {
                    tbl_cart crt = db.tbl_cart.Where(i => i.id == ct.id).FirstOrDefault();
                    crt.address_id = id;
                    db.SaveChanges();
                }
            }
            catch
            {

            }
            return Redirect("~/Public/DelivaryMethod");
        }

        public ActionResult ConfirmAddress(int id)
        {
            int c = db.tb_address.Where(i => i.ID == id).Count();
            if (c > 0)
            {
                db.SaveChanges();
            }
            string user_name = Session["username"].ToString();

            List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
            foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
            {
                tbl_cart crt = db.tbl_cart.Where(i => i.id == ct.id).FirstOrDefault();
                crt.address_id = id;
                db.SaveChanges();
            }

            return Redirect("~/Public/DelivaryMethod");

        }
        public ActionResult Deleteaddress(int id)
        {
            string user_name = Session["username"].ToString();
            try
            {
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                tb_address add = db.tb_address.Where(i => i.ID == id).FirstOrDefault();
                db.tb_address.Remove(add);
                db.SaveChanges();
            }
            catch { }
            return Redirect("~/Public/checkout?uname=" + user_name);
        }
        [HttpGet]
        public ActionResult DelivaryMethod()
        {
            try
            {
                string user_name = Session["username"].ToString();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == user_name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == user_name).Count();
                ViewBag.cartlist = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                tbl_cart id = db.tbl_cart.Where(i => i.user_id == user_name).FirstOrDefault();
                ViewBag.address = db.tb_address.Where(i => i.ID == id.address_id).FirstOrDefault();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).ToList();
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
            }
            catch { }
            return View();
        }
        [HttpPost]
        public ActionResult DelivaryMethod(FormCollection collect)
        {
            try
            {
                string user_name = Session["username"].ToString();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                {
                    tbl_cart crt = db.tbl_cart.Where(i => i.id == ct.id).FirstOrDefault();
                    crt.shipping = collect["delivary"];
                    db.SaveChanges();
                }

            }
            catch { }
            return Redirect("Placeorder");
        }



        [HttpGet]
        public ActionResult PlaceOrder()
        {
            try
            {
                string user_name = Session["username"].ToString();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).ToList();

                foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                {
                    int qty = Convert.ToInt32(ct.pro_qty);
                    string type = ct.Type;
                    if (type == "Offer")
                    {
                        if (qty > 1)
                        {
                            tblProduct pd = db.tblProduct.Where(i => i.pid == ct.prod_id).FirstOrDefault();
                            int nqty = qty - 1;
                            decimal subtotal = Convert.ToDecimal(pd.ProdNewPrice * nqty);


                            tbl_cart crt = new tbl_cart();
                            crt.user_id = user_name;
                            crt.prod_id = ct.prod_id;
                            crt.cat_id = ct.cat_id;
                            crt.pro_name = ct.pro_name;
                            crt.pro_price = Convert.ToString(pd.ProdNewPrice);
                            crt.pro_image = ct.pro_image;
                            crt.pro_qty = Convert.ToString(nqty);
                            crt.sub_total = Convert.ToString(subtotal);
                            crt.total = Convert.ToString(subtotal);
                            crt.Type = "Offer";
                            crt.Discount = ct.Discount;
                            crt.shipping = ct.shipping;
                            crt.Gift_wrap = ct.Gift_wrap;
                            crt.Tax = ct.Tax;
                            crt.address_id = ct.address_id;
                            db.tbl_cart.Add(crt);
                            int x = db.SaveChanges();
                            if (x > 0)
                            {
                                ct.pro_qty = Convert.ToString("1");
                                decimal price = Convert.ToDecimal(ct.pro_price);
                                ct.sub_total = Convert.ToString(price * 1);
                                ct.total = Convert.ToString(price * 1);
                                db.SaveChanges();

                            }
                        }

                    }
                    if (type == "Today")
                    {
                        if (qty > 1)
                        {
                            tblProduct pd = db.tblProduct.Where(i => i.pid == ct.prod_id).FirstOrDefault();
                            int nqty = qty - 1;
                            decimal subtotal = Convert.ToDecimal(pd.ProdNewPrice * nqty);


                            tbl_cart crt = new tbl_cart();
                            crt.user_id = user_name;
                            crt.prod_id = ct.prod_id;
                            crt.cat_id = ct.cat_id;
                            crt.pro_name = ct.pro_name;
                            crt.pro_price = Convert.ToString(pd.ProdNewPrice);
                            crt.pro_image = ct.pro_image;
                            crt.pro_qty = Convert.ToString(nqty);
                            crt.sub_total = Convert.ToString(subtotal);
                            crt.total = Convert.ToString(subtotal);
                            crt.Type = "Today";
                            crt.Discount = ct.Discount;
                            crt.shipping = ct.shipping;
                            crt.Gift_wrap = ct.Gift_wrap;
                            crt.Tax = ct.Tax;
                            crt.address_id = ct.address_id;
                            db.tbl_cart.Add(crt);
                            int x = db.SaveChanges();
                            if (x > 0)
                            {
                                ct.pro_qty = Convert.ToString("1");
                                decimal price = Convert.ToDecimal(ct.pro_price);
                                ct.sub_total = Convert.ToString(price * 1);
                                ct.total = Convert.ToString(price * 1);
                                db.SaveChanges();

                            }
                        }

                    }


                }




                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == user_name).FirstOrDefault().RegName;
                }
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == user_name).Count();
                ViewBag.cartlist = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                tbl_cart id = db.tbl_cart.Where(i => i.user_id == user_name).FirstOrDefault();
                ViewBag.address = db.tb_address.Where(i => i.ID == id.address_id).FirstOrDefault();
                ViewBag.data = db.tblProduct.Distinct();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
               
            }
            catch { }
            return View();
        }
        //[HttpPost]
        //public ActionResult PlaceOrder(FormCollection collect)
        //{
        //    string user_name = Session["username"].ToString();
        //    string date = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
        //    string time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();

        //    List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
        //    int countOrder = db.tblOrders.Count();

        //    if (countOrder == 0)
        //    {
        //        int invoice_id = 1000000000;
        //        foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
        //        {
        //            int VendorId = Convert.ToInt32(db.tblProduct.Where(i => i.pid == ct.prod_id).FirstOrDefault().Vendor);
        //            tblOrders order = new tblOrders();
        //            order.Userid = ct.user_id;
        //            order.Invoice_Id = invoice_id;
        //            order.ProId = ct.prod_id;
        //            order.ProdName = ct.pro_name;
        //            order.Vendor = VendorId;
        //            order.Qty = Convert.ToInt16(ct.pro_qty);
        //            order.Price = Convert.ToDecimal(ct.pro_price);
        //            order.Discount = Convert.ToString(ct.Discount);
        //            order.Total = Convert.ToDecimal(ct.sub_total);
        //            order.Gift = ct.Gift_wrap;
        //            order.Shipping = Convert.ToInt16(ct.shipping);
        //            order.Tax = Convert.ToString(ct.Tax);

        //            tb_address ad = db.tb_address.Where(i => i.user_Id == ct.user_id).FirstOrDefault();

        //            order.Address = ad.ID;
        //            order.DeliveryMethod = "not set";
        //            order.PaymentMethod = "not set";
        //            order.status = Convert.ToString("false");
        //            order.OrderDate = Convert.ToDateTime(DateTime.Now.Date);
        //            order.OrderTime = Convert.ToString(time);
        //            db.tblOrders.Add(order);
        //            db.SaveChanges();
        //        }
        //    }
        //    else
        //    {
        //        int invoice_id = Convert.ToInt32(db.tblOrders.OrderByDescending(p => p.Invoice_Id).FirstOrDefault().Invoice_Id);
        //        invoice_id = invoice_id + 1;
        //        foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
        //        {
        //            int VendorId = Convert.ToInt32(db.tblProduct.Where(i => i.pid == ct.prod_id).FirstOrDefault().Vendor);
        //            tblOrders order = new tblOrders();
        //            order.Userid = ct.user_id;
        //            order.Invoice_Id = invoice_id;
        //            order.ProId = ct.prod_id;
        //            order.ProdName = ct.pro_name;
        //            order.Vendor = VendorId;
        //            order.Qty = Convert.ToInt16(ct.pro_qty);
        //            order.Price = Convert.ToDecimal(ct.pro_price);
        //            order.Discount = Convert.ToString(ct.Discount);
        //            order.Total = Convert.ToDecimal(ct.sub_total);
        //            order.Gift = ct.Gift_wrap;
        //            order.Shipping = Convert.ToInt16(ct.shipping);
        //            order.Tax = Convert.ToString(ct.Tax);

        //            tb_address ad = db.tb_address.Where(i => i.user_Id == ct.user_id).FirstOrDefault();

        //            order.Address = ad.ID;
        //            order.DeliveryMethod = "not set";
        //            order.PaymentMethod = "not set";
        //            order.status = Convert.ToString("false");
        //            order.OrderDate = Convert.ToDateTime(DateTime.Now.Date);
        //            order.OrderTime = Convert.ToString(time);
        //            db.tblOrders.Add(order);
        //            db.SaveChanges();
        //        }
        //    }
        //    return Redirect("Gateway");
        //}

        public ActionResult gateway()
        {
            try
            {


                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                decimal tax = 0;
                decimal total = 0;
                decimal ship = 0;
                decimal gift = 0;
                decimal gtotal = 0;
                foreach (TradeKerala2017.Models.tbl_cart tt in ViewBag.cart)
                {

                    total = total + Convert.ToDecimal(@tt.sub_total);

                }
                foreach (TradeKerala2017.Models.tbl_cart tt in ViewBag.cart)
                {

                    tax = tax + Convert.ToDecimal(@tt.Tax);

                }
                foreach (TradeKerala2017.Models.tbl_cart tt in ViewBag.cart)
                {

                    ship = ship + Convert.ToDecimal(@tt.shipping);
                }
                foreach (TradeKerala2017.Models.tbl_cart tt in ViewBag.cart)
                {

                    gift = gift + Convert.ToDecimal(@tt.Gift_wrap);

                }
                gtotal = total + tax + ship + gift;
                ViewBag.gtotal = gtotal;
                int countOrder = db.tblOrders.Count();
                int invoice_id;
                if (countOrder == 0)
                {
                    invoice_id = 1000000000;

                }
                else
                {
                    string invoice =db.tblOrders.OrderByDescending(p => p.Invoice_Id).FirstOrDefault().Invoice_Id;
                    char[] MyChar = { 't', 'r', 'k', ' ' };
                    string Newinvoice = invoice.TrimStart(MyChar);
                    int invo = Convert.ToInt32(Newinvoice);
                    invoice_id = invo + 1;
                }


                ViewBag.invoice_id = "trk" + invoice_id;
                Session["invoice_id"] ="trk"+ invoice_id;
                tbl_cart id = db.tbl_cart.Where(i => i.user_id == log_uname).FirstOrDefault();
                ViewBag.address = db.tb_address.Where(i => i.ID == id.address_id).FirstOrDefault();

                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();


            }
            catch { }

            return View();
        }

        [HttpPost]
        public ActionResult gateway(FormCollection collect)
        {
            
        CCACrypto ccaCrypto = new CCACrypto();
            string workingKey = "E6F33A00755B9F000D0895361753D721";//put in the 32bit alpha numeric key in the quotes provided here 	
            string ccaRequest = "";
            string strMerchantId = "174974";
            string strEncRequest = "";
            string strAccessCode = "AVYG77FD74CL97GYLC";// put the access key in the quotes provided here.

            foreach (string name in Request.Form)
            {
                if (name != null)
                {
                    if (!name.StartsWith("_"))
                    {
                        ccaRequest = ccaRequest + name + "=" + HttpUtility.UrlEncode(Request.Form[name]) + "&";
                        /* Response.Write(name + "=" + Request.Form[name]);
                          Response.Write("</br>");*/
                    }
                }
            }
            strEncRequest = ccaCrypto.Encrypt(ccaRequest, workingKey);
            strMerchantId = Request.Form["merchant_id"].ToString();

            ViewBag.strEncRequest = strEncRequest;
            ViewBag.strMerchantId = strMerchantId;
            ViewBag.acceskey = strAccessCode;
            return Redirect("ConfirmGateway?resp=" + strEncRequest);
        }

        public ActionResult ConfirmGateway(string resp)
        {
            try
            {
                string encResp = resp;
                string strAccessCode = "AVYG77FD74CL97GYLC";
                ViewBag.strEncRequest = encResp;
                ViewBag.acceskey = strAccessCode;
                string workingKey = "E6F33A00755B9F000D0895361753D721";//put in the 32bit alpha numeric key in the quotes provided here
                CCACrypto ccaCrypto = new CCACrypto();
                string encResponse = ccaCrypto.Decrypt(encResp, workingKey);
                NameValueCollection Params = new NameValueCollection();
                string[] segments = encResponse.Split('&');
                foreach (string seg in segments)
                {
                    string[] parts = seg.Split('=');
                    if (parts.Length > 0)
                    {
                        string Key = parts[0].Trim();
                        string Value = parts[1].Trim();
                        Params.Add(Key, Value);
                    }
                }

                for (int i = 0; i < Params.Count; i++)
                {
                    Response.Write(Params.Keys[i] + " = " + Params[i] + "<br>");
                }


            }

            catch
            {

            }
            return View();
        }


        public ActionResult OrderView()
        {
            try
            {

                string date = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();
                string time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                string user_name = Session["username"].ToString();
                string invoice_id = Session["invoice_id"].ToString();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == user_name).FirstOrDefault().RegName;
                }
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();

                List<tbl_cart> TbCrt = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                foreach (TradeKerala2017.Models.tbl_cart ct in TbCrt)
                {
                    int VendorId = Convert.ToInt32(db.tblProduct.Where(i => i.pid == ct.prod_id).FirstOrDefault().Vendor);
                    tblOrders order = new tblOrders();
                    order.Userid = ct.user_id;
                    order.Invoice_Id = invoice_id;
                    order.ProId = ct.prod_id;
                    order.ProdName = ct.pro_name;
                    order.Vendor = VendorId;
                    order.Qty = Convert.ToInt16(ct.pro_qty);
                    order.Price = Convert.ToDecimal(ct.pro_price);
                    order.Discount = Convert.ToString(ct.Discount);
                    order.Total = Convert.ToDecimal(ct.sub_total);
                    order.Gift = ct.Gift_wrap;
                    order.Shipping = Convert.ToInt16(ct.shipping);
                    order.Tax = Convert.ToString(ct.Tax);
                    order.Address = ct.address_id;
                    order.DeliveryMethod = "not set";
                    order.PaymentMethod = "not set";
                    order.status = Convert.ToString("false");
                    order.OrderDate = Convert.ToDateTime(DateTime.Now.Date);
                    order.OrderTime = Convert.ToString(time);
                    db.tblOrders.Add(order);
                    db.SaveChanges();

                    string type = ct.Type.ToString();
                    
                    int prod = Convert.ToInt32(ct.prod_id);
                    int qty = Convert.ToInt32(ct.pro_qty);
                    tblStock st = db.tblStocks.Where(i => i.ProductId == prod).FirstOrDefault();
                    tblProduct pd = db.tblProduct.Where(i => i.pid == prod).FirstOrDefault();
                    pd.Qty = pd.Qty - qty;
                    db.SaveChanges();
                    st.Qty = st.Qty - qty;
                    db.SaveChanges();
                       

                    db.tbl_cart.Remove(ct);
                    db.SaveChanges();

                }


               

                //int clog = db.tblRegistration.Where(i => i.RegEmail == user_name && i.type == "Guest").Count();
                //if (clog > 0)
                //{
                //    tblRegistration log = db.tblRegistration.Where(i => i.RegEmail == user_name && i.type == "Guest").FirstOrDefault();
                //    db.tblRegistration.Remove(log);
                //    db.SaveChanges();
                //}
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == user_name).Count();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                
                tblOrders tb = db.tblOrders.Where(i => i.Userid == user_name && i.Invoice_Id== invoice_id).FirstOrDefault();
                ViewBag.invoice_number = invoice_id;
                int Address = Convert.ToInt16(tb.Address);
                ViewBag.address = db.tb_address.Where(i => i.ID == Address).FirstOrDefault();
                ViewBag.order = db.tblOrders.Where(i => i.Userid == user_name && i.Invoice_Id == invoice_id).ToList();
            }
            catch { }
            return View();
        }


        public ActionResult products(FormCollection c, string keyword, string Catword, string range, string Colour, string Weight, string Size, string Brand, string pageNo)

        {
            try
            {
                string keywords = keyword;
                string Catwords = Catword;
                ViewBag.keyword = keyword;
                ViewBag.catword = Catword;
                string imin = c["Pmin"];
                string imax = c["Pmax"];
                string Wt = "";
                string sz = "";
                string Br = "";
                string min = "";
                string max = "";
                string Cvalue = "";
                int Page;
                int RowsPerPage = 20;


                if (String.IsNullOrEmpty(pageNo))
                { Page = 1; }
                else
                { Page = Convert.ToInt32(pageNo); }

                ViewBag.page = Page;

                /*Set Range*/
                if (String.IsNullOrEmpty(range))
                {

                    min = Convert.ToString(-1);
                    max = Convert.ToString(-1);
                }
                else
                {
                    string[] split = range.Split(',');
                    min = split[0];
                    max = split[1];
                }

                /*Set colour*/
                if (String.IsNullOrEmpty(Colour))
                {
                    Cvalue = Convert.ToString(-1);
                }
                else
                {
                    Cvalue = "#" + Colour;
                }

                /*Set Weight*/
                if (String.IsNullOrEmpty(Weight))
                {
                    Wt = Convert.ToString(-1);

                }
                else
                {
                    Wt = Weight;
                }

                /*Set Size*/
                if (String.IsNullOrEmpty(Size))
                {
                    sz = Convert.ToString(-1);
                }
                else
                {
                    sz = Size;
                }

                /*Set Brand*/
                if (String.IsNullOrEmpty(Brand))
                {
                    Br = Convert.ToString(-1);
                }
                else
                {
                    Br = Brand;
                }
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string user_name = Session["username"].ToString();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == user_name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == user_name).Count();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                ViewBag.product = db.tblCategories.ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).ToList();
                ViewBag.newproducts = db.Database.SqlQuery<sp_newproducts>("exec sp_newproducts").ToList();

                var parameter1 = new List<object>();
                var param1 = new SqlParameter("@keywords", keywords);
                parameter1.Add(param1);
                var param2 = new SqlParameter("@Catwords", Catwords);
                parameter1.Add(param2);
                var param3 = new SqlParameter("@min", min);
                parameter1.Add(param3);
                var param4 = new SqlParameter("@max", max);
                parameter1.Add(param4);
                var param5 = new SqlParameter("@Cvalue", Cvalue);
                parameter1.Add(param5);
                var param6 = new SqlParameter("@Wt", Wt);
                parameter1.Add(param6);
                var param7 = new SqlParameter("@sz", sz);
                parameter1.Add(param7);
                var param8 = new SqlParameter("@Br", Br);
                parameter1.Add(param8);
                var param9 = new SqlParameter("@PageNo", Page);
                parameter1.Add(param9);
                var param10 = new SqlParameter("@RowsPerPage", RowsPerPage);
                parameter1.Add(param10);

                ViewBag.list = db.Database.SqlQuery<sp_search_products>("exec sp_search_products @keywords,@Catwords,@min,@max,@Cvalue,@Wt,@sz,@Br,@PageNo,@RowsPerPage",
               new SqlParameter("@keywords", keywords),
               new SqlParameter("@Catwords", Catwords),
               new SqlParameter("@min", min),
               new SqlParameter("@max", max),
               new SqlParameter("@Cvalue", Cvalue),
               new SqlParameter("@Wt", Wt),
               new SqlParameter("@sz", sz),
               new SqlParameter("@Br", Br),
               new SqlParameter("@PageNo", Page),
               new SqlParameter("@RowsPerPage", RowsPerPage)
               ).ToList();

                ViewBag.listCount = db.Database.SqlQuery<sp_search_productspaging>("exec sp_search_productspaging @keywords,@Catwords,@min,@max,@Cvalue,@Wt,@sz,@Br",
                new SqlParameter("@keywords", keywords),
                new SqlParameter("@Catwords", Catwords),
                new SqlParameter("@min", min),
                new SqlParameter("@max", max),
                new SqlParameter("@Cvalue", Cvalue),
                new SqlParameter("@Wt", Wt),
                new SqlParameter("@sz", sz),
                new SqlParameter("@Br", Br)
                 ).Count();

            }
            catch { }
            return View();
        }

        [HttpGet]
        public ActionResult GuestAccoutnt(string from, string name)
        {
            ViewBag.from = from;
            ViewBag.uname = name;
            return View();
        }

        [HttpPost]
        public ActionResult GuestAccoutnt(FormCollection collect)
        {
            try
            {
                string email = collect["log_Email"];
                int RegCount = db.tblRegistration.Where(i => i.RegEmail == email).Count();
                if (RegCount > 0)
                {
                    ViewBag.Message = "Email address you have given is already associated with traderkerala, Please verify";
                    return Redirect("/Public/GuestAccoutnt?Msg=" + ViewBag.Message);

                }
                string from = collect["from"];
                string uname = collect["uname"];
                tblRegistration reg = new tblRegistration();
                reg.RegName = collect["name"];
                reg.RegEmail = collect["log_Email"];
                reg.RegMobile = collect["Mobile"];
                reg.type = "Guest";
                reg.status = "true";
                db.tblRegistration.Add(reg);
                int x = db.SaveChanges();
                if (x > 0)
                {
                    Session["username"] = collect["log_Email"];
                    //tblLogin log = new tblLogin();
                    //log.username = collect["log_Email"];
                    //log.lastlogin = Convert.ToDateTime(DateTime.Now);
                    //log.status = Convert.ToString("true");
                    //log.type = Convert.ToString("Guest");
                    //db.tblLogins.Add(log);
                    //db.SaveChanges();
                    ViewBag.Message = "Your Guest Account Registration Completed ";
                    if (from == "cart")
                    {
                        List<tbl_cart> TblCart = db.tbl_cart.Where(i => i.user_id == uname).ToList();

                        foreach (TradeKerala2017.Models.tbl_cart ct in TblCart)
                        {
                            tbl_cart tblcrt = db.tbl_cart.Where(o => o.id == ct.id).FirstOrDefault();

                            ct.user_id = email;
                            db.SaveChanges();
                        }

                        ViewBag.cart = db.tbl_cart.Where(i => i.user_id == uname).ToList();
                        return Redirect("~/public/cart");
                    }
                    else
                    {
                        return Redirect("~/public/Newindex");
                    }

                }


            }
            catch
            {
                ViewBag.Message = "Your Guest Account Registration Not Completed ";
            }
            return Redirect("/Public/GuestAccoutnt?Msg=" + ViewBag.Message);
        }


        [HttpPost]
        public ActionResult searchdetails(FormCollection c)
        {
            string keyword = c["txt_search"];
            string Catword = c["Hidden_name"];
            string[] chars = new string[] { ",", ".", "!", "@", "#", "$", "%", "^", "&", "*", "'", ";", "_", "(", ")", ":", "[", "]" };
            for (int q = 0; q < chars.Length; q++)
            {
                if (keyword.Contains(chars[q]))
                {
                    keyword = keyword.Replace(chars[q], "");
                }
            }

            try
            {
                if (Catword == "" && keyword == "")
                {
                    return Redirect("~/Public/NewIndex");
                }
            }
            catch { }
            return Redirect("~/Public/products?keyword=" + keyword + "&Catword=" + Catword);
        }

        [HttpPost]
        public ActionResult DisplayCatList()
        {
            ViewBag.catlist = db.tblCategories.ToList();

            return PartialView("~/Views/Shared/_PublicLayout.cshtml", ViewBag.catlist);
        }
        public ActionResult FeedBack()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
            }
            catch { }
            return View();
        }
        [HttpPost]
        public ActionResult FeedBack(FormCollection collect)
        {
            try
            {
                tblNewsletter nws = new tblNewsletter();
                nws.name = collect["Name"];
                nws.Fromemail = collect["Email"];
                nws.subject = collect["Message"];
                db.tblNewsletters.Add(nws);
                db.SaveChanges();
                string from = collect["Email"];
                string email = "subair301@gmail.com,shafnashaan.azi@gmail.com";
                string subject = "";
                string b = collect["Message"];
                subject = "Feedback Of TraderKerala";
                string body = "<p>Hi User,</p><p>Dear customer.<b>" + b + "</b> from :" + from + "</b></p><p>Thanks and regards,<br>Traderkerala</p>";
                FeedBackMail(body, email, subject);
                ViewBag.Message = "Your Feed Back Is Succesfully SubMitted";
            }
            catch
            {

            }
            return Redirect("~/Public/FeedBack?msg=" + ViewBag.Message);
        }



        public void FeedBackMail(string Body, string To, string Subject)
        {
            MailMessage MyMailMessage = new MailMessage();

            MyMailMessage.From = new MailAddress("customer@fabstudioz.com");
            MyMailMessage.To.Add(To);
            MyMailMessage.Subject = Subject;
            MyMailMessage.Body = Body;
            MyMailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure |
                        DeliveryNotificationOptions.OnSuccess |
                        DeliveryNotificationOptions.Delay;
            MyMailMessage.IsBodyHtml = true;
            SmtpClient SMTPServer = new SmtpClient("mail.fabstudioz.com");
            SMTPServer.Port = 587;
            SMTPServer.Credentials = new System.Net.NetworkCredential("customer@fabstudioz.com", "F@bweb2016");
            SMTPServer.EnableSsl = false;


            try
            {

                SMTPServer.Send(MyMailMessage);
            }

            catch (Exception)
            {
            }
        }

        [HttpGet]
        public ActionResult CustomerServices()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
            }
            catch { }
            return View();
        }

        [HttpGet]
        public ActionResult UserAccount()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;
                }
                name = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=UserAccount&name=" + name);
                }
                else
                {
                    int guest = db.tblLogins.Where(i => i.username == name).Count();
                    if (guest < 1)
                    {
                        ViewBag.Message = "Your are now logged in as Guest account,Have no authorised acess to specific content";
                        return Redirect("~/Public/login?from=UserAccount&name=" + name + "&msg=" + ViewBag.Message);
                    }
                    else
                    {
                        ViewBag.RegDetail = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault();
                        return View();
                    }
                }


            }

            catch
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult UserAccount(FormCollection collect)
        {
            try
            {
                name = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                ViewBag.RegDetail = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault();

                tblRegistration reg = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault();
                reg.RegName = collect["name"];
                reg.RegMobile = collect["Mobile"];
                db.SaveChanges();

            }
            catch { }

            return View();
        }


        [HttpGet]
        public ActionResult EditMail(string regmail, string from)
        {
            try
            {

                string name = regmail;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                ViewBag.RegDetail = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
                ViewBag.Confirm = 0;
                ViewBag.from = from;
            }
            catch
            {

            }
            return View();
        }

        [HttpPost]
        public ActionResult EditMail(FormCollection c)
        {
            string Username = c["Uname"];
            string Password = c["Pword"];
            string from = c["from"];
            try
            {
                string name = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
                ViewBag.RegDetail = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault();

                int Count = db.tblLogins.Where(i => i.username == Username && i.password == Password).Count();
                if (Count > 0)
                {
                    if (from == "mail")
                    {
                        ViewBag.Confirm = 1;
                    }
                    else if (from == "pass")
                    {
                        ViewBag.Confirm = 2;
                    }
                }
                else
                {
                    ViewBag.Confirm = 0;
                    ViewBag.Message = "Somthing gone Wrong Please Verify";
                    return Redirect("~/public/EditMail?msg=" + ViewBag.Message + "&from=" + from);
                }
            }
            catch
            {
            }
            return View();
        }

        [HttpPost]
        public ActionResult Addemail(FormCollection c)
        {
            string uname = c["NewUname"];

            string mail = Session["username"].ToString();
            try
            {
                int RegCount = db.tblRegistration.Where(i => i.RegEmail == uname).Count();
                if (RegCount > 0)
                {
                    ViewBag.Message = "Email address you have given is already associated with traderkerala, Please verify";
                    return Redirect("/Public/UserAccount?Msg=" + ViewBag.Message);

                }

                int Count = db.tblLogins.Where(i => i.username == mail).Count();

                if (Count > 0)
                {
                    Session["username"] = uname;
                    tblLogin log = db.tblLogins.Where(i => i.username == mail).FirstOrDefault();
                    log.username = uname;
                    log.lastlogin = Convert.ToDateTime(DateTime.Now);
                    log.status = Convert.ToString("False");
                    log.type = Convert.ToString("user");
                    int k = db.SaveChanges();
                    if (k > 0)
                    {
                        tblRegistration reg = db.tblRegistration.Where(i => i.RegEmail == mail).FirstOrDefault();
                        reg.RegEmail = uname;
                        db.SaveChanges();
                        foreach (TradeKerala2017.Models.tblRecentView rv in db.tblRecentView.Where(i => i.Username == mail).ToList())
                        {
                            rv.Username = uname;
                            db.SaveChanges();
                        }
                        foreach (TradeKerala2017.Models.tblOrders od in db.tblOrders.Where(i => i.Userid == mail).ToList())
                        {
                            od.Userid = uname;
                            db.SaveChanges();
                        }
                        foreach (TradeKerala2017.Models.tbl_wishlist ws in db.tbl_wishlist.Where(i => i.user_id == mail).ToList())
                        {
                            ws.user_id = uname;
                            db.SaveChanges();
                        }
                        foreach (TradeKerala2017.Models.tbl_Notfication nt in db.tbl_Notfication.Where(i => i.Username == mail).ToList())
                        {
                            nt.Username = uname;
                            db.SaveChanges();
                        }
                        foreach (TradeKerala2017.Models.tbl_cart ct in db.tbl_cart.Where(i => i.user_id == mail).ToList())
                        {
                            ct.user_id = uname;
                            db.SaveChanges();
                        }
                        foreach (TradeKerala2017.Models.tb_address ct in db.tb_address.Where(i => i.user_Id == mail).ToList())
                        {
                            ct.user_Id = uname;
                            db.SaveChanges();
                        }


                        string email = uname;
                        string subject = "";
                        string body = "";
                        subject = "Your Email Confirmation for Traderkerala.com";
                        body = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi user</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'>Greetings from Traderkerala.com Per your request, we have successfully changed your email.Should you need to contact us for any reason, please know that we can give out order information only to the name and e - mail address associated with your account.Thank you again for shopping with us. Please click the following button to complete your Registration</td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 25px 0 0 0;' class='padding-copy'><table border = '0' cellspacing='0' cellpadding='0' class='responsive-table'><tr><td align = 'center' ><a href='http://traderkerala.com/Public/ConfirmMail?mail=" + email + "' style='padding: 10px 16px;text-decoration: none;font-size: 18px;line-height: 1.33;border-radius: 5px;color: #fff;background-color: #428bca;border-color: #357ebd;'> Confirm Registration</a></td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";

                        SendMail(body, email, subject);
                        ViewBag.Message = "Successfully Updated the Email Address,Please Confirm Your mail Address and Enjoy shopping";
                        return Redirect("~/public/UserAccount?msg=" + ViewBag.Message);

                    }
                }
                else
                {
                    ViewBag.Message = "Somthing gone Wrong Please Verify";
                }
            }
            catch
            {
                ViewBag.Message = "Somthing gone Wrong Please Verify";
            }
            return Redirect("~/public/UserAccount?msg=" + ViewBag.Message);

        }


        [HttpPost]
        public ActionResult AddPword(FormCollection collect)
        {
            string Username = Session["username"].ToString();
            string Password = collect["npass"];
            string confrmPword = collect["cpass"];
            try
            {
                if (Password == confrmPword)
                {
                    int Count = db.tblLogins.Where(i => i.username == Username).Count();

                    if (Count > 0)
                    {
                        tblLogin log = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
                        log.password = Password;

                        log.lastlogin = Convert.ToDateTime(DateTime.Now);
                        log.status = Convert.ToString("true");
                        log.type = Convert.ToString("user");
                        int x = db.SaveChanges();
                        if (x > 0)
                        {
                            tblRegistration reg = db.tblRegistration.Where(i => i.RegEmail == Username).FirstOrDefault();
                            reg.RegPassword = Password;
                            db.SaveChanges();
                            string email = Username;
                            string subject = "";
                            string body = "";
                            subject = "Your Password Confirmation for Traderkerala.com";
                            body = "<p>Hi User,</p><p>Greetings from Traderkerala.com Per your request, we have successfully changed your Password.</p><p>Should you need to contact us for any reason, please know that we can give out order information only to the name and e - mail address associated with your account.Thank you again for shopping with us.</p><p><br>Traderkerala</p>";
                            SendMail(body, email, subject);
                            ViewBag.Message = "PassWord Successfully Updated";
                            return Redirect("~/public/UserAccount?msg=" + ViewBag.Message);
                        }
                        else
                        {
                            ViewBag.Message = "Miss matching In password.Please Retype your Password";
                        }

                    }
                }
                else
                {
                    ViewBag.Message = "Miss matching In password.Please Retype your Password";
                }
            }
            catch
            {
                ViewBag.Message = "Miss matching In password.Please Retype your Password";
            }
            return Redirect("~/public/EditMail?msg=" + ViewBag.Message + "&from=pass");
        }

        [HttpGet]
        public ActionResult UserAddress(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                ViewBag.addcode = 0;

            }
            else
            {
                ViewBag.addcode = 1;
                int ID = Convert.ToInt16(id);
                ViewBag.editcode = db.tb_address.Where(i => i.ID == ID).FirstOrDefault();
            }

            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;
                }
                name = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == name).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == name).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == name).ToList();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == name).ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=UserAddress&name=" + name);
                }
                else
                {
                    ViewBag.cartlist = db.tbl_cart.Where(i => i.user_id == name).ToList();
                    ViewBag.address = db.tb_address.Where(i => i.user_Id == name).ToList();
                    ViewBag.addcount = db.tb_address.Where(i => i.user_Id == name).Count();
                    ViewBag.data = db.tblProduct.Where(i => i.Hide != "Hided").Distinct();
                    ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
                    return View();
                }
            }

            catch
            {

            }
            return View();

        }
        [HttpPost]
        public ActionResult UserAddress(FormCollection collect)
        {
            try
            {
                string user_name = Session["username"].ToString();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == user_name).FirstOrDefault().RegName;
                }
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == user_name).Count();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).ToList();
                string add1 = collect["Address1"];
                string add2 = collect["Address2"];
                string add3 = collect["Address3"];
                string city = collect["city"];
                string address = add1 + " " + " " + add2 + "," + " " + add3;
                tb_address add = new tb_address();
                add.user_Id = user_name;
                add.fullname = collect["Full_name"];
                add.Phone = Convert.ToInt64(collect["Mobile_Number"]);
                add.Address = address;
                add.pincode = Convert.ToInt64(collect["Pincode"]);
                add.state = collect["state"];
                add.city = city;
                db.tb_address.Add(add);
                db.SaveChanges();
                ViewBag.address = db.tb_address.Where(i => i.user_Id == user_name);
            }
            catch
            {

            }
            return Redirect("~/Public/UserAddress");
        }
        public ActionResult Deleteadd(int id)
        {

            try
            {
                string user_name = Session["username"].ToString();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                tb_address add = db.tb_address.Where(i => i.ID == id).FirstOrDefault();
                db.tb_address.Remove(add);
                db.SaveChanges();
            }
            catch { }
            return Redirect("~/Public/UserAddress");
        }
        [HttpPost]
        public ActionResult Editadd(FormCollection collect)
        {
            try
            {
                string user_name = Session["username"].ToString();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == user_name).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == user_name).FirstOrDefault().RegName;
                }
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == user_name).Count();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == user_name).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == user_name).ToList();

                int id = Convert.ToInt16(collect["Id"]);
                tb_address add = db.tb_address.Where(i => i.ID == id).FirstOrDefault();
                add.user_Id = user_name;
                add.fullname = collect["Full_name"];
                add.Phone = Convert.ToInt64(collect["Mobile_Number"]);
                add.Address = collect["Address"];
                add.city = collect["city"];
                add.pincode = Convert.ToInt64(collect["Pincode"]);
                add.state = collect["state"];
                db.SaveChanges();
                ViewBag.address = db.tb_address.Where(i => i.user_Id == user_name);
            }
            catch
            {

            }
            return Redirect("~/Public/UserAddress");
        }

        [HttpGet]

        public ActionResult returnpolicy()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();

            }
            catch
            {

            }
            return View();
        }

        [HttpGet]

        public ActionResult privacypolicy()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();

            }
            catch
            {

            }
            return View();
        }



        public ActionResult PaymentandShipping()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
            }
            catch
            {

            }
            return View();
        }



        public ActionResult brands()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();

            }
            catch
            {

            }
            return View();
        }




        public ActionResult SellerReg()
        {
            try
            {

                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();

            }
            catch { }
            return View();
        }
        [HttpPost]
        public ActionResult SellerReg(FormCollection collect, HttpPostedFileBase[] files)
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;
                }

                if (files != null)
                {
                    string vname = collect["Full_name"];
                    string email = collect["E-mail"];

                    tbl_Vendor vd = new tbl_Vendor();
                    vd.Vendor_Name = collect["Full_name"];
                    vd.Address = collect["Address"];
                    vd.Email = collect["E-mail"];
                    vd.Mobile = Convert.ToInt64(collect["Mobile_Number"]);
                    vd.LandLine = Convert.ToInt64(collect["LandLine"]);
                    vd.Pro_Info = collect["Pro_Info"];
                    vd.Pincode = collect["Pincode"];
                    vd.state = collect["State"];
                    vd.Status = "Online";
                    db.tbl_Vendor.Add(vd);
                    db.SaveChanges();


                    string subject = "";
                    string body = "";
                    subject = "Your selling request for Traderkerala.com";
                    body = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi  User</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'>Dear customer, Your Request for selling at Traderkerala.com account is successfully submitted. Our Team will contact you soon</td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 25px 0 0 0;' class='padding-copy'><table border = '0' cellspacing='0' cellpadding='0' class='responsive-table'><tr><td align = 'center' ></td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";
                    SendMail(body, email, subject);

                    string ImagePath = Server.MapPath("~/images/Product/");
                    int k = 1;
                    foreach (HttpPostedFileBase file in files)
                    {
                        //Checking file is available to save.  
                        if (file != null)
                        {
                            string ext = Path.GetExtension(file.FileName).ToLower();
                            string name1 = vname + "-" + k;
                            string url = ImagePath + name1 + ext;

                            if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                            {


                                file.SaveAs(url);
                                string ImageUrl = "/images/Product/" + name1 + ext;
                                tbl_SellerImage tbImg = new tbl_SellerImage();
                                tbImg.Image = ImageUrl;
                                tbImg.Vendor_id = vd.Vendor_id;
                                db.tbl_SellerImage.Add(tbImg);
                                db.SaveChanges();

                            }
                            else
                            {
                                ViewBag.Message = "Invalid Image Extention!!!";
                            }

                            k = k + 1;

                        }

                    }
                    ViewBag.Message = "Vendor Details are submitted succesfully,Our Team will contact you soon";
                }
                else
                {
                    tbl_Vendor vd = new tbl_Vendor();
                    vd.Vendor_Name = collect["Full_name"];
                    vd.Address = collect["Address"];
                    vd.Email = collect["E-mail"];
                    vd.Mobile = Convert.ToInt64(collect["Mobile_Number"]);
                    vd.LandLine = Convert.ToInt64(collect["LandLine"]);
                    vd.Pro_Info = collect["Pro_Info"];
                    vd.Pincode = collect["Pincode"];
                    vd.state = collect["State"];
                    vd.Status = "Online";
                    db.tbl_Vendor.Add(vd);
                    db.SaveChanges();
                    ViewBag.Message = "Vendor Details are submitted succesfully,Our Team will contact you soon";
                    string subject1 = "";
                    string body1 = "";
                    string email1 = collect["E-mail"];
                    subject1 = "Your selling request for Traderkerala.com";
                    body1 = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi  User</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'>Dear customer, Your Request for selling at Traderkerala.com account is successfully submitted. Our Team will contact you soon</td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 25px 0 0 0;' class='padding-copy'><table border = '0' cellspacing='0' cellpadding='0' class='responsive-table'><tr><td align = 'center' ></td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";
                    SendMail(body1, email1, subject1);
                }

                string email2 = collect["E-mail"];
                string subject2 = "";
                string body2 = "";
                subject2 = "Your selling request for Traderkerala.com is under Processing";
                body2 = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi  User</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'>Dear customer, Your Request for selling at Traderkerala.com account is successfully submitted.Your Request Under Processing and You will get mail when processing is copmpleted</td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 25px 0 0 0;' class='padding-copy'><table border = '0' cellspacing='0' cellpadding='0' class='responsive-table'><tr><td align = 'center' ></td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";
                SendMail(body2, email2, subject2);

                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
            }
            catch
            {

                ViewBag.Message = "Vendor Details are not submitted,please verify and try again";
            }

            return Redirect("~/Public/SellerReg?msg=" + ViewBag.Message);
        }

        [HttpGet]
        public ActionResult BrowsingHistory()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();

                int w = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (w > 0)
                {
                    ViewBag.recentlog = 1;
                    int recent = db.tblRecentView.Where(i => i.Username == log_uname).Count();
                    if (recent > 0)
                    {
                        ViewBag.Myrecentbag = db.tblRecentView.Where(i => i.Username == log_uname).ToList();
                        ViewBag.recentCount = 1;
                    }
                    else
                    {
                        ViewBag.recentCount = 0;
                    }
                    ViewBag.recent = db.tblRecentView.Take(12).ToList();
                }
                else
                {
                    ViewBag.recentlog = 0;
                    HttpCookie myRecent = Request.Cookies["MyRecentCookie"];
                    if (myRecent != null)
                    {
                        ViewBag.Myrecentbag = myRecent.Value;
                        ViewBag.recentCount = 1;
                    }
                    else
                    {
                        ViewBag.recentCount = 0;
                    }

                }





            }
            catch
            {
            }
            return View();
        }

        public ActionResult ClearHistory()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;
                }
                string log_uname = Session["username"].ToString();
                int w = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (w > 0)
                {
                    int recent = db.tblRecentView.Where(i => i.Username == log_uname).Count();
                    if (recent > 0)
                    {
                        foreach (TradeKerala2017.Models.tblRecentView rv in db.tblRecentView.Where(i => i.Username == log_uname).ToList())
                        {
                            db.tblRecentView.Remove(rv);
                            db.SaveChanges();
                        }
                        ViewBag.Message = "Browsing History Cleared successfully";

                    }
                    else
                    {
                        ViewBag.Message = "Browsing History Cleared successfully";
                    }

                }
                else
                {
                    if (Request.Cookies["MyRecentCookie"] != null)
                    {
                        Response.Cookies["MyRecentCookie"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["MyTestCookie"].Expires = DateTime.Now.AddDays(-1);
                        ViewBag.Message = "Browsing History Cleared successfully";
                    }
                }
            }
            catch { }
            return Redirect("~/Public/BrowsingHistory?msg=" + ViewBag.Message);
        }




        public ActionResult SpicesDemo()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
            }
            catch
            {

            }

            return View();
        }


        [HttpGet]
        public ActionResult Prebooking()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.name = log_uname;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.prebook = db.tbl_PreBooking.Where(i => i.Email == log_uname && i.Status == "OnProgress").ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=Prebooking&name=" + log_uname);
                }
            }
            catch { }
            return View();
        }

        [HttpPost]
        public ActionResult Prebooking(FormCollection collect, HttpPostedFileBase file1)
        {
            try
            {

                string contact = collect["contact"];
                string[] chars = new string[] { ",", ".", "!", "@", "#", "$", "%", "^", "&", "*", "'", ";", "_", "(", ")", ":", "[", "]", " " };
                for (int q = 0; q < chars.Length; q++)
                {
                    if (contact.Contains(chars[q]))
                    {
                        contact = contact.Replace(chars[q], "");
                    }
                }
                string proImage = "";
                string ImageName = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string ImagePath = Server.MapPath("~/images/Product/");
                if (file1 != null)
                {
                    string ext = Path.GetExtension(file1.FileName).ToLower();

                    if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                    {
                        file1.SaveAs(ImagePath + ImageName + ext);
                        proImage = "/images/Product/" + ImageName + ext;
                    }
                    tbl_PreBooking pre = new tbl_PreBooking();
                    pre.Name = collect["Full_name"];
                    pre.Address = collect["Address"];
                    pre.Contact = Convert.ToInt64(contact);
                    pre.Email = collect["E-mail"];
                    pre.Pro_Enquiry = collect["Pro_Info"];
                    pre.Quantity = collect["Quant"];
                    pre.Pro_Image = proImage;
                    pre.Delivary = collect["deldate"];
                    pre.Status = "OnProgress";
                    db.tbl_PreBooking.Add(pre);
                    db.SaveChanges();
                    ViewBag.Message = "Pre Booking Details are submitted succesfully,Our Team will contact you soon";
                }
                else
                {
                    tbl_PreBooking pre = new tbl_PreBooking();
                    pre.Name = collect["Full_name"];
                    pre.Address = collect["Address"];
                    pre.Contact = Convert.ToInt64(contact);
                    pre.Email = collect["E-mail"];
                    pre.Pro_Enquiry = collect["Pro_Info"];
                    pre.Quantity = collect["Quant"];
                    pre.Delivary = collect["deldate"];
                    pre.Status = "OnProgress";
                    db.tbl_PreBooking.Add(pre);
                    db.SaveChanges();
                    ViewBag.Message = "Pre Booking Details are submitted succesfully,Our Team will contact you soon";
                }
            }
            catch
            {
                ViewBag.Message = "Pre Booking Details are not submitted,please verify and try again";
            }

            return Redirect("~/Public/Prebooking?msg=" + ViewBag.Message);
        }

        public ActionResult Prebookdelete(int id)
        {
            try
            {
                tbl_PreBooking tb = db.tbl_PreBooking.Where(i => i.Pre_Id == id).FirstOrDefault();
                db.tbl_PreBooking.Remove(tb);
                db.SaveChanges();
                ViewBag.Message = "Booking Details are Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Booking Details are is Not Deleted!!";
            }

            return Redirect("~/Public/Prebooking?Msg=" + ViewBag.Message);
        }
        public ActionResult Career()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.name = log_uname;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.prebook = db.tbl_PreBooking.Where(i => i.Email == log_uname && i.Status == "OnProgress").ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
                DateTime current = DateTime.Now;
                int cd = db.tbl_Job.Count();
                if (cd > 0)
                {
                    foreach (TradeKerala2017.Models.tbl_Job tb in db.tbl_Job.ToList())
                    {
                        System.DateTime dtpurchase = Convert.ToDateTime(tb.End_Date);
                        System.DateTime dtcurrent = Convert.ToDateTime(current);
                        System.TimeSpan diffResult = dtpurchase.Subtract(dtcurrent);
                        if (diffResult.Days > 0)
                        {
                            if (careerid == null)
                            {
                                careerid = Convert.ToString(tb.Job_Id);
                            }
                            else
                            {
                                careerid = careerid + "~" + Convert.ToString(tb.Job_Id);
                            }
                        }
                        if (careerid == null)
                        {
                            ViewBag.careerCount = 0;
                            ViewBag.career = careerid;
                        }
                        else
                        {
                            ViewBag.careerCount = 1;
                            ViewBag.career = careerid;
                        }
                    }
                }
                else
                {
                    ViewBag.careerCount = 0;
                }
            }
            catch { }
            return View();
        }

        [HttpGet]
        public ActionResult Jobseeker(string id)
        {
            try
            {
                int Id = Convert.ToInt32(id);
                if (String.IsNullOrEmpty(id))
                {
                    ViewBag.Count = 0;
                }
                else
                {
                    ViewBag.Count = 1;
                    ViewBag.Career = db.tbl_Job.Where(i => i.Job_Id == Id).FirstOrDefault();
                }
                if (Session["username"] == null)
                {
                    Session["username"] = UID;
                }
                string log_uname = Session["username"].ToString();
                ViewBag.name = log_uname;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
            }
            catch
            {
            }
            return View();
        }

        [HttpPost]
        public ActionResult Jobseeker(FormCollection collect, HttpPostedFileBase file1)
        {
            try
            {
                string resume = "";
                string rname = collect["Full_name"];
                string resumeName = rname + DateTime.Now.Second.ToString();
                string resumePath = Server.MapPath("~/images/Product/");
                if (file1 != null)
                {
                    string ext = Path.GetExtension(file1.FileName).ToLower();

                    if (ext == ".pdf" || ext == ".docx")
                    {
                        file1.SaveAs(resumePath + resumeName + ext);
                        resume = "/images/Product/" + resumeName + ext;
                    }
                    tbl_JobSeeker sk = new tbl_JobSeeker();
                    sk.Seerker_Name = collect["Full_name"];
                    sk.Address = collect["Address"];
                    sk.Job_Applied = collect["Category"];
                    sk.Contact = collect["contact"];
                    sk.Email = collect["E-mail"];
                    sk.Qualification = collect["Qualifi"];
                    sk.Resume = resume;
                    db.tbl_JobSeeker.Add(sk);
                    db.SaveChanges();

                    ViewBag.Message = "Resume Submitted Successfully,our team will contact you Once your details are matching with our requirments";
                }
                else
                {
                    ViewBag.Message = "Something gone wrong,Please verify";
                }

            }
            catch
            {
                ViewBag.Message = "Something gone wrong,Please verify";
            }
            return Redirect("~/Public/Jobseeker?msg=" + ViewBag.Message);
        }



        public ActionResult ClearNotification()
        {

            string log_uname = Session["username"].ToString();

            foreach (TradeKerala2017.Models.tbl_Notfication ct in db.tbl_Notfication.Where(i => i.Username == log_uname).ToList())
            {
                db.tbl_Notfication.Remove(ct);
                db.SaveChanges();
            }

            return Redirect("~/Public/NewIndex");
        }




        public ActionResult notifyme(int id)
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                name = Session["username"].ToString();
                int log = db.tblRegistration.Where(i => i.RegEmail == name).Count();
                if (log < 1)
                {
                    ViewBag.Message = "Please login with your credential to get notification";
                    return Redirect("/Public/NewProductDetails?id=" + id + "&Msg=" + ViewBag.Message);
                }

                tbl_Notfication noti = new tbl_Notfication();
                noti.Username = name;
                noti.pro_id = id;
                db.tbl_Notfication.Add(noti);
                db.SaveChanges();
            }
            catch { }
            ViewBag.Message = "Notification Alert added succesfully";
            return Redirect("/Public/NewProductDetails?id=" + id + "&Msg=" + ViewBag.Message);
        }

        

        public ActionResult resp()
        {

            return View();
        }

        public ActionResult cancel()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.name = log_uname;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.prebook = db.tbl_PreBooking.Where(i => i.Email == log_uname && i.Status == "OnProgress").ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
                int log = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=Prebooking&name=" + log_uname);
                }
            }
            catch { }
            return View();
        }




        [HttpGet]
        public ActionResult bulkorder()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                ViewBag.name = log_uname;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();
                
            }
            catch { }
            return View();
        }

        [HttpPost]
        public ActionResult bulkorder(FormCollection collect, HttpPostedFileBase file1)
        {
            try
            {

                string contact = collect["contact"];
                string[] chars = new string[] { ",", ".", "!", "@", "#", "$", "%", "^", "&", "*", "'", ";", "_", "(", ")", ":", "[", "]", " " };
                for (int q = 0; q < chars.Length; q++)
                {
                    if (contact.Contains(chars[q]))
                    {
                        contact = contact.Replace(chars[q], "");
                    }
                }
                int pro=Convert.ToInt16(collect["productId"]);
                int qua = Convert.ToInt16(collect["Quant"]);
                tbl_BulkOrder pre = new tbl_BulkOrder();
                    pre.Name = collect["Full_name"];
                    pre.Address = collect["Address"];
                    pre.Contact = collect["contact"];
                    pre.Email = collect["E-mail"];
                    pre.product= collect["productId"];
                    pre.Quantity = collect["Quant"];
                    pre.Delivary = collect["deldate"];
                    tblProduct tb = db.tblProduct.Where(i => i.pid == pro).FirstOrDefault();
                    decimal price= qua *Convert.ToDecimal( tb.ProdNewPrice);
                    pre.price = price.ToString();
                    db.tbl_BulkOrder.Add(pre);
                    db.SaveChanges();
                    ViewBag.Message = "Order Details are submitted succesfully,Our Team will contact you soon";
                
            }
            catch
            {
                ViewBag.Message = "Order Details are not submitted,please verify and try again";
            }

            return Redirect("~/Public/bulkorder?msg=" + ViewBag.Message);
        }


        [HttpGet]
        public ActionResult trackyourorder()
        {
            try
            {
                if (Session["username"] == null)
                {
                    Session["username"] = UID;

                }
                string log_uname = Session["username"].ToString();
                int log = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (log < 1)
                {
                    return Redirect("~/Public/login?from=Prebooking&name=" + log_uname);
                }
                ViewBag.name = log_uname;
                ViewBag.ItemCount = db.tbl_cart.Where(i => i.user_id == log_uname).Count();
                ViewBag.user = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                int ue = db.tblRegistration.Where(i => i.RegEmail == log_uname).Count();
                if (ue > 0)
                {
                    ViewBag.username = db.tblRegistration.Where(i => i.RegEmail == log_uname).FirstOrDefault().RegName;
                }
                ViewBag.catlist = db.tblCategories.Where(i => i.ParentId == 0).ToList();
                ViewBag.cart = db.tbl_cart.Where(i => i.user_id == log_uname).ToList();
                ViewBag.ordercount = db.tblOrders.Where(i => i.Userid == log_uname).Count();
                ViewBag.orderlist = db.tblOrders.Where(i => i.Userid == log_uname).ToList();
                ViewBag.notiCount = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).Count();
                ViewBag.notification = db.tbl_Notfication.Where(i => i.status != null && i.Username == log_uname).ToList();

            }
            catch { }
            return View();
        }





        #region Methods
        [HttpPost]
        public JsonResult KeywordList(string Prefix)
        {

            Prefix = Prefix.ToLower();

            List<KeywordClass> keyList = new List<KeywordClass>();


            foreach (sp_autocomplete_Result tp in db.sp_autocomplete(Prefix).ToList())
            {
                KeywordClass k = new KeywordClass();
                k.label = tp.ProdName;
                k.value = tp.pid.ToString();
                keyList.Add(k);

            }


            return this.Json(keyList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CatKeywordList(string Prefix, string Sufix)

        {

            Prefix = Prefix.ToLower();
            Sufix = Sufix.ToLower();
            List<CatKeywordClass> keyList = new List<CatKeywordClass>();


            foreach (sp_Catautocomplete_Result tp in db.sp_Catautocomplete(Prefix, Sufix).ToList())
            {
                CatKeywordClass k = new CatKeywordClass();

                k.label = tp.ProdName;
                k.brand = tp.Brand;
                k.value = tp.pid.ToString();



                keyList.Add(k);

            }


            return this.Json(keyList, JsonRequestBehavior.AllowGet);
        }


        public string pincheck(string pincode)
        {
            int pin = Convert.ToInt32(pincode);

            int pi = db.tbl_Pincode.Where(i => i.Pincode == pin).Count();

            string s = "";

            if(pi<1)
            {
                s = "Invalid Pin code";
            }
            return s;
        }


        #endregion




    }


    public class sp_newproducts
    {
        public int id { get; set; }
        public int pid { get; set; }
        public string ProdName { get; set; }
        public int Catid { get; set; }

        public decimal ProdNewPrice { get; set; }
        public decimal ProdOldPrice { get; set; }


    }
    public class sp_search_products
    {
        public int pid { get; set; }
        public string ProdName { get; set; }
        public int Catid { get; set; }
        public string CatName { get; set; }
        public int Vendor { get; set; }
        public decimal ProdNewPrice { get; set; }
        public decimal ProdOldPrice { get; set; }
        public decimal Dollar_NewPrice { get; set; }
        public decimal Dollar_OldPrice { get; set; }
        public string ProdTitle { get; set; }
        public string ProdDesc { get; set; }
        public string ProdInfo { get; set; }
        public int Qty { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }
        public string ParCategories { get; set; }
    }

    public class sp_search_productspaging
    {
        public int pid { get; set; }
        public string ProdName { get; set; }
        public int Catid { get; set; }
        public string CatName { get; set; }
        public int Vendor { get; set; }
        public decimal ProdNewPrice { get; set; }
        public decimal ProdOldPrice { get; set; }
        public decimal Dollar_NewPrice { get; set; }
        public decimal Dollar_OldPrice { get; set; }
        public string ProdTitle { get; set; }
        public string ProdDesc { get; set; }
        public string ProdInfo { get; set; }
        public int Qty { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }
        public string ParCategories { get; set; }
    }

    public class KeywordClass
    {
        public String label { get; set; }
        public String value { get; set; }
    }

    public class CatKeywordClass
    {
        public String label { get; set; }
        public string brand { get; set; }
        public String value { get; set; }
    }

    
}