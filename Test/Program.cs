// See https://aka.ms/new-console-template for more information
var shockApi = new ShockApi.ShockApi(ShockApi.Provider.PISHOCK, "<snip>", "<snip>", "API Test");
await shockApi.Populate();

ShockApi.CommandOptions options = new ShockApi.CommandOptions();
options.shocker = shockApi.GetOwnShockers().First().Value;
options.mode = ShockApi.Mode.VIBERATE;
options.intensity = 50;
options.duration = 10;
options.sendWarning = false;

(bool err, string message) = await shockApi.SendCommandToShocker(options);

if (err) {
    Console.WriteLine("Failed to send");
}
Console.WriteLine(message);
