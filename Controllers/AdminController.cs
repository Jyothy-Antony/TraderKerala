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
    public class AdminController : Controller
    {
        DbTradekeralaEntities1 db = new DbTradekeralaEntities1();


        //[HttpPost]
        //public ActionResult Index(FormCollection collect)
        //{


        //foreach (TradeKerala2017.Models.tblProduct tbB in db.tblProduct.ToList())
        //{
        //        string brand = tbB.Brand;
        //        if (string.IsNullOrEmpty(brand))
        //        { }
        //        else
        //        {
        //            int brandid = Convert.ToInt32(brand);
        //            foreach (TradeKerala2017.Models.tblBrand tb in db.tblBrands.Where(i => i.Brandid == brandid).ToList())
        //            {
        //                tbB.Brand = tb.BrandName;
        //                db.SaveChanges();
        //            }
        //        }
        //}


        //    return View();
        //}





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
                    if (type == "Admin")
                    {

                        tblLogin Log = db.tblLogins.Where(i => i.username == Username && i.password == Password).FirstOrDefault();
                        Session["Admin"] = Log.username;
                        Log.lastlogin = DateTime.Now;
                        Log.status = Convert.ToString("true");
                        db.SaveChanges();

                        return Redirect("~/Admin/Index");
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
                string username = Session["Admin"].ToString();
                tblLogin tblLogin = db.tblLogins.Where(i => i.username == username).FirstOrDefault();
                tblLogin.status = Convert.ToString(" false");
                db.SaveChanges();

                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();
            }
            catch { }

            return Redirect("~/Admin/Login");
        }
       
        public ActionResult offerlist()
        {
            return View(db.tbl_specialoffer.ToList());

        }


        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddBanner()
        {
            try
            {
                ViewBag.categories = db.tblCategories.ToList();
                ViewBag.offer = db.tbl_Banner.ToList();

            }
            catch { }
            return View();
        }

        [HttpPost]
        public ActionResult AddBanner(FormCollection collect, HttpPostedFileBase file1)
        {
            try
            {

                int ca = db.tbl_Banner.Count();
                if (ca >= 5)
                {
                    ViewBag.Message = "Sorry You can Only Add 5 Banners; please Verify!!";
                }
                else
                {
                    string Bannername = collect["txtBanner"];
                    string Related_to = collect["TxtRelated"];
                    string BannerImage = "";
                    string ImageName = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    string ImagePath = Server.MapPath("~/images/Banner/");
                    DateTime date = DateTime.Now.Date;

                    if (file1 != null)
                    {
                        string ext = Path.GetExtension(file1.FileName).ToLower();

                        if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                        {
                            file1.SaveAs(ImagePath + ImageName + ext);
                            BannerImage = "/images/Banner/" + ImageName + ext;


                            tbl_Banner tbB = new tbl_Banner();
                            tbB.BannerName = Bannername;
                            tbB.BannerImage = BannerImage;
                            tbB.Status = "show";
                            tbB.Related_To = Related_to;
                            db.tbl_Banner.Add(tbB);
                            db.SaveChanges();
                            ViewBag.Message = "Banner Image Added Successfully!!";

                        }
                        else
                        {
                            ViewBag.Message = "Banner Image Not Added Successfully!!";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Image is not Uploaded";
                    }
                }
            }
            catch { }
            ViewBag.categories = db.tblCategories.ToList();
            ViewBag.offer = db.tbl_Banner.ToList();
            return Redirect("/Admin/AddBanner?Msg=" + ViewBag.Message);

        }
        public ActionResult DeleteBanner(int id, string name)
        {

            try
            {
                tbl_Banner tbC = db.tbl_Banner.Where(i => i.Banner_id == id).FirstOrDefault();
                db.tbl_Banner.Remove(tbC);
                db.SaveChanges();
                ViewBag.Message = "Banner " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Banner " + name + " is Not Deleted Successfully!!";
            }

            return Redirect("/Admin/AddBanner?Msg=" + ViewBag.Message);
        }
        public ActionResult EditBanner(int id)
        {
            try
            {
                ViewBag.EditBanner = db.tbl_Banner.Where(i => i.Banner_id == id).FirstOrDefault();
                ViewBag.categories = db.tblCategories.ToList();

            }
            catch { }
            return View();
        }

        [HttpPost]
        public ActionResult EditBanner(FormCollection collect, HttpPostedFileBase file1)
        {
            int Id = Convert.ToInt32(collect["HiddenId"]);
            string NewImage = "";
            string CurrentBannerImage = collect["HiddenImage"];
            string Bannername = collect["txtBanner"];
            string Related_to = collect["TxtRelated"];
            string ImageName = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

            string ImagePath = Server.MapPath("~/images/Banner/");

            try
            {
                if (file1 != null)
                {
                    string ext = Path.GetExtension(file1.FileName).ToLower();
                    string url = ImagePath + ImageName + ext;

                    if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                    {


                        file1.SaveAs(url);
                        NewImage = "/images/Banner/" + ImageName + ext;
                        tbl_Banner tbB = db.tbl_Banner.Where(o => o.Banner_id == Id).FirstOrDefault();
                        tbB.BannerName = Bannername;
                        tbB.Related_To = Related_to;
                        tbB.BannerImage = NewImage;



                        db.SaveChanges();
                        ViewBag.Message = "Banner " + Bannername + " Is Updated Successfully!!";
                    }
                    else
                    {
                        ViewBag.Message = "Banner Not Updated Successfully!!";
                    }
                }
                else
                {
                    tbl_Banner tbB = db.tbl_Banner.Where(o => o.Banner_id == Id).FirstOrDefault();
                    tbB.BannerName = Bannername;
                    tbB.Related_To = Related_to;
                    tbB.BannerImage = CurrentBannerImage;

                    db.SaveChanges();
                    ViewBag.Message = "Banner " + Bannername + " Is Updated Successfully!!";
                }
            }
            catch
            {
                ViewBag.Message = " Banner" + Bannername + " Is Not Updated Successfully!!";
            }
            return Redirect("/Admin/AddBanner?Msg=" + ViewBag.Message);
        }

        public ActionResult HideBanner(int id)
        {
            try
            {
                tbl_Banner tbB = db.tbl_Banner.Where(o => o.Banner_id == id).FirstOrDefault();
                tbB.Status = "Hided";
                db.SaveChanges();
                ViewBag.Message = "Banner Is Hided Successfully!!";
            }
            catch { }
            return Redirect("/Admin/AddBanner?Msg=" + ViewBag.Message);
        }
        public ActionResult ShowBanner(int id)
        {
            try
            {
                tbl_Banner tbB = db.tbl_Banner.Where(o => o.Banner_id == id).FirstOrDefault();
                tbB.Status = "show";
                db.SaveChanges();
                ViewBag.Message = "Banner Is Un Hided Successfully!!";
            }
            catch { }
            return Redirect("/Admin/AddBanner?Msg=" + ViewBag.Message);
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            try
            {
                ViewBag.CategoryDetails = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
            }
            catch { }
            return View();
        }
        [HttpPost]
        public ActionResult AddCategory(FormCollection collect)
        {
            string Category = collect["txtCategory"];
            DateTime date = DateTime.Now.Date;
            try
            {
                tblCategory tbC = new tblCategory();
                tbC.CatName = Category;
                tbC.Addeddate = DateTime.Now;
                tbC.ParentId = 0;
                db.tblCategories.Add(tbC);
                db.SaveChanges();
                ViewBag.Message = "Category Added Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Category Not Added Successfully!!";
            }

            ViewBag.CategoryDetails = db.tblCategories.Where(i => i.ParentId == 0).OrderByDescending(I => I.Addeddate).ToList();
            return View();
        }

        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            ViewBag.EditCategory = db.tblCategories.Where(i => i.Catid == id).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public ActionResult EditCategory(FormCollection collect)
        {
            string Category = collect["txtCategory"];
            int id = Convert.ToInt32(collect["HiddenId"]);
            DateTime date = DateTime.Now.Date;
            try
            {
                tblCategory tbC = db.tblCategories.Where(i => i.Catid == id).FirstOrDefault();
                tbC.CatName = Category;
                tbC.Addeddate = DateTime.Now;
                tbC.ParentId = 0;
                db.SaveChanges();
                ViewBag.Message = "Category Updated Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Category Not Updated Successfully!!";
            }

            return Redirect("~/Admin/AddCategory?Msg=" + ViewBag.Message);
        }
        public ActionResult DeleteCategory(int id, string name)
        {
            try
            {
                tblCategory tbC = db.tblCategories.Where(i => i.Catid == id).FirstOrDefault();
                db.tblCategories.Remove(tbC);
                db.SaveChanges();
                ViewBag.Message = "Category " + name + " is Deleted Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Category " + name + " is Not Deleted Successfully!!";
            }

            return Redirect("~/Admin/AddCategory?Msg=" + ViewBag.Message);
        }

        public ActionResult RefreshCategory(int id, string name)
        {
            try
            {
                tblCategory tbC = db.tblCategories.Where(i => i.Catid == id).FirstOrDefault();
                tbC.Addeddate = DateTime.Now;
                db.SaveChanges();
                ViewBag.Message = "Category " + name + " is Refreshed Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Category " + name + " is Not Refreshed Successfully!!";
            }

            return Redirect("~/Admin/AddCategory?Msg=" + ViewBag.Message);
        }



        //Adding Subcategory//
        [HttpGet]
        public ActionResult AddSubCategory()
        {
            ViewBag.SubCategoryDetails = db.tblCategories.Where(o => o.ParentId != 0).OrderByDescending(I => I.Catid).ToList();
            ViewBag.CategoryDetails = db.tblCategories.Where(i => i.ParentId == 0).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult AddSubCategory(FormCollection collect)
        {
            int Catid = Convert.ToInt32(collect["CategoryId"]);
            int ParentId = Convert.ToInt32(collect["HiddenParentId"]);
            string Subcat = collect["txtSubCat"];
            try
            {
                tblCategory tbC = new tblCategory();
                tbC.CatName = Subcat;
                tbC.ParentId = ParentId;
                tbC.Addeddate = DateTime.Now;
                db.tblCategories.Add(tbC);
                db.SaveChanges();
                ViewBag.Message = "Sub Category Added Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Sub Category Not Added Successfully!!";
            }
            ViewBag.SubCategoryDetails = db.tblCategories.Where(o => o.ParentId != 0).OrderByDescending(I => I.Catid).ToList();
            ViewBag.CategoryDetails = db.tblCategories.Where(i => i.ParentId == 0).ToList();
            return View();
        }

        [HttpGet]
        public ActionResult EditSubCategory(int id)
        {
            ViewBag.EditSubCategory = db.tblCategories.Where(i => i.Catid == id).FirstOrDefault();
            ViewBag.CategoryDetails = db.tblCategories.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult EditSubCategory(FormCollection collect)
        {
            int Catid = Convert.ToInt32(collect["dCatid"]);
            string Category = collect["dCatid"];
            string Subcat = collect["txtSubCat"];
            int id = Convert.ToInt32(collect["HiddenId"]);
            try
            {
                tblCategory tbl = db.tblCategories.Where(i => i.Catid == id).FirstOrDefault();
                string oldname = tbl.CatName;
                tbl.ParentId = Convert.ToInt16(Category);
                tbl.CatName = Subcat;
                db.SaveChanges();

                foreach (tblProduct tb in db.tblProduct.Where(i => i.CatName == oldname && i.Catid == id).ToList())
                {
                    string ParentIds = "";
                    int cat_id = id;
                    string Parent_Name = db.tblCategories.Where(i => i.Catid == cat_id).FirstOrDefault().CatName;
                    ParentIds = cat_id.ToString() + "|" + Parent_Name;
                    while (cat_id > 0)
                    {
                        int Parent_id = Convert.ToInt16(db.tblCategories.Where(i => i.Catid == cat_id).FirstOrDefault().ParentId);

                        if (Parent_id != 0)
                        {
                            string ParentName = db.tblCategories.Where(i => i.Catid == Parent_id).FirstOrDefault().CatName;
                            ParentIds = ParentIds + "~" + Parent_id + "|" + ParentName;
                        }
                        cat_id = Convert.ToInt16(Parent_id);
                    }

                    tb.Catid = id;
                    tb.CatName = Subcat;
                    tb.ParCategories = ParentIds;
                    db.SaveChanges();
                }
                foreach (tblStock st in db.tblStocks.Where(i => i.Category == oldname && i.CategoryId == id).ToList())
                {
                    st.Category = Subcat;
                    db.SaveChanges();
                }

                    ViewBag.Message = "Sub Category  Updated Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Sub Category Not Updated Successfully!!";
            }

            return Redirect("~/Admin/AddSubCategory?Msg=" + ViewBag.Message);
        }

        public ActionResult DeleteSubCategory(int id, string name)
        {

            try
            {
                tblCategory tbl = db.tblCategories.Where(i => i.Catid == id).FirstOrDefault();
                db.tblCategories.Remove(tbl);
                db.SaveChanges();
                ViewBag.Message = "Sub Category " + name + " is Deleted Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Sub Category " + name + " is Not Deleted Successfully!!";
            }

            return Redirect("~/Admin/AddSubCategory?Msg=" + ViewBag.Message);
        }

        //Adding Brand
        public ActionResult AddBrand()
        {

            ViewBag.BrandDetails = db.tblBrands.OrderByDescending(I => I.Brandid).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult AddBrand(FormCollection collect, HttpPostedFileBase file1)
        {

            string BrandName = collect["txtBrand"];
            string BrandLogo = "";
            string BrandDesc = collect["txtBrandDesc"];
            string ImagePath = Server.MapPath("~/images/Brand/");


            DateTime date = DateTime.Now.Date;
            //try
            //{
            if (file1 != null)
            {
                string ext = Path.GetExtension(file1.FileName).ToLower();

                if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                {
                    file1.SaveAs(ImagePath + BrandName + ext);
                    BrandLogo = "/images/Brand/" + BrandName + ext;

                    tblBrand tbB = new tblBrand();
                    tbB.BrandName = BrandName;
                    tbB.BrandLogo = BrandLogo;
                    tbB.BrandDesc = BrandDesc;
                    db.tblBrands.Add(tbB);
                    db.SaveChanges();
                    ViewBag.Message = "Product Brand Added Successfully!!";

                }
                else
                {
                    ViewBag.Message = "Product Brand Not Added Successfully!!";
                }
            }
            else
            {
                ViewBag.Message = "Image is not Uploaded";
            }
            //}
            //catch
            //{
            //    ViewBag.Message = "Product Brand Not Added Successfully!!";
            //}

            ViewBag.BrandDetails = db.tblBrands.OrderByDescending(I => I.Brandid).ToList();


            return View();
        }

        public ActionResult EditBrand(int id)
        {
            ViewBag.EditBrand = db.tblBrands.Where(i => i.Brandid == id).FirstOrDefault();

            return View();
        }
        [HttpPost]
        public ActionResult EditBrand(FormCollection collect, HttpPostedFileBase file1)
        {
            int id = Convert.ToInt32(collect["HiddenId"]);
            string BrandName = collect["txtBrand"];
            string BrandLogo = "";
            string CurrentBrandLogo = collect["HiddenImage"];
            string BrandDesc = collect["txtBrandDesc"];
            string ImagePath = Server.MapPath("~/images/Brand/");

            try
            {



                if (file1 != null)
                {
                    string ext = Path.GetExtension(file1.FileName).ToLower();
                    string url = ImagePath + BrandName + ext;

                    if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                    {


                        file1.SaveAs(url);
                        BrandLogo = "/images/Brand/" + BrandName + ext;
                        tblBrand tbB = db.tblBrands.Where(o => o.Brandid == id).FirstOrDefault();
                        tbB.BrandName = BrandName;
                        tbB.BrandLogo = BrandLogo;
                        tbB.BrandDesc = BrandDesc;

                        db.SaveChanges();
                        ViewBag.Message = "Brand " + BrandName + " Is Updated Successfully!!";

                    }
                    else
                    {
                        ViewBag.Message = "Product Brand Not Updated Successfully!!";
                    }
                }
                else
                {
                    BrandLogo = CurrentBrandLogo;

                    tblBrand tbB = db.tblBrands.Where(o => o.Brandid == id).FirstOrDefault();
                    tbB.BrandName = BrandName;
                    tbB.BrandLogo = BrandLogo;
                    tbB.BrandDesc = BrandDesc;

                    db.SaveChanges();
                    ViewBag.Message = "Brand " + BrandName + " Is Updated Successfully!!";

                }


            }
            catch
            {
                ViewBag.Message = "Brand " + BrandName + " Is Not Updated Successfully!!";
            }

            return Redirect("~/Admin/AddBrand?Msg=" + ViewBag.Message);
        }

        public ActionResult DeleteBrand(int id, string name)
        {

            try
            {
                tblBrand tbB = db.tblBrands.Where(o => o.Brandid == id).FirstOrDefault();
                db.tblBrands.Remove(tbB);
                db.SaveChanges();
                ViewBag.Message = "Brand " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Brand " + name + " is Not Deleted Successfully!!";
            }

            return Redirect("~/Admin/AddBrand?Msg=" + ViewBag.Message);
        }
        [HttpGet]
        public ActionResult vendors()
        {
            ViewBag.vendorslist = db.tbl_Vendor.OrderByDescending(I => I.Vendor_id).Where(i => i.Status == "Direct").ToList();
            return View();
        }
        [HttpPost]
        public ActionResult vendors(FormCollection collect)
        {

            try
            {
                string email = collect["Email"];

                int c = db.tbl_Vendor.Where(i => i.Email == email).Count();

                if (c < 1)
                {
                    tbl_Vendor vnd = new tbl_Vendor();
                    vnd.Vendor_Name = collect["txtVendor"];
                    vnd.Address = collect["VendorAddress"];
                    vnd.Mobile = Convert.ToInt64(collect["MobileNumber"]);
                    vnd.Email = email;
                    vnd.Status = "Direct";
                    vnd.SignIn = "False";
                    db.tbl_Vendor.Add(vnd);
                    db.SaveChanges();

                    ViewBag.Message = "Vendor Details are Added Successfully!!";
                }
                else
                {
                    ViewBag.Message = "Email Address You Entered is Already Associated with a Vendor Please Check and verify";
                }
            }
            catch
            {
                ViewBag.Message = "Vendor Details are Not Added!!";
            }
            ViewBag.vendorslist = db.tbl_Vendor.OrderByDescending(I => I.Vendor_id).Where(i => i.Status == "Direct").ToList();

            return Redirect("~/Admin/vendors?Msg=" + ViewBag.Message);
        }


        public ActionResult EditVendor(int id)
        {
            ViewBag.EditVendor = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public ActionResult EditVendor(FormCollection collect)
        {

            int id = Convert.ToInt32(collect["HiddenId"]);
            try
            {
                

                    tbl_Vendor vnd = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();
                    vnd.Vendor_Name = collect["txtVendor"];
                    vnd.Address = collect["VendorAddress"];
                    vnd.Mobile = Convert.ToInt64(collect["MobileNumber"]);
                    vnd.Email = collect["Email"];
                    vnd.Status = "Direct";
                    db.SaveChanges();
               
                ViewBag.Message = "Vendor Details Updated Successfully!!";
                
               
            }
            catch
            {
                ViewBag.Message = "Vendor Details Not Updated Successfully!!";
            }

            return Redirect("~/Admin/vendors?Msg=" + ViewBag.Message);
        }

        public ActionResult DeleteVendor(int id, string name)
        {

            try
            {
                tbl_Vendor tbC = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();
                db.tbl_Vendor.Remove(tbC);
                int j = db.SaveChanges();
                ViewBag.Message = "Vendor " + name + " is Deleted Successfully!!";

                if (j > 0)
                {
                    List<tblProduct> prdct = db.tblProduct.Where(i => i.Vendor == id).ToList();
                    foreach (TradeKerala2017.Models.tblProduct ct in prdct)
                    {
                        db.tblProduct.Remove(ct);
                        db.SaveChanges();
                    }
                }

            }
            catch
            {
                ViewBag.Message = "Vendor " + name + " is Not Deleted Successfully!!";
            }

            return Redirect("~/Admin/vendors?Msg=" + ViewBag.Message);
        }
        


        public ActionResult SendVendor(int id)
        {
            
            try
            {
                tbl_Vendor vnd = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();
                
                string email = vnd.Email;
                if(email!="" && email!=null)
                {
                    tblLogin log = new tblLogin();
                    Random r = new Random();
                    var x = r.Next(0, 100000000);
                    string s = x.ToString("00000000");

                    log.username = email;
                    log.password = s;
                    log.type = "Vendor";
                    log.lastlogin = Convert.ToDateTime(DateTime.Now);
                    log.status = Convert.ToString("True");
                    db.tblLogins.Add(log);
                    db.SaveChanges();

                    string subject = "Traderkerala Vendor Assitance";
                    string body = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi user</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'><p>Hi Vendor,</p><p>Here Your Credentials For signing To Traderkerala vendor Managament<br><p>Username:<b> " + email + "</b></p><p><b>Password:<b> " + s + "</b></p></td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";
                    SendMail(body, email, subject);
                    vnd.SignIn = "True";
                    db.SaveChanges();
                    ViewBag.Message = "Registration completed succesfully; Please check and verify your mail for signin";
                    return Redirect("~/Admin/vendors?Msg=" + ViewBag.Message);
                    
                }
                else
                {
                    ViewBag.Message = "Please Check Your Vendor Details are Correct and retry";
                }
                ViewBag.Message = "Vendor Details Updated Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Vendor Details Not Updated Successfully!!";
            }

            return Redirect("~/Admin/vendors?Msg=" + ViewBag.Message);
        }

        

             public ActionResult ResendVendor(int id)
        {

            try
            {
                tbl_Vendor vnd = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();

                string email = vnd.Email;
                if (email != "" && email != null)
                {
                    tblLogin log = db.tblLogins.Where(i => i.username == email && i.type == "Vendor").FirstOrDefault();
                    Random r = new Random();
                    var x = r.Next(0, 100000000);
                    string s = x.ToString("00000000");

                    log.username = email;
                    log.password = s;
                    log.type = "Vendor";
                    log.lastlogin = Convert.ToDateTime(DateTime.Now);
                    log.status = Convert.ToString("True");

                    db.SaveChanges();

                    string subject = "Traderkerala Vendor Assitance";
                    string body = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi user</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'><p>Hi Vendor,</p><p>Here Your Credentials For signing To Traderkerala vendor Managament<br><p>Username:<b> " + email + "</b></p><p><b>Password:<b> " + s + "</b></p></td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";
                    SendMail(body, email, subject);
                    vnd.SignIn = "True";
                    db.SaveChanges();
                    ViewBag.Message = "Registration completed succesfully; Please check and verify your mail for signin";
                    return Redirect("~/Admin/vendors?Msg=" + ViewBag.Message);

                }
                else
                {
                    ViewBag.Message = "Please Check Your Vendor Details are Correct and retry";
                }
                ViewBag.Message = "Vendor Details Updated Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Vendor Details Not Updated Successfully!!";
            }

            return Redirect("~/Admin/vendors?Msg=" + ViewBag.Message);
        }












        public ActionResult Product()
        {

            ViewBag.ProductDetails = db.tblProduct.OrderByDescending(I => I.AddedDate).ToList();
            ViewBag.procount = db.tblProduct.Count();
            return View();
        }


        public ActionResult AddProduct()
        {
            ViewBag.ProductDetails = db.tblProduct.ToList();
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddProduct(FormCollection collect, HttpPostedFileBase[] files)
        {
            try
            {
                int CategoryId = Convert.ToInt32(collect["HiddenParentId"]);
                string CatName = db.tblCategories.Where(i => i.Catid == CategoryId).FirstOrDefault().CatName;
                int vendorId = Convert.ToInt32(collect["Ddvendor"]);
                string ProductName = collect["txtProduct"].Trim();
                string Manufacture = collect["DDBrand"];
                int qty = Convert.ToInt32(collect["txtQty"].ToString());
                decimal OldPrice = Convert.ToDecimal(collect["txtOldPrice"]);
                decimal NewPrice = Convert.ToDecimal(collect["txtNewPrice"]);
                decimal Old_Dollar_Price = Convert.ToDecimal(collect["txtOldDollarPrice"]);
                decimal New_Dollar_Price = Convert.ToDecimal(collect["txtNewDollarPrice"]);

                Boolean ChkBest = Convert.ToBoolean(collect["ChkBest"] != null ? true : false);
                Boolean ChkOffer = Convert.ToBoolean(collect["ChkOffer"] != null ? true : false);

                string TitleDesc = collect["txtDesc1"];
                string Description = collect["txtDetailDesc"];
                string prodinfo = collect["txtproductinfo"];
                string Unid = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();

                string Color = collect["txtColor"];
                string Weight = (String.IsNullOrEmpty(collect["SelWeight"]) ? "0" : collect["SelWeight"] + " " + collect["SelWeightM"]);
                string Prodsize = (String.IsNullOrEmpty(collect["SelSize"]) ? "0" : collect["SelSize"] + " " + collect["SelSizeM"]);
                string ProdHeight = (String.IsNullOrEmpty(collect["SelHeight"]) ? "0" : collect["SelHeight"] + " " + collect["SelHeightM"]);
                string ProdWidth = (String.IsNullOrEmpty(collect["SelWidth"]) ? "0" : collect["SelWidth"] + " " + collect["SelWidthM"]);
                string organic = collect["Hideid"];
                if(organic=="")
                {
                    organic = null;
                }


                string ParentIds = "";
                int cat_id = CategoryId;
                string Parent_Name = db.tblCategories.Where(i => i.Catid == cat_id).FirstOrDefault().CatName;
                ParentIds = cat_id.ToString() + "|" + Parent_Name;
                while (cat_id > 0)
                {
                    int Parent_id = Convert.ToInt16(db.tblCategories.Where(i => i.Catid == cat_id).FirstOrDefault().ParentId);

                    if (Parent_id != 0)
                    {
                        string ParentName = db.tblCategories.Where(i => i.Catid == Parent_id).FirstOrDefault().CatName;
                        ParentIds = ParentIds + "~" + Parent_id + "|" + ParentName;
                    }
                    cat_id = Convert.ToInt16(Parent_id);
                }


                tblProduct tbP = new tblProduct();
                tbP.ProdName = ProductName;
                tbP.Catid = CategoryId;
                tbP.CatName = CatName;
                tbP.Vendor = vendorId;
                tbP.ProdNewPrice = NewPrice;
                tbP.ProdOldPrice = OldPrice;
                tbP.Dollar_NewPrice = New_Dollar_Price;
                tbP.Dollar_OldPrice = Old_Dollar_Price;
                tbP.ProdTitle = TitleDesc;
                tbP.ProdDesc = Description;
                tbP.ProdInfo = prodinfo;
                tbP.Brand = Manufacture;
                tbP.Qty = qty;
                tbP.Color = Color;
                tbP.Size = Prodsize;
                tbP.Weight = Weight;
                tbP.Height = ProdHeight;
                tbP.Width = ProdWidth;
                tbP.Status = true;
                tbP.Bestsell = ChkBest;
                tbP.Isoffer = ChkOffer;
                tbP.AddedDate = DateTime.Now;
                tbP.ParCategories = ParentIds;
                tbP.Organic = organic;
                tbP.Hide = "Show";
           

                db.tblProduct.Add(tbP);
                db.SaveChanges();

                tblStock tbS = new tblStock();
                tbS.ProductId = tbP.pid;
                tbS.Product = ProductName;
                tbS.Category = CatName;
                tbS.CategoryId = CategoryId;
                tbS.Price = NewPrice;
                tbS.Dollar__Price = New_Dollar_Price;
                tbS.Brand = Manufacture;
                tbS.Qty = qty;
                tbS.Discount = 0;
                tbS.Totalamount = NewPrice * qty;
                tbS.Dollar__Price = New_Dollar_Price * qty;
                tbS.Date = DateTime.Now;
                tbS.Type = "P";

                db.tblStocks.Add(tbS);
                db.SaveChanges();

                string ImagePath = Server.MapPath("~/images/Product/");
                int k = 1;
                foreach (HttpPostedFileBase file in files)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        string ext = Path.GetExtension(file.FileName).ToLower();
                        string name1 = Unid + "-" + k;
                        string url = ImagePath + name1 + ext;

                        if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                        {


                            file.SaveAs(url);
                            string ImageUrl = "/images/Product/" + name1 + ext;
                            tblProdImage tbImg = new tblProdImage();
                            tbImg.Image = ImageUrl;
                            tbImg.Prodid = tbP.pid;
                            tbImg.position = k;
                            db.tblProdImages.Add(tbImg);
                            db.SaveChanges();

                        }
                        else
                        {
                            ViewBag.Message = "Invalid Image Extention!!!";
                        }

                        k = k + 1;
                        //var InputFileName = Path.GetFileName(file.FileName);
                        // var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                        //Save file to server folder  
                        //file.SaveAs(ServerSavePath);
                        //assigning file uploaded status to ViewBag for showing message to user.  
                        // ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                    }

                }

                ViewBag.Message = "Product  " + ProductName + " Is Added Successfully!!";



            }
            catch { ViewBag.Message = "Product Is Not Added Successfully!!"; }
            ViewBag.ProductDetails = db.tblProduct.ToList();
            return Redirect("/Admin/Product?Msg=" + ViewBag.Message);
        }

        //Edit Product

        public ActionResult EditProduct(int id)
        {
            ViewBag.ProductDetails = db.tblProduct.Where(i => i.pid == id).FirstOrDefault();
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditProduct(FormCollection collect, HttpPostedFileBase[] files)
        {
            try
            {
                int CategoryId = Convert.ToInt32(collect["HiddenParentId"]);
                string CatName = db.tblCategories.Where(i => i.Catid == CategoryId).FirstOrDefault().CatName;
                int HiddenID = Convert.ToInt32(collect["HiddenID"]);
                int vendorId = Convert.ToInt32(collect["Ddvendor"]);
                string HiddenCatName = collect["HiddenCatName"];
                string ProductName = collect["txtProduct"];
                string Manufacture = collect["DDBrand"];
                int qty = Convert.ToInt32(collect["txtQty"].ToString());
                decimal OldPrice = Convert.ToDecimal(collect["txtOldPrice"]);
                decimal NewPrice = Convert.ToDecimal(collect["txtNewPrice"]);
                decimal Old_Dollar_Price = Convert.ToDecimal(collect["txtOldDollarPrice"]);
                decimal New_Dollar_Price = Convert.ToDecimal(collect["txtNewDollarPrice"]);

                decimal Point = 0;

                Boolean ChkBest = Convert.ToBoolean(collect["ChkBest"] != null ? true : false);
                Boolean ChkOffer = Convert.ToBoolean(collect["ChkOffer"] != null ? true : false);
                string ImagePath = Server.MapPath("~/images/Product/");
                
                string TitleDesc = collect["txtDesc1"];
                string Description = collect["txtDetailDesc"];
                string product_info = collect["txtproductinfo"];
                string Unid = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string Color = collect["txtColor"];
                string Weight = (String.IsNullOrEmpty(collect["SelWeight"]) ? "0" : collect["SelWeight"] + " " + collect["SelWeightM"]);
                string Prodsize = (String.IsNullOrEmpty(collect["SelSize"]) ? "0" : collect["SelSize"] + " " + collect["SelSizeM"]);
                string ProdHeight = (String.IsNullOrEmpty(collect["SelHeight"]) ? "0" : collect["SelHeight"] + " " + collect["SelHeightM"]);
                string ProdWidth = (String.IsNullOrEmpty(collect["SelWidth"]) ? "0" : collect["SelWidth"] + " " + collect["SelWidthM"]);
                string organic = collect["Hideid"];
                if (organic == "")
                {
                    organic = null;
                }
                string ParentIds = "";
                int cat_id = CategoryId;
                string Parent_Name = db.tblCategories.Where(i => i.Catid == cat_id).FirstOrDefault().CatName;
                ParentIds = cat_id.ToString() + "|" + Parent_Name;
                while (cat_id > 0)
                {
                    int Parent_id = Convert.ToInt16(db.tblCategories.Where(i => i.Catid == cat_id).FirstOrDefault().ParentId);

                    if (Parent_id != 0)
                    {
                        string ParentName = db.tblCategories.Where(i => i.Catid == Parent_id).FirstOrDefault().CatName;
                        ParentIds = ParentIds + "~" + Parent_id + "|" + ParentName;
                    }
                    cat_id = Convert.ToInt16(Parent_id);
                }



                try
                {
                    tblProduct tbP = db.tblProduct.Where(o => o.pid == HiddenID).FirstOrDefault();
                    tbP.ProdName = ProductName;
                    tbP.Vendor = vendorId;
                    tbP.Catid = CategoryId;
                    tbP.CatName = CatName;
                    tbP.ProdNewPrice = NewPrice;
                    tbP.ProdOldPrice = OldPrice;
                    tbP.Dollar_NewPrice = New_Dollar_Price;
                    tbP.Dollar_OldPrice = Old_Dollar_Price;
                    tbP.ProdTitle = TitleDesc;
                    tbP.ProdDesc = Description;
                    tbP.ProdInfo = product_info;
                    tbP.Brand = Manufacture;
                    tbP.Qty = qty;
                    tbP.Color = Color;
                    tbP.Size = Prodsize;
                    tbP.Weight = Weight;
                    tbP.Height = ProdHeight;
                    tbP.Width = ProdWidth;
                    tbP.Status = true;
                    tbP.Bestsell = ChkBest;
                    tbP.Isoffer = ChkOffer;
                    tbP.AddedDate = DateTime.Now;
                    tbP.ParCategories = ParentIds;
                    tbP.Organic = organic;
                    db.SaveChanges();

                    tblStock tbS = db.tblStocks.Where(i => i.ProductId == HiddenID).FirstOrDefault();
                    tbS.ProductId = tbP.pid;
                    tbS.Product = ProductName;
                    tbS.Category = CatName;
                    tbS.CategoryId = CategoryId;
                    tbS.Price = NewPrice;
                    tbS.Dollar__Price = New_Dollar_Price;
                    tbS.Brand = Manufacture;
                    tbS.Qty = qty;
                    tbS.Discount = 0;
                    tbS.Totalamount = NewPrice * qty;
                    tbS.Dollar__Price = Old_Dollar_Price * qty;
                    tbS.Date = DateTime.Now;
                    tbS.Type = "P";


                    db.SaveChanges();

                    int k = Convert.ToInt32(db.tblProdImages.Where(i => i.Prodid == tbP.pid).OrderByDescending(i => i.position).Take(1).FirstOrDefault().position) + 1;

                    foreach (HttpPostedFileBase file in files)
                    {
                        //Checking file is available to save.  
                        if (file != null)
                        {
                            string ext = Path.GetExtension(file.FileName).ToLower();
                            string name1 = Unid + "-" + k;
                            string url = ImagePath + name1 + ext;

                            if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                            {


                                file.SaveAs(url);
                                string ImageUrl = "/images/Product/" + name1 + ext;
                                tblProdImage tbImg = new tblProdImage();
                                tbImg.Image = ImageUrl;
                                tbImg.Prodid = tbP.pid;
                                tbImg.position = k;
                                db.tblProdImages.Add(tbImg);
                                db.SaveChanges();

                            }
                            else
                            {
                                ViewBag.Message = "Invalid Image Extention!!!";
                            }

                            k = k + 1;
                            //var InputFileName = Path.GetFileName(file.FileName);
                            // var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                            //Save file to server folder  
                            //file.SaveAs(ServerSavePath);
                            //assigning file uploaded status to ViewBag for showing message to user.  
                            // ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                        }

                    }

                    ViewBag.Message = "Product  " + ProductName + " Is Updated Successfully!!";

                }
                catch { ViewBag.Message = "Product  " + ProductName + " Is Not Updated Successfully!!"; }


            }
            catch { }
            return Redirect("/Admin/Product?Msg=" + ViewBag.Message);
        }

        public ActionResult DeleteProduct(int id, string name)
        {

            try
            {
                tblProduct tbP = db.tblProduct.Where(o => o.pid == id).FirstOrDefault();
                db.tblProduct.Remove(tbP);
                db.SaveChanges();

                tblStock pd = db.tblStocks.Where(o => o.ProductId == id).FirstOrDefault();
                db.tblStocks.Remove(pd);
                db.SaveChanges();

                ViewBag.Message = "Product " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Product " + name + " is Not Deleted Successfully!!";
            }

            return Redirect("/Admin/Product?Msg=" + ViewBag.Message);
        }
        public ActionResult DeleteImage(int img, int id)
        {

            String Path = "";
            tblProduct tbP = db.tblProduct.Where(u => u.pid == id).FirstOrDefault();

            tblProdImage Image = db.tblProdImages.Where(u => u.id == img).FirstOrDefault();
            db.tblProdImages.Remove(Image);
            db.SaveChanges();

            String FullPath = Server.MapPath(Path);
            FileInfo F = new FileInfo(FullPath);
            F.Delete();

            return Redirect("/Admin/EditProduct?id=" + id);
        }

        public ActionResult HideProduct(int id)
        {

            try
            {
                tblProduct tbP = db.tblProduct.Where(o => o.pid == id).FirstOrDefault();
                tbP.Hide = "Hided";
                db.SaveChanges();

                ViewBag.Message = "Product Hided Successfully";
            }
            catch
            {
                ViewBag.Message = "Product is Not Hided!!, Something Gone Wrong";
            }

            return Redirect("/Admin/Product?Msg=" + ViewBag.Message);
        }
        
        public ActionResult showProduct(int id)
        {

            try
            {
                tblProduct tbP = db.tblProduct.Where(o => o.pid == id).FirstOrDefault();
                tbP.Hide = "Show";
                db.SaveChanges();
                ViewBag.Message = "Product Showed Successfully";
            }
            catch
            {
                ViewBag.Message = "Product is Not Showed!!, Something Gone Wrong";
            }

            return Redirect("/Admin/Product?Msg=" + ViewBag.Message);
        }

        public ActionResult HiddenPro()
        {
            
            try
            {
                ViewBag.ProductDetails = db.tblProduct.Where(i => i.Hide == "Hided").ToList();
            }
            catch
            {
            }
            return View();
        }



        [HttpGet]
        public ActionResult RelatedProduct()
        {
            ViewBag.RelatedDetails = db.tbl_RelatedProducts.OrderByDescending(I => I.Id).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult RelatedProduct(FormCollection collect)
        {
            try
            {
                tbl_RelatedProducts rp = new tbl_RelatedProducts();
                int pid = Convert.ToInt32(collect["product"]);
                int rid = Convert.ToInt32(collect["Related"]);
                string PName = db.tblProduct.Where(i => i.pid == pid).FirstOrDefault().ProdName;
                string RName = db.tblProduct.Where(i => i.pid == rid).FirstOrDefault().ProdName;

                //string[] Related = collect["HiddenId"].Split(',');
                //int Lengths = Related.Length;
                //for (int i = 0; i < Lengths; i++)
                //{
                //    int rid = Convert.ToInt32(Related[i]);
                //    string RName = db.tblProduct.Where(k=> k.pid == rid).FirstOrDefault().ProdName;


                //    rp.Prodct_Id = pid;
                //    rp.prodct_Name = PName;
                //    rp.Related_Id = rid;
                //    rp.Related_Product = RName;
                //    db.tbl_RelatedProducts.Add(rp);
                //    db.SaveChanges();
                //}

                rp.Prodct_Id = pid;
                rp.prodct_Name = PName;
                rp.Related_Id = rid;
                rp.Related_Product = RName;
                db.tbl_RelatedProducts.Add(rp);
                db.SaveChanges();
                ViewBag.Message = "Relationship is added Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Relationship is not added!!";
            }
            ViewBag.RelatedDetails = db.tbl_RelatedProducts.OrderByDescending(I => I.Id).ToList();

            return Redirect("/Admin/RelatedProduct?Msg=" + ViewBag.Message);

        }
        public ActionResult DeleteRelation(int id, string name, string rname)
        {

            try
            {
                tbl_RelatedProducts tbP = db.tbl_RelatedProducts.Where(o => o.Id == id).FirstOrDefault();
                db.tbl_RelatedProducts.Remove(tbP);
                db.SaveChanges();

                ViewBag.Message = "Relation between" + name + " and " + rname + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Relation between" + name + " and " + rname + " is not Deleted!!";
            }
            ViewBag.RelatedDetails = db.tbl_RelatedProducts.ToList();
            return Redirect("/Admin/RelatedProduct?Msg=" + ViewBag.Message);
        }


        [HttpGet]
        public ActionResult TopCategories()
        {
            ViewBag.TopCat = db.tb_TopCategory.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult TopCategories(FormCollection collect, HttpPostedFileBase file1)
        {
            try
            {
                int ca = db.tb_TopCategory.Count();
                if (ca >= 10)
                {
                    ViewBag.Message = "Sorry You can Only Add 10 top categories; please Verify!!";
                }
                else
                {
                    int catgory = Convert.ToInt32(collect["product"]);
                    string CatImage = "";

                    string ImageName = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    string ImagePath = Server.MapPath("~/images/Banner/");
                    DateTime date = DateTime.Now.Date;
                    if (file1 != null)
                    {
                        string ext = Path.GetExtension(file1.FileName).ToLower();

                        if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                        {
                            file1.SaveAs(ImagePath + ImageName + ext);
                            CatImage = "/images/Banner/" + ImageName + ext;

                            tb_TopCategory cat = new tb_TopCategory();
                            cat.Cat_Id = catgory;
                            string cname = db.tblCategories.Where(k => k.Catid == catgory).FirstOrDefault().CatName;
                            cat.Cat_Name = cname;
                            cat.Image = CatImage;
                            cat.Hide = "show";
                            db.tb_TopCategory.Add(cat);
                            db.SaveChanges();
                            ViewBag.Message = "Category Image Added Successfully!!";

                        }
                        else
                        {
                            ViewBag.Message = "Category Image Not Added Successfully!!";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Image is not Uploaded";
                    }
                    ViewBag.TopCat = db.tb_TopCategory.ToList();
                }
            }
            catch
            {
                ViewBag.Message = "Something Gone Wrong,Please verify and Try Again!!";
            }
            return Redirect("/Admin/TopCategories?Msg=" + ViewBag.Message);
        }


        public ActionResult DeletetopCategory(int id, string name)
        {
            try
            {
                tb_TopCategory tbP = db.tb_TopCategory.Where(o => o.Id == id).FirstOrDefault();
                db.tb_TopCategory.Remove(tbP);
                db.SaveChanges();

                ViewBag.Message = "Top Category " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Top Category " + name + " is not Deleted!!";
            }

            return Redirect("/Admin/TopCategories?Msg=" + ViewBag.Message);
        }


        public ActionResult HideCategory(int id)
        {
            tb_TopCategory tbB = db.tb_TopCategory.Where(o => o.Id == id).FirstOrDefault();

            tbB.Hide = "Hided";
            db.SaveChanges();
            ViewBag.Message = "Category Is Hided Successfully!!";
            return Redirect("/Admin/TopCategories?Msg=" + ViewBag.Message);
        }
        public ActionResult ShowCategory(int id)
        {
            tb_TopCategory tbB = db.tb_TopCategory.Where(o => o.Id == id).FirstOrDefault();

            tbB.Hide = "show";
            db.SaveChanges();
            ViewBag.Message = "Category Is Un Hided Successfully!!";
            return Redirect("/Admin/TopCategories?Msg=" + ViewBag.Message);
        }

        public ActionResult Edittopcat(int id)
        {
            ViewBag.Edittopcat = db.tb_TopCategory.Where(i => i.Id == id).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public ActionResult Edittopcat(FormCollection collect, HttpPostedFileBase file1)
        {
            int Id = Convert.ToInt32(collect["HiddenId"]);
            string OffrName = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString(); ;
            string NewImage = "";
            string CurrentOfferImage = collect["HiddenImage"];
            string ImagePath = Server.MapPath("~/images/Banner/");
            try
            {
                if (file1 != null)
                {
                    string ext = Path.GetExtension(file1.FileName).ToLower();
                    string url = ImagePath + OffrName + ext;

                    if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                    {


                        file1.SaveAs(url);
                        NewImage = "/images/Banner/" + OffrName + ext;
                        tb_TopCategory tbB = db.tb_TopCategory.Where(o => o.Id == Id).FirstOrDefault();
                        tbB.Image = NewImage;
                        db.SaveChanges();
                        ViewBag.Message = "Top Category  Is Updated Successfully!!";
                    }
                    else
                    {
                        ViewBag.Message = "Top Category Not Updated Successfully!!";
                    }
                }
                else
                {
                    tb_TopCategory tbB = db.tb_TopCategory.Where(o => o.Id == Id).FirstOrDefault();
                    tbB.Image = CurrentOfferImage;
                    db.SaveChanges();
                    ViewBag.Message = "Top Category Is Updated Successfully!!";
                }
            }
            catch
            {
                ViewBag.Message = "Top Category  Is Not Updated Successfully!!";
            }
            return Redirect("/Admin/TopCategories?Msg=" + ViewBag.Message);
        }




        [HttpGet]
        public ActionResult TopBrands()
        {
            ViewBag.ct = db.tb_TopBrand.Where(i => i.Hide == "show").Count();
            return View();
        }


        [HttpPost]
        public ActionResult TopBrands(FormCollection collect, HttpPostedFileBase file1)
        {
            int Brand = Convert.ToInt32(collect["product"]);
            string BrandImage = "";

            string ImageName = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            string ImagePath = Server.MapPath("~/images/Brand/");
            DateTime date = DateTime.Now.Date;
            if (file1 != null)
            {
                string ext = Path.GetExtension(file1.FileName).ToLower();

                if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                {
                    file1.SaveAs(ImagePath + ImageName + ext);
                    BrandImage = "/images/Brand/" + ImageName + ext;

                    tb_TopBrand cat = new tb_TopBrand();
                    cat.Brand_Id = Brand;
                    string cname = db.tblBrands.Where(k => k.Brandid == Brand).FirstOrDefault().BrandName;
                    cat.Brand_Name = cname;
                    cat.Image = BrandImage;
                    cat.Hide = "show";
                    db.tb_TopBrand.Add(cat);
                    db.SaveChanges();
                    ViewBag.Message = "Brand Image Added Successfully!!";

                }
                else
                {
                    ViewBag.Message = "Brand Image Not Added Successfully!!";
                }
            }
            else
            {
                ViewBag.Message = "Image is not Uploaded";
            }

            return Redirect("/Admin/TopBrands?Msg=" + ViewBag.Message);
        }


        public ActionResult DeletetopBrand(int id, string name)
        {
            try
            {
                tb_TopBrand tbP = db.tb_TopBrand.Where(o => o.Id == id).FirstOrDefault();
                db.tb_TopBrand.Remove(tbP);
                db.SaveChanges();

                ViewBag.Message = "Top Brand " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Top Brand " + name + " is not Deleted!!";
            }

            return Redirect("/Admin/TopBrands?Msg=" + ViewBag.Message);
        }


        public ActionResult HideBrand(int id)
        {
            tb_TopBrand tbB = db.tb_TopBrand.Where(o => o.Id == id).FirstOrDefault();

            tbB.Hide = "Hided";
            db.SaveChanges();
            ViewBag.Message = "Brand Is Hided Successfully!!";
            return Redirect("/Admin/TopBrands?Msg=" + ViewBag.Message);
        }
        public ActionResult ShowBrands(int id)
        {
            tb_TopBrand tbB = db.tb_TopBrand.Where(o => o.Id == id).FirstOrDefault();

            tbB.Hide = "show";
            db.SaveChanges();
            ViewBag.Message = "Brand Is Un Hided Successfully!!";
            return Redirect("/Admin/TopBrands?Msg=" + ViewBag.Message);
        }






        [HttpGet]
        public ActionResult TrendingProduct(string id)
        {
            string Id;
            if (String.IsNullOrEmpty(id))
            {
                Id = Convert.ToString("-1");
                ViewBag.trend = "Select Category";
            }
            else
            {
                Id = id;
                ViewBag.trend = id;
            }

            ViewBag.trendingDetails = db.tbl_trendingProducts.OrderByDescending(I => I.AddedDate).ToList();
            ViewBag.Procount = db.sp_TrendPro(id).Count();
            ViewBag.list = db.sp_TrendPro(id).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult TrendingProduct(FormCollection collect, string id)
        {
            string Id;
            if (String.IsNullOrEmpty(id))
            {
                Id = Convert.ToString("-1");
                ViewBag.trend = "Select Category";
            }
            else
            {
                Id = id;
                ViewBag.trend = id;
            }


            ViewBag.trendingDetails = db.tbl_trendingProducts.OrderByDescending(I => I.AddedDate).ToList();
            ViewBag.Procount = db.sp_TrendPro(id).Count();
            ViewBag.list = db.sp_TrendPro(id).ToList();

            try
            {
                tbl_trendingProducts trd = new tbl_trendingProducts();
                string[] Related = collect["HiddenId"].Split(',');
                int Lengths = Related.Length;
                for (int i = 0; i < Lengths; i++)
                {
                    int rid = Convert.ToInt32(Related[i]);
                    string RName = db.tblProduct.Where(k => k.pid == rid).FirstOrDefault().ProdName;
                    trd.pro_id = rid;
                    trd.pro_name = RName;
                    trd.AddedDate = DateTime.Now;
                    db.tbl_trendingProducts.Add(trd);
                    db.SaveChanges();
                }
                ViewBag.Message = "Trending is done Successfully!!";
            }
            catch
            {
                ViewBag.Message = "Trending is not added!!";
            }
            return View();
        }


        public ActionResult Deletetrending(int id, string name)
        {

            try
            {
                tbl_trendingProducts tbP = db.tbl_trendingProducts.Where(o => o.Id == id).FirstOrDefault();
                db.tbl_trendingProducts.Remove(tbP);
                db.SaveChanges();

                ViewBag.Message = "Trending item" + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Trending item" + name + " is not Deleted!!";
            }
            ViewBag.RelatedDetails = db.tbl_RelatedProducts.ToList();
            return Redirect("/Admin/TrendingProduct?Msg=" + ViewBag.Message);
        }


        [HttpGet]
        public ActionResult special_offer()
        {

            ViewBag.category = db.tblProduct.ToList();
            ViewBag.offer = db.tbl_specialoffer.OrderByDescending(I => I.AddedDate).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult special_offer(FormCollection collect, HttpPostedFileBase file1)
        {


            string offername = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            string a = DateTime.Now.Day.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            string OfferImage = "";
            string position = collect["pos"];


            string ImagePath = Server.MapPath("~/images/Brand/");


            DateTime date = DateTime.Now.Date;
            //try
            //{
            if (file1 != null)
            {
                string ext = Path.GetExtension(file1.FileName).ToLower();

                if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                {
                    file1.SaveAs(ImagePath + offername + ext);
                    OfferImage = "/images/Brand/" + offername + ext;


                    tbl_specialoffer tbB = new tbl_specialoffer();
                    tbB.offer_name = collect["txtoffer"];
                    tbB.offer_image = OfferImage;
                    tbB.position = "NoPosition";
                    tbB.Hide = "Show";
                    tbB.AddedDate = DateTime.Now;
                    db.tbl_specialoffer.Add(tbB);
                    db.SaveChanges();
                    ViewBag.Message = "Special offer Added Successfully!!";

                }
                else
                {
                    ViewBag.Message = "Special offer Not Added Successfully!!";
                }
            }
            else
            {
                ViewBag.Message = "Image is not Uploaded";
            }
            ViewBag.category = db.tblProduct.ToList();
            ViewBag.offer = db.tbl_specialoffer.OrderByDescending(I => I.AddedDate).ToList();
            return Redirect("/Admin/special_offer?Msg=" + ViewBag.Message);
        }

        public ActionResult DeleteOffer(int id, string name)
        {

            try
            {
                tbl_specialoffer tbP = db.tbl_specialoffer.Where(o => o.id == id).FirstOrDefault();
                int offerId = tbP.id;
                List<tbl_ProductOffer> prdct = db.tbl_ProductOffer.Where(k => k.Offer_Id == offerId).ToList();

                foreach (TradeKerala2017.Models.tbl_ProductOffer ct in prdct)
                {
                    tbl_ProductOffer pd = db.tbl_ProductOffer.Where(o => o.Id == ct.Id).FirstOrDefault();

                    db.tbl_ProductOffer.Remove(pd);
                    db.SaveChanges();
                }


                db.tbl_specialoffer.Remove(tbP);
                int i = db.SaveChanges();


                ViewBag.Message = "Offer" + name + " & Related Products are Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Offer " + name + "  & Related Products are not Deleted!!";
            }
            ViewBag.offer = db.tbl_specialoffer.ToList();
            return Redirect("/Admin/special_offer?Msg=" + ViewBag.Message);
        }

        public ActionResult EditOffer(int id)
        {
            ViewBag.EditOffer = db.tbl_specialoffer.Where(i => i.id == id).FirstOrDefault();

            return View();
        }
        [HttpPost]
        public ActionResult EditOffer(FormCollection collect, HttpPostedFileBase file1)
        {
            int Id = Convert.ToInt32(collect["HiddenId"]);
            string OffrName = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            string NewImage = "";
            string CurrentOfferImage = collect["HiddenImage"];
            string ImagePath = Server.MapPath("~/images/Brand/");

            try
            {
                if (file1 != null)
                {
                    string ext = Path.GetExtension(file1.FileName).ToLower();
                    string url = ImagePath + OffrName + ext;

                    if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                    {


                        file1.SaveAs(url);
                        NewImage = "/images/Brand/" + OffrName + ext;
                        tbl_specialoffer tbB = db.tbl_specialoffer.Where(o => o.id == Id).FirstOrDefault();
                        tbB.offer_name = collect["txtOffer"];
                        tbB.offer_image = NewImage;
                        tbB.AddedDate = DateTime.Now;

                        db.SaveChanges();
                        ViewBag.Message = "Offer Is Updated Successfully!!";
                    }
                    else
                    {
                        ViewBag.Message = "Offer Not Updated Successfully!!";
                    }
                }
                else
                {
                    tbl_specialoffer tbB = db.tbl_specialoffer.Where(o => o.id == Id).FirstOrDefault();
                    tbB.offer_name = collect["txtOffer"];
                    tbB.offer_image = CurrentOfferImage;
                    tbB.AddedDate = DateTime.Now;

                    db.SaveChanges();
                    ViewBag.Message = "Offer Is Updated Successfully!!";
                }
            }
            catch
            {
                ViewBag.Message = " Offer Is Not Updated Successfully!!";
            }
            return Redirect("/Admin/special_offer?Msg=" + ViewBag.Message);
        }
        public ActionResult HideOffer(int id)
        {
            tbl_specialoffer tbB = db.tbl_specialoffer.Where(o => o.id == id).FirstOrDefault();
            tbB.AddedDate = DateTime.Now;
            tbB.Hide = "Hided";
            db.SaveChanges();
            ViewBag.Message = "Offer Is Hided Successfully!!";
            return Redirect("/Admin/special_offer?Msg=" + ViewBag.Message);
        }
        public ActionResult ShowOffer(int id)
        {
            tbl_specialoffer tbB = db.tbl_specialoffer.Where(o => o.id == id).FirstOrDefault();
            tbB.AddedDate = DateTime.Now;
            tbB.Hide = "Show";
            db.SaveChanges();
            ViewBag.Message = "Offer Is Un Hided Successfully!!";
            return Redirect("/Admin/special_offer?Msg=" + ViewBag.Message);
        }


        [HttpGet]
        public ActionResult AddProductOffer()
        {
            ViewBag.product = db.tblProduct.ToList();
            ViewBag.Offer = db.tbl_specialoffer.ToList();
            ViewBag.OfferPro = db.tbl_ProductOffer.OrderByDescending(I => I.Id).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddProductOffer(FormCollection collect)
        {
            try
            {
                tbl_ProductOffer pro = new tbl_ProductOffer();
                int Pid = Convert.ToInt32(collect["ProId"]);
                string Pname = db.tblProduct.Where(i => i.pid == Pid).FirstOrDefault().ProdName;
                int Offer_Id = Convert.ToInt32(collect["OfferId"]);

                pro.Pid = Pid;
                pro.ProdName = Pname;
                pro.Offer_Id = Offer_Id;
                pro.Discount = Convert.ToInt16(collect["txtDiscount"]);
                pro.NewPrice = Convert.ToDecimal(collect["txtPrice"]);
                pro.DollarPrice = Convert.ToDecimal(collect["txtDollarPrice"]);
                db.tbl_ProductOffer.Add(pro);
                db.SaveChanges();

                ViewBag.Message = "Product Offer Added Succesfully";

            }
            catch
            {
                ViewBag.Message = "Product Offer Not Added";

            }

            ViewBag.product = db.tblProduct.ToList();
            ViewBag.Offer = db.tbl_specialoffer.ToList();
            ViewBag.OfferPro = db.tbl_ProductOffer.OrderByDescending(I => I.Id).ToList();
            return Redirect("/Admin/AddProductOffer?Msg=" + ViewBag.Message);
        }

        public ActionResult DeleteProOffer(int id, string name)
        {

            try
            {
                tbl_ProductOffer tbP = db.tbl_ProductOffer.Where(o => o.Id == id).FirstOrDefault();
                db.tbl_ProductOffer.Remove(tbP);
                db.SaveChanges();

                

                ViewBag.Message = "Product Offer Of" + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Product Offer Of" + name + " is not Deleted!!";
            }
            ViewBag.product = db.tblProduct.ToList();
            ViewBag.Offer = db.tbl_specialoffer.ToList();
            ViewBag.OfferPro = db.tbl_ProductOffer.ToList();
            return Redirect("/Admin/AddProductOffer?Msg=" + ViewBag.Message);
        }

        public ActionResult EditProOffer(int id)
        {
            ViewBag.EditProOffer = db.tbl_ProductOffer.Where(i => i.Id == id).FirstOrDefault();
            ViewBag.product = db.tblProduct.ToList();
            ViewBag.Offer = db.tbl_specialoffer.ToList();
            ViewBag.OfferPro = db.tbl_ProductOffer.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult EditProOffer(FormCollection collect)
        {
            try
            {
                int id = Convert.ToInt16(collect["HiddenId"]);
                tbl_ProductOffer pro = db.tbl_ProductOffer.Where(i => i.Id == id).FirstOrDefault();
                int Pid = Convert.ToInt32(collect["ProId"]);
                string Pname = db.tblProduct.Where(i => i.pid == Pid).FirstOrDefault().ProdName;
                int Offer_Id = Convert.ToInt32(collect["OfferId"]);

                pro.Pid = Pid;
                pro.ProdName = Pname;
                pro.Offer_Id = Offer_Id;
                pro.Discount = Convert.ToInt16(collect["txtDiscount"]);
                pro.NewPrice = Convert.ToDecimal(collect["txtPrice"]);
                pro.DollarPrice = Convert.ToDecimal(collect["txtDollarPrice"]);
                db.SaveChanges();

                ViewBag.Message = "Product Offer Details Updated Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Product Offer Not Updated Successfully!!";
            }

            return Redirect("~/Admin/AddProductOffer?Msg=" + ViewBag.Message);
        }

        [HttpGet]
        public ActionResult AddTodaysDeal()
        {
            ViewBag.product = db.tblProduct.ToList();
            ViewBag.OfferPro = db.tbl_TodaysDeal.OrderByDescending(I => I.Id).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddTodaysDeal(FormCollection collect)
        {
            try
            {
                tbl_TodaysDeal pro = new tbl_TodaysDeal();
                int Pid = Convert.ToInt32(collect["ProId"]);
                string Pname = db.tblProduct.Where(i => i.pid == Pid).FirstOrDefault().ProdName;
                pro.Pid = Pid;
                pro.ProdName = Pname;
                pro.Offer_Name = "Today's Deal";
                pro.Discount = Convert.ToInt16(collect["txtDiscount"]);
                pro.NewPrice = Convert.ToDecimal(collect["txtPrice"]);
                pro.DollarPrice = Convert.ToDecimal(collect["txtDollarPrice"]);

                db.tbl_TodaysDeal.Add(pro);
                db.SaveChanges();

                ViewBag.Message = "Today's Deal Added Succesfully";

            }
            catch
            {
                ViewBag.Message = "Today's Deal Not Added";

            }

            ViewBag.product = db.tblProduct.ToList();
            ViewBag.OfferPro = db.tbl_TodaysDeal.OrderByDescending(I => I.Id).ToList();

            return Redirect("/Admin/AddTodaysDeal?Msg=" + ViewBag.Message);
        }

        public ActionResult DeleteTodaysDeal(int id, string name)
        {

            try
            {
                tbl_TodaysDeal tbP = db.tbl_TodaysDeal.Where(o => o.Id == id).FirstOrDefault();
                db.tbl_TodaysDeal.Remove(tbP);
                db.SaveChanges();

                ViewBag.Message = "Today's Deals Of " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Today's Deals Of" + name + " is not Deleted!!";
            }
            ViewBag.product = db.tblProduct.ToList();
            ViewBag.Offer = db.tbl_specialoffer.ToList();
            ViewBag.OfferPro = db.tbl_TodaysDeal.ToList();
            return Redirect("/Admin/AddTodaysDeal?Msg=" + ViewBag.Message);
        }

        public ActionResult EditTodaysDeal(int id)
        {
            ViewBag.EditProOffer = db.tbl_TodaysDeal.Where(i => i.Id == id).FirstOrDefault();
            ViewBag.product = db.tblProduct.ToList();
            ViewBag.OfferPro = db.tbl_TodaysDeal.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult EditTodaysDeal(FormCollection collect)
        {
            try
            {
                int id = Convert.ToInt16(collect["HiddenId"]);
                tbl_TodaysDeal pro = db.tbl_TodaysDeal.Where(i => i.Id == id).FirstOrDefault();
                int Pid = Convert.ToInt32(collect["ProId"]);
                string Pname = db.tblProduct.Where(i => i.pid == Pid).FirstOrDefault().ProdName;
                int Offer_Id = Convert.ToInt32(collect["OfferId"]);

                pro.Pid = Pid;
                pro.ProdName = Pname;
                pro.Discount = Convert.ToInt32(collect["txtDiscount"]);
                pro.NewPrice = Convert.ToDecimal(collect["txtPrice"]);
                pro.DollarPrice = Convert.ToDecimal(collect["txtDollarPrice"]);

                db.SaveChanges();

                ViewBag.Message = "Today's Deals Details Updated Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Today's Deals  Not Updated Successfully!!";
            }

            return Redirect("~/Admin/AddTodaysDeal?Msg=" + ViewBag.Message);
        }







        //Add Stock
        public ActionResult AddStock()
        {
            ViewBag.ProductList = db.tblStocks.OrderByDescending(I => I.ProductId).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult AddStock(FormCollection collect)
        {

            ViewBag.ProductList = db.tblStocks.OrderByDescending(I => I.ProductId).ToList();
            return View();
        }

        //Update Stock
        [HttpPost]
        public ActionResult UpdateStock(FormCollection collect)
        {
            int ProdId = Convert.ToInt32(collect["HiddenId"]);
            int Qty = Convert.ToInt32(collect["txtNewQty"]);
            try
            {


                tblProduct tbP = db.tblProduct.Where(o => o.pid == ProdId).FirstOrDefault();
                if(Qty>1 && tbP.Qty<1)
                {
                    string subject = "";
                    string body = "";
                    subject = "New Stocks available for "+tbP.ProdName+" visit Traderkerala.com ";
                    body = "<p>Hi User,</p><p>Greetings from Traderkerala.com Per your wishlist request, we have successfully updated the stock for " + tbP.ProdName + ".</p><p>Should you need to contact us for any reason, please know that we can give out order information only to the name and e - mail address associated with your account.Thank you again for shopping with us.</p><p><br>Traderkerala</p>";

                    foreach (tbl_Notfication tb in db.tbl_Notfication.Where(i => i.pro_id == ProdId).ToList())
                    {
                        string email = tb.Username;
                        tb.status = "New Stocks available for " + tbP.ProdName;
                        db.SaveChanges();
                        SendMail(body, email, subject);
                    }
                }
                int newqty = Convert.ToInt16(tbP.Qty + Qty);
                tbP.Qty = newqty;
                db.SaveChanges();

                tblStock tbS = db.tblStocks.Where(o => o.ProductId == ProdId).FirstOrDefault();
                int neqty = Convert.ToInt16(tbS.Qty + Qty);
                tbS.Qty = newqty;
                db.SaveChanges();
                ViewBag.Message = "Stock Added Successfully";

            }
            catch(Exception ex)
            {
                ViewBag.Message = "Stock Not Added Successfully";
            }


            return Redirect("/Admin/AddStock?Msg=" + ViewBag.Message);
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

        public ActionResult ViewProducts(int id)
        {
            ViewBag.ProductDetails = db.tblProduct.Where(i => i.pid == id).FirstOrDefault();
            return View();
        }
        //Add Sales
        public ActionResult AddSales()
        {
            //List<OfferDetails> tbList = db.tblSales.Join(db.tblProduct, s => s.ProdId, sa => sa.Prodid, (s, sa) => new { Sales = s, Product = sa }).Select(i => new { i.Sales.id, i.Product.Prodid, i.Product.ProdName,i.Sales.Oldprice,i.Sales.Newprice }).ToList();

            ViewBag.ProductList = db.Sp_GetOffers().ToList();

            return View();
        }
        [HttpPost]
        public ActionResult AddSales(FormCollection collect)
        {
            try
            {
                tblSale tbS = new tblSale();
                tbS.ProdId = Convert.ToInt32(collect["Product"]);
                tbS.Oldprice = Convert.ToDecimal(collect["txtActual"]);
                tbS.Newprice = Convert.ToDecimal(collect["txtOffer"]);
                db.tblSales.Add(tbS);
                db.SaveChanges();

                tblProduct tbP = db.tblProduct.Where(q => q.pid == tbS.ProdId).FirstOrDefault();
                tbP.ProdNewPrice = tbS.Newprice;
                tbP.ProdOldPrice = tbS.Oldprice;
                tbP.Isoffer = true;
                db.SaveChanges();

                ViewBag.Msg = "Offer Added Successfully";
            }
            catch
            {
                ViewBag.Msg = "Offer Not Added Successfully";
            }
            // ViewBag.ProductList = db.tblSales.ToList();
            ViewBag.ProductList = db.Sp_GetOffers().ToList();
            return View();
        }

        public ActionResult ViewSeller()
        {
            ViewBag.sellerlist = db.tbl_Vendor.Where(i => i.Status == "Online").ToList();
            return View();
        }


        public ActionResult deleteViewSeller(int id, string name)
        {

            try
            {
                tbl_Vendor tbC = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();
                db.tbl_Vendor.Remove(tbC);
                db.SaveChanges();
                ViewBag.Message = "Vendor " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Vendor " + name + " is Not Deleted Successfully!!";
            }

            return Redirect("/Admin/ViewSeller?Msg=" + ViewBag.Message);
        }

        public ActionResult addViewSeller(int id, string name)
        {

            try
            {
                tbl_Vendor tbC = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();
                tbC.Status = "Direct";
                db.SaveChanges();
                ViewBag.Message = "Vendor " + name + " is Added to vendorlist Successfully!!";
                string subject = "";
                string body = "";
                string email = tbC.Email;
                subject = "Welcome to Traderkerala.com";
                body = "<body style ='margin:0;padding:0;'><table border = '0' cellpadding = '0' cellspacing = '0' width='100%' style='table-layout:fixed;'><tr><td bgcolor ='#ffffff' align ='center' style = 'padding: 70px 15px 70px 15px;' class='section-padding'><table border = '0 cellpadding='0' cellspacing='0' width='500' class='responsive-table'><tr><td><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><!-- HERO IMAGE --><table width = '100%' border='0' cellspacing='0' cellpadding=0'><tbody><tr><td class='padding-copy'><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td><a href = 'http://traderkerala.com/' target='_blank'><img src = 'http://traderkerala.com/NewLayout/images/unnamed.jpg' width='500' height='200' border='0' alt='Can an email really be responsive?' style='display: block; padding: 0; color: #666666; text-decoration: none; font-family: Helvetica, arial, sans-serif; font-size: 16px; width: 500px; height: 200px;' class='img-max'></a></td></tr></table></td></tr></tbody></table></td></tr><tr><td><!-- COPY --><table width = '100%' border='0' cellspacing='0' cellpadding='0'><tr><td align = 'center' style='font-size: 25px; font-family: Helvetica, Arial, sans-serif; color: #333333; padding-top: 30px;' class='padding-copy'>Hi  User</td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 16px; line-height: 25px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class'padding-copy'>Dear customer, Your Request for selling at Traderkerala.com account is successfully Processed. Welcome to traderkerala community</td></tr></table></td></tr><tr><td><!-- BULLETPROOF BUTTON --><table width = '100%' border= '0' cellspacing= '0' cellpadding= '0' class='mobile-button-container'><tr><td align = 'center' style='padding: 25px 0 0 0;' class='padding-copy'><table border = '0' cellspacing='0' cellpadding='0' class='responsive-table'><tr><td align = 'center' ></td></tr><tr><td align = 'center' style='padding: 20px 0 0 0; font-size: 10px; line-height: 15px; font-family: Helvetica, Arial, sans-serif; color: #666666;' class='padding-copy'>TraderKerala takes your account security very seriously.TraderKerala will never email you and ask you to disclose or verify your TraderKerala password, credit card, or banking account number.If you receive a suspicious email with a link to update your account information, do not click on the link—instead, report the email to TraderKerala for investigation.We hope to see you again soon</td></tr></table></td></tr></table></td></tr></table></td></tr></table></td></tr></table></body>";
                SendMail(body, email, subject);
            }
            catch
            {
                ViewBag.Message = "Vendor " + name + " is Not Added to vendorlist!!";
            }


            return Redirect("/Admin/ViewSeller?Msg=" + ViewBag.Message);
        }

        public ActionResult ViewSellerimage(int id, string name)
        {

            try
            {
                ViewBag.Viewseller = db.tbl_Vendor.Where(i => i.Vendor_id == id).FirstOrDefault();
            }
            catch { }
            return View();
        }


        public ActionResult PrebookingView()
        {
            try { }
            catch { }
            return View();
        }

        public ActionResult deletePrebookingView(int id, string name)
        {

            try
            {
                tbl_PreBooking tbC = db.tbl_PreBooking.Where(i => i.Pre_Id == id).FirstOrDefault();
                db.tbl_PreBooking.Remove(tbC);
                db.SaveChanges();
                ViewBag.Message = "Pre Booking of" + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Pre Booking of" + name + " is Not Deleted Successfully!!";
            }

            return Redirect("/Admin/PrebookingView?Msg=" + ViewBag.Message);
        }

        public ActionResult confirmbooking(int id, string name)
        {

            try
            {
                tbl_PreBooking tbC = db.tbl_PreBooking.Where(i => i.Pre_Id == id).FirstOrDefault();
                tbC.Status = "Confirmed";
                db.SaveChanges();
                ViewBag.Message = "Pre Booking of " + name + " is Confirmed Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Pre Booking of " + name + " is Not Confirmed!!";
            }

            return Redirect("/Admin/PrebookingView?Msg=" + ViewBag.Message);
        }

        [HttpGet]
        public ActionResult AddJob()
        {
            try
            {
                ViewBag.jobdes = db.tbl_Job.ToList();
            }
            catch { }
            return View();
        }

        [HttpPost]
        public ActionResult AddJob(FormCollection Collect)
        {
            try
            {
                tbl_Job jb = new tbl_Job();
                jb.Job_Name = Collect["txtPost"];
                jb.job_Category = Collect["Category"];
                jb.Job_Description = Collect["txtDesc1"];
                jb.key_skills = Collect["txtKey"];
                jb.Experience = Collect["txtExperience"];
                jb.location = Collect["Location"];
                jb.Posted_date = Convert.ToDateTime(Collect["txtPdate"]);
                jb.End_Date = Convert.ToDateTime(Collect["txtEdate"]);
                db.tbl_Job.Add(jb);
                db.SaveChanges();
                ViewBag.Message = "Job description are added succesfully";
            }
            catch
            {
                ViewBag.Message = "Job description are Not Added";
            }

            return Redirect("~/Admin/AddJob?Msg=" + ViewBag.Message);
        }

        public ActionResult Deletejob(int id, string name)
        {

            try
            {
                tbl_Job tbC = db.tbl_Job.Where(i => i.Job_Id == id).FirstOrDefault();
                db.tbl_Job.Remove(tbC);
                db.SaveChanges();
                ViewBag.Message = "Post Of" + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Post Of" + name + " is Not Deleted Successfully!!";
            }

            return Redirect("~/Admin/AddJob?Msg=" + ViewBag.Message);
        }
        [HttpGet]
        public ActionResult Editjob(int id, string name)
        {
            try
            {
                ViewBag.jobdes = db.tbl_Job.Where(i => i.Job_Id == id).FirstOrDefault();
            }
            catch
            {

            }
            return View();
        }
        [HttpPost]
        public ActionResult Editjob(FormCollection Collect)
        {
            try
            {
                int id = Convert.ToInt32(Collect["HiddenId"]);
                tbl_Job jb = db.tbl_Job.Where(i => i.Job_Id == id).FirstOrDefault();
                jb.Job_Name = Collect["txtPost"];
                jb.key_skills = Collect["txtKey"];
                jb.Experience = Collect["txtExperience"];
                jb.location = Collect["Location"];
                jb.job_Category = Collect["Category"];
                jb.Job_Description = Collect["txtDesc1"];
                jb.Posted_date = Convert.ToDateTime(Collect["txtPdate"]);
                jb.End_Date = Convert.ToDateTime(Collect["txtEdate"]);
                db.SaveChanges();
                ViewBag.Message = "Job description are Updated succesfully";
            }
            catch
            {

                ViewBag.Message = "Job description are not Upadated";
            }
            return Redirect("~/Admin/AddJob?Msg=" + ViewBag.Message);
        }
        [HttpGet]
        public ActionResult JobSeekers()
        {
            try
            {

            }
            catch { }
            return View();

        }

        public ActionResult DeleteJobseeker(int id)
        {
            try
            {
                tbl_JobSeeker tbC = db.tbl_JobSeeker.Where(i => i.Seeker_Id == id).FirstOrDefault();
                db.tbl_JobSeeker.Remove(tbC);
                db.SaveChanges();
                ViewBag.Message = "Application For the job  is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Application For the job is Not Deleted Successfully!!";
            }

            return Redirect("~/Admin/JobSeekers?Msg=" + ViewBag.Message);
        }



        [HttpGet]
        public ActionResult OrganicCertificte()
        {
            try
            { }
            catch
            {

            }
            return View();
         }

        [HttpPost]
        public ActionResult OrganicCertificte(FormCollection Collect,HttpPostedFileBase file1)
        {
            try
            {
                string organicImage = "";
                string ImageName = DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string ImagePath = Server.MapPath("~/images/Brand/");
                DateTime date = DateTime.Now.Date;

                if (file1 != null)
                {
                    string ext = Path.GetExtension(file1.FileName).ToLower();

                    if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                    {
                        file1.SaveAs(ImagePath + ImageName + ext);
                        organicImage = "/images/Brand/" + ImageName + ext;


                        tbl_Organic org = new tbl_Organic();
                        org.Organic_name = Collect["txtname"];
                        org.Description = Collect["txtDesc1"];
                        org.Image = organicImage;
                        db.tbl_Organic.Add(org);
                        db.SaveChanges();
                        ViewBag.Message = "Organic Certificate Added Successfully!!";

                    }
                    else
                    {
                        ViewBag.Message = "Organic Certificate Not Added Successfully!!";
                    }
                }
                else
                {
                    ViewBag.Message = "Image is not Uploaded";
                }

                

            }
            catch (Exception ex)
            {
               
            }

            return Redirect("~/Admin/OrganicCertificte?Msg=" + ViewBag.Message);
        }
        
        public ActionResult Deleteorganic(int id,string name)
        {

            try
            {
                tbl_Organic tb = db.tbl_Organic.Where(i => i.Organic_Id == id).FirstOrDefault();
                db.tbl_Organic.Remove(tb);
                db.SaveChanges();
                ViewBag.Message = "Organic Certificate " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Organic Certificate " + name + " is Not Deleted Successfully!!";
            }

            return Redirect("~/Admin/OrganicCertificte?Msg=" + ViewBag.Message);
        }

        [HttpGet]
        public ActionResult Editorganic(int id)
        {
            ViewBag.Editorganic = db.tbl_Organic.Where(i => i.Organic_Id == id).FirstOrDefault();
            return View();
        }

        [HttpPost]
        public ActionResult Editorganic(FormCollection collect, HttpPostedFileBase file1)
        {
            int id = Convert.ToInt32(collect["HiddenId"]);
            string OrganicName = collect["txtname"];
            string OrganicLogo = "";
            string OrganicDesc = collect["txtDesc1"];
            string ImagePath = Server.MapPath("~/images/Brand/");

            try
            {



                if (file1 != null)
                {
                    string ext = Path.GetExtension(file1.FileName).ToLower();
                    string url = ImagePath + OrganicName + ext;

                    if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif")
                    {
                        file1.SaveAs(url);
                        OrganicLogo = "/images/Brand/" + OrganicName + ext;
                        tbl_Organic tb = db.tbl_Organic.Where(o => o.Organic_Id == id).FirstOrDefault();
                        tb.Organic_name = OrganicName;
                        tb.Image = OrganicLogo;
                        tb.Description = OrganicDesc;
                        db.SaveChanges();
                        ViewBag.Message = "Organic Certificate " + OrganicName + " Is Updated Successfully!!";
                    }
                    else
                    {
                        ViewBag.Message = "Organic Certificate  Not Updated !!";
                    }
                }
                else
                {
                    tbl_Organic tb = db.tbl_Organic.Where(o => o.Organic_Id == id).FirstOrDefault();
                    tb.Organic_name = OrganicName;
                    tb.Description = OrganicDesc;
                    db.SaveChanges();
                    ViewBag.Message = "Organic Certificate " + OrganicName + " Is Updated Successfully!!";

                }


            }
            catch
            {
                ViewBag.Message = "Organic Certificate  Not Updated!!";
            }

            return Redirect("~/Admin/OrganicCertificte?Msg=" + ViewBag.Message);
        }

        [HttpGet]
        public ActionResult Review()
        {
            return View();
        }



        public ActionResult DeleteReview(int id, string name)
        {

            try
            {
                tblreviews tbC = db.tblreviews.Where(i => i.id == id).FirstOrDefault();
                db.tblreviews.Remove(tbC);
                db.SaveChanges();
                ViewBag.Message = "Review of " + name + " is Deleted Successfully!!";

            }
            catch
            {
                ViewBag.Message = "Rveiew" + name + " is Not Deleted Successfully!!";
            }

            return Redirect("/Admin/Review?Msg=" + ViewBag.Message);
        }

        [HttpGet]
        public ActionResult ViewReview(int id)
        {
            ViewBag.ViewReview= db.tblreviews.Where(i => i.id == id).FirstOrDefault();


            return View();
        }

        [HttpGet]
        public ActionResult AddReview()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddReview(FormCollection c1)
        {
            try
            {
                string date = System.DateTime.Now.ToShortDateString();
                string name = c1["Name"];
                int pro = Convert.ToInt16(c1["Related"]);
                string phone = "9876543210";
                string reviews = c1["Review"];
                string star = c1["rating"];


                tblreviews tr = new tblreviews();
                tr.prodid = pro;
                tr.starValue = Convert.ToInt16(star);
                tr.date = date;
                tr.name = name;
                tr.email = "Username@gmail.com";
                tr.phone = phone;
                tr.reviews = reviews;
                tr.type = "admin";
                db.tblreviews.Add(tr);
                db.SaveChanges();
                ViewBag.Message = "Review Submitted successfully";

            }
            catch
            {
                ViewBag.Message = "Review not Submitted";
            }
            return Redirect("/Admin/AddReview?Msg=" + ViewBag.Message);
        }


        [HttpGet]
        public ActionResult changepassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult changepassword(FormCollection collect)
        {
            string Username = collect["log_email"];
            string Password = collect["Password"];
            string New_Pword = collect["New_password"];
            string ConfirmNew_Pword = collect["Re_newpassword"];
            ViewBag.email = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
            try
            {
                if (New_Pword == ConfirmNew_Pword)
                {
                    int Count = db.tblLogins.Where(i => i.username == Username && i.password == Password).Count();

                    if (Count > 0)
                    {
                        tblLogin log = db.tblLogins.Where(i => i.username == Username).FirstOrDefault();
                        log.password = ConfirmNew_Pword;
                        log.lastlogin = Convert.ToDateTime(DateTime.Now);
                        db.SaveChanges();
                        ViewBag.Message = "Password Changed successfully";
                    }
                    else
                    {
                        ViewBag.Message = "Miss matching In your credentials.Please verify";
                    }

                }
                else
                {
                    ViewBag.Message = "Miss matching In password.Please Retype your Password";
                }
            }
            catch
            {
                ViewBag.Message = "Miss matching In your credentials.Please verify";
            }
            return Redirect("/Admin/changepassword?Msg=" + ViewBag.Message);
        }

        [HttpGet]
        public ActionResult UserReport()
        {
            ViewBag.reg = db.tblRegistration.ToList();
            return View();
        }

       
        public ActionResult OrderView()
        {
            ViewBag.order = db.tblOrders.ToList();
            return View();
        }


        #region Methods
        public JsonResult GetSubcategory(int ParentId)
        {
            //  List<tbl_City> City = db.tbl_City.Where(i => i.CountryID == country).ToList();
            List<tblCategory> CategoryList = db.tblCategories.Where(y => y.ParentId == ParentId).ToList();
            List<string> st = new List<string>();
            foreach (tblCategory c in CategoryList)
            {
                string s = c.Catid.ToString() + "-" + c.CatName;
                st.Add(s);
            }

            return Json(st.Distinct(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult KeywordList(string Prefix)
        {
            Prefix = Prefix.ToLower();

            List<KeywordClass> keyList = new List<KeywordClass>();


            foreach (tblProduct tp in db.tblProduct.Where(i => i.ProdName.Contains(Prefix)).ToList())
            {
                KeywordClass k = new KeywordClass();
                k.label = tp.ProdName;
                k.value = tp.pid.ToString();
                keyList.Add(k);

            }


            return this.Json(keyList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ViewProductDetails(int Id)
        {
            //  List<tbl_City> City = db.tbl_City.Where(i => i.CountryID == country).ToList();
            List<tblProduct> ProductList = db.tblProduct.Where(y => y.pid == Id).ToList();
            //List<string> st = new List<string>();

            return Json(ProductList.Distinct(), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
    #region Classes
    //public class KeywordClass
    //{
    //    public String label { get; set; }
    //    public String value { get; set; }
    //}

    #endregion
}