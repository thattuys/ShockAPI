using System.Text.Json;
using System.Text.RegularExpressions;
using ShockApi.Services.PiShock.API;

namespace ShockApi.Services.PiShock;

public class Core : Interfaces.Services
{
    private int? UserID;
    private readonly String APIKey;
    private readonly String UserName;
    private readonly String Origin;
    public Dictionary<String, Shocker> shockers;

    public Core(String userName, String APIKey, String origin ) {
        this.APIKey = APIKey;
        this.UserName = userName;
        this.Origin = origin;
        shockers = [];
    }

    public async Task PopulateNeeded() {
        String uriWithParms =  $"https://auth.pishock.com/Auth/GetUserIfAPIKeyValid?apikey={APIKey}&username={UserName}";
       
        using (HttpClient httpClient = new HttpClient())
        {
            try {
                var res = await httpClient.GetAsync(uriWithParms).ConfigureAwait(false);
                res.EnsureSuccessStatusCode();

                var responseBody = await res.Content.ReadAsStringAsync();
                
                UserID = JsonSerializer.Deserialize<API.UserInfo>(responseBody)!.UserId;
            }
            catch (HttpRequestException ex) {
                Console.WriteLine($"Exception: {ex}");
            }
        }
    }

    public async Task PopulateShockers() {
        if (UserID == 0 || UserID == null) {
            await PopulateNeeded();
        }

        using (HttpClient httpClient = new HttpClient())
        {
            String uriWithParms = $"https://ps.pishock.com/PiShock/GetUserDevices?UserId={UserID}&Token={APIKey}&api=true";
            try {
                var res = await httpClient.GetAsync(uriWithParms).ConfigureAwait(false);
                res.EnsureSuccessStatusCode();
                var responseBody = await res.Content.ReadAsStringAsync();

                var userDevices = JsonSerializer.Deserialize<API.UserDevices[]>(responseBody)!;
                foreach (var usr in userDevices)
                {
                    foreach (var dev in usr.shockers!) {
                        var shocker = new Shocker();
                        shocker.Name = dev.name;
                        shocker.MaxIntensity = 100;
                        shocker.OwnShocker = true;
                        shocker.ShareCode = "";
                        shocker.ShockerId = dev.shockerId;
                        shocker.Owner = UserName;
                        shockers.Add(dev.name!, shocker);
                    }
                }

            }
            catch (HttpRequestException ex) {
                Console.WriteLine($"Exception: {ex}");
            }

            try {
                int[] ids;
                { // Get a list of shockers shared with us.
                    uriWithParms = $"https://ps.pishock.com/PiShock/GetShareCodesByOwner?UserId={UserID}&Token={APIKey}&api=true";
                    var res = await httpClient.GetAsync(uriWithParms).ConfigureAwait(false);
                    res.EnsureSuccessStatusCode();
                    var responseBody = await res.Content.ReadAsStringAsync();
                    var strippedRes = Regex.Replace(responseBody, @".*\[", "[").TrimEnd('}');
                    ids = JsonSerializer.Deserialize<int[]>(strippedRes)!;
                }
                
                { // Parse the shared shockers.
                    uriWithParms = $"https://ps.pishock.com/PiShock/GetShockersByShareIds?UserId={UserID}&Token={APIKey}&api=true&shareIds=";
                    uriWithParms += string.Join("&shareIds=", ids.Select(x => x.ToString()).ToArray());
                    var res = await httpClient.GetAsync(uriWithParms).ConfigureAwait(false);
                    res.EnsureSuccessStatusCode();
                    var content = await res.Content.ReadAsStringAsync();

                    var sharedShockers = JsonSerializer.Deserialize<Dictionary<String, SharedShocker[]>>(content)!;
                    foreach (var user in sharedShockers.Keys)
                    {
                        foreach (var dev in sharedShockers[user]) {
                            var shocker = new Shocker();
                            shocker.Name = dev.shockerName;
                            shocker.MaxIntensity = dev.maxIntensity;
                            shocker.OwnShocker = false;
                            shocker.CanBeep = dev.canBeep;
                            shocker.CanShock = dev.canShock;
                            shocker.CanViberate = dev.canVibrate;
                            shocker.ShareCode = dev.shareCode;
                            shocker.ShockerId = dev.shockerId;
                            shocker.Owner = user;
                            shockers.Add($"{user} - {dev.shockerName} - {dev.shareCode}", shocker);
                        }
                    }
                }
            }
            catch (HttpRequestException ex) {
                Console.WriteLine($"Exception: {ex}");
            }
        }
    }

    public Dictionary<String, Shocker> GetShockers() {
        return shockers;
    }
}