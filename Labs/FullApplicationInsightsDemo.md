# Full demo of application insights

First of all, [deploy the infrastructure required for the demos](./DeployIInfrastructure.ipynb)

## Install EntityFramework

Install nuget packages:

- Microsoft.Entity.Framework.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.Design
- AutoMapper
- Newtonsoft.Json

This is what you should have

![](Misc/1.png)

## Generate CRUD

> NOTE: Please, use the database created in the [DeployInfrastructure.ipynb](./DeployIInfrastructure.ipynb) 

Open Package manage console

![](Misc/2.png)

And execute the following code

```powershell
Scaffold-DbContext "Server=your server;Database=YourDatabase;User ID=YourUser;Password=YourPassword" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

for example:
```powershell
Scaffold-DbContext "Server=mslearn-appinsights-sqlserver.database.windows.net;Database=AdventureWorksDemo;User ID=administrador;Password=PaSSw0rdñ." Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

>NOTE: Make sure you are in the folder of the **ApplicationInsightsDemoApi**

![](Misc/3.png)

And after a couple of seconds

![](Misc/4.png)

### Configure DbContext

AdventureWorksDemoContext.cs contains our DbContext WITH THE CONNECTION STRING PLAIN TEXT IN THE CODE

![](Misc/5.png)

Let´s **delete** that method and write to the appsettings.json the following

```json
"ConnectionStrings": {
    "SQLServerDbContext": "Server=mslearn-appinsights-sqlserver.database.windows.net;Database=AdventureWorksDemo;User ID=administrador;Password=PaSSw0rdñ."
  },
```

### Add a new folder "DTOs"

![](Misc/6.png)

### Add all DTOs (Data Transfer Objects)

Add new class for each Model and change the ICollection to List

For example:

```csharp
    public partial class AddressDTO
    {
        public AddressDTO()
        {
            CustomerAddress = new List<CustomerAddressDTO>();
            SalesOrderHeaderBillToAddress = new List<SalesOrderHeaderDTO>();
            SalesOrderHeaderShipToAddress = new List<SalesOrderHeaderDTO>();
        }

        public int AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string CountryRegion { get; set; }
        public string PostalCode { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual List<CustomerAddressDTO> CustomerAddress { get; set; }
        public virtual List<SalesOrderHeaderDTO> SalesOrderHeaderBillToAddress { get; set; }
        public virtual List<SalesOrderHeaderDTO> SalesOrderHeaderShipToAddress { get; set; }
    }
```

### Configure AutoMapper

AutoMapperConfiguration will be responsible for mapping DTOs to controllers


## Startup.cs

Include AutoMapper configuration