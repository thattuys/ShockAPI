using System.Reflection.Metadata.Ecma335;

namespace ShockApi;

public class CommandOptions
{
    public Shocker? shocker { get; set; }
    public Mode? mode { get; set; }
    public int intensity { get; set; }
    private int _duration;
    public int duration {
        get => _duration;
        set {
            if (value < 100) {
                _duration = value * 1000;
            } else {
                _duration = value;
            }
        }
    }
    public bool sendWarning { get; set; }
}