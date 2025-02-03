namespace ShockApi.Interfaces;

interface Services {
    public Task PopulateNeeded();
    public Task PopulateShockers();
    public Dictionary<string, Shocker> GetShockers();
    public Task<(bool, string)> SendCommandToShocker(CommandOptions options);
}