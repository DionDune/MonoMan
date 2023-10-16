using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    internal class Tile
    {
        public int x, y;

        public bool IsWall { get; set; }
        public string TextureTag { get; set; }

        public Tile()
        {
            x = 0;
            y = 0;
            IsWall = false;
            TextureTag = "Default";
        }
    }
}
