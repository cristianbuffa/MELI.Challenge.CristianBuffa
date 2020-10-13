using System;
using System.Collections.Generic;

namespace MELI.Challenge.API.Model
{   
    public class SatelliteGettingData
    {
        public string name { get; set; }
        public float distance { get; set; }
        public string[] message { get; set; }
    }
}
