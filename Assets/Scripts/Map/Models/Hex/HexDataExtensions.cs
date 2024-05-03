using System;

namespace Map.Models.Hex
{
    public static class HexDataExtensions
    {
        public static int DistanceTo(this IHexData self, IHexData other)
        {
            return Math.Abs(self.X - other.X) + Math.Abs(self.Y - other.Y);
        }
    }
}