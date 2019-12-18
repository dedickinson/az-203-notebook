namespace TableApp
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

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

    class Program
    {
        private const string ENV_VAR_SA_KEY = "AZ203_SA_KEY";
        private const string ENV_VAR_SA_NAME = "AZ203_SA_NAME";

        private static readonly string StorageAccountName = Environment.GetEnvironmentVariable(ENV_VAR_SA_NAME);
        private static readonly string StorageAccountKey = Environment.GetEnvironmentVariable(ENV_VAR_SA_KEY);

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            Console.WriteLine($"Storage account: {StorageAccountName}");

            var connectionString = $"DefaultEndpointsProtocol=https;AccountName={StorageAccountName};AccountKey={StorageAccountKey};EndpointSuffix=core.windows.net";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            CloudTable table = tableClient.GetTableReference("pets");

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

            TableOperation retrieveOperation = TableOperation.Retrieve<PetEntity>("cat", "paws");
            TableResult queryResult = table.Execute(retrieveOperation);
            PetEntity paws = queryResult.Result as PetEntity;
            Console.WriteLine($"Retrieved: {paws.RowKey} ({paws.PartitionKey}) is {paws.age} years old.");

            TableOperation deleteOperation = TableOperation.Delete(paws);
            table.Execute(deleteOperation);
            Console.WriteLine($"Deleted {paws.RowKey}");

            TableQuery<PetEntity> queryDogs = new TableQuery<PetEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "dog"));

            Console.WriteLine("Dogs:");
            foreach (PetEntity pet in table.ExecuteQuery(queryDogs)) {
                Console.WriteLine($"  - {pet.RowKey}");
            }
        }
    }
}
