using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList.Mvc;
using PagedList;
using eMarketing_project.Models;
using System.Data;
using System.Data.Entity;

namespace eMarketing_project.Controllers
{
    public class UserController : Controller
    {
        dbmarketingEntities db = new dbmarketingEntities();
        // GET: User
        public ActionResult Index(int? page)
        {  
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == true).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public string uploadingfile(HttpPostedFileBase file)       //help to upload file
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try    //if all conditions are correct,file will be uploaded in (~/Content/upload)
                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";            //indicates error and category will not be added in the database
            }
            return path;
        }

        [HttpGet]
        public ActionResult signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult signup(tbl_user uvm,HttpPostedFileBase imgfile)
        {
            string path = uploadingfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_password = uvm.u_password;
                u.u_email = uvm.u_email;
                u.u_image = path;
                u.u_contact = uvm.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }//method.......................end...................

        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_user avm)
        {
            tbl_user u = db.tbl_user.Where(x => x.u_name == avm.u_name && x.u_password == avm.u_password).SingleOrDefault();
            if (u != null)
            {
                Session["u_id"] = u.u_id.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Invalid Username or Password";
            }
            return View();
        }

        [HttpGet]
        public ActionResult createAD()
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult createAD(tbl_product pvm,HttpPostedFileBase imgfile)
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");

            string path = uploadingfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded";
            }
            else
            {
                tbl_product p = new tbl_product();
                p.pro_name = pvm.pro_name;
                p.pro_image = path;
                p.pro_des = pvm.pro_des;
                p.pro_price = pvm.pro_price;
                p.pro_status = true;
                p.pro_fk_cat = pvm.pro_fk_cat;
                p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
                db.tbl_product.Add(p);
                db.SaveChanges();
                Response.Redirect("index");
            }
            return View();
        }

        public ActionResult Ads(int ?id, int?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_fk_cat==id && x.pro_status == true).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        [HttpPost]
        public ActionResult Ads(int? id, int? page,string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
        public ActionResult ViewAd(int? id)
        {
            Adviewmodel ad = new Adviewmodel();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id ).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_name;
            ad.pro_price = p.pro_price;
            ad.pro_des = p.pro_des;
            ad.pro_image = p.pro_image;
            tbl_category cat = db.tbl_category.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ad.u_name = u.u_name;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;
            ad.pro_fk_user = u.u_id;


            return View(ad);
        }
        public ActionResult logout()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index");

        }

        public ActionResult deleteAD(int? id)
        {
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            //db.tbl_product.Remove(p);
            p.pro_status = false;
            db.Entry(p).State =EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Restock(int? id)
        {
            List<tbl_product> li = db.tbl_product.Where(x=>x.pro_status==false).ToList();
            //ViewBag.categorylist = new SelectList(li, "pro_id", "pro_name");
            return View(li);
            
        }
        [HttpGet]
        public ActionResult AfterRestock(int? id)
        {
            //List<tbl_product> li = db.tbl_product.Where(x => x.pro_status == ).ToList();
            //ViewBag.categorylist = new SelectList(li, "pro_id", "pro_name");
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            p.pro_status = true;
            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Restock");

        }
        [HttpGet]
        public ActionResult AddToCart()
        {
            int u_id = Convert.ToInt32(Session["u_id"].ToString());
            
            return View(allCartItems(u_id));
        }
        [HttpPost]
        public ActionResult AddToCart(Adviewmodel model)
        { 
            int u_id = Convert.ToInt32(Session["u_id"].ToString());
            tbl_cart cart = new tbl_cart();
            cart.pro_id = model.pro_id;
            cart.user_id = u_id;
            cart.Quantity = model.Quantity;
            cart.CartPro_Status = true;
            cart.AddedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            db.tbl_cart.Add(cart);
            db.SaveChanges();
            return View(allCartItems(u_id));
        }
        public ActionResult DeleteCartItem(int cart_id)
        {

            tbl_cart c = db.tbl_cart.Where(x => x.cart_id == cart_id).SingleOrDefault();
            c.CartPro_Status = false;
            db.Entry(c).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AddToCart");
        }
        public List<tbl_cart> allCartItems(int u_id)
        {
            List<tbl_cart> cartlist = db.tbl_cart.Where(X => X.user_id == u_id && X.CartPro_Status == true).ToList();

            foreach (tbl_cart c in cartlist)
            {
                tbl_product p = db.tbl_product.Where(x => x.pro_id == c.pro_id).SingleOrDefault();

                c.pro_name = p.pro_name;
            }
            return cartlist;
        }

    }
}