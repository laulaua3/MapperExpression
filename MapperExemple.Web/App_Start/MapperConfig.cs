using MapperExemple.Entity.EF;
using MapperExemple.Entity.Interface;
using MapperExemple.Web.Models;
using MapperExpression;
using Microsoft.Practices.ServiceLocation;
using Nelibur.ObjectMapper;
using System.Diagnostics;
using System.Web.Mvc;

namespace MapperExemple.Web.App_Start
{
    public class MapperConfig
    {

        public static void Initialise()
        {
            //For simple exemple
            Mapper.CreateMap<Customer, CustomerModel>();

            //For exemple with custom mapping
            Mapper.CreateMap<Order, OrderModel>()
                //Custom mapping
                .ForMember(s => s.Customer.CompanyName, d => d.CustomerName, true);

            Mapper.CreateMap<OrderDetail, OrderDetailModel>();

            //Exemple with Ioc
            Mapper.ConstructServicesUsing((x) => DependencyResolver.Current.GetService(x));
            //other exemple
            //Mapper.ConstructServicesUsing((x) => ServiceLocator.Current.GetInstance(x));
            Mapper.CreateMap<Product, IExempleProduct>().ConstructUsingServiceLocator()
                .ReverseMap();
            Mapper.CreateMap<IExempleProduct, ProductModel>();

            //Other exemple
            Mapper.CreateMap<Product, ProductModel>();
            Stopwatch initWatch = Stopwatch.StartNew();
            //IMPORTANT!!!
            Mapper.Initialize();
            initWatch.Stop();
            Debug.WriteLine("Init MapperExpression :" + initWatch.Elapsed.ToString());


            //AutoMapper
            AutoMapper.Mapper.Initialize((cfg) =>
            {
                //For simple exemple
                cfg.CreateMap<Customer, CustomerModel>();

                //For exemple with custom mapping
                cfg.CreateMap<Order, OrderModel>()
                    .ForMember((s) => s.CustomerName, (m) => m.MapFrom(d => d.Customer.CompanyName));

                cfg.CreateMap<OrderDetail, OrderDetailModel>();


                //Exemple with Ioc
                cfg.ConstructServicesUsing((x) => ServiceLocator.Current.GetInstance(x));
                cfg.CreateMap<Product, IExempleProduct>().ConstructUsingServiceLocator()
                    .ReverseMap();
                cfg.CreateMap<IExempleProduct, ProductModel>();
                //Other exemple
                cfg.CreateMap<Product, ProductModel>();

                cfg.Seal();
            });

            //TinyMapper don't find Ioc
            TinyMapper.Bind<Customer, CustomerModel>();

            //For exemple with custom mapping
            TinyMapper.Bind<Order, OrderModel>((config) =>
            {
                config.Bind(s => s.Customer.CompanyName, d => d.CustomerName);
                
            });
            TinyMapper.Bind<OrderDetail, OrderDetailModel>();
            TinyMapper.Bind<Product, ProductModel>();
        }
    }
}