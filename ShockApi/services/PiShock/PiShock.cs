using System.Text.Json;
using System.Text.RegularExpressions;
using ShockApi.Services.PiShock.API;
using System.Net.WebSockets;
using System.Text;

namespace ShockApi.Services.PiShock;

public class Core : Interfaces.Services
{
    private int? userID;
    private readonly String apikey;
    private readonly String username;
    private readonly String origin;
    public Dictionary<String, Shocker> shockers;

    private ClientWebSocket? wsClient;

    public Core(String userName, String APIKey, String origin ) {
        this.apikey = APIKey;
        this.username = userName;
        this.origin = origin;
        shockers = [];
    }

    public async Task PopulateNeeded() {
        String uriWithParms =  $"https://auth.pishock.com/Auth/GetUserIfAPIKeyValid?apikey={apikey}&username={username}";
       
        using (HttpClient httpClient = new HttpClient())
        {
            try {
                var res = await httpClient.GetAsync(uriWithParms).ConfigureAwait(false);
                res.EnsureSuccessStatusCode();

                var responseBody = await res.Content.ReadAsStringAsync();
                
                userID = JsonSerializer.Deserialize<API.UserInfo>(responseBody)!.UserId;
            }
            catch (HttpRequestException ex) {
                Console.WriteLine($"Exception: {ex}");
            }
        }

        var pingMessage = "{ \"Operation\":\"PING\" }";

        for (int i = 0; i < 10; i++)
        {
            wsClient = new();
            await wsClient.ConnectAsync(new Uri($"wss://broker.pishock.com/v2?Username={username}&ApiKey={apikey}"), default);
            await wsClient.SendAsync(Encoding.UTF8.GetBytes(pingMessage), WebSocketMessageType.Text, false, CancellationToken.None);
            var bytes = new byte[1024];
            var result = await wsClient.ReceiveAsync(bytes, default);
            string res = Encoding.UTF8.GetString(bytes, 0, result.Count);

            var parsedRes = JsonSerializer.Deserialize<API.WebSocketResponse>(res)!;
            if(!parsedRes.IsError) break;
            await wsClient.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", default);
            await Task.Delay(2000);
        }
    }

    public async Task PopulateShockers() {
        if (userID == 0 || userID == null) {
            await PopulateNeeded();
        }

        using (HttpClient httpClient = new HttpClient())
        {
            String uriWithParms = $"https://ps.pishock.com/PiShock/GetUserDevices?UserId={userID}&Token={apikey}&api=true";
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
                        shocker.ClientId = usr.clientId;
                        shocker.Owner = username;
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
                    uriWithParms = $"https://ps.pishock.com/PiShock/GetShareCodesByOwner?UserId={userID}&Token={apikey}&api=true";
                    var res = await httpClient.GetAsync(uriWithParms).ConfigureAwait(false);
                    res.EnsureSuccessStatusCode();
                    var responseBody = await res.Content.ReadAsStringAsync();
                    var strippedRes = Regex.Replace(responseBody, @".*\[", "[").TrimEnd('}');
                    ids = JsonSerializer.Deserialize<int[]>(strippedRes)!;
                }
                
                { // Parse the shared shockers.
                    uriWithParms = $"https://ps.pishock.com/PiShock/GetShockersByShareIds?UserId={userID}&Token={apikey}&api=true&shareIds=";
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
                            shocker.ClientId = dev.clientId;
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

    // TODO: Make a "CommandOptions"
    public async Task<(bool, string)> SendCommandToShocker(Shocker shocker, Mode mode, int intensity, int duration) {
        if (userID == null) {
            return (true, "Did not call Populate");
        }
        if (wsClient == null) {
            return (true, "WS Client is null, call Populate()");
        }

        var req = new API.WebSocketRequest();
        req.Operation = "PUBLISH";

        var command = new API.WebSocketCommand();
        command.Body = new WebSocketBody();
        command.Body.Log = new WebSocketLog();

        command.Target = $"c{shocker.ClientId}-";
        command.Target += shocker.OwnShocker ? "ops" : $"sops-{shocker.ShareCode}";

        command.Body.ShockerId = shocker.ShockerId;
        command.Body.Mode = mode switch
        {
            Mode.SHOCK => "s",
            Mode.VIBERATE => "v",
            Mode.BEEP => "b",
            _ => ""
        };
        if (command.Body.Mode == "") {
            return (true, "Invalid Mode");
        }
        command.Body.Intensity = intensity;
        command.Body.Duration = duration < 100 ? duration * 1000 : duration;
        command.Body.Repeating = true;

        command.Body.Log.UserId = userID;
        command.Body.Log.Held = false;
        command.Body.Log.SendWarning = false; // TODO: add as option
        command.Body.Log.Origin = origin;
        command.Body.Log.Type = shocker.OwnShocker ? "api" : "sc";

        req.PublishCommands = [command];

        var jsonReq = JsonSerializer.Serialize(req);
        await wsClient.SendAsync(Encoding.UTF8.GetBytes(jsonReq), WebSocketMessageType.Text, false, CancellationToken.None);

        var bytes = new byte[1024];
        var result = await wsClient.ReceiveAsync(bytes, default);
        string res = Encoding.UTF8.GetString(bytes, 0, result.Count);

        var parsedRes = JsonSerializer.Deserialize<API.WebSocketResponse>(res)!;

        return (parsedRes.IsError, parsedRes.Message == null ? "" : parsedRes.Message);
    }

    public Dictionary<String, Shocker> GetShockers() {
        return shockers;
    }
}