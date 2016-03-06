# MapperExpression
Mapper simple like AutoMapper without all options but more efficient for performances
# Code Exemple
### _Configuration at boot application_
```C#
//For simple exemple
Mapper.CreateMap<Customer, CustomerModel>();
 //For exemple with custom mapping
Mapper.CreateMap<Order, OrderModel>()
    //Custom mapping
    .ForMember(s => s.Customer.CompanyName, d => d.CustomerName, true);
Mapper.CreateMap<OrderDetail, OrderDetailModel>();

//Exemple with Ioc
Mapper.ConstructServicesUsing((x) => DependencyResolver.Current.GetService(x));
//Mapper.ConstructServicesUsing((x) => ServiceLocator.Current.GetInstance(x));
Mapper.CreateMap<Product, IExempleProduct>().ConstructUsingServiceLocator()
  .ReverseMap();
 Mapper.CreateMap<IExempleProduct, ProductModel>();
 //Other exemple
Mapper.CreateMap<Product, ProductModel>();
// VERY IMPORTANT (this method seal your mapping)
Mapper.Initialize();
```
### _Use_
```C#
ExempleEntity exemple1 = new ExempleEntity();
Customer result = exemple1.GetFirstCustomer();
//exemple to Map a object
var model = Mapper.Map<Customer, CustomerModel>(result);
//Or (New!!)
var model = Mapper<CustomerModel>.Map(result);
```
**For more details see the project of exemple**
