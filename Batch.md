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
