# Logic Apps

## Sample ARM template

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {},
    "variables": {},
    "resources": [
        {
            "type": "Microsoft.Logic/workflows",
            "apiVersion": "2017-07-01",
            "name": "az-203",
            "location": "centralus",
            "properties": {
                "state": "Enabled",
                "definition": {
                    "$schema": "https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#",
                    "contentVersion": "1.0.0.0",
                    "parameters": {
                        "$connections": {
                            "defaultValue": {},
                            "type": "Object"
                        }
                    },
                    "triggers": {
                        "manual": {
                            "type": "Request",
                            "kind": "Http",
                            "inputs": {
                                "schema": {
                                    "properties": {
                                        "DueDate": {
                                            "type": "string"
                                        },
                                        "Name": {
                                            "type": "string"
                                        }
                                    },
                                    "type": "object"
                                }
                            }
                        }
                    },
                    "actions": {
                        "List_items": {
                            "runAfter": {},
                            "type": "ApiConnection",
                            "inputs": {
                                "host": {
                                    "connection": {
                                        "name": "@parameters('$connections')['az203-todo-app']['connectionId']"
                                    }
                                },
                                "method": "get",
                                "path": "/items"
                            }
                        }
                    },
                    "outputs": {}
                },
                "parameters": {
                    "$connections": {
                        "value": {
                            "az203-todo-app": {
                                "connectionId": "/subscriptions/e999a128-5ba0-4de2-80f6-8ac9869a1d72/resourceGroups/az-203-training/providers/Microsoft.Web/connections/az203-todo-app",
                                "connectionName": "az203-todo-app",
                                "id": "/subscriptions/e999a128-5ba0-4de2-80f6-8ac9869a1d72/resourceGroups/az-203-training/providers/Microsoft.Web/customApis/az203-todo-app"
                            }
                        }
                    }
                }
            }
        }
    ]
}
```

## Custom connector

```yaml
swagger: '2.0'
info:
  title: ToDo API
  description: A simple example ASP.NET Core Web API
  termsOfService: https://example.com/terms
  contact: {name: Fred Nurk, url: 'http://www.example.com', email: test@example.com}
  license: {name: Use under LICX, url: 'https://example.com/license'}
  version: v1
host: BLAH.azurewebsites.net
basePath: /
schemes: [https]
consumes: []
produces: []
paths:
  /items:
    get:
      tags: [TodoItems]
      responses:
        '200': {description: Success}
      summary: List items
      operationId: ListTodo
      description: List items
    post:
      tags: [TodoItems]
      responses:
        '200': {description: Success}
      summary: Add an item
      operationId: AddTodo
      description: Add an item
definitions: {}
parameters: {}
responses: {}
securityDefinitions: {}
security: []
tags: []
```
