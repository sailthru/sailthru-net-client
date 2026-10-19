sailthru-net-client
====================

For installation instructions, documentation, and examples please visit: [https://getstarted.sailthru.com/developers/api-client/net/](https://getstarted.sailthru.com/developers/api-client/net/)

A simple client library to remotely access the `Sailthru REST API` as per [http://getstarted.sailthru.com/developers/api](http://getstarted.sailthru.com/developers/api)

As of commit b8714e2, UserRequest.OptoutEmail is now an enum rather than a string. Replace `"all"` with "OptoutStatus.All", `"blast"` with "OptoutStatus.Blast", "basic" with "OptoutStatus.Basic", and `"none"` with "OptoutStatus.None". For example, "userRequest.OptoutEmail = "all";" should now be "userRequest.OptoutEmail = OptoutStatus.All;"

#### API Rate Limiting

Here is an example how to check rate limiting and throttle API calls based on that. For more information about Rate Limiting, see [Sailthru Documentation](https://getstarted.sailthru.com/new-for-developers-overview/api/api-technical-details/#Rate_Limiting)


```csharp
SailthruClient sailthruClient = new SailthruClient(apiKey, apiSec);

// ... make some api calls ....

Hashtable lastRateLimitInfo = sailthruClient.getLastRateLimitInfo("user", "post");

// getLastRateLimitInfo returns null if given endpoint/method wasn't triggered previously
if (lastRateLimitInfo != null) {
	int limit = lastRateLimitInfo['limit'];
	int remaining = lastRateLimitInfo['remaining'];
	DateTime reset = (DateTime)lastRateLimitInfo['reset'];

    // throttle api calls based on last rate limit info
    if (remaining <= 0) {
        TimeSpan time_span_till_reset = reset.Subtract(DateTime.now());
        // sleep or perform other business logic before next user api call
        Thread.Sleep(time_span_till_reset);
    }
}
```

#### Quality checks

CI tests .NET 8, 9, 10, and the newest supported stable release (`current`) on PRs,
pushes to `master`, weekly, and manual runs. Library packages retain `netstandard2.0`.

With the .NET 10 SDK installed, run from the repository root:

```shell
dotnet restore Sailthru/Sailthru.sln -p:ClientTestFramework=net10.0
dotnet build Sailthru/Sailthru.sln --no-restore --configuration Release -p:ClientTestFramework=net10.0
dotnet test Sailthru/Sailthru.sln --no-build --configuration Release -p:ClientTestFramework=net10.0
bash scripts/test-package.sh net10.0
```

The package check installs the built NuGet artifact into a temporary consumer and
makes a local request with dummy credentials. These commands do not publish; the
existing tag-release workflow runs the package check before publishing.
