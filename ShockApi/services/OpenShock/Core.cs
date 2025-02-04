namespace ShockApi.Services.OpenShock;

public class Core : Interfaces.Services
{
    private int? userID;
    private readonly string apikey;
    private readonly string username;
    private readonly string origin;
    private readonly string serverTLD;
    public Dictionary<string, Shocker> shockers;

    public Core(string userName, string APIKey, string origin, string serverTLD) {
        this.apikey = APIKey;
        this.origin = origin;
        this.username = userName;
        this.serverTLD = serverTLD;
        shockers = [];
    }

    public async Task PopulateNeeded() {
        return;
    }

    public async Task PopulateShockers() {
        return;
    }

    public async Task<(bool, string)> SendCommandToShocker(CommandOptions options) {
        return (true, "Not impl");
    }

    public Dictionary<string, Shocker> GetShockers() {
        return shockers;
    }
}