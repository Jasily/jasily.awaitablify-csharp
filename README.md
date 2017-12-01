# Jasily.Awaitablify

If you have a unknown object,
it canbe anything,
awaitable (like `Task`) or not (`string` or `null`).

And you want:

* If the object is awaitable, then `await` it.
* Else, ignore it.

How would you do that?

## Usage

``` cs
var awaitablifier = new Awaitablifier();

await awaitablifier.Awaitablify(1).HasResult<int>(); // 1
await awaitablifier.Awaitablify(Task.Run(() => 1)).HasResult<int>(); // 1

var deepTask = Task.Run<Task<Task<string>>>(() => Task.Run<Task<string>>(() => Task.Run(() => "xyz")));
await awaitablifier.UnpackAsync(task); // "xyz"
```
