# Blobs

List containers:

    az storage container list --account-name wbdatalakedev

List blobs in a container:

    az storage blob list --account-name wbdatalakedev --container-name geonames-org

Download a blob:

    az storage blob download --account-name wbdatalakedev --container-name geonames-org --name postcodes/transform/AU.csv --file ~/Downloads/AU.csv

Download with `azcopy`:

    azcopy login
    azcopy copy "https://wbdatalakedev.blob.core.windows.net/geonames-org/postcodes/transform/AU.csv?<SAS_TOKEN>" "/tmp/AU.csv"


## Leases

Create a 30 second lease:

    StorageLease=$(az storage blob lease acquire --account-name wbdatalakedev --container-name geonames-org --blob-name postcodes/transform/AU.csv --lease-duration 30 -o tsv)

    echo $StorageLease

Release the lease:

    az storage blob lease release --account-name wbdatalakedev --container-name geonames-org --blob-name postcodes/transform/AU.csv --lease-id $StorageLease

Break a lease:

    az storage blob lease break --account-name wbdatalakedev --container-name geonames-org --blob-name postcodes/transform/AU.csv


## Metadata

```bash
StorageAccount=wbdatalakedev
ContainerName=geonames-org
BlobName=postcodes/transform/AU.csv

az storage blob metadata update --account-name $StorageAccount \
    --container-name $ContainerName \
    --name $BlobName \
    --metadata Source=GeoNames Environment=Dev

az storage blob metadata show --account-name $StorageAccount \
    --container-name $ContainerName \
    --name $BlobName
```

## Tiers

Create an `archive` container, copy some data and change the tier:

```bash
StorageAccount=wbdatalakedev
ContainerName=geonames-org
BlobName=postcodes/transform/AU.csv

az storage container create --account-name $StorageAccount \
    --name archive

az storage blob copy start --account-name $StorageAccount \
                           --destination-blob $BlobName \
                           --destination-container archive \
                           --source-blob $BlobName \
                           --source-container $ContainerName

az storage blob set-tier --account-name $StorageAccount \
                        --container-name archive \
                        --name $BlobName \
                        --tier Cool

az storage blob set-tier --account-name $StorageAccount \
                        --container-name archive \
                        --name $BlobName \
                        --tier Archive
```
