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
