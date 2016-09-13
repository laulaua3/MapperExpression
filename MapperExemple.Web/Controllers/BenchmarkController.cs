using FizzWare.NBuilder;
using MapperExemple.Entity.EF;
using MapperExemple.Web.Models;
using System.Diagnostics;
using System.Web.Mvc;
using Omu.ValueInjecter;
using Nelibur.ObjectMapper;
using System;
using System.Threading.Tasks;

namespace MapperExemple.Web.Controllers
{
    public class BenchmarkController : Controller
    {

        // GET: Benchmark
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BenchMarkMapperExpression(int nbIteration)
        {
            Task<ActionResult> result = Task.Run<ActionResult>(() =>
        {
            BenchmarkModel resultModel = new BenchmarkModel();
            resultModel.Mapper = "MapperExpression";
            //Generate data
            Customer source = Builder<Customer>.CreateNew().Build();
            Stopwatch watcher = Stopwatch.StartNew();

            for (int i = 0; i < nbIteration; i++)
            {
                var model = MapperExpression.Mapper<CustomerModel>.Map(source);
            }
            watcher.Stop();
            resultModel.TimeExecuting = watcher.Elapsed.ToString();
            return Json(resultModel, JsonRequestBehavior.AllowGet);
        });
            return await result;
        }

        public async Task<ActionResult> BenchMarkAutoMapper(int nbIteration)
        {
            Task<ActionResult> result = Task.Run<ActionResult>(() =>
            {
                BenchmarkModel resultModel = new BenchmarkModel();
                resultModel.Mapper = "AutoMapper";
                //Generate data
                Customer source = Builder<Customer>.CreateNew().Build();
                Stopwatch watcher = Stopwatch.StartNew();
                for (int i = 0; i < nbIteration; i++)
                {
                    var model = AutoMapper.Mapper.Map<CustomerModel>(source);
                }
                watcher.Stop();
                resultModel.TimeExecuting = watcher.Elapsed.ToString();
                return Json(resultModel, JsonRequestBehavior.AllowGet);
            });
            return await result;
        }


        public async Task<ActionResult> BenchMarkValueInjecter(int nbIteration)
        {
            Task<ActionResult> result = Task.Run<ActionResult>(() =>
            {
                BenchmarkModel resultModel = new BenchmarkModel();
                resultModel.Mapper = "ValueInjecte";
                //Generate data
                Customer source = Builder<Customer>.CreateNew().Build();
                Stopwatch watcher = Stopwatch.StartNew();
                for (int i = 0; i < nbIteration; i++)
                {
                    CustomerModel model = new CustomerModel();
                    model.InjectFrom(source);
                }
                watcher.Stop();
                resultModel.TimeExecuting = watcher.Elapsed.ToString();
                return Json(resultModel, JsonRequestBehavior.AllowGet);
            });
            return await result;
        }

        public async Task<ActionResult> BenchMarkDirect(int nbIteration)
        {
            Task<ActionResult> result = Task.Run<ActionResult>(() =>
            {
                BenchmarkModel resultModel = new BenchmarkModel();
                resultModel.Mapper = "Direct";
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
                resultModel.TimeExecuting = watcher.Elapsed.ToString();
                return Json(resultModel, JsonRequestBehavior.AllowGet);
            });
            return await result;
        }

        public async Task<ActionResult> BenchMarkTinyMapper(int nbIteration)
        {
            Task<ActionResult> result = Task.Run<ActionResult>(() =>
            {
                BenchmarkModel resultModel = new BenchmarkModel();
                resultModel.Mapper = "TinyMapper";
                //Generate data
                Customer source = Builder<Customer>.CreateNew().Build();
                Stopwatch watcher = Stopwatch.StartNew();
                for (int i = 0; i < nbIteration; i++)
                {
                    var model = TinyMapper.Map<CustomerModel>(source);
                }
                watcher.Stop();
                resultModel.TimeExecuting = watcher.Elapsed.ToString();
                return Json(resultModel, JsonRequestBehavior.AllowGet);
            });
            return await result;
        }
    }
}