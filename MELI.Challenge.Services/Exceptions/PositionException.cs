using System;
using System.Collections.Generic;
using System.Text;

namespace MELI.Challenge.Services
{ 
    public class PositionException:Exception
    { 
        private const string MESSAGE = "Coordinates Could not be determined.";
        public PositionException() : base(MESSAGE)
        {
        }
    }
}
