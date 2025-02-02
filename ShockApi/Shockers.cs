namespace ShockApi;

public class Shocker
{
    private bool _canBeep;
    private bool _canShock;
    private bool _canViberate;

    public String? Name { get; set; }
    public String? Owner { get; set; }
    public String? ShareCode { get; set; }
    public int ShockerId { get; set; }
    public bool IsPaused { get; set; }
    public bool CanBeep { 
        get => _canBeep || OwnShocker;
        set => _canBeep = value;
    }
    public bool CanShock { 
        get => _canShock || OwnShocker;
        set => _canShock = value;
    }
    public bool CanViberate { 
        get => _canViberate || OwnShocker;
        set => _canViberate = value;
    }
    public int MaxIntensity { get; set; }
    public bool OwnShocker { get; set; }
    public int ClientId { get; set; } // I think is is for PiShock only? not sure
}