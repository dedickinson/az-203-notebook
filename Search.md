# Azure Search

* MSFT Docs: https://docs.microsoft.com/en-us/azure/search/
* Project: https://github.com/weather-balloon/deploy-search

## Sample C# Code

### Connect

```C#
SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
```

### Create an Index

Setup the model:

```C#
using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;


public partial class Postcode
{
    [IsSearchable, IsFilterable, IsSortable, IsFacetable]
    public string City { get; set; }

    [IsSearchable, IsFilterable, IsSortable, IsFacetable]
    public string StateProvince { get; set; }

    [IsSearchable, IsFilterable, IsSortable, IsFacetable]
    public string PostalCode { get; set; }

    [IsSearchable, IsFilterable, IsSortable, IsFacetable]
    public string Country { get; set; }
}
```

Generate the index:

```C#
var definition = new Index()
{
    Name = indexName,
    Fields = FieldBuilder.BuildForType<Postcode>()
};

serviceClient.Indexes.Create(definition);
```

### Add Documents

```C#
var actions = new IndexAction<Hotel>[]
{
    IndexAction.Upload(
        new Hotel()
    {
        // set attributes
    },
    IndexAction.Upload(
        new Hotel()
    {
        // set attributes
    }
}

var batch = IndexBatch.New(actions);
indexClient.Documents.Index(batch);
```

### Perform Search

```C#
parameters = new SearchParameters()
{
    Select = new[] { "City", "PostalCode" },
    Top = 1
};

results = indexClient.Documents.Search<Postcode>("Gimpy", parameters);
```

### Sample JSON Config

Submit JSON-based config via REST API

### Sample Index

```json
{
    "name": "postcodes",
    "fields": [
        {
            "name": "id",
            "type": "Edm.String",
            "key": true,
            "retrievable": true,
            "filterable": true,
            "searchable": true,
            "sortable": true,
            "facetable": false
        },
        {
            "name": "country_code",
            "type": "Edm.String",
            "key": false,
            "retrievable": true,
            "filterable": true,
            "searchable": true,
            "sortable": true,
            "facetable": true
        },
        {
            "name": "postal_code",
            "type": "Edm.String",
            "key": false,
            "retrievable": true,
            "filterable": true,
            "searchable": true,
            "sortable": true,
            "facetable": true
        },
        {
            "name": "place_name",
            "type": "Edm.String",
            "key": false,
            "retrievable": true,
            "filterable": true,
            "searchable": true,
            "sortable": true,
            "facetable": false
        },
        {
            "name": "state_name",
            "type": "Edm.String",
            "key": false,
            "retrievable": true,
            "filterable": true,
            "searchable": true,
            "sortable": true,
            "facetable": true
        },
        {
            "name": "state_code",
            "type": "Edm.String",
            "key": false,
            "retrievable": true,
            "filterable": true,
            "searchable": true,
            "sortable": true,
            "facetable": true
        },
        {
            "name": "location",
            "type": "Edm.GeographyPoint",
            "key": false,
            "retrievable": true,
            "filterable": true,
            "searchable": false,
            "sortable": true,
            "facetable": false
        }
    ],
    "suggesters": [
        {
            "name": "place_name",
            "searchMode": "analyzingInfixMatching",
            "sourceFields": [
                "place_name"
            ]
        }
    ],
    "scoringProfiles": [
        {
            "name": "geo",
            "text": {
                "weights": {
                    "place_name": 5,
                    "postal_code": 5
                }
            },
            "functions": [
                {
                    "type": "distance",
                    "boost": 5,
                    "fieldName": "location",
                    "interpolation": "logarithmic",
                    "distance": {
                        "referencePointParameter": "currentLocation",
                        "boostingDistance": 10
                    }
                }
            ]
        }
    ]
}
```

### Sample Datasource

```json
{
    "name": "postcodes-blob-datasource",
    "description": "Configures a Blob as the postcode datasource",
    "type": "azureblob",
    "container": {
        "name": "geonames-org",
        "query": "postcodes/transform"
    },
    "credentials": {
        "connectionString": ""
    }
}
```

### Sample Indexer

Indexes a Blob-based CSV file.

```json
{
    "name": "postcodes-csv-indexer",
    "dataSourceName": "postcodes-blob-datasource",
    "targetIndexName": "postcodes",
    "parameters": {
        "configuration": {
            "parsingMode": "delimitedText",
            "indexedFileNameExtensions": ".csv",
            "firstLineContainsHeaders": true
        }
    },
    "fieldMappings": [
        {
            "sourceFieldName": "id",
            "targetFieldName": "id"
        },
        {
            "sourceFieldName": "country_code",
            "targetFieldName": "country_code"
        },
        {
            "sourceFieldName": "postal_code",
            "targetFieldName": "postal_code"
        },
        {
            "sourceFieldName": "place_name",
            "targetFieldName": "place_name"
        },
        {
            "sourceFieldName": "admin_name1",
            "targetFieldName": "state_name"
        },
        {
            "sourceFieldName": "admin_code1",
            "targetFieldName": "state_code"
        },
        {
            "sourceFieldName": "location",
            "targetFieldName": "location"
        }
    ]
}
```

## Example Searches - postcodes

Basic initial search with facet count for the `state_name`

    $count=true&facet=state_name

Add in the `country_code` facet:

    $count=true&facet=state_name&facet=country_code

### Suggester

The `place_name` suggester is a handy way to help guide the user with possible matches:

    https://wb-search-dev.search.windows.net/indexes/postcodes/docs/suggest?api-version=2019-05-06&suggesterName=place_name&search=Melr

The suggester results can be enhanced with extra fields:

    https://wb-search-dev.search.windows.net/indexes/postcodes/docs/suggest?api-version=2019-05-06&suggesterName=place_name&search=Melr&$select=place_name,state_name,postal_code

### Map searching

Say we're at (`-33.8521,151.0646`) lat/long the following search will find the
places (suburbs) no more than 5km from us:

    $count=true&$filter=geo.distance(location, geography'POINT(151.0646 -33.8521)') le 5

Add in some facets to help the user:

    $count=true&$filter=geo.distance(location, geography'POINT(151.0646 -33.8521)') le 5&facet=postal_code&facet=state_code
