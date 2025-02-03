namespace ShockApi.Interfaces;

interface Services {
    public Task PopulateNeeded();
    public Task PopulateShockers();
    public Dictionary<String, Shocker> GetShockers();
    public Task<(bool, string)> SendCommandToShocker(Shocker shocker, Mode mode, int intensity, int duration);
}