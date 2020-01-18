# Cosmos DB

* Projects:
    * https://github.com/dedickinson/CosmosLoader
    * https://github.com/weather-balloon/loader-bom-observations/blob/master/ObservationLoader/Cosmos/
* See also: [Tables](Tables.md)

## SQL

* Docs: https://docs.microsoft.com/en-us/azure/cosmos-db/sql-query-getting-started
* Playground: https://www.documentdb.com/sql/demo

```sql
SELECT * FROM postcodes
```

```sql
SELECT TOP 5 * FROM c
```

`ROOT` can be used instead of the container name:

```sql
SELECT TOP 5 * FROM ROOT
```

Count of records:

```sql
SELECT VALUE COUNT(c) FROM c
```

Filter (using "table" name):

```sql
SELECT *
FROM food
WHERE food.foodGroup = "Breakfast Cereals"
```

Filter (with an alias):

```sql
SELECT *
FROM food f
WHERE f.foodGroup = "Breakfast Cereals"
```

Project and filter (with an alias - could use table name too):

```sql
SELECT f.id, f.description
FROM food f
WHERE f.foodGroup = "Breakfast Cereals"
```

### Connecting

```C#
CosmosClientOptions options = new CosmosClientOptions() { AllowBulkExecution = true };
var Client = new CosmosClient(Uri, Key, options);
Database CosmosDatabase = await Client.CreateDatabaseIfNotExistsAsync(Db);
Container CosmosContainer = await CosmosDatabase.CreateContainerIfNotExistsAsync(Container, "/admin_code1");
```

### Data

Upsert:

```C#
CosmosContainer.UpsertItemAsync(r)
```

### Queries

```C#
QueryDefinition queryDefinition = new QueryDefinition(query);
FeedIterator<dynamic> queryResultSetIterator = container.GetItemQueryIterator<dynamic>(query);

while (queryResultSetIterator.HasMoreResults)
{
    FeedResponse<dynamic> currentResultSet = await queryResultSetIterator.ReadNextAsync();
    foreach (var result in currentResultSet)
    {
        Console.WriteLine(result);
    }
}
```

Instead of `dynamic` you can use a specific Type.

## MongoDB

### Connecting

Example connection string:

```
mongodb://wb-cosmos-dev:<KEY>@<COSMOS_NAME>.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@<COSMOS_NAME>@
```

```C#
MongoClientSettings settings;

settings = MongoClientSettings.FromUrl(
    new MongoUrl(config.ConnectionString));

// Cosmos DB appears to need this disabled
settings.RetryWrites = false;

settings.UseTls = config.UseTls;

// Although this is deprecated in favour of UseTls, Cosmos uses this property
settings.UseSsl = config.UseTls;

client = new MongoClient(settings);

database = this.client.GetDatabase(config.DatabaseName);
var collection = database.GetCollection<WeatherStationObservation>(config.CollectionName);
```

### Data

Check if an `_id` exists:

```C#
var filter = Builders<WeatherStationObservation>.Filter.Eq("_id", checkId);

if (collection.CountDocuments(filter) > 0) {}
```

Insert a collection of records (`IEnumerable<WeatherStationObservation>`):

```C#
collection.InsertMany(observationsArr);
```
