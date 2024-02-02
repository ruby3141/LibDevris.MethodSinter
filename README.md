 LibDevris.MethodSinter <br>
[![NuGet Version](https://img.shields.io/nuget/v/Devris.LibDevris.MethodSinter)](https://www.nuget.org/packages/Devris.LibDevris.MethodSinter)
====

Define multiple times, Call at once.

## Raison d'être

Suppose you have a `partial class`, with multiple files,
and you are about to add a `private` field to use it internally.

If you know that it would serve perpose on narrow scope, like one or two methods on one file,
defining it near usage will be good for readability.

And there's a problem. You need to initialize it.
Field initializer cannot access to any instance members(because it runs prior to the constructor).<br>
If your field need to read configuration or whatever to initialize,
it need to be initialized separately on or after the constructor.

One option is to lazy-load it manually with property.
Because `Lazy<T>` is unusable on this use case, result looks somewhat bulky.

The other way is just initialize it on constructor, which is on other file.
It makes initialization separated with definition or usage, hurting readability.

Thus this was made to provide another option.

## How to use

[Read this.](/Devris.LibDevris.MethodSinter/README.md)
