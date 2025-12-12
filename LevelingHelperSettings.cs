using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
namespace LevelingHelper
{
    public class LevelingHelperSettings : ISettings
    {
        public LevelingHelperSettings()
        {
            Enable = new ToggleNode(false);
            AudioDelay = new RangeNode<int>(10, 5, 100);
            Debug = new ToggleNode(false);
            ExpPenaltyWarning = new RangeNode<int>(90, 0, 100);
        }

        [Menu("Enable Plugin")]
        public ToggleNode Enable { get; set; }
        [Menu("Audio Delay (seconds)")]
        public RangeNode<int> AudioDelay { get; set; }
        [Menu("Enabled Debugging Text")]
        public ToggleNode Debug { get; set; }
        [Menu("Exp Penalty Warning")]
        public RangeNode<int> ExpPenaltyWarning { get; set; }
   
    }
}
