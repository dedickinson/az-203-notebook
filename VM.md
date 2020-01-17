# Virtual Machines

## Encrypted Disks

URL: https://docs.microsoft.com/en-us/azure/virtual-machines/windows/encrypt-disks#encrypt-a-virtual-machine

Setup some variables:

```powershell
$keyVaultName = "kvaz203duncan"
$rgName = "az-203-training"
$vmName = "az203vm"
$keyName = "VMKey"
```

Create an encryption key:

```powershell
Add-AzKeyVaultKey -VaultName $keyVaultName -Name $keyName -Destination 'Software'
```

Encrypt the VM:

```powershell
$keyVault = Get-AzKeyVault -VaultName $keyVaultName -ResourceGroupName $rgName;
$diskEncryptionKeyVaultUrl = $keyVault.VaultUri;
$keyVaultResourceId = $keyVault.ResourceId;
$keyEncryptionKeyUrl = (Get-AzKeyVaultKey -VaultName $keyVaultName -Name $keyName).Key.kid;

Set-AzVMDiskEncryptionExtension -ResourceGroupName $rgName `
    -VMName $vmName `
    -DiskEncryptionKeyVaultUrl $diskEncryptionKeyVaultUrl `
    -DiskEncryptionKeyVaultId $keyVaultResourceId `
    -KeyEncryptionKeyUrl $keyEncryptionKeyUrl `
    -KeyEncryptionKeyVaultId $keyVaultResourceId
```

Check if a VM has encrypted disks - before:

```powershell
PS Azure:\> Get-AzVmDiskEncryptionStatus  -ResourceGroupName $rgName -VMName $vmName

OsVolumeEncrypted          : NotEncrypted
DataVolumesEncrypted       : NotEncrypted
OsVolumeEncryptionSettings :
ProgressMessage            : No Encryption extension or metadata found on the VM
```

... and after:

```powershell
PS Azure:\> Get-AzVmDiskEncryptionStatus  -ResourceGroupName $rgName -VMName $vmName

OsVolumeEncrypted          : Encrypted
DataVolumesEncrypted       : NoDiskFound
OsVolumeEncryptionSettings : Microsoft.Azure.Management.Compute.Models.DiskEncryptionSettings
ProgressMessage            : Provisioning succeeded
```
