# ServiceBus

Setup:

    export AZ203_RG=az-203-training
    export AZ203_SB_NAMESPACE=az-203-ddsb
    export AZ203_SB_CONNECTIONSTRING=$(az servicebus namespace authorization-rule keys list \
        --resource-group $AZ203_RG \
        --namespace-name $AZ203_SB_NAMESPACE \
        --name RootManageSharedAccessKey \
        --query primaryConnectionString --output tsv)

Create a queue:

    export AZ203_SB_QUEUE=BasicQueue

    az servicebus queue create --resource-group $AZ203_RG --namespace-name $AZ203_SB_NAMESPACE --name $AZ203_SB_QUEUE

Create a topic:

    export AZ203_SB_TOPIC=basictopic
    az servicebus topic create --resource-group $AZ203_RG --namespace-name $AZ203_SB_NAMESPACE --name $AZ203_SB_TOPIC

    export AZ203_SB_SUBSCRIPTION=demo_subscription
    az servicebus topic subscription create --resource-group $AZ203_RG \
        --namespace-name $AZ203_SB_NAMESPACE \
        --name $AZ203_SB_SUBSCRIPTION \
        --topic-name $AZ203_SB_TOPIC
