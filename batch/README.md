# Azure Batch

- Docs: https://docs.microsoft.com/en-us/azure/batch/tutorial-parallel-python
- Install Batch Explorer: https://azure.github.io/BatchExplorer/
- CLI Extensions: https://github.com/Azure/azure-batch-cli-extensions
- Python examples: https://github.com/Azure-Samples/azure-batch-samples/tree/master/Python/Batch

## Create an account

    RESOURCE_GROUP=az-203-training
    RESOURCE_LOCATION=centralus
    STORAGE_ACCOUNT=saaz203duncan
    BATCH_ACCOUNT=batchaz203duncan
    BATCH_POOL=az203
    BATCH_JOB=az203job
    BATCH_TASK=az203task
    KEY_VAULT=kvaz203duncan

    # Create the Storage Account
    az storage account create --name $STORAGE_ACCOUNT \
        --location $RESOURCE_LOCATION \
        --resource-group $RESOURCE_GROUP \
        --sku Standard_LRS

    az keyvault create --name $KEY_VAULT \
        --location $RESOURCE_LOCATION \
        --resource-group $RESOURCE_GROUP \
        --enabled-for-disk-encryption true

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

## A basic job & tasks

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

    # Track the status
    az batch task list --job-id $BATCH_JOB --out table --query '[].{ID:id,State:state}'

    # Look at the available files
    for i in {1..4}
    do
        az batch task file list \
        --task-id $BATCH_TASK$i \
        --job-id $BATCH_JOB \
        --output table
    done

    az batch job delete --job-id $BATCH_JOB

    az batch pool delete --pool-id $BATCH_POOL


## Run a python-based task

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

    # Add in the cli batch extension to provide us with
    # See https://github.com/Azure/azure-batch-cli-extensions
    az extension add -n azure-batch-cli-extensions

    az batch job create --template job_factorial.json
