﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Web_Ban_Hang.Models;

namespace Web_Ban_Hang.Controllers
{
    public class OrderDetailController : Controller
    {
        DBSportStoreEntities db = new DBSportStoreEntities();
        // GET: OrderDetail
        public ActionResult GroupByTop5()
        {
            List<OrderDetail> orderD = db.OrderDetails.ToList();
            List<Product> proList = db.Products.ToList();
            var query = from od in orderD
                        join p in proList on od.IDProduct equals p.ProductID into tbl
                        group od by new
                        {
                            idPro = od.IDProduct,
                            namePro = od.Product.NamePro,
                            imagePro = od.Product.ImagePro,
                            price = od.Product.Price,
                            desPro = od.Product.DecriptionPro
                        } into gr
                        orderby gr.Sum(s => s.Quantity) descending
                        select new ViewModel
                        {
                            IDPro = gr.Key.idPro,
                            NamePro = gr.Key.namePro,
                            ImgPro = gr.Key.imagePro,
                            pricePro = (decimal)gr.Key.price,
                            DesPro = gr.Key.desPro,
                            Sum_Quantity = gr.Sum(s => s.Quantity)
                        };
            return PartialView(query.Take(5).ToList());
        }
        public ActionResult Banner()
        {
            return PartialView();
        }
    }
}