1. Execute the following AzCLI commands to activate Refresh Token ([Offline_token][Offline-token]) on App Service Resource
   ```bash
        az extension add --name authV2
        authSettings=$(az webapp auth show -g rg-innovationday -n web-trash-we)
        authSettings=$(echo "$authSettings" | jq '.properties' | jq '.identityProviders.azureActiveDirectory.login += {"loginParameters":["scope=openid offline_access api://fb7e8694-1568-4fb4-a160-4e6fe4d39cb3/user_impersonation"]}')
        az webapp auth set --resource-group rg-innovationday --name web-trash-we --body "$authSettings"
   ```
1. Show the auth config of frontend resource :
   ```bash
       az webapp auth show -g rg-innovationday -n web-trash-we
   ```

[Offline-token]: https://learn.microsoft.com/en-us/azure/app-service/tutorial-auth-aad?pivots=platform-linux#configure-app-service-to-return-a-usable-access-token
