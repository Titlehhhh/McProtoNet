using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Protocol754.Data.World.Map
{
    public struct MapIcon
    {
        public MapIconType IconType { get; private set; }
        public int X { get; private set; }
        public int Z { get; private set; }
        public int IconRotation { get; private set; }
        public string DisplayName { get; private set; }

        public MapIcon(MapIconType iconType, int x, int z, int iconRotation, string displayName)
        {
            IconType = iconType;
            X = x;
            Z = z;
            IconRotation = iconRotation;
            DisplayName = displayName;
        }
    }
}
