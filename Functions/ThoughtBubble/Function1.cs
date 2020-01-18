using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace ThoughtBubble
{

    public class ThoughtBubble: TableEntity
    {
        public string Thought { get; set; }
    }

    public class ThoughtSubmission
    {
        public string User { get; set; }
        public string Thought { get; set; }
    }

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
}
