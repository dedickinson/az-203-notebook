# Azure Batch

* MSFT Docs: https://docs.microsoft.com/en-us/azure/batch/
* Demo: [batch/README.md](batch/README.md)

## Sample Code

Example of a basic workflow of:

1. Create Batch Account
2. Login to the account
3. Create a VM Pool
4. Create a Job
5. Create a Task in the Job

```bash
# Create the Batch Account
az batch account create --name $BATCH_ACCOUNT \
    --location $RESOURCE_LOCATION \
    --resource-group $RESOURCE_GROUP \
    --storage-account $STORAGE_ACCOUNT

# Login to the Batch Account to set the context for further requests
az batch account login \
    --name $BATCH_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --shared-key-auth

# Create a small pool
az batch pool create \
    --id $BATCH_POOL --vm-size Standard_A1_v2 \
    --target-dedicated-nodes 2 \
    --image canonical:ubuntuserver:16.04-LTS \
    --node-agent-sku-id "batch.node.ubuntu 16.04"

az batch pool show --pool-id $BATCH_POOL \
    --query "allocationState"

# Create a Job
az batch job create \
    --id $BATCH_JOB \
    --pool-id $BATCH_POOL

# Kick off some basic tasks
for i in {1..4}
do
az batch task create \
    --task-id $BATCH_TASK$i \
    --job-id $BATCH_JOB \
    --command-line "/bin/bash -c 'printenv | grep AZ_BATCH; sleep 90s'"
done
```

### Applications

You can register applications and have them deployed to VMs in a pool:

```bash
zip task_factorial task_factorial.py

az batch application create --application-name task_factorial \
    --name $BATCH_ACCOUNT \
    --resource-group $RESOURCE_GROUP

az batch application package create --application-name task_factorial \
    --name $BATCH_ACCOUNT \
    --resource-group $RESOURCE_GROUP \
    --package-file task_factorial.zip \
    --version-name 1.0.0

az batch pool create \
    --id $BATCH_POOL \
    --vm-size Standard_A1_v2 \
    --target-low-priority-nodes 2 \
    --application-package-references task_factorial#1.0.0 \
    --image canonical:ubuntuserver:18.04-LTS \
    --node-agent-sku-id "batch.node.ubuntu 18.04" \
    --max-tasks-per-node 1
```

### Launch in C#

Snippet of (basic) [task creation](https://github.com/Azure-Samples/batch-dotnet-quickstart/blob/master/BatchDotnetQuickstart/Program.cs):

```C#
List<CloudTask> tasks = new List<CloudTask>();

// loop {
CloudTask task = new CloudTask(taskId, taskCommandLine);
task.ResourceFiles = new List<ResourceFile> { inputFiles[i] };
tasks.Add(task);
// } loop

batchClient.JobOperations.AddTask(JobId, tasks);
IEnumerable<CloudTask> addedTasks = batchClient.JobOperations.ListTasks(JobId);

batchClient.Utilities.CreateTaskStateMonitor().WaitAll(addedTasks, TaskState.Completed, timeout);

Console.WriteLine("All tasks reached state Completed.");
```


[Parallel tasks](https://github.com/Azure-Samples/batch-dotnet-ffmpeg-tutorial/blob/master/BatchDotnetTutorialFfmpeg/Program.cs)

```C#
// Obtain a shared access signature that provides write access to the output container to which
// the tasks will upload their output.
string outputContainerSasUrl = GetContainerSasUrl(blobClient, outputContainerName, SharedAccessBlobPermissions.Write);

// CREATE BATCH CLIENT / CREATE POOL / CREATE JOB / ADD TASKS

// Create a Batch client and authenticate with shared key credentials.
// The Batch client allows the app to interact with the Batch service.
BatchSharedKeyCredentials sharedKeyCredentials = new BatchSharedKeyCredentials(BatchAccountUrl, BatchAccountName, BatchAccountKey);

using (BatchClient batchClient = BatchClient.Open(sharedKeyCredentials))
{
    // Create the Batch pool, which contains the compute nodes that execute the tasks.
    await CreatePoolIfNotExistAsync(batchClient, PoolId);

    // Create the job that runs the tasks.
    await CreateJobAsync(batchClient, JobId, PoolId);

    // Create a collection of tasks and add them to the Batch job.
    // Provide a shared access signature for the tasks so that they can upload their output
    // to the Storage container.
    await AddTasksAsync(batchClient, JobId, inputFiles, outputContainerSasUrl);

    // Monitor task success or failure, specifying a maximum amount of time to wait for
    // the tasks to complete.
    await MonitorTasks(batchClient, JobId, TimeSpan.FromMinutes(30));
```

Snippet of (parallel) Task creation:

```C#
List<CloudTask> tasks = new List<CloudTask>();
// loop {
    CloudTask task = new CloudTask(taskId, taskCommandLine);
    task.ResourceFiles = new List<ResourceFile> { inputFiles[i] };
    task.OutputFiles = outputFileList;
    tasks.Add(task);
// } loop

// Call BatchClient.JobOperations.AddTask() to add the tasks as a collection rather than making a
// separate call for each. Bulk task submission helps to ensure efficient underlying API
// calls to the Batch service.
await batchClient.JobOperations.AddTaskAsync(jobId, tasks);
```
