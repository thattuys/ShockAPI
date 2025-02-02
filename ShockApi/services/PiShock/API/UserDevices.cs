namespace ShockApi.Services.PiShock.API;

class Shocker
{
    public String? name { get; set; }
    public int shockerId { get; set; }
    public bool isPaused { get; set; }
    public int shockerType { get; set; }
}

class UserDevices
{
    public int clientId { get; set; }
    public String? name { get; set; }
    public int userId { get; set; }
    public String? userName { get; set; }
    public Shocker[]? shockers { get; set; }
}