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
1. Add a Cors validation policy to the product pointing at the frontend FQDN
1. Make sure imported AzFunc APIs are set this way :
   ![APIs Azure Function](./API-settings.png)
1. Add a `validate-jwt` policy to the APIM Users Product :
   ```xml
   <policies>
    <inbound>
        <base />
        <cors allow-credentials="false">
            <allowed-origins>
                <origin>https://web-trash-we.azurewebsites.net</origin>
            </allowed-origins>
            <allowed-methods>
                <method>GET</method>
                <method>POST</method>
            </allowed-methods>
        </cors>
        <validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Unauthenticated client">
            <openid-config url="https://sts.windows.net/dbf66fc6-f491-4387-b638-101810058f9c/.well-known/openid-configuration" />
        </validate-jwt>
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
    </outbound>
    <on-error>
        <base />
    </on-error>
   </policies>
   ```
1. Create a JSON Schema named New-User-API:

   ```JSON
   {
      "$schema": "http://json-schema.org/draft-04/schema#",
      "type": "object",
      "properties": {
      "FirstName": {
         "type": "string"
      },
      "LastName": {
         "type": "string"
      },
      "PartitionKey": {
         "type": "string"
      },
      "RowKey": {
         "type": "string"
      }
      },
      "required": [
      "FirstName",
      "LastName",
      "PartitionKey",
      "RowKey"
      ]
   }
   ```

1. Create a Json Validate Content (Schema Validation Policy) attached to the `Create Users` operations:

   ```XML
      <policies>
         <inbound>
            <base />
            <validate-content unspecified-content-type-action="ignore" max-size="1024" size-exceeded-action="prevent" errors-variable-name="usersValidationError">
                  <content type="application/json" validate-as="json" action="ignore" schema-id="New-Users-API-Schema" />
            </validate-content>
         </inbound>
         <backend>
            <base />
         </backend>
         <outbound>
            <base />
         </outbound>
         <on-error>
            <base />
         </on-error>
      </policies>
   ```

   [Offline-token]: https://learn.microsoft.com/en-us/azure/app-service/tutorial-auth-aad?pivots=platform-linux#configure-app-service-to-return-a-usable-access-token
