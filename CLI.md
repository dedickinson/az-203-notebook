# Az CLI commands

## Web Apps

    az webapp list

    az webapp show -n az203-todo-api -g az-203-training

    az webapp webjob continuous list -n az203-todo-api -g az-203-training

    az webapp stop -n az203-todo-api -g az-203-training

App service plans:

    az appservice plan list

    az appservice plan show -n az203-todo-api -g az-203-training

    az appservice plan update -n az203-todo-api -g az-203-training --sku B2
