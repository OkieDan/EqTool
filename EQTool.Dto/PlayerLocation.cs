using System;

namespace EQTool.Dto
{
    public class PlayerLocation
    {
        public string ServerName { get; set; }
        public string PlayerName { get; set; }
        public string MapName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
