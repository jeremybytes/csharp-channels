C# Channels
=======================
The aim of this project is to provide an overview of Channel&lt;T&gt; in the C# language. The example presented here mirrors a project in Go (golang). For more information, see [Go for the C# Developer](https://github.com/jeremybytes/go-for-csharp-dev).  

Motivation
----------
I first encoutered channels when looking at Go (golang) several years ago. Channels allow for communication between concurrent operations. In .NET Core 3.0, C# got channels as well -- with Channel&lt;T&gt;.  

To compare the usage of channels in C#, this project mirrors a Go (golang) project. This is not an extensive look at channels. It is a brief example of how they can be used.

Project Layout
--------------
To build and run the code, you will need to have .NET 5 installed on your machine.

**/async** contains a console application that uses channels program  
**/net-people-service** contains a service (used by the console application)  

The "async" program is a console application that calls the service and displays the output. In order to show concurrency, the application gets each record individually.

Running the Service
-------------------  
The **.NET service** can be started from the command line by navigating to the ".../net-people-service" directory and typing `dotnet run`. This provides endpoints at the following locations:

* http://localhost:9874/people  
Provides a list of "Person" objects. This service will delay for 3 seconds before responding. Sample result:

```json
[{"id":1,"givenName":"John","familyName":"Koenig","startDate":"1975-10-17T00:00:00-07:00","rating":6,"formatString":null},  
{"id":2,"givenName":"Dylan","familyName":"Hunt","startDate":"2000-10-02T00:00:00-07:00","rating":8,"formatString":null}, 
{...}]
```

* http://localhost:9874/people/ids  
Provides a list of "id" values for the collection. Sample:  

```json
[1,2,3,4,5,6,7,8,9]
```

* http://localhost:9874/people/1  
Provides an individual "Person" record based on the "id" value. This service will delay for 1 second before responding. Sample record:

```json
{"id":1,"givenName":"John","familyName":"Koenig","startDate":"1975-10-17T00:00:00-07:00","rating":6,"formatString":null}
```

The Console Application
---------------------
The **/async** folder contains a console application that uses channels. The relevant code is in the "Program.cs" file.  

Articles
------------
A set of articles accompanies this example to walk through how channels are used in conjunction with concurrent (async) operations.  

C# Articles:
* [Introduction to Channels in C#](https://jeremybytes.blogspot.com/2021/02/an-introduction-to-channels-in-c.html)  

If you are interested in the Go (golang) sample, there is a set of articles that describe goroutines, channels, WaitGroup, and anonymous functions.
Go Articles:
* [Go (golang) Goroutines - Running Functions Asynchronously](https://jeremybytes.blogspot.com/2021/01/go-golang-goroutines-running-functions.html)  
* [Go (golang) Channels - Moving Data Between Concurrent Processes](https://jeremybytes.blogspot.com/2021/01/go-golang-channels-moving-data-between.html)  
* [Go (golang) WaitGroup - Signal that a Concurrent Operation is Complete](https://jeremybytes.blogspot.com/2021/02/go-golang-waitgroup-signal-that.html)  
* [Go (golang) Anonymous Functions - Inlining Code for Goroutines](https://jeremybytes.blogspot.com/2021/02/go-golang-anonymous-functions-inlining.html)  

  