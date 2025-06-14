using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

namespace LuckyDefuse
{
    public partial class LuckyDefuse : BasePlugin
    {
        public override string ModuleName => "Lucky Defuse Plugin";
        public override string ModuleAuthor => "Jon-Mailes Graeffe <mail@jonni.it>";

        private readonly Color[] _colors =
        [
            Color.Red, Color.Green, Color.Blue, Color.Yellow
        ];
        private readonly char[] _chatColors =
        [
            ChatColors.Red, ChatColors.Green, ChatColors.Blue, ChatColors.Yellow
        ];
        private CCSPlayerController? _defuser;
        private CCSPlayerController? _planter;
        private readonly WireMenu _planterMenu;
        private readonly WireMenu _defuserMenu;
        private int _wire;
        private bool _wireChosenManually;
        private bool _roundEnded;

        public LuckyDefuse()
        {
            string[] options = new string[_colors.Length];
            for (int i = 0; i < _colors.Length; ++i)
            {
                options[i] = $"<span color=\"{_colors[i].Name.ToLower(System.Globalization.CultureInfo.CurrentCulture)}\">{i + 1}. {_colors[i].Name}</span>";
            }

            _planterMenu = new(this, "Choose the hot wire:", options);
            _defuserMenu = new(this, "Test your luck and cut a wire:", options);
        }

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventRoundStart>((_, _) =>
            {
                _roundEnded = false;
                return HookResult.Continue;
            });

            RegisterEventHandler<EventRoundEnd>((_, _) =>
            {
                _planter = null;
                _wireChosenManually = false;
                _roundEnded = true;
                _defuserMenu.Close();
                _planterMenu.Close();
                return HookResult.Continue;
            });

            RegisterEventHandler<EventBombPlanted>((@event, _) =>
            {
                if (_roundEnded || @event.Userid == null || !@event.Userid.IsValid)
                {
                    return HookResult.Continue;
                }

                _wire = Random.Shared.Next(_colors.Length - 1);
                _planter = @event.Userid;
                _planterMenu.Open(@event.Userid);
                AddTimer(Config.NotificationDelay, Notify, TimerFlags.STOP_ON_MAPCHANGE);
                AddTimer(Config.PlanterMenuDuration, () =>
                {
                    _planterMenu.Close();
                    if (_wireChosenManually || _planter == null || !_planter.IsValid)
                    {
                        return;
                    }

                    _planter.PrintToChat(Localizer["randomWireChosen"].Value.Replace("{wire}", $"{_chatColors[_wire]}{_colors[_wire].Name.ToLower(System.Globalization.CultureInfo.CurrentCulture)}"));
                }, TimerFlags.STOP_ON_MAPCHANGE);
                return HookResult.Continue;
            });

            RegisterEventHandler<EventBombBegindefuse>((@event, _) =>
            {
                if (@event.Userid == null
                    || !@event.Userid.IsValid)
                {
                    return HookResult.Continue;
                }

                _defuser = @event.Userid;
                _defuserMenu.Open(@event.Userid);
                return HookResult.Continue;
            });

            RegisterEventHandler<EventBombAbortdefuse>((_, _) =>
            {
                _defuserMenu.Close();
                _planterMenu.Close();
                return HookResult.Continue;
            });

            RegisterEventHandler<EventBombExploded>((_, _) =>
            {
                _planter = null;
                _defuserMenu.Close();
                _planterMenu.Close();
                return HookResult.Continue;
            });

            RegisterEventHandler<EventBombDefused>((_, _) =>
            {
                _planter = null;
                _defuserMenu.Close();
                _planterMenu.Close();
                return HookResult.Continue;
            });

            _planterMenu.OnOptionConfirmed += option =>
            {
                _wire = option;
                _wireChosenManually = true;
                _planter!.PrintToChat(Localizer["wireChosen"].Value.Replace("{wire}", $"{_chatColors[option]}{_colors[option].Name.ToLower(System.Globalization.CultureInfo.CurrentCulture)}"));
            };
            _defuserMenu.OnOptionConfirmed += CutWire;

            AddCommand("ld_choose_wire", "choose a wire", (player, info) =>
            {
                if (player == null)
                {
                    Server.PrintToConsole("consoleNotAllowed");
                    return;
                }
                if (info.ArgCount < 2)
                {
                    info.ReplyToCommand(Localizer["missingArgument"]);
                    return;
                }

                if (!int.TryParse(info.GetArg(1), out int option) || option <= 0 || option > _colors.Length)
                {
                    info.ReplyToCommand(Localizer["malformedArgument"]);
                    return;
                }

                --option;

                if (_defuser != null
                    && _defuser.IsValid
                    && player.AuthorizedSteamID == _defuser.AuthorizedSteamID)
                {
                    CutWire(option);
                }
                else if (_planter != null && player.AuthorizedSteamID == _planter.AuthorizedSteamID)
                {
                    _wire = option;
                    _wireChosenManually = true;
                    info.ReplyToCommand(Localizer["wireChosen"].Value.Replace("{wire}", $"{_chatColors[option]}{_colors[option].Name.ToLower(System.Globalization.CultureInfo.CurrentCulture)}"));
                    _planterMenu.Close();
                }
                else
                {
                    info.ReplyToCommand(Localizer["noBomb"]);
                }
            });

            _planterMenu.Load();
            _defuserMenu.Load();
        }

        private void Notify()
        {
            if (_planter == null || !_planter.IsValid)
            {
                return;
            }

            Server.PrintToChatAll(Localizer["notification"]);
        }

        private void CutWire(int wire)
        {
            IEnumerable<CPlantedC4> bombs = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4");
            CPlantedC4? bomb = bombs.FirstOrDefault();
            if (bomb == null || !bomb.IsValid || _defuser == null || !_defuser.IsValid)
            {
                Server.PrintToChatAll("Huh?");
                return;
            }

            if (_wire != wire)
            {
                bomb.C4Blow = 1f;
                Server.PrintToChatAll(Localizer["cutWrongWire"].Value.Replace("{player}", _defuser!.PlayerName).Replace("{wire}", $"{_chatColors[wire]}{_colors[wire].Name.ToLower(System.Globalization.CultureInfo.CurrentCulture)}"));
                return;
            }

            bomb.DefuseCountDown = 0f;
            Server.PrintToChatAll(Localizer["cutCorrectWire"].Value.Replace("{player}", _defuser!.PlayerName).Replace("{wire}", $"{_chatColors[wire]}{_colors[wire].Name.ToLower(System.Globalization.CultureInfo.CurrentCulture)}"));
        }
    }
}