using FizzWare.NBuilder;
using MapperExemple.Entity.EF;
using MapperExemple.Web.Models;
using System.Diagnostics;
using System.Web.Mvc;
using Omu.ValueInjecter;
namespace MapperExemple.Web.Controllers
{
    public class BenchmarkController : Controller
    {

        // GET: Benchmark
        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult BenchMarkMapperExpression(int nbIteration)
        {
            BenchmarkModel result = new BenchmarkModel();
            result.Mapper = "MapperExpression";
            //Generate data
            Customer source = Builder<Customer>.CreateNew().Build();
            Stopwatch watcher = Stopwatch.StartNew();
            for (int i = 0; i < nbIteration; i++)
            {
                var model = MapperExpression.Mapper<CustomerModel>.Map(source);
            }
            watcher.Stop();
            result.TimeExecuting = watcher.Elapsed.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult BenchMarkAutoMapper(int nbIteration)
        {
            BenchmarkModel result = new BenchmarkModel();
            result.Mapper = "AutoMapper";
            //Generate data
            Customer source = Builder<Customer>.CreateNew().Build();
            Stopwatch watcher = Stopwatch.StartNew();
            for (int i = 0; i < nbIteration; i++)
            {
                var model = AutoMapper.Mapper.Map<CustomerModel>(source);
            }
            watcher.Stop();
            result.TimeExecuting = watcher.Elapsed.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult BenchMarkValueInjecter(int nbIteration)
        {
            BenchmarkModel result = new BenchmarkModel();
            result.Mapper = "ValueInjecte";
            //Generate data
            Customer source = Builder<Customer>.CreateNew().Build();
            Stopwatch watcher = Stopwatch.StartNew();
            for (int i = 0; i < nbIteration; i++)
            {
                CustomerModel model = new CustomerModel();
                model.InjectFrom(source);
            }
            watcher.Stop();
            result.TimeExecuting = watcher.Elapsed.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult BenchMarkDirect(int nbIteration)
        {
            BenchmarkModel result = new BenchmarkModel();
            result.Mapper = "Direct";
            //Generate data
            Customer source = Builder<Customer>.CreateNew().Build();
            Stopwatch watcher = Stopwatch.StartNew();
            for (int i = 0; i < nbIteration; i++)
            {
                CustomerModel model = new CustomerModel()
                {
                    Address = source.Address,
                    City = source.City,
                    CompanyName = source.ContactName,
                    ContactName = source.ContactName,
                    ContactTitle = source.ContactTitle,
                    Country = source.Country,
                    Fax = source.Fax,
                    Phone = source.Phone,
                    PostalCode = source.PostalCode,
                    Region = source.Region
                };

            }
            watcher.Stop();
            result.TimeExecuting = watcher.Elapsed.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}