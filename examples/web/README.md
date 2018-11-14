# Akkatecture Api Example

This api example serves as a reference as to how one might want to integrate Akkatecture into their Aspnet Core applications.

The example also shows the standard way of representing long running operations RESTfully.

### Description

When using async workflows in your API it becomes more tricky to coordinate long running operations that may complete at arbitrary times. In this example we will simulate this workflow.

### Startup

One of the main issues with `IActorRef` is that people get confused as to how they can inject them into their Aspnet Core application. There are a few approaches to this, one being creating a static class of many `IActorRef` properties, this is how it has been suggested by [petabridge](https://petabridge.com/blog/akkadotnet-aspnet/). [Other community members](https://havret.io/akka-net-asp-net-core) use a clever way of typing the `IActorRef` by injecting a delegate that returns that `IActorRef`. All these approaches are perfectly fine to be honest, Akkatecture has its own way of doing the same thing. 

#### IAkkatectureBuilder and ActorRefProvider<T>

Akkatecture has built in a new IServiceCollection extension method to help you inject your akka.net dependancies. In your `ConfigureServices` method do the following.

```
public void ConfigureServices(IServiceCollection services)
{
    var actorSystem = ActorSystem.Create("my-system");
    var myActor = actorSystem.ActorOf(Props.Create(() => new MyActor()),"MyActor");

    services
        .AddAkkatecture(actorSystem)
        .AddActorReference<MyActor>(myActor);

    services
        .AddTransient<IMyService, MyService>()
}

```

Then you can inject that `ActorRefProvider<T>` into any dependancy:

```
public class MyService : IMyService
{
    private readonly ActorRefProvider<MyActor> _myActor;

    public MyService(ActorRefProvider<MyActor> myActor)
    {
        _myActor = myActor;
    }
}
```

And `ActorRefProvider<T>` can be used like any `IActorRef`

```
public void DoSomething(ActorRefProvider<MyActor> actorRefProvider)
{
	actorRefProvider.Tell("Hello");
}
```


Now that we know how to setup our actor system, within the aspnet core di container, we can go onto seeing how we can model long running stepwise processes from an API design standpoint.

### Api Reference

The Api has the following endpoints:

**Resources**

The resources are the aggregates that we will create.

`POST api/resources`

`GET api/resources`

`GET api/resources/:id`

**Operations**

The operations represent the sagas / long running operations of creating a `resource`.

`GET api/operations`

`GET api/operations/:id`

### How Does It Work

Essentially it is an [ammended](https://github.com/Microsoft/api-guidelines/issues/10) version of Microsoft's [guidelines](https://github.com/Microsoft/api-guidelines/blob/vNext/Guidelines.md#13-long-running-operations) to exposing long running processes via an API. This is very similar to how Azure's Api's expose the provisioning of resources in Azure (creating databases, webapps etc).

When creating a resource with an asynchronous workflow you should respond with an `Accepted 202`, with a link to a polling resource in the `Location` header.  If you want to include a representation of the polling resource in the 202 response, then you can *also* set `Content-Location` to the same URI as `Location` (the URI of the polling resource).  As the client polls the polling resource, once it is complete, then it will use a `SeeOther 303` and `Location` to redirect to the resource that is the result of the completed action.


### Using The Example

**Create a Resource Request**
```
POST api/resources
```
**Create a Resource Response**
```
202 Accepted
Location : api/operations/485669a7-a20f-47a5-a81a-1d55cbc7c210
Content-Location : api/resources/485669a7-a20f-47a5-a81a-1d55cbc7c210
Content-Type : application/json; charset=utf-8
{
    "id": "485669a7-a20f-47a5-a81a-1d55cbc7c210",
    "operationId": "485669a7-a20f-47a5-a81a-1d55cbc7c210"
}
```

**Polling The Running Operation Request**

```
GET api/operations/485669a7-a20f-47a5-a81a-1d55cbc7c210
```
**Polling The Running Operation Response**
```
200 OK
{
    "id": "485669a7-a20f-47a5-a81a-1d55cbc7c210",
    "percentage": 27,
    "elapsed": 4,
    "status": "Running",
    "startedAt": "2018-10-07T08:12:38.890335Z"
}
```
**Polling The Finished Operation Request**
```
GET api/operations/485669a7-a20f-47a5-a81a-1d55cbc7c210
```

**Polling The Finished Operation Request**

```
303 See Other
Location : api/resources/a6442d6c-0130-4c76-8cf3-98be83f0ca63
```

Use the Location header to redirect your next request.

**Requesting The Created Resource**

```
GET api/resources/a6442d6c-0130-4c76-8cf3-98be83f0ca63
```

```
{
    "id": "a6442d6c-0130-4c76-8cf3-98be83f0ca63",
    "elapsedTimeToCreation": 18,
    "createdAt": "2018-10-07T08:12:56.938187Z"
}
```

