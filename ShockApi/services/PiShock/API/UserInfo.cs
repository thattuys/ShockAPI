namespace ShockApi.Services.PiShock.API;

public class Permissions {
    public int UserPermissionsId { get; set; }
    public string? User { get; set; } // ???
    public bool DenyAPI { get; set; }
    public bool admin { get; set; }
    public bool QA { get; set; }
    public int UserId { get; set; }
}

public class UserInfo {
    public int UserId { get; set; }
/* -- We REALLY don't need all this so do not parse it;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? LastLogin { get; set; }
    public string? Password { get; set; } // REALLY? COME ON GUYS
    public string? ConfirmationCode { get; set; } // ??
    public string? ApiKey { get; set; } // AGAIN? JUST STRIP THIS SHIT
    public string? IPAddress { get; set; } // why are you like this

    public Permissions? permissions { get; set; }

    public int AccountId { get; set; }
    public string? Accounts { get; set; } // ???
    public int UserPermissionsId { get; set; }
    public string? Sessions { get; set; }
*/
}