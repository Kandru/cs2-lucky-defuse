using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
namespace LuckyDefuse
{

    public class PluginConfig : BasePluginConfig
    {
        [JsonPropertyName("planter_menu_duration")]
        public int PlanterMenuDuration { get; set; } = 10;

        [JsonPropertyName("notification_delay")]
        public int NotificationDelay { get; set; } = 30;
    }

    public partial class LuckyDefuse : IPluginConfig<PluginConfig>
    {
        public PluginConfig Config { get; set; } = null!;

        public void OnConfigParsed(PluginConfig? config)
        {
            if (config == null)
            {
                return;
            }

            Config = config;
        }
    }
}