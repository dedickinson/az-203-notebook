# API Management

## Policies

### Returning a response

This example performs an addition calculation on two query params (`num1` & `num2`)
and returns the result without using the backend.

    <policies>
        <inbound>
            <base />
            <return-response>
                <set-status code="200" reason="Success" />
                <set-body>
                @{
                    var responseBody = new Dictionary<string, int>
                    {
                        {"num1", Int32.Parse(context.Request.Url.Query["num1"].First())},
                        {"num2", Int32.Parse(context.Request.Url.Query["num2"].First())}
                    };
                    responseBody.Add("result", responseBody["num1"] + responseBody["num2"]);

                    return JsonConvert.SerializeObject(responseBody);
                }
                </set-body>
            </return-response>
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


### Caching

The following example will cache a response and return the cached value - note the `vary-by`:

    <policies>
        <inbound>
            <cache-lookup vary-by-developer="false" vary-by-developer-groups="false" downstream-caching-type="none">
                <vary-by-header>Accept</vary-by-header>
                <vary-by-header>Accept-Charset</vary-by-header>
                <vary-by-header>Authorization</vary-by-header>
                <vary-by-query-parameter>id</vary-by-query-parameter>
            </cache-lookup>
            <base />
        </inbound>
        <backend>
            <base />
        </backend>
        <outbound>
            <cache-store duration="60" />
            <base />
        </outbound>
        <on-error>
            <base />
        </on-error>
    </policies>

Lookup and set a cache value:

    <policies>
        <inbound>
            <cache-lookup-value key="magic-number" variable-name="magicnumber" default-value="0" />
            <base />
        </inbound>
        <backend>
            <base />
        </backend>
        <outbound>
            <cache-store-value key="magic-number" value="42" duration="30" />
            <set-header name="magic" exists-action="override">
                <value>@((string)context.Variables["magicnumber"])</value>
            </set-header>
            <base />
        </outbound>
        <on-error>
            <base />
        </on-error>
    </policies>

### Quotas

Rate limit based on subscription:

    <rate-limit calls="2" renewal-period="120" />

Rate limit based on a key (IP address):

    <rate-limit-by-key calls="5" renewal-period="120" counter-key="@(context.Request.IpAddress)" />

Quota for a subscription - can be set at product or operation level:

    <quota calls="2" bandwidth="40000" renewal-period="3600" />

Quota for a key:

    <quota-by-key calls="2" bandwidth="100" renewal-period="3600" counter-key="@(context.Request.IpAddress)" />
