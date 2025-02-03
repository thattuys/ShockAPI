using System.Text.Json.Serialization;

namespace ShockApi.Services.PiShock.API;

/*
{
	"Operation": "PUBLISH",
	"PublishCommands":
	[
		{
			"Target": "channelhere", //for example c{clientId}-ops or c{clientId}-sops-{sharecode}
			"Body":
			{
				"id": "<shocker.id>",
				"m": "<mode>", // 'v', 's', 'b', or 'e'
				"i": "<intensity>", // Could be vibIntensity, shockIntensity or a randomized value
				"d": "<duration>", // Calculated duration in milliseconds
				"r": "<repeating>", // true or false, always set to true.
				"l":
				{
	                "u": "<userID>", // User ID from first step
	                "ty": "<type>", // 'sc' for ShareCode, 'api' for Normal
	                "w": "<warning_flag>", // true or false, if this is a warning vibrate, it affects the logs
	                "h": "<hold>", // true if button is held or continuous is being sent.
	                "o": "<origin>" // send to change the name shown in the logs.
	            }
			}
		}
	]
}
*/

public class WebSocketLog
{
    [JsonPropertyName("u")]
    public int? UserId { get; set; } // userId
    
    [JsonPropertyName("ty")]
    public string? Type { get; set; } // Type 'sc' for ShareCode, 'api' for Normal
    
    [JsonPropertyName("w")]
    public bool SendWarning { get; set; } // true or false, if this is a warning vibrate, it affects the logs
    
    [JsonPropertyName("h")]
    public bool Held { get; set; } // true if button is held or continuous is being sent.
    
    [JsonPropertyName("o")]
    public string? Origin { get; set; } // send to change the name shown in the logs.
}

public class WebSocketBody
{
    [JsonPropertyName("id")]
    public int ShockerId { get; set; } // shocker id
    
    [JsonPropertyName("m")]
    public string? Mode { get; set; } // mode
    
    [JsonPropertyName("i")]
    public int Intensity { get; set; } // intensity

    [JsonPropertyName("d")]
    public int Duration { get; set; } // duration

    [JsonPropertyName("r")]
    public bool Repeating { get; set; } // repeating - set to true

    [JsonPropertyName("l")]
    public WebSocketLog? Log { get; set; } // log block
}

public class WebSocketCommand
{
    public string? Target { get; set; } // for example c{clientId}-ops or c{clientId}-sops-{sharecode}
    public WebSocketBody? Body { get; set; }
}

public class WebSocketRequest
{
    public string? Operation { get; set; } // "PUBLISH"
    public WebSocketCommand[]? PublishCommands { get; set; }
}