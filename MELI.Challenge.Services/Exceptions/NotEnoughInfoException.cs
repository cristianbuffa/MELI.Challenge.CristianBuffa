using System;
using System.Collections.Generic;
using System.Text;

namespace MELI.Challenge.Services
{ 
    public class NotEnoughInfoException : Exception
    { 
        private const string MESSAGE = "The information provided is not enough.";
        public NotEnoughInfoException() : base(MESSAGE)
        {
        }
    }
}
