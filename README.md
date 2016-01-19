sailthru-net-client
====================

For installation instructions, documentation, and examples please visit: [http://getstarted.sailthru.com/new-for-developers-overview/api-client-library/net](http://getstarted.sailthru.com/new-for-developers-overview/api-client-library/net)

A simple client library to remotely access the `Sailthru REST API` as per [http://getstarted.sailthru.com/developers/api](http://getstarted.sailthru.com/developers/api)

UserRequest.OptoutEmail is now an enum rather than a string. Replace `"all"` with "OptoutStatus.All", `"blast"` with "OptoutStatus.Blast", "basic" with "OptoutStatus.Basic", and `"none"` with "OptoutStatus.None". For example, "userRequest.OptoutEmail = "all";" should now be "userRequest.OptoutEmail = OptoutStatus.All;"
