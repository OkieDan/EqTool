﻿using EQToolApis.DB;

namespace EQToolApis.Models
{
    public class Player
    {
        public Servers Server { get; set; }
        public string PlayerName { get; set; }
        public string ZoneName { get; set; }
    }

    public class PlayerLocation
    {
        public Servers Server { get; set; }
        public string PlayerName { get; set; }
        public string ZoneName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}