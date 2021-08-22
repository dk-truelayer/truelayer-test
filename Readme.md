
TrueLayer take home exercise solution.

This solution is running .Net 5.0.

```shell
dotnet test # Run the tests.
dotnet run  # Run the API.
```

# Docker support

I intended to add docker support, but I couldn't do it with my current environment.
I'm away from my usual development machine in a place without much internet, so I
couldn't get all the prerequisites in place.

# Testing

Two test projects discriminate the two levels of tests. 

- `TrueLayer.Api.Tests` contains unit tests for the components of the codebase where this is sensible.
  In this case there weren't many 'sensible' places though.
- `TrueLayer.Api.Tests.Acceptance` contains acceptance tests, which work against the API endpoints.

# Approach

My approach to this challenge was similar to a 'red-green-refactor'. I started
by writing a stubbed controller and acceptance tests covering the specification.
Then, I implemented the functionality in services, with these functions doing
everything. Finally, I refactored the functions into other injected modules,
until the services simply wired together inner layers.

For external services, I focused on making easily swappable implementations. For
both the pokemon and translation clients, it is easy to switch provider. This is
used in integration testing (which uses the dummy providers), but could also be
used for offline development. The implementation can be chosen in the 
`appsettings.json` files. 

With the main layout done, I optimised by adding a caching layer. This ensures
that if the same pokemon is requested twice, with or without translation, 
external API's are called only once. The current implementation uses the built-in
`IMemoryCache`, which is local to each instance. In production, an implementation
using a distributed cache such as redis would be needed. This architecture makes
it very easy to add an additional implementation.

# Things that would be done differently in production.

In it's current state, the app is mostly ready to be pushed to production, but
it is missing some key features.

Firstly it isn't completely stateless. The caching implementation is single-node,
so if the API was run in a cluster the chances of cache misses would be much 
higher. Implementing a distributed cache would resolve this.

Logging isn't completely implemented yet. I'd like to add something like serilog,
that can spit out .ndjson or something that can easily be incorporated into our
observability stack. APM can also be added.

There is no security on this API. I'd at least consider adding an API key 
mechanism, at least to add rate limiting. Also, I'd think about adding CORS.

Rate limiting should be done at the infrastructure layer, otherwise it will 
be affected by load balancers.

# Decisions

## Using `IHttpClientFactory`

Instantiating `HttpClient` everywhere is beyond a bad practice. Under heavy loads,
this could lead to socket exhaustion, because each instance owns it's own socket.

Additionally, a singleton `HttpClient` isn't great either, because it is not 
guaranteed to respect DNS updates.

The mitigation was introduced in .net core 2.1, which is the `IHttpClientFactory`.
This cycles the message handlers, preventing stale DNS; and also pools the handlers,
preventing socket exhaustion.

## String cleaning

The `flavor_text` returned by PokeApi often had a lot of escaped whitespace 
characters (`\r\n`) in it. I wrote an extension method to clean replace all
whitespace blocks with a single space. 

## Whether or not to use a client library

The `PokeApi` has a client library, which I believe is generated from OpenApi
files. I chose not to use this for two reasons. Firstly, I thought this was
simple enough not to need the external dependency. Secondly, I wanted to 
demonstrate I knew how to use the `HttpClient`.

## Swagger

I added swagger simply to make it easy to test the API manually. In production,
I would go further defining endpoint metadata, but in this environment I didn't
want to use the time.

## Caching

Caching the pokemon and translation results massively improves performance on 
repeated requests. 

This implementation only considers single node caching, which
means multiple nodes in a cluster would maintain their own cache.
We could fix this using redis as a cache backend.

The cached data structure is slightly coupled to the implementation, however.
Each cache entry may contain an untranslated pokemon, and a translated pokemon.
This operates under the assumption that the translation language won't change. 
With an in-memory cache, this is ok, because changing the translation language
requires restarting the server and losing the cache. A distributed cache doesn't
have this property. If a distributed cache were to be added, we would likely need
to add a mechanism of specifying which language is being stored.

## Configuration

Each feature is responsible for it's own configuration, which is the injected
through a static class in `Microsoft.Extensions.DependencyInjection`.
This allows the internal representation of the feature to be independent from
the rest of the application. We could separate each feature into a separate 
library, but that would incur some complexity unnecessary at this time.

## Logging

Usually I'd add some detailed logs but I didn't have the time here. I would
have liked to have added a logging library like Serilog, and put statements
on important events to ease debugging at least.

There is one place I found logs particularly useful in development - tracking
cache hits and misses.

# Architecture

The architecture is a vague onion. The reason it's vague is that the
substance of this project is the external API integrations, with very
little in the domain core.

The hierarchy goes Controller -> Service -> Manager. Controllers know
about HTTP, services coordinate components, managers are the 'functional core'.

Features contains the modules that are designed to be changeable. They each
expose an API, but the implementation is kept hidden. They could be in
separate dll's, but this incurs some unnecessary complexity right now.

# Layout

This section explains decisions surrounding codebase layout.

## Services
Because service interface definitions, such as `IPokemonService` usually have
only one tightly coupled implementation, I like to keep them in the same file.
In my experience, I've never written an application service and needed to 
reimplement the interface.
