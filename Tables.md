# Storage Account Tables

* MSFT Docs: https://docs.microsoft.com/en-au/azure/storage/
* Demo: [Storage Account - Tables](Storage\ Account\ -\ Tables/README.md)

Packages:

- `Microsoft.Azure.Cosmos.Table`

## Connection String

    DefaultEndpointsProtocol=https;AccountName=saaz203duncan;AccountKey=<KEY>;EndpointSuffix=core.windows.net

## Sample code

### Entity

Subsclass the `TableEntity`:

```C#
public class PetEntity : TableEntity {
    public int age {get; set;}

    public PetEntity(){

    }

    public PetEntity(string specie, string name, int age){
        this.PartitionKey = specie;
        this.RowKey = name;
        this.age = age;
    }
}
```

### Connect

```C#
var connectionString = $"DefaultEndpointsProtocol=https;AccountName={StorageAccountName};AccountKey={StorageAccountKey};EndpointSuffix=core.windows.net";

CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

CloudTable table = tableClient.GetTableReference("pets");
```

### Add entries

```C#
table.CreateIfNotExists();

PetEntity[] pets = {
    new PetEntity("cat", "mittens", 4),
    new PetEntity("dog", "fido", 8),
    new PetEntity("bird", "tweey", 1),
    new PetEntity("cat", "paws", 4),
    new PetEntity("dog", "ralf", 3)
};

foreach (var pet in pets) {
    TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(pet);
    TableResult result = table.Execute(insertOrMergeOperation);
}
```

### Retrieve a record and delete it

```C#
TableOperation retrieveOperation = TableOperation.Retrieve<PetEntity>("cat", "paws");
TableResult queryResult = table.Execute(retrieveOperation);
PetEntity paws = queryResult.Result as PetEntity;
Console.WriteLine($"Retrieved: {paws.RowKey} ({paws.PartitionKey}) is {paws.age} years old.");

TableOperation deleteOperation = TableOperation.Delete(paws);
table.Execute(deleteOperation);
Console.WriteLine($"Deleted {paws.RowKey}");
```

### Get all entries in a partition

```C#
TableQuery<PetEntity> queryDogs = new TableQuery<PetEntity>()
    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "dog"));

Console.WriteLine("Dogs:");
foreach (PetEntity pet in table.ExecuteQuery(queryDogs)) {
    Console.WriteLine($"  - {pet.RowKey}");
}
```
