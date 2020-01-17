# Web Apps

* MSFT Docs: https://docs.microsoft.com/en-us/azure/app-service/
* [Demo List App](https://github.com/dedickinson/az203-listapp)

## CLI

```bash
az webapp list

az webapp show -n az203-todo-api -g az-203-training

az webapp webjob continuous list -n az203-todo-api -g az-203-training

az webapp stop -n az203-todo-api -g az-203-training
```

App service plans:

```bash
az appservice plan list

az appservice plan show -n az203-todo-api -g az-203-training

az appservice plan update -n az203-todo-api -g az-203-training --sku B2
```

## Web API

Key packages:
- `Swashbuckle.AspNetCore`: Generate OpenApi

### Generate OpenAPI

In `Startup.cs`:

```C#
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ToDo API",
            Description = "A simple example ASP.NET Core Web API",
            TermsOfService = new Uri("https://example.com/terms"),

            Contact = new OpenApiContact
            {
                Name = "Fred Nurk",
                Email = "test@example.com",
                Url = new Uri("http://www.example.com"),
            },
            License = new OpenApiLicense
            {
                Name = "Use under LICX",
                Url = new Uri("https://example.com/license"),
            }
        });
        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });
    // ...
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
        c.RoutePrefix = string.Empty;
    });

    // ...
}
```

## Authentication

Sample project: <WebAppIndividualAuth>

Key packages:

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.AspNetCore.Authentication.Google`

### Endpoints

- Azure Active Directory: /.auth/login/aad
- Microsoft Account: /.auth/login/microsoftaccount
- Facebook: /.auth/login/facebook
- Google: /.auth/login/google
- Twitter: /.auth/login/twitter

### Sample Code

In `Startup.cs`:

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(
            Configuration.GetConnectionString("DefaultConnection")));
    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();
    services.AddRazorPages();

    services.AddAuthentication()
        .AddGoogle(options =>
        {
            IConfigurationSection googleAuthNSection =
                Configuration.GetSection("Authentication:Google");

            options.ClientId = googleAuthNSection["ClientId"];
            options.ClientSecret = googleAuthNSection["ClientSecret"];
        });
}
```


## Web Jobs

Key Packages:

- `Microsoft.Azure.WebJobs.Extensions`
- `Microsoft.Azure.WebJobs.Extensions.Storage`

### Sample Code

Basic startup (snippet):

```C#
var config = Host.CreateDefaultBuilder(args).ConfigureWebJobs(b =>
{
    b.AddAzureStorageCoreServices()
    .AddAzureStorage()
    .AddTimers();
})

var host = config.Build();

using (host)
{
    await host.RunAsync();
}
```

#### Example job

```C#
public class Functions
{
    private readonly TodoContext _dbContext;

    public Functions(TodoContext context)
    {
        _dbContext = context;
    }

    public void CheckDueDates(
        [TimerTrigger("00:00:10")]TimerInfo timer,
        [Queue(queueName:"overdue", Connection = "QueueStorageConnection")] ICollector<TodoItem> items,
        ILogger logger)
    {
        var todoList = _dbContext
            .TodoItems
            .Where(i => DateTime.Now > i.DueDate)
            .ToList();

        logger.LogInformation($"Overdue items: {todoList.Count}");

        foreach (var item in todoList)
        {
            items.Add(item);
            logger.LogInformation($"Item add to queue: {item.Id}");
        }
    }
}
```

## Container-based Webapps

Refer to [azure-webapp-container](https://github.com/dedickinson/webapp-variations/blob/master/deploy/azure-webapp-container/README.md)

```
az appservice plan create --name $RESOURCE_GROUP \
                          --resource-group $RESOURCE_GROUP \
                          --sku B1 \
                          --is-linux

az webapp create --resource-group $RESOURCE_GROUP \
                 --plan $RESOURCE_GROUP \
                 --name $WEB_APP_NAME \
                 --deployment-container-image-name $ACR_LOGIN_SERVER/$IMAGE_NAME:$IMAGE_VERSION

az webapp config container set --name $WEB_APP_NAME \
                               --resource-group $RESOURCE_GROUP \
                               --docker-custom-image-name $ACR_LOGIN_SERVER/$IMAGE_NAME:$IMAGE_VERSION \
                               --docker-registry-server-url $ACR_LOGIN_SERVER \
                               --docker-registry-server-user $(az keyvault secret show --vault-name $AKV_NAME -n $ACR_NAME-pull-usr --query value -o tsv) \
                               --docker-registry-server-password $(az keyvault secret show --vault-name $AKV_NAME -n $ACR_NAME-pull-pwd --query value -o tsv)

az webapp config appsettings set --name $WEB_APP_NAME \
                                 --resource-group $RESOURCE_GROUP \
                                 --settings WEBSITES_PORT=3000
```
