namespace ShockApi.Services.PiShock.API;

class Shocker
{
    public string? name { get; set; }
    public int shockerId { get; set; }
    public bool isPaused { get; set; }
    public int shockerType { get; set; }
}

class UserDevices
{
    public int clientId { get; set; }
    public string? name { get; set; }
    public int userId { get; set; }
    public string? userName { get; set; }
    public Shocker[]? shockers { get; set; }
}