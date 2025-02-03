﻿using System.Threading.Tasks;

namespace ShockApi;

public enum Provider
{
    PISHOCK,
    OPENSHOCK
}

public enum Mode
{
    BEEP,
    VIBERATE,
    SHOCK
}

public class ShockApi
{
    private Interfaces.Services _service;
    
    /// <summary>
    /// Create a new instance of ShockAPI and setup a connection to the provider's api.
    /// </summary>
    /// <param name="provider">Enum selecting which API you want to use on the back end</param>
    /// <param name="username">Username for connecting to the providers API</param>
    /// <param name="apiKey">API key for the provider</param>
    /// <param name="origin">The name of the application you want to show up in logs</param>
    public ShockApi(Provider provider, String username, String apiKey, String origin) {
        switch (provider)
        {
            case Provider.PISHOCK:
                _service = new Services.PiShock.Core(username, apiKey, origin);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Populates the back end services with things they need from the API and
    /// what shockers they have access to. Must call this
    /// </summary>
    /// <returns>null</returns>
    public async Task Populate() {
        await _service.PopulateNeeded();
        await _service.PopulateShockers();
    }

    /// <summary>
    /// Get's a list of the users owned shockers
    /// </summary>
    /// <returns>IEnumerable of a KVP where the key is the shocker name, and the value is an instance of Shocker</returns>
    public IEnumerable<KeyValuePair<string, Shocker>> GetOwnShockers() {
        return _service.GetShockers().Where( pair => pair.Value.OwnShocker == true);
    }

    /// <summary>
    /// Gets a shocker by share code
    /// </summary>
    /// <param name="ShareCode">String containing the share code you want to get</param>
    /// <returns>Null or a single instance of Shocker matching the ShareCode</returns>
    public Shocker GetShockerByShareCode(string ShareCode) {
        return _service.GetShockers().Where(
            pair => pair.Value.ShareCode == ShareCode
        ).First().Value;
    }

    /// <summary>
    /// Gets a shocker by the shocker's name
    /// </summary>
    /// <param name="Name">Name of the shocker to return</param>
    /// <returns>Null or a single instance of a Shocker matching the name</returns>
    public Shocker GetShockerByName(string Name) {
        return _service.GetShockers().Where(
            pair => pair.Value.Name == Name
        ).First().Value;
    }

    /// <summary>
    /// Sends a command to a shocker
    /// </summary>
    /// <param name="shocker">An instance of Shocker to send the command to</param>
    /// <param name="mode">The command you want to send, Mode.BEEP, Mode.SHOCK, Mode.VIBERATE</param>
    /// <param name="intensity">The intensity to send the command at</param>
    /// <param name="duration">How long the command should fire</param>
    /// <returns>A Boolean if the shocker _ALLOWS_ the requested command, Not actually if the shocker fired the command</returns>
    public async Task<(bool, string)> SendCommandToShocker(Shocker shocker, Mode mode, int intensity, int duration) {
        switch (mode) {
            case Mode.BEEP:
                if (!shocker.CanBeep) return (false, "Beep not supported");
                break;
            case Mode.SHOCK:
                if (!shocker.CanShock) return (false, "Shock not supported");
                break;
            case Mode.VIBERATE:
                if (!shocker.CanViberate) return (false, "Viberate not supported");
                break;
            default:
                return (false, "Unsupported mode");
        }
        if (intensity > shocker.MaxIntensity)
            intensity = shocker.MaxIntensity;

        (var err, var message) = await _service.SendCommandToShocker(shocker, mode, intensity, duration);

        return (err, message);
    }
}