using MapperExemple.Entity;
using MapperExemple.Web.Models;
using MapperExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MapperExpression.Extensions;
namespace MapperExemple.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            ExempleEntity exemple1 = new ExempleEntity();

            Customer result = exemple1.GetFirstCustomer();
            //exemple to Map a object
            var model = Mapper.Map<Customer, CustomerModel>(result);


            ViewBag.Message = "Simple exemple to map Customer to CustomerModel";
            return View(model);
        }


        public ActionResult Exemple2()
        {
            ViewBag.Message = "Mapping of a List";
            //this exemple show how map a list of customer to customerModel
            ExempleEntity exemple2 = new ExempleEntity();

            var result = exemple2.GetCustomersList();
            //Map to list
            var model = result.Select(Mapper.GetQuery<Customer, CustomerModel>());

            return View(model);
        }
        public ActionResult Exemple3()
        {
            ViewBag.Message = "Mapping of a IQuerable";
            //this exemple show how map a IQuerable of customer to List of customerModel
            ExempleEntity exemple2 = new ExempleEntity();

            var result = exemple2.GetCustomers();
            //Map to list with using the select extention of the mapper
            var model = result.Select<Customer, CustomerModel>().ToList();

            return View("Exemple2", model);
        }

        [HttpGet]
        public ActionResult Exemple4()
        {
            //Default page
            ViewBag.Message = "Exemple for OrderBy extentions";
            //this exemple show how map a IQuerable of customer to List of customerModel
            ExempleEntity exemple4 = new ExempleEntity();

            var result = exemple4.GetCustomers();

            SortedCustomerModel model = new SortedCustomerModel();
           
            model.Customers = result.Select<Customer, CustomerModel>().ToList();

            return View(model);
        }
        [HttpPost]
        public ActionResult Exemple4(SortedCustomerModel model)
        {
            ViewBag.Message = "Exemple for OrderBy extentions";
            //this exemple show how map a IQuerable of customer with the OrderBy extention
            ExempleEntity exemple4 = new ExempleEntity();

            var result = exemple4.GetCustomers();
            if (model.SortDirection == "ascending")
            {
                //this create a sql request include ORDER BY, see the console output to see the request.
                model.Customers = result
                    .OrderBy<Customer, CustomerModel>(model.SortField)
                    .Select<Customer, CustomerModel>().ToList();
            }
            else
            {
                model.Customers = result
                    .OrderByDescending<Customer, CustomerModel>(model.SortField)
                    .Select<Customer, CustomerModel>().ToList();
            }
            
            //ThenBy and ThenByDescending are also implemented
            return View(model);
        }

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