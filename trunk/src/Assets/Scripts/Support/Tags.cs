using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contains definitions for tags. Makes it more easier to refactor tag names.
/// </summary>
static class Tags {
    public const string Shepherd = "Shepherd";

    public const string Foe = "Foe";

    public const string Enemy = Tags.Foe;

    public const string Sheep = "Sheep";

    public const string World = "World";

    public const string Untagged = "Untagged";

    public const string Trap = "Trap";

    public const string LevelBounds = "LevelBounds";
}