// See https://aka.ms/new-console-template for more information
var shockApi = new ShockApi.ShockApi(ShockApi.Provider.PISHOCK, "<snip>", "<snip>", "API Test");
await shockApi.Populate();

var shocker = shockApi.GetOwnShockers().First().Value;

(bool err, string message) = await shockApi.SendCommandToShocker(shocker, ShockApi.Mode.VIBERATE, 1, 1);

if (err) {
    Console.WriteLine("Failed to send");
}
Console.WriteLine(message);