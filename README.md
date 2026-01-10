# Tanuki.Atlyss
Tanuki.Atlyss is a plugin framework for .NET based on BepInEx for the game Atlyss.<br>
It offers simple plugin localization, command support, patches, and more.
## Features
### Commands
Commands for each plugin are configured in JSON files.<br>
> `BepInEx/config/{Plugin}/default.commands.json`

Each command can have multiple names (aliases), the first of which will be the main name of the command.<br>
All names must be in lowercase. If a command name is already in use, it will be automatically removed.<br>
The active command file is automatically cleared and supplemented with commands that exist but have not yet been added.<br>
Commands support arguments that are passed to them as an array of strings.<br>
> Example<br>
> `/example 123 "argument with spaces" 456 'argument with other quotes'`<br>
> The command will receive the following array:
> ```c
> ["123", "argument with spaces", "456", "argument with other quotes"]
> ```
> If there are no arguments, an empty array will be passed.

Basic bootstrap commands:
- `/help [plugin names separated by spaces]` - Display a list of plugin commands.
- `/reload [plugin names separated by spaces]` - Reload the specified plugins.
### Translation
Plugin translation using json documents, support for different languages.<br>
> `BepInEx/config/{Plugin}/default.translations.json` - Default translation file.<br>
> `BepInEx/config/{Plugin}/russian.translations.json` - Russian translation file.

Commands are translated in a similar way. You can add or remove command aliases by editing the `Names` list of commands.<br>
> `BepInEx/config/{Plugin}/default.commands.json` - Default command file.<br>
> `BepInEx/config/{Plugin}/russian.commands.json` - Russian command file.

To change the language of all plugins, you need to replace it in the bootstrap configuration.<br>
> `BepInEx/config/9c00d52e-10b8-413f-9ee4-bfde81762442.cfg`

The specified language will be used in all plugins where it is available, otherwise the first one encountered will be used.<br>
You should use the full names of languages and only lowercase letters. For example: `russian`, `english`, `spanish`.
### Other
Patches, accessors, and extensions are located in [Tanuki.Atlyss.Game](../../tree/main/Tanuki.Atlyss.Game) project.<br>
Examples of using this framework can be found in the [Tanuki.ExamplePlugin](../../tree/main/Tanuki.Atlyss.ExamplePlugin) project.
## Getting Started
### Thunderstore
This framework on [Thunderstore](https://thunderstore.io/c/atlyss/p/Tanuki/Tanuki_Atlyss/).
### Manual installation
1. Install [BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html). It's recommended to use the [preconfigured package](https://thunderstore.io/c/atlyss/p/BepInEx/BepInExPack/).
2. Install the [Tanuki.Atlyss](../../releases) files.
## Anything else?
[Contacts](https://tanu.su/)