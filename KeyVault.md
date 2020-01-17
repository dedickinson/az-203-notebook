# Key Vault

    az keyvault list

## Secrets

Create a secret:

    az keyvault secret set --value "Secret Squirrel" --name "SpecialAgent" --vault-name "kvaz203duncan"

List secrets:

    az keyvault secret list --vault-name "kvaz203duncan"

Get a secret:

    az keyvault secret show --name "SpecialAgent" --vault-name "kvaz203duncan"

## Keys

List:

    az keyvault key list --vault-name "kvaz203duncan"

Create:

    az keyvault key create --vault-name "kvaz203duncan" --name RebelCodeKey

Get:

    az keyvault key show --vault-name "kvaz203duncan" --name RebelCodeKey
