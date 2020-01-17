# Containerised Solutions

* Project: https://github.com/dedickinson/webapp-variations

## Azure Container Registry

Create:

```bash
az acr create --name $ACR_NAME \
              --resource-group $RESOURCE_GROUP \
              --location $RESOURCE_GROUP_LOCATION \
              --sku Standard
```

Build an image using ACR:

```bash
az acr build --registry $ACR_NAME -t $IMAGE_NAME .
```

Access locally:

```bash
az acr login --name $ACR_NAME
docker pull $ACR_NAME.azurecr.io/$IMAGE_NAME:cj2
```

## Azure Container Instance

Start:

```bash
ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME \
                               --resource-group $RESOURCE_GROUP \
                               --query "loginServer" \
                               --output tsv)

az container create --resource-group $RESOURCE_GROUP \
                    --name $IMAGE_NAME \
                    --image $ACR_LOGIN_SERVER/$IMAGE_NAME:$IMAGE_VERSION \
                    --cpu 1 --memory 1 \
                    --registry-login-server $ACR_LOGIN_SERVER \
                    --registry-username $(az keyvault secret show --vault-name $AKV_NAME -n $ACR_NAME-pull-usr --query value -o tsv) \
                    --registry-password $(az keyvault secret show --vault-name $AKV_NAME -n $ACR_NAME-pull-pwd --query value -o tsv) \
                    --dns-name-label $IMAGE_NAME-$RANDOM \
                    --query ipAddress.fqdn \
                    --ports 3000
```

Stop:

```bash
az container stop --name $IMAGE_NAME --resource-group $RESOURCE_GROUP
```

## Azure Kubernetes Services (AKS)

Create:

```bash
az aks create --resource-group $RESOURCE_GROUP \
            --location $RESOURCE_GROUP_LOCATION \
            --name $AKS_NAME \
            --kubernetes-version 1.12.6 \
            --node-vm-size Standard_B2s \
            --node-count 2 \
            --generate-ssh-keys
```

Connect:

```bash
sudo az aks install-cli
az aks get-credentials --resource-group $RESOURCE_GROUP --name $AKS_NAME
```

_Now use `kubectl`_
