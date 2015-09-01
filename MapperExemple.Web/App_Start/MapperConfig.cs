using MapperExemple.Entity;
using MapperExemple.Entity.Interface;
using MapperExemple.Web.Models;
using MapperExpression;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        }
    }
}