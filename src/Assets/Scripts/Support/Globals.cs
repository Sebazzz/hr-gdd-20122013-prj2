using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Defines global constants
/// </summary>
static class Globals {
    public const string GroupObjectPrefix = "Group: ";

    public const string GroupObjectNameSuffix = " (generated)";

    public const string SheepRootGameObjectName = GroupObjectPrefix + "Sheep" + GroupObjectNameSuffix;

    public const string EnemyRootGameObjectName = GroupObjectPrefix + "Enemy" + GroupObjectNameSuffix;
}