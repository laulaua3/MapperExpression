using MapperExemple.Entity;
using MapperExemple.Web.Models;
using MapperExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MapperExpression.Extensions;
using Microsoft.Practices.ServiceLocation;
using MapperExemple.Entity.Interface;

namespace MapperExemple.Web.Controllers
{
    //see  MappingConfig class in App_Start for create the mapping
    public class HomeController : Controller
    {
        private const int nbItemPerPage = 10;
        #region Simple Mapping

        public ActionResult Index()
        {

            ExempleCustomer exemple1 = new ExempleCustomer();

            Customer result = exemple1.GetFirstCustomer();
            //exemple to Map a object
            var model = Mapper.Map<Customer, CustomerModel>(result);


            ViewBag.Message = "Simple exemple to map Customer to CustomerModel";
            return View(model);
        }


        public ActionResult Exemple2()
        {
            ViewBag.Message = "Mapping a List";
            //this exemple show how map a list of customer to customerModel
            ExempleCustomer exemple2 = new ExempleCustomer();

            var result = exemple2.GetCustomersList();
            //Map to list
            var model = result.Select(Mapper.GetQuery<Customer, CustomerModel>());

            return View(model);
        }
        public ActionResult Exemple3()
        {
            ViewBag.Message = "Mapping a IQueryable";
            //this exemple show how map a IQueryable of customer to List of customerModel
            ExempleCustomer exemple3 = new ExempleCustomer();

            var result = exemple3.GetCustomers();
            //Map to list with using the select extention of the mapper
            var model = result.Select<Customer, CustomerModel>().ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult Exemple4()
        {
            //Default page
            ViewBag.Message = "Exemple for OrderBy extentions";
            //this exemple show how map a IQueryable of customer to List of customerModel
            ExempleCustomer exemple4 = new ExempleCustomer();

            var result = exemple4.GetCustomers();

            SortedAndPagingCustomerModel model = new SortedAndPagingCustomerModel();
            model.PageIndex = 0;
            model.Customers = result.Select<Customer, CustomerModel>().Take(nbItemPerPage).ToList();
            model.NumberOfPage = Math.Round(Convert.ToDouble(result.Count() / nbItemPerPage));
            return View(model);
        }

        [HttpPost]
        public ActionResult Exemple4(SortedAndPagingCustomerModel model)
        {
            ViewBag.Message = "Exemple for OrderBy extentions";
            //this exemple show how map a IQueryable of customer with the OrderBy extention
            ExempleCustomer exemple4 = new ExempleCustomer();
            model.PageIndex = model.PageIndex == 0 ? 0 : model.PageIndex - 1;
            var result = exemple4.GetCustomers();
            var skipValue = model.PageIndex * nbItemPerPage;
            if (skipValue < 0)
                skipValue = nbItemPerPage;
            //ThenBy and ThenByDescending are also implemented
            model.NumberOfPage = Math.Round(Convert.ToDouble(result.Count() / nbItemPerPage));
            if (!string.IsNullOrEmpty(model.SortDirection))
            {
                if (model.SortDirection == "ascending")
                {
                    //this create a sql request include ORDER BY, see the console output to see the request.
                    model.Customers = result
                        .OrderBy<Customer, CustomerModel>(model.SortField)
                        .Skip(skipValue)
                        .Take(nbItemPerPage)
                        .Select<Customer, CustomerModel>()
                        .ToList();
                }
                else
                {
                    model.Customers = result
                        .OrderByDescending<Customer, CustomerModel>(model.SortField)
                        .Skip(skipValue)
                        .Take(nbItemPerPage)
                        .Select<Customer, CustomerModel>()
                        .ToList();
                }
            }
            else
            {
                model.Customers = result
                        .OrderBy(x => x.CustomerId)
                        .Skip(skipValue)
                        .Take(nbItemPerPage)
                        .Select<Customer, CustomerModel>()
                        .ToList();
            }



            return View(model);
        }


        #endregion

        #region Mapping with sub object

        public ActionResult Exemple5()
        {
            //Default page
            ViewBag.Message = "Exemple for custom mapping";
            //this exemple show the map with custom mapping
            ExempleOrder exemple5 = new ExempleOrder();

            var result = exemple5.GetFirstOrder();

            var model = Mapper.Map<Order, OrderModel>(result);

            return View(model);
        }
        public ActionResult Exemple6()
        {
            //Default page
            ViewBag.Message = "Exemple for custom mapping";
            //this exemple show how map a IQueryable to List  with custom mapping
            ExempleOrder exemple6 = new ExempleOrder();

            var result = exemple6.GetOrders();
            //see sql request in console output
            var model = result.Select<Order, OrderModel>().ToList();

            return View(model);
        }

        #endregion

        #region Ioc

        public ActionResult Exemple7()
        {
            //Default page
            ViewBag.Message = "Exemple with ioc";
            //this exemple show the map  with ioc
            IExempleProduct exemple7 = ServiceLocator.Current.GetInstance<IExempleProduct>();

            var result = exemple7.GetFirstProduct();

            var model = Mapper.Map<IExempleProduct, ProductModel>(result);

            return View(model);
        }

        public ActionResult Exemple8()
        {
            //Default page
            ViewBag.Message = "Exemple map a list with ioc";

            IExempleProduct exemple8 = ServiceLocator.Current.GetInstance<IExempleProduct>();
            //Exemple map a list with ioc
            var result = exemple8.GetProductsList();

            var model = result.Select(Mapper.GetQuery<IExempleProduct, ProductModel>());

            return View(model);
        }

        public ActionResult Exemple9()
        {
            //Default page
            ViewBag.Message = "Exemple map a IQueryable with ioc";

            IExempleProduct exemple9 = ServiceLocator.Current.GetInstance<IExempleProduct>();
            //Exemple map a IQueryable with ioc
            var result = exemple9.GetProducts();

            var model = result.Select<IExempleProduct, ProductModel>().ToList();

            return View(model);
        }
        public ActionResult Exemple10()
        {
            //Default page
            ViewBag.Message = "Other exemple map a IQueryable";

            IExempleProduct exemple10 = ServiceLocator.Current.GetInstance<IExempleProduct>();
            //Exemple map a IQueryable 
            //Mapper.CreateMap<Product, ProductModel>();
            var result = exemple10.GetProducts2(Mapper.GetQueryExpression<Product, ProductModel>());


            var model = result;

            return View(model);
        }

        #endregion

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}