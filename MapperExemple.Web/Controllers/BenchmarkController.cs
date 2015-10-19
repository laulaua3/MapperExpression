using FizzWare.NBuilder;
using MapperExemple.Entity;
using MapperExemple.Entity.EF;
using MapperExemple.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
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
        [Route("[controller]/[action]/[nbIteration]")]
        public ActionResult BenchMarkMapperExpression(int nbIteration)
        {
            BenchmarkModel result = new BenchmarkModel();
            result.Mapper = "MapperExpression";
            //Generate data
            Customer source = Builder<Customer>.CreateNew().Build();
            Stopwatch watcher = Stopwatch.StartNew();
            for (int i = 0; i < nbIteration; i++)
            {
                var model = MapperExpression.Mapper.Map<Customer, CustomerModel>(source);
            }
            watcher.Stop();
            result.TimeExecuting = watcher.Elapsed.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Route("[controller]/[action]/[nbIteration]")]
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

        [Route("[controller]/[action]/[nbIteration]")]
        public ActionResult BenchMarkValueInjecter(int nbIteration)
        {
            BenchmarkModel result = new BenchmarkModel();
            result.Mapper = "AutoMapper";
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
    }
}