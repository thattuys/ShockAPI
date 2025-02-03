namespace ShockApi.Services.PiShock.API;

class SharedShocker {
    public int shareId { get; set; }
    public int clientId { get; set; }
    public int shockerId { get; set; }
    public string? shockerName { get; set; }
    public bool isPaused { get; set; }
    public int maxIntensity { get; set; }
    public bool canContinuous { get; set; }
    public bool canShock { get; set; }
    public bool canVibrate { get; set; }
    public bool canBeep { get; set; }
    public bool canLog { get; set; }
    public string? shareCode { get; set; }
}