# BYONDTopic

BYONDTopic is a .NET Standard 2.1 package for interfacing with BYOND servers VIA world topic calls.

## Installation

You can install the package from [Nuget](https://www.nuget.org/packages/ByondTopic).

## Usage

The API is designed to be very simple to use, and can be consumed in one of the two following ways. Note that the object returned from the ``Query`` method is a ``QueryResponse`` object, which could contain be a textual ``TextQueryResponse``, or a float ``FloatQueryResponse``.

```csharp
using ByondTopic;
< ... >
var topic = new TopicSource("bagil.tgstation13.org", 2337);
var queryResponse = topic.Query("status");
var keyValueDictionary = queryResponse.AsText.AsDictionary;

// If we called AsText here, this would throw a null exception as the response
// of AsText would be null.
var queryResponseFloat = topic.Query("playing");
var floatValue = queryResponseFloat.AsFloat.Response;
```

Or

```csharp
using ByondTopic;
< ... >
var topic = new TopicSource("byond.paradisestation.org", 6666);
var queryResponse = topic.QueryJson<Dictionary<string, Dictionary<string, string>>>("manifest");
```

Note that the ``QueryJson`` method is just a convenient wrapper for deserializing returned JSON using ``System.Text.Json``. Also note that it will throw an exception if the response to the wrapped Query is not a ``TextQueryResponse``.

If given an invalid command, response, or server, the package will typically throw a ``InvalidResponseException`` which can be anticipated and consumed for error handling.