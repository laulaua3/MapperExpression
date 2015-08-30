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

        public static void  Initialise()
        {
            Mapper.CreateMap<Customer, CustomerModel>();


            Mapper.Initialize();
        }
    }
}