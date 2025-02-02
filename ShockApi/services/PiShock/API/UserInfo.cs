namespace ShockApi.Services.PiShock.API;

public class Permissions {
    public int UserPermissionsId { get; set; }
    public String? User { get; set; } // ???
    public bool DenyAPI { get; set; }
    public bool admin { get; set; }
    public bool QA { get; set; }
    public int UserId { get; set; }
}

public class UserInfo {
    public int UserId { get; set; }
/* -- We REALLY don't need all this so do not parse it;
    public String? Username { get; set; }
    public String? Email { get; set; }
    public String? LastLogin { get; set; }
    public String? Password { get; set; } // REALLY? COME ON GUYS
    public String? ConfirmationCode { get; set; } // ??
    public String? ApiKey { get; set; } // AGAIN? JUST STRIP THIS SHIT
    public String? IPAddress { get; set; } // why are you like this

    public Permissions? permissions { get; set; }

    public int AccountId { get; set; }
    public String? Accounts { get; set; } // ???
    public int UserPermissionsId { get; set; }
    public String? Sessions { get; set; }
*/
}