using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace LuckyDefusePlugin;

public class WireMenu(BasePlugin plugin, string title, string[] options)
{
    public delegate void OptionConfirmedEventHandler(int option);
    public event OptionConfirmedEventHandler OnOptionConfirmed = delegate {};
    
    private CCSPlayerController? _player;
    private int _selectedLine;
    private int _delayInput;
    
    public void Load()
    {
        plugin.RegisterListener<Listeners.OnTick>(() =>
        {
            if (_player == null) return;
            
            if ((_player.Buttons & PlayerButtons.Forward) > 0 && _delayInput <= 0)
            {
                _selectedLine -= 1;
                if (_selectedLine < 0) _selectedLine += options.Length;
                _delayInput = 10;
            }
            else if ((_player.Buttons & PlayerButtons.Back) > 0 && _delayInput <= 0)
            {
                _selectedLine = (_selectedLine + 1) % options.Length;
                _delayInput = 10;
            }
            else if ((_player.Buttons & PlayerButtons.Jump) > 0)
            {
                _player = null;
                OnOptionConfirmed.Invoke(_selectedLine);
                return;
            }
            --_delayInput;

            var html = $"<b>{title}</b><br>";
            for (int i = 0; i < options.Length; ++i)
            {
                if (i == _selectedLine) html += "\u25b6 ";
                html += options[i];
                if (i == _selectedLine) html += " \u25c0";
                html += "<br>";
            }
            html += "<br>[W][S] up/down&nbsp;&nbsp;&nbsp;&nbsp;[SPACE] confirm";
            _player.PrintToCenterHtml(html);
        });
    }

    public void Open(CCSPlayerController player)
    {
        _player = player;
    }

    public void Close()
    {
        _player = null;
        _selectedLine = 0;
        _delayInput = 10;
    }
}