using DonanımPazar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Web.UI;


namespace DonanımPazar.Controllers
{
    public class AdminController : Controller
    {
        DonanımPazarEntities db = new DonanımPazarEntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tbl_admin avm)
        {
            tbl_admin ad = db.tbl_admin.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();
            if (ad != null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("ViewCategory");
            }
            else
            {
                ViewBag.error = "Geçersiz Kullanıcı Adı veya Şifre";
            }

            return View();
        }

        public ActionResult Create()
        {
            if (Session["ad_id"]==null)
            {
                return RedirectToAction("login");

            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(tbl_category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Resim Yüklenemedi....";
            }
            else
            {
                tbl_category cat = new tbl_category();
                cat.cat_name = cvm.cat_name;
                cat.cat_image = path;
                cat.cat_status = 1;
                cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                db.tbl_category.Add(cat);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }

            return View();
        }

        public ActionResult ViewCategory(int?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
            
        }

        public ActionResult ManageAds(int? page)
        {
            if (Session["ad_id"] == null)
            {

                return RedirectToAction("Login");
            }

            int pageSize = 10;
            int pageIndex = page ?? 1;

            
            var ads = db.tbl_product.OrderBy(x => x.pro_id).ToPagedList(pageIndex, pageSize);

            return View(ads);
        }


        [HttpPost]
        public ActionResult DeleteAd(int id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            var ad = db.tbl_category.FirstOrDefault(x => x.cat_id == id);

            if (ad != null)
            {
                db.tbl_category.Remove(ad);
                db.SaveChanges();
                return RedirectToAction("ManageAds");
            }
            else
            {
                ViewBag.Error = "Ilan bulunamadı";
                return View("ManageAds");
            }
        }

        public ActionResult ManageUsers()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            var users = db.tbl_user.ToList();
            return View(users);
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }

            var user = db.tbl_user.FirstOrDefault(x => x.u_id == id);

            if (user != null)
            {
                db.tbl_user.Remove(user);
                db.SaveChanges();
                return RedirectToAction("ManageUsers");
            }
            else
            {
                ViewBag.Error = "Kullanıcı bulunamadı";
                return View("ManageUsers");
            }
        }



        public string uploadimgfile(HttpPostedFileBase file)
            {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "/Content/upload/" + random + Path.GetFileName(file.FileName);

                           // ViewBag.Message = "Dosya Ekleme Başarılı";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Sadece jpg ,jpeg or png formatlar geçerlidir....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Lütfen Dosya Seçin'); </script>");
                path = "-1";
            }



            return path;
        }



    }
}