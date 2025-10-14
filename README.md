[![GitHub release](https://flat.badgen.net/github/release/FrikandelbroodjeCommunity/SCP-650-LabAPI/)](https://github.com/FrikandelbroodjeCommunity/SCP-650-LabAPI/releases/latest)
[![LabAPI Version](https://flat.badgen.net/static/LabAPI%20Version/v1.1.3)](https://github.com/northwood-studios/LabAPI)
[![Original](https://flat.badgen.net/static/Original/Draakoor?icon=github)](https://github.com/Draakoor/Scp650Plugin)

# About SCP-650

Plugin for SCP-650 schematic of MER plugin based on Exiled framework of SCP: Secret Laboratory.

Every round SCP-650 will spawn somewhere in Site-02, attempting to jumpscare its personnel.
If SCP-650 sees you, it will start chasing you, constantly sneaking up behind you.

> [!TIP]
> You can prevent SCP-650 from teleporting by looking at them, however this does not work if you are targeted by
> SCP-650.

# Installation

> [!IMPORTANT]
> **Required dependencies:**
> - [FrikanUtils](https://github.com/FrikandelbroodjeCommunity/FrikanUtils/blob/master/FrikanUtils/README.md)
> - [ProjectMER](https://github.com/Michal78900/ProjectMER/releases/latest)

Install the dependencies above, together with
the [latest release](https://github.com/FrikandelbroodjeCommunity/SCP-650-LabAPI/releases/latest) of the SCP-650 plugin
and place them in your LabAPI plugin folder.

The plugin requires SCP-650 to be provided as a schematic, download it from
the [releases page](https://github.com/FrikandelbroodjeCommunity/SCP-650-LabAPI/releases/latest) and place it in the
correct folder. By default, this will be <code>Configs/{port/global}/FrikanUtils/Maps/SCP650.json</code>.

Additionally, you must download the poses, download the `global.yml` file from
the [releases page](https://github.com/FrikandelbroodjeCommunity/SCP-650-LabAPI/releases/latest) and place it in the
correct folder. By default, this will be <code>Configs/{port/global}/FrikanUtils/Poses/global.yml</code>.

# Custom poses

1. Download [SL-CustomObjects](https://github.com/Michal78900/SL-CustomObjects).
2. Import the SCP-650 schematic.
3. Add `PoseRecorder.cs` script to root object: `SCP-650`.
4. Edit property `size` of `Objects` of the script to 13.
5. Create a new pose by changing the `mixamorig:{joint}` objects. *Changing the scale is not supported.*
6. Press play button. Then there will be a log on console tab.
7. Press it to see all of the log contents and copy it.
8. Open `global.yml`. And add the contents below:

```
- pose_name: {pose_name_here}
  transform_per_joint:
{paste_console_content_here}
```

# Menus

The plugins adds a debug menu to the Server Specific Settings, which can be viewed if you have the `frikanutils.debug`
permission.

The menu shows the position of all SCP-650 instances. The update only runs every 10 seconds, so the contents may be
slightly outdated.

# Config

| Config                                       | Default       | Meaning                                                                                                                                                                                                                     |
|----------------------------------------------|---------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `schematic_folder`                           | `Maps`        | Folder used by the FrikanUtils file system to search for the schematic.                                                                                                                                                     |
| `schematic_name`                             | `SCP650.json` | Name of the SCP-650 schematic file.                                                                                                                                                                                         |
| `pose_folder`                                | `Poses`       | Folder used by the FrikanUtils file system to search for the poses file.                                                                                                                                                    |
| `pose_file`                                  | `global.yml`  | Name of the file containing the poses.                                                                                                                                                                                      |           
| `maximum_spawn_number`                       | `1`           | The maximum amount of instances that can be spawned. Each round it will attempt to spawn this many, only not spawning if it failed.                                                                                         |
| `spawnable_zone`                             | ...           | The zones SCP-650 can spawn in.                                                                                                                                                                                             |
| `observe_effectable_factions`                | ...           | The factions that influence SCP-650 when looking at it.                                                                                                                                                                     |
| `observe_effectable_blacklist_roles`         | ...           | Roles that will have no influence when looking at SCP-650, even if they are part of a faction which can.                                                                                                                    |
| `targetable_factions`                        | ...           | The factions that can be targeted (or chased) by SCP-650.                                                                                                                                                                   |
| `target_blacklist_roles`                     | ...           | The roles that can not be targeted by SCP-650, even if they are part of a targetable faction.                                                                                                                               |
| `overlap_target`                             | `false`       | Whether multiple SCP-650 instances can have the same target.                                                                                                                                                                |
| `target_follow_min_time`                     | `50`          | The minimum time in seconds SCP-650 follows its target                                                                                                                                                                      |
| `target_follow_max_time`                     | `120`         | The maximum time in seconds SCP-650 follows its target                                                                                                                                                                      |
| `smooth_change_target`                       | `true`        | If SCP-650 stops following its target because the chase time is up, with this enabled it will still follow its previous target until SCP-650 finds another one. With this disabled SCP-650 will immediately come to a halt. |
| `change_target_to_killer_when_target_killed` | `true`        | If a targetable player kills the current target, the killer becomes the new target of SCP-650.                                                                                                                              |
| `teleport_min_cool_time`                     | `5`           | The minimum amount of time in seconds between teleports.                                                                                                                                                                    |
| `teleport_max_cool_time`                     | `10`          | The maximum amount of time in seconds between teleports. It may exceed this time if SCP-650 is being looked at, or does not have a safe space to teleport to.                                                               |
| `follow_target_to_pocket_dimension`          | `true`        | Whether SCP-650 can chase its target into the pocket dimension.                                                                                                                                                             |
