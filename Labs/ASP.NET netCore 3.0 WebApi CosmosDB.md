# ASP.NET Core WebApi and CosmosDB

>NOTE: Please first ensure you have completed [ASP.NET Core3 WebApi with SQL Server and EntityFramework](ASP.NET%20netCore%203.0%20WebApi%20SQL%20Server%20and%20EntityFramework.md)

# nuget

- Microsoft.EntityFrameworkCore.Cosmos
- Newtonsoft.json

# Add key in appsettings.json

```json
  "CosmosDb": {
    "Endpoint": "<URI IN THE KEYS SECTION>",
    "Key": "<PRIMARY OR SECONDARY KEY IN THE KEYS SECTION>",
    "DatabaseName": "<NAME OF THE DATABASE>",
    "ContainerName": "<NAME OF THE CONTAINER>"
  },
```

# Create Model

We are going to use the classes from the previous demos

1. Create "Models" folder
2. Copy-Paste the Model classes created before 

![](Misc/13.png)

3. Edit the files and update the namespace acordingly. In this case:

```csharp
namespace WebApiEFCosmosDb.Models
```


# Create DbContext

1. Create new folder named "Services"

1. Create a new interface for the cosmosdb context

![](Misc/12.png)

1. Add the following methods to the interface

```csharp
    public interface ICosmosDbService
    {
        Task<IActionResult> PutProduct(int id, Product product);
        Task<ActionResult<Product>> DeleteProduct(int id);
        bool ProductExists(int id);
        Task<ActionResult<Product>> GetProduct(int id);
        Task<ActionResult<IEnumerable<Product>>> GetProduct();
    }
```

1. Add the CosmosDbService

![](Misc/14.png)

## Initialize methods

Go to Startup.cs and Add the cosmos initialization method

```csharp
/// <summary>
/// Creates a Cosmos DB database and a container with the specified partition key. 
/// </summary>
/// <returns></returns>
private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
{
    string databaseName = configurationSection.GetSection("DatabaseName").Value;
    string containerName = configurationSection.GetSection("ContainerName").Value;
    string account = configurationSection.GetSection("Endpoint").Value;
    string key = configurationSection.GetSection("Key").Value;
    CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
    CosmosClient client = clientBuilder
                        .WithConnectionModeDirect()
                        .Build();
    CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
    DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/productid");
    return cosmosDbService;
}
```

And also the following line in the ConfigureServices method

```csharp
public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        }
```

>Now, EFCore is linked to cosmosdb. The rest is to create the controllers

# Add the ProductController class

Now it´s time to create our controller. We need to do it manually, since EFCore at the time of writing this demo does not automatically generates the CRUD for us.

![](Misc/15.png)

And implement some methods. At least, try to implement methods to Write data into the CosmosDB and to extract data from CosmosdDB

take a look for example to the methods 

```csharp
// GET: api/Products/5
[HttpGet("{id}")]
public async Task<ActionResult<Product>> GetProduct(int id)
{
    var product = await _context.GetProduct(id);

    if (product == null)
    {
        throw new NotImplementedException();
    }

    return product;
}

// POST: api/products/postproduct
// To protect from overposting attacks, please enable the specific properties you want to bind to, for
// more details see https://aka.ms/RazorPagesCRUD.
[Route("postproduct")]
[HttpPost]
public async Task PostProduct(Product product)
{
    try
    {
        await _context.PostProduct(product);
    }
    catch (Exception e)
    { throw e; }
}
```
# Prepare serialization

Partition key in the object "Product", is integer, but CosmosDB requires to be "string". 

Create a converter to transform the column 

```csharp
public class JsonToStringConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override bool CanRead
    {
        get { return false; }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
```

## Configure serialization of Product.cs

Override some parameters

```csharp
[JsonConverter(typeof(JsonToStringConverter))]
[JsonProperty(PropertyName = "id")]
public int ProductId { get; set; }

//Ignore thumbnails
[JsonIgnore]
public byte[] ThumbNailPhoto { get; set; }
```

# Deploy

Deploy the web app

![](Misc/webapp-cosmos.png)


# Test

Since we still don´t have data in cosmosdb, this is ok

![](Misc/test-nodata.png)

>Note: navigate to /api/products