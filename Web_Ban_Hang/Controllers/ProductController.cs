using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Controllers
{
    public class ProductController : Controller
    {
        DBSportStoreEntities db = new DBSportStoreEntities();
        // GET: Product
        public ActionResult Index_Admin()
        {
            var products = db.Products.Include(p => p.Category1);
            return View(products.ToList());
        }      
        public ActionResult Index(string category,int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);
            if (category == null)
            {
                var productList = db.Products.OrderByDescending(x => x.NamePro);
                return View(productList.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var productList = db.Products.OrderByDescending(x => x.NamePro).Where(p => p.Category == category);
                return View(productList.ToPagedList(pageNum,pageSize));
            }
           
        }
        public ActionResult Create()
        {
            List<Category> list = db.Categories.ToList();
            ViewBag.listCategory = new SelectList(list, "IDCate", "NameCate", "");
            Product pro = new Product();
            return View(pro);
        }
        [HttpPost]
        public ActionResult Create(Product pro)
        {
            List<Category> list = db.Categories.ToList();
            try
            {
                if (pro.UploadImage != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(pro.UploadImage.FileName);
                    string extent = Path.GetExtension(pro.UploadImage.FileName);
                    filename = filename + extent;
                    pro.ImagePro = "~/Content/images/" + filename;
                    pro.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));
                }
                db.Products.Add(pro);
                db.SaveChanges();
                return RedirectToAction("Index_Admin");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            return View(db.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(int id, Product cate)
        {
            db.Entry(cate).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index_Admin");
        }
        public ActionResult Details(int id)
        {
            return View(db.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
        public ActionResult DetailCus(int id)
        {

            return View(db.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
        public ActionResult Remove(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartItem(id);
            return RedirectToAction("Index_Admin", "Product");
        }
        public ActionResult Delete(int id)
        {
            return View(db.Products.Where(s => s.ProductID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(int id, Product cate)
        {
            try
            {
                cate = db.Products.Where(s => s.ProductID == id).FirstOrDefault();
                db.Products.Remove(cate);
                db.SaveChanges();
                return RedirectToAction("Index_Admin");
            }
            catch
            {
                return Content("Xóa không thành công. Dữ liệu dã được sử dụng cho một bảng đã tồn tại");
            }
        }
        public ActionResult SelectCate()
        {
            Category se_cate = new Category();
            se_cate.ListCate = db.Categories.ToList<Category>();
            return PartialView(se_cate);
        }
        public ActionResult SearchOption(string category, int? page, double min = double.MinValue, double max = double.MaxValue)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);
            if (category == null)
            {
                var list = db.Products.OrderByDescending(x => x.NamePro);
                return View(list.ToPagedList(pageNum, pageSize));

            }
            else
            {
                var list = db.Products.Where(p => (double)p.Price >= min && (double)p.Price <= max).ToList();
                return View(list.ToPagedList(pageNum, pageSize));
            }
            //    var list = db.Products.Where(p => (double)p.Price >= min && (double)p.Price <= max).ToList();
            //return View(list);
        }
        //public ActionResult SearchOption(double min = double.MinValue, double max = double.MaxValue)
        //{
        //    var list = db.Products.Where(p => (double)p.Price >= min && (double)p.Price <= max).ToList();
        //    return View(list);
        //}
        public ActionResult Search(string _name)
        {
            if (_name == null)
                return View(db.Products.ToList());
            else
                return View(db.Products.Where(s => s.NamePro.Contains(_name)).ToList());
        }
        public ActionResult TopNew()
        {
            List<OrderDetail> orderD = db.OrderDetails.ToList();
            List<Product> proList = db.Products.ToList();
            var query = from od in proList
                        join p in proList on od.ProductID equals p.ProductID into tbl
                        group od by new
                        {
                            idPro = od.ProductID,
                            namePro = od.NamePro,
                            imagePro = od.ImagePro,
                            price = od.Price
                        } into gr
                        orderby gr.Max(s => s.ProductID) descending
                        select new ViewModel
                        {
                            IDPro = gr.Key.idPro,
                            NamePro = gr.Key.namePro,
                            ImgPro = gr.Key.imagePro,
                            pricePro = (decimal)gr.Key.price,
                            Top_New = gr.Max(s => s.ProductID)
                        };
            return PartialView(query.Take(5).ToList());
        }

        //public PartialViewResult PartialProduct(string NameCate, int? page, double min = double.MinValue, double max = double.MaxValue)
        //{
        //    int pageSize = 4;
        //    int pageNumber = (page ?? 1);
        //    if(NameCate == null)
        //    {
        //        var productList = db.Products.Where(x => x.Price != null && (double)x.Price >= min && (double)x.Price <= max).OrderByDescending(x => x.NamePro);
        //        return PartialView(productList.ToPagedList(pageNumber, pageSize));
        //    }    
        //    else
        //    {
        //        var productList = db.Products.OrderByDescending(x => x.NamePro).Where(p => p.Category == NameCate);
        //        return PartialView(productList);
        //    }    
        //}

        //if (category == null)
        //{
        //    var productList = db.Products.OrderByDescending(x => x.NamePro);
        //    return View(productList.ToPagedList(pageNum, pageSize));
        //}
        //else
        //{
        //    var productList = db.Products.OrderByDescending(x => x.Category).Where(x => x.Category == category);
        //    return View(productList.ToPagedList(pageNum, pageSize));
        //}
    }
}