namespace ShockApi.Interfaces;

interface Services {
    public Task PopulateNeeded();
    public Task PopulateShockers();
    public Dictionary<String, Shocker> GetShockers();
}