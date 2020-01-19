# Azure Functions

Notes:

- AppInsights is configured with the `APPINSIGHTS_INSTRUMENTATIONKEY` setting

## C# Functions

### Queue trigger

```C#
using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace fn
{
    public static class OverdueHandler
    {
        [FunctionName("OverdueHandler")]
        public static void Run([QueueTrigger("overdue", Connection = "QueueStorageConnection")]string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }

    }
}
```

### Timer Trigger

[Cron format](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-timer?tabs=csharp#ncrontab-expressions):

    {second} {minute} {hour} {day} {month} {day-of-week}

Run every 5 minutes:

```C#
public static class TimerTriggerFunc
{
    [FunctionName("TimerTriggerFunc")]
    public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
    }
}
```

### HTTP Trigger

```C#
public static class HttpTriggerCSharp
{
    [FunctionName("HttpTriggerCSharp")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string name = req.Query["name"];

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name = name ?? data?.name;

        return name != null
            ? (ActionResult)new OkObjectResult($"Hello, {name}")
            : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
    }
}
```

### HTTP Trigger with Table output

* Project: [Functions/ThoughtBubble](Functions/ThoughtBubble)

_Note_: If a specific table connection string is not provided, the
`AzureWebJobsStorage` connection is used.

This HTTP function creates a new row in a Table:

```C#
public static class ThoughtBubbleSubmit
{
    [FunctionName("ThoughtBubbleSubmit")]
    [return: Table("ThoughtTable")]
    public static ThoughtBubble Submit(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] ThoughtSubmission req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        return new ThoughtBubble {
            PartitionKey = req.User,
            RowKey = Guid.NewGuid().ToString(),
            Thought = req.Thought };
    }
}
```

This one takes the table as an input and queries it to provide a response:

```C#
public static class ThoughtBubbleList
{
    [FunctionName("ThoughtBubbleList")]
    public static async Task<IActionResult> Submit(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        [Table("ThoughtTable")] CloudTable thoughtTable,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string user = req.Query["user"];

        TableQuerySegment<ThoughtBubble> thoughts;
        TableQuery<ThoughtBubble> queryUser;

        if (String.IsNullOrEmpty(user))
        {
            queryUser = new TableQuery<ThoughtBubble>();

        }
        else
        {
            queryUser = new TableQuery<ThoughtBubble>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, user));

        }
        thoughts = await thoughtTable.ExecuteQuerySegmentedAsync(queryUser, null);

        var thoughtList = new List<ThoughtSubmission>();
        foreach (var thought in thoughts)
        {
            thoughtList.Add(new ThoughtSubmission()
            {
                User = thought.PartitionKey,
                Thought = thought.Thought
            });
        }

        return (ActionResult)new OkObjectResult(thoughtList);
    }
}
```

## Python Functions

`host.json`:

```json
{
    "version": "2.0",
    "extensionBundle": {
        "id": "Microsoft.Azure.Functions.ExtensionBundle",
        "version": "[1.*, 2.0.0)"
    }
}
```

`function.json`:

```json
{
  "scriptFile": "__init__.py",
  "bindings": [
    {
      "name": "stationData",
      "type": "blobTrigger",
      "direction": "in",
      "path": "bom-gov-au/stations/stations.txt",
      "connection": "BlobStoreBindingConnection"
    }
  ]
}
```

The function (partial):

```python
import azure.functions as func
from azure.cosmosdb.table import TableService, TableBatch, Entity
from parse_bom_stations import parse_station_list, WeatherStationTuple

async def main(stationData: func.InputStream):
    """ Azure function body """
    logging.info(
        'Python blob trigger function processed blob (%s) - %s bytes',
        stationData.name, stationData.length)
```

## Durable Functions

Declare an orchestrator function:

```C#
[FunctionName("DurableFunctionsOrchestrationCSharp")]
public static async Task<List<string>> RunOrchestrator(
    [OrchestrationTrigger] DurableOrchestrationContext context)
```

Orchestrator adds an activity function call with:

    outputs.Add(await context.CallActivityAsync<string>("DurableFunctionsOrchestrationCSharp_Hello", "Tokyo"));

Activity function:

```C#
[FunctionName("DurableFunctionsOrchestrationCSharp_Hello")]
public static string SayHello([ActivityTrigger] string name, ILogger log)
```

Kick of the orchastrator with an HTTP-trigger-based function:

```C#
[FunctionName("DurableFunctionsOrchestrationCSharp_HttpStart")]
public static async Task<HttpResponseMessage> HttpStart(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
    [OrchestrationClient]DurableOrchestrationClient starter,
    ILogger log)
{
    // Function input comes from the request content.
    string instanceId = await starter.StartNewAsync("DurableFunctionsOrchestrationCSharp", null);

    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

    return starter.CreateCheckStatusResponse(req, instanceId);
}
```
