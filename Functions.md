# Azure Functions

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
