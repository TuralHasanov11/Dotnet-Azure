### Tutorial: Access Microsoft Graph from a secured .NET app as the app

```sh
az login

webAppName="app-dotazure-webapp-dev-001"

spId=$(az resource list -n $webAppName --query [*].identity.principalId --out tsv)

graphResourceId=$(az ad sp list --display-name "Microsoft Graph" --query [0].id --out tsv)

appRoleId=$(az ad sp list --display-name "Microsoft Graph" --query "[0].appRoles[?value=='User.Read.All' && contains(allowedMemberTypes, 'Application')].id" --output tsv)

uri=https://graph.microsoft.com/v1.0/servicePrincipals/$spId/appRoleAssignments

body="{'principalId':'$spId','resourceId':'$graphResourceId','appRoleId':'$appRoleId'}"

az rest --method post --uri $uri --body $body --headers "Content-Type=application/json"
```


### Create a webjob
```sh
Compress-Archive -Path run.sh, WebJob1\bin\Debug\net9.0\win-x64\* -DestinationPath WebJob1.zip

Compress-Archive -Path run.sh -DestinationPath WebJob1.zip
Compress-Archive -Path WebJob1\bin\Debug\net9.0\win-x64\* -DestinationPath WebJob1.zip -Update
```