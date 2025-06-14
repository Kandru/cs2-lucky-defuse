# CounterstrikeSharp - LuckyDefuse

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-lucky-defuse?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-lucky-defuse/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-lucky-defuse](https://img.shields.io/github/issues/Kandru/cs2-lucky-defuse)](https://github.com/Kandru/cs2-lucky-defuse/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

This plug-in provides the ability to defuse the bomb by having a 25% chance to cut the correct wire.

## Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-lucky-defuse/releases/).
2. Move the "LuckyDefuse" folder to the `/addons/counterstrikesharp/plugins/` directory.
3. (Re)start the server and wait for it to be completely loaded.
4. Restart the server again because it maybe applied some Gamedata entries for the plug-in to work correctly.

Updating is even easier: simply overwrite all plugin files and they will be reloaded automatically. To automate updates please use our [CS2 Update Manager](https://github.com/Kandru/cs2-update-manager/).


## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/LuckyDefuse/LuckyDefuse.json`.

```json

```

## Compile Yourself

Clone the project:

```bash
git clone https://github.com/Kandru/cs2-lucky-defuse.git
```

Go to the project directory

```bash
  cd cs2-lucky-defuse
```

Install dependencies

```bash
  dotnet restore
```

Build debug files (to use on a development game server)

```bash
  dotnet build
```

Build release files (to use on a production game server)

```bash
  dotnet publish
```

## FAQ

TBD

## License

Released under [GPLv3](/LICENSE) by [@Kandru](https://github.com/Kandru).

## Authors

- [@jmgraeffe](https://www.github.com/jmgraeffe)
- [@derkalle4](https://www.github.com/derkalle4)
