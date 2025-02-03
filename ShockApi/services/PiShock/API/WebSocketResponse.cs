namespace ShockApi.Services.PiShock.API;

public class WebSocketResponse
{
    public string? ErrorCode { get; set; }
    public bool IsError { get; set; }
    public string? Message { get; set; }
    public string? OriginalCommand { get; set; }
}