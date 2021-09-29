using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using E_Commerce.Models;

namespace E_Commerce.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            GetProductList();
            return View("ProductList");
        }

        public ActionResult ProductDetail()
        {
            return View();
        }

        public ActionResult CreateProduct()
        {
            return View();
        }

        public ActionResult ViewProduct()
        {
            return View("ViewProduct");
        }
        
        public ActionResult CreateProductDetails(ProductDetailsViewModel productDetails, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new Models.DEMO_ECOMMERCEEntities())
                    {
                        var IsProductNameExists = (from cus in db.PRODUCTS where cus.PRD_NAME == productDetails.ProductName select cus.PRD_ID).Any();
                        if (IsProductNameExists)
                        {
                            ModelState.AddModelError(string.Empty, "Product Name already exists");
                            ViewBag.Message = "Product Name already exists";
                            return View("CreateProduct");
                        }
                        string FileName = string.Empty;
                        if (file != null)
                        {
                            FileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string FileExtension = Path.GetExtension(file.FileName);
                            FileName = FileName + DateTime.Now.ToString("yymmssfff") + FileExtension;
                            productDetails.ProductImage =  FileName;
                            FileName = Path.Combine(Server.MapPath("~/Images/"), FileName);
                            file.SaveAs(FileName);
                        }
                        int UserId;
                        int.TryParse(Session[Common.SESSION_USERID] + string.Empty, out UserId);

                        PRODUCT product = new PRODUCT();
                        product.PRD_NAME = productDetails.ProductName;
                        product.PRD_PRICE = productDetails.Price;
                        product.PRD_IMGNAME = productDetails.ProductImage;
                        product.PRD_CREATEDBY = UserId;
                        product.PRD_CREATEDAT = DateTime.Now;

                        db.PRODUCTS.Add(product);
                        db.SaveChanges();
                    }
                }
                else
                {
                    return View("CreateProduct");
                }
                return RedirectToAction("Index");
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

        public ActionResult EditProduct(int? ID)
        {
            try
            {
                using (var db = new Models.DEMO_ECOMMERCEEntities())
                {
                    ProductDetailsViewModel prd = (from product in db.PRODUCTS
                                select new ProductDetailsViewModel()
                                {
                                    ProductName = product.PRD_NAME,
                                    Price = product.PRD_PRICE,
                                    ProductImage = product.PRD_IMGNAME,
                                    ProductId = product.PRD_ID
                                }).FirstOrDefault();
                    string ProductImgPath = Server.MapPath("//Images//" + prd.ProductImage);
                    ViewBag.ProductImagePath = ProductImgPath;
                    return View(prd);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult SearchProduct(string ProductName)
        {
            try
            {
                GetProductList(ProductName);
            }
            catch (Exception)
            {
                throw;
            }
            return View("ProductList");
        }

        private void GetProductList(string ProductName = "")
        {
            try
            {
                using (var db = new Models.DEMO_ECOMMERCEEntities())
                {
                    ProductDetailsViewModel prd = new ProductDetailsViewModel();
                    if (ProductName == "")
                    {
                        var prds = (from product in db.PRODUCTS
                                    select new ProductDetailsViewModel()
                                    {
                                        ProductName = product.PRD_NAME,
                                        Price = product.PRD_PRICE,
                                        ProductImage = product.PRD_IMGNAME,
                                        ProductId = product.PRD_ID
                                    }).OrderBy(x => x.ProductName).ToList();
                        if (prds != null)
                        {
                            BindProductlis(prds);
                        }

                    }
                    else
                    {
                        var prds = (from product in db.PRODUCTS
                                    where product.PRD_NAME.Contains(ProductName)
                                    select new ProductDetailsViewModel()
                                    {
                                        ProductName = product.PRD_NAME,
                                        Price = product.PRD_PRICE,
                                        ProductImage = product.PRD_IMGNAME,
                                        ProductId = product.PRD_ID
                                    }).OrderBy(x => x.ProductName).ToList();
                        if (prds != null)
                        {
                            BindProductlis(prds);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ModelState.Clear();
            }
        }

        public void BindProductlis(List<ProductDetailsViewModel> prds)
        {
            try
            {
                using (var db = new Models.DEMO_ECOMMERCEEntities())
                {
                    int index = 0;
                    StringBuilder sb = new StringBuilder();
                    string ProductImagePath = "";
                    int userid;
                    int.TryParse(Session[Common.SESSION_USERID] + string.Empty, out userid);
                    foreach (var p in prds)
                    {
                        if (index == 0)
                        {
                            sb.Append("<div class='col-md-12'>");
                        }
                        sb.Append("<div class='col-md-4' style='border:unset'>");
                        sb.Append("<form class='form-horizontal'>");
                        if (!string.IsNullOrEmpty(Session[Common.SESSION_ISADMIN] + string.Empty))
                        {
                            if (Convert.ToBoolean(Session[Common.SESSION_ISADMIN] + string.Empty))
                            {
                                sb.Append("<span class='spnProduct' style='cursor:pointer;' id='" + p.ProductId + "' onclick=EditProduct(this)>" + p.ProductName + "</span>");
                                sb.Append("<span class='spnDelete' style='cursor:pointer;' title='Delete' id='" + p.ProductId + "' onclick='DeleteProduct(this)' title='delete product'><i class='fa fa-trash'></i></span>");
                            }
                            else
                            {
                                sb.Append("<span class='spnProduct'>" + p.ProductName + "</span>");

                            }
                        }
                        else
                        {
                            sb.Append("<span class='spnProduct'>" + p.ProductName + "</span>");
                        }

                        if (!string.IsNullOrEmpty(p.ProductImage))
                        {
                            ProductImagePath = p.ProductImage;
                            if (!System.IO.File.Exists(Server.MapPath("//Images//"+ProductImagePath)))
                            {
                                ProductImagePath = "../Images/NoImage.jpg";
                            }
                            else
                            {
                                ProductImagePath = "../Images/" + p.ProductImage;
                            }
                        }
                        else
                        {
                            ProductImagePath = "../Images/NoImage.jpg";
                        }

                        sb.Append("<img src='" + ProductImagePath + "'  class='img-responsive' alt='Responsive image' width='307' height='240' id='" + p.ProductId + "'/>");

                        sb.Append("<br><span class='spnPrice'>Price : ₹ " + p.Price + " </span>");

                        var IsAlreadyProductAdded = (from upl in db.USERSPRODUCTLISTs
                                                     where upl.PDL_PRODUCTID == p.ProductId && upl.PDL_USERID == userid
                                                     select upl.PDL_ID).Any();
                        if (!IsAlreadyProductAdded)
                        {
                            sb.Append("<span class='spnCard' style='cursor:pointer;' title='Add to Card' id='" + p.ProductId + "' onclick='AddToCard(this)'><i class='fa fa-cart-arrow-down'></i></span>");
                        }
                        else
                        {
                            sb.Append("<span class='spnCardadded' title='Added to Card'><i class='fa fa-check-circle'></i></span>");
                        }
                        sb.Append("</form>");
                        sb.Append("</div>");

                        if (index == 2)
                        {
                            sb.Append("</div><div class='col-md-12' style='padding-top: 30px;'></div>");
                            index = 0;
                        }
                        else
                        {
                            index = index + 1;
                        }
                    }

                    if (index < 2)
                    {
                        sb.Append("</div>");
                    }

                    ViewBag.ProductList = sb.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult Order(int ID)
        {
            try
            {
                using (var db = new Models.DEMO_ECOMMERCEEntities())
                {
                    USERSPRODUCTLIST usp = new USERSPRODUCTLIST();
                    usp.PDL_PRODUCTID = ID;
                    usp.PDL_USERID = Convert.ToInt32(Session[Common.SESSION_USERID]);
                    usp.PDL_CREATEDAT = DateTime.Now;
                    db.USERSPRODUCTLISTs.Add(usp);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                ModelState.Clear();
            }
            return Json(data: "Ordered", behavior: JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int? ID)
        {
            try
            {
                using (DEMO_ECOMMERCEEntities context = new DEMO_ECOMMERCEEntities())
                {
                    var ProdcutListID = (from up in context.USERSPRODUCTLISTs
                                               where up.PDL_PRODUCTID == ID
                                               select up.PDL_ID).FirstOrDefault();

                    var userproduct = context.USERSPRODUCTLISTs.Find(ProdcutListID);
                    if (userproduct != null)
                    {
                        context.USERSPRODUCTLISTs.Remove(userproduct);
                        context.SaveChanges();
                    }
                  
                    var product = context.PRODUCTS.Find(ID);
                    if (product != null)
                    {
                        context.PRODUCTS.Remove(product);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(data: "not Deleted", behavior: JsonRequestBehavior.AllowGet);
                throw;
            }
            finally
            {
                ModelState.Clear();
            }
            return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveProduct(int? ID)
        {
            try
            {
                using (DEMO_ECOMMERCEEntities context = new DEMO_ECOMMERCEEntities())
                {
                    var ProdcutListID = (from up in context.USERSPRODUCTLISTs
                                         where up.PDL_PRODUCTID == ID
                                         select up.PDL_ID).FirstOrDefault();

                    var userproduct = context.USERSPRODUCTLISTs.Find(ProdcutListID);
                    if (userproduct != null)
                    {
                        context.USERSPRODUCTLISTs.Remove(userproduct);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(data: "not Removed", behavior: JsonRequestBehavior.AllowGet);
            }
            finally
            {
                ModelState.Clear();
            }
            return Json(data: "Removed", behavior: JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(int? ID, ProductDetailsViewModel prdview, HttpPostedFileBase file)
        {
            try
            {
                using (DEMO_ECOMMERCEEntities db = new DEMO_ECOMMERCEEntities())
                {
                    var IsProductNameExists = (from pd in db.PRODUCTS
                                               where pd.PRD_NAME == prdview.ProductName
                        && pd.PRD_ID != ID
                                               select pd.PRD_ID).Any();
                    if (IsProductNameExists)
                    {
                        ViewBag.Message = "Product name already exists";
                        return View("EditProduct");
                    }

                    if (ID == null)
                    {
                        ViewBag.Message = "Can't Update";
                        return View("EditProduct");
                    }
                       
                    var prd = db.PRODUCTS.Find(ID);

                    if (prd != null)
                    {
                        string FileName = string.Empty;
                        if (file != null)
                        {
                            FileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string FileExtension = Path.GetExtension(file.FileName);
                            FileName = FileName + DateTime.Now.ToString("yymmssfff") + FileExtension;
                            prdview.ProductImage = FileName;
                            FileName = Path.Combine(Server.MapPath("~/Images/"), FileName);
                            file.SaveAs(FileName);
                        }
                        int UserId;
                        int.TryParse(Session[Common.SESSION_USERID] + string.Empty, out UserId);

                        prd.PRD_NAME = prdview.ProductName;
                        prd.PRD_PRICE = prdview.Price;
                        prd.PRD_IMGNAME = prdview.ProductImage;
                        prd.PRD_UPDATEDAT = DateTime.Now;
                        prd.PRD_UPDATEDBY = UserId;
                        db.Entry(prd).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.Message = "Content Updated Successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "No data found";
                        return View("EditProduct");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
             finally
            {
                ModelState.Clear();
            }
        }

        public ActionResult CheckOut()
        {
            using (DEMO_ECOMMERCEEntities context = new DEMO_ECOMMERCEEntities())
            {
                int userid = 0;
                int.TryParse(Session[Common.SESSION_USERID] + string.Empty, out userid);
                ProductDetailsViewModel prd = new ProductDetailsViewModel();
                var prds = (from product in context.PRODUCTS
                            join user in context.USERSPRODUCTLISTs on product.PRD_ID equals user.PDL_PRODUCTID
                            where user.PDL_USERID == userid
                            select new ProductDetailsViewModel()
                            {
                                ProductName = product.PRD_NAME,
                                Price = product.PRD_PRICE,
                                ProductImage = product.PRD_IMGNAME,
                                ProductId = product.PRD_ID
                            }).OrderBy(x => x.ProductName).ToList();
                return View(prds);
            }
        }
        
        public JsonResult PlaceOrder()
        {
            try
            {
                int userid;
                int.TryParse(Session[Common.SESSION_USERID] + string.Empty, out userid);
                using (DEMO_ECOMMERCEEntities context = new DEMO_ECOMMERCEEntities())
                {
                    var ProdcutListID = (from up in context.USERSPRODUCTLISTs
                                         where up.PDL_USERID == userid
                                         select up.PDL_ID).ToList();
                    foreach(var id in ProdcutListID)
                    {
                        var userproduct = context.USERSPRODUCTLISTs.Find(id);
                        if (userproduct != null)
                        {
                            context.USERSPRODUCTLISTs.Remove(userproduct);
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(data: "not Deleted", behavior: JsonRequestBehavior.AllowGet);
                throw;
            }
            finally
            {
                ModelState.Clear();
            }
            return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
        }

        public ActionResult Thankyou()
        {
            return View();
        }
    }
}