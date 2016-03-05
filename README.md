# MapperExpression
Mapper simple like AutoMapper without all options but more efficient for performances
# Code Exemple
*At the application start*
```C#
//Create a simple mapping
Mapper.CreateMap<Customer, CustomerMode>();
//Create a mapping with custom mapping
Mapper.CreateMap<Order, OrderMode>()
    //Custom mapping with true to test if the sub class is null
    .ForMember(s => s.Customer.CompanyName, d =>; d.CustomerName, true);
//Important !!!
Mapper.Initialize();
```
*Use*
```C#
ExempleEntity exemple1 = new ExempleEntity();
Customer result = exemple1.GetFirstCustomer();
//exemple to Map a object
var model = Mapper.Map<Customer, CustomerModel>(result);
//Or (New!!)
var model = Mapper<CustomerMode>.Map(result);
```
