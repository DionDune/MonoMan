using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    internal class Ghost
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Speed { get; set; }
        public string Direction { get; set; }
        public string State { get; set; }

        public string Name { get; set; }

        public Ghost()
        {
            X = 0;
            Y = 0;

            Speed = 1;
            Direction = "Left";
            State = "Calm";

            Name = "Test";
        }
    }
}
