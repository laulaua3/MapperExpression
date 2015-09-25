using MapperExemple.Entity.EF;
using MapperExemple.Entity.Interface;
using MapperExemple.Web.Models;
using MapperExpression;
using Microsoft.Practices.ServiceLocation;

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
            Mapper.ConstructServicesUsing((x) => ServiceLocator.Current.GetInstance(x));
            Mapper.CreateMap<Product, IExempleProduct>().ConstructUsingServiceLocator();
            Mapper.CreateMap<IExempleProduct,ProductModel>();
            //Other exemple
            Mapper.CreateMap<Product, ProductModel>();
           
            //Important!!!
            Mapper.Initialize();



            //AutoMapper
            AutoMapper.Mapper.Initialize((cfg) =>
            {
                //For simple exemple
                cfg.CreateMap<Customer, CustomerModel>();

                //For exemple with custom mapping
                cfg.CreateMap<Order, OrderModel>()
                    .ForMember((s)=>s.CustomerName, (m) => m.MapFrom(d=> d.Customer.CompanyName));

                cfg.CreateMap<OrderDetail, OrderDetailModel>();

                //Exemple with Ioc
                cfg.ConstructServicesUsing((x) => ServiceLocator.Current.GetInstance(x));
                cfg.CreateMap<Product, IExempleProduct>().ConstructUsingServiceLocator();
                cfg.CreateMap<IExempleProduct, ProductModel>();
                //Other exemple
                cfg.CreateMap<Product, ProductModel>();

                cfg.Seal();
            });
        }
    }
}