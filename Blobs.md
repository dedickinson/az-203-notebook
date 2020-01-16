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

    az storage blob lease acquire --account-name wbdatalakedev --container-name geonames-org --blob-name postcodes/transform/AU.csv --lease-duration 30
