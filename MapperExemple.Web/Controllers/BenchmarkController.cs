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
            return await Execute((source) =>
            {
                return MapperExpression.Mapper<CustomerModel>.Map(source);
            }, "MapperExpression", nbIteration);
        }

        public async Task<ActionResult> BenchMarkAutoMapper(int nbIteration)
        {
            return await Execute((source) =>
                {
                    return AutoMapper.Mapper.Map<CustomerModel>(source);
                }, "AutoMapper", nbIteration);

        }

        public async Task<ActionResult> BenchMarkValueInjecter(int nbIteration)
        {
            return await Execute((source) =>
                 {
                     CustomerModel model = new CustomerModel();
                     model.InjectFrom(source);
                     return model;
                 }, "ValueInjecte", nbIteration);
        }

        public async Task<ActionResult> BenchMarkDirect(int nbIteration)
        {
            return await Execute((source) =>
                {
                    return new CustomerModel()
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
                }, "Direct", nbIteration);
        }

        public async Task<ActionResult> BenchMarkTinyMapper(int nbIteration)
        {
            return await Execute((s) =>
                {
                    return TinyMapper.Map<CustomerModel>(s);
                }, "TinyMapper", nbIteration);
        }

        Task<ActionResult> Execute(Func<Customer, CustomerModel> mapp, string mapperName, int nbIteration)
        {
            return Task.Run<ActionResult>(() =>
             {
                 BenchmarkModel resultModel = new BenchmarkModel();
                 resultModel.Mapper = mapperName;
                 // Generate data.
                 Customer source = Builder<Customer>.CreateNew().Build();
                 Stopwatch watcher = Stopwatch.StartNew();
                 for (int i = 0; i < nbIteration; i++)
                 {
                      mapp(source);
                 }
                 watcher.Stop();
                 resultModel.TimeExecuting = watcher.Elapsed.ToString();
                 return Json(resultModel, JsonRequestBehavior.AllowGet);
             });

        }
    }
}