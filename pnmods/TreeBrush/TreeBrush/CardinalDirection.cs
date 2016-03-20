using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scenery_Picker.TreeBrush
{
    public class CardinalDirection
    {
        public enum Direction
        {
            unknown = 0, north, northeast, east, southeast, south, southwest, west, northwest
        }

        public static Direction[] getPermutation()
        {
            Direction[] res = new Direction[8];
            var rand = new System.Random();
            Array directions = Enum.GetValues(typeof(Direction));
            int remain = 8;
            foreach (Direction dir in directions)
            {
                if (dir == Direction.unknown) continue;
                int counter = -1, pos = -1, roll = rand.Next(remain);
                do {
                    ++pos;
                    if (res[pos] == Direction.unknown)
                        ++counter;
                } while (counter < roll);
                res[pos] = dir;
                --remain;
            }
            return res;
        }

        public static Vector2 getDirectionVector(Direction dir)
        {
            switch (dir)
            {
                case Direction.north:
                    return new Vector2(1, 0);
                case Direction.northeast:
                    return new Vector2(1, 1);
                case Direction.east:
                    return new Vector2(0, 1);
                case Direction.southeast:
                    return new Vector2(-1, 1);
                case Direction.south:
                    return new Vector2(-1, 0);
                case Direction.southwest:
                    return new Vector2(-1, -1);
                case Direction.west:
                    return new Vector2(0, -1);
                case Direction.northwest:
                    return new Vector2(1, -1);
                default:
                    return new Vector2(0, 0);
            }
        }
    }
}
