// See https://aka.ms/new-console-template for more information
var shockApi = new ShockApi.ShockApi(ShockApi.Provider.PISHOCK, "<snip>", "<snip>", "API Test");
await shockApi.Populate();

foreach (var item in shockApi.GetOwnShockers())
{
    Console.WriteLine(item.Key);
}