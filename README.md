# BYONDTopic

BYONDTopic is a .NET Standard 2.1 package for interfacing with BYOND servers VIA world topic calls.

## Installation

You can install the package from [Nuget](https://www.nuget.org/packages/ByondTopic).

## Usage

The API is designed to be very simple to use, and can be consumed in one of the two following ways. Note that the object returned from the ``Query`` method is a ``QueryResponse`` object, which has a ToString override for the raw text value, as well as a utility method for attempting to convert it into a key,value dictionary.

```csharp
using ByondTopic;
< ... >
var topic = new TopicSource("bagil.tgstation13.org", 2337);
var queryResponse = topic.Query("status");
var keyValueDictionary = queryResponse.AsDictionary;
```

Or

```csharp
using ByondTopic;
< ... >
var topic = new TopicSource("byond.paradisestation.org", 6666);
var queryResponse = topic.QueryJson<Dictionary<string, Dictionary<string, string>>>("manifest");
```

Note that the QueryJson method is just a convenient wrapper for deserializing returned JSON using System.Text.Json.