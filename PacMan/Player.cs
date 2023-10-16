using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    internal class Player
    {
        public int x { get; set; }
        public int y { get; set; }

        public string Direction { get; set; }
        public string DirectionAwait { get; set; }

        public int Speed { get; set; }

        public bool IsPoweredUp { get; set; }
        public string PowerUp { get; set; }

        public int TextureTag { get; set; }

        public Player()
        {
            x = 0;
            y = 0;

            Direction = "Still";
            DirectionAwait = "Still";
            Speed = 1;

            IsPoweredUp = false;
            PowerUp = "None";

            TextureTag = 0;
        }
    }
}
