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

namespace MapperExemple.Web.Controllers
{
    public class BenchmarkController : Controller
    {
        
        // GET: Benchmark
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult BenchMarkMapperExpression(int id)
        {
            BenchmarkModel result = new BenchmarkModel();
            result.Mapper = "MapperExpression";
            //Generate data
            Customer source = Builder<Customer>.CreateNew().Build();
            Stopwatch watcher = Stopwatch.StartNew();
            for (int i = 0; i < id; i++)
            {
                var model = MapperExpression.Mapper.Map<Customer, CustomerModel>(source);
            }
            watcher.Stop();
            result.TimeExecuting = watcher.Elapsed.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
 
        public ActionResult BenchMarkAutoMapper(int id)
        {
            BenchmarkModel result = new BenchmarkModel();
            result.Mapper = "AutoMapper";
            //Generate data
            Customer source = Builder<Customer>.CreateNew().Build();
            Stopwatch watcher = Stopwatch.StartNew();
            for (int i = 0; i < id; i++)
            {
                var model = AutoMapper.Mapper.Map<CustomerModel>(source);
            }
            watcher.Stop();
            result.TimeExecuting = watcher.Elapsed.ToString();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}