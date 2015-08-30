using MapperExemple.Entity;
using MapperExemple.Web.Models;
using MapperExpression;
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

            Mapper.Initialize();
        }
    }
}