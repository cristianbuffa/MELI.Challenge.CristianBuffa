using System;
using System.Collections.Generic;
using System.Text;

namespace MELI.Challenge.Services
{ 
    public class MessageException:Exception
    { 
        private const string MESSAGE = "The Top Secret Message's is not valid.";
        public MessageException() : base(MESSAGE)
        {
        }
    }
}
