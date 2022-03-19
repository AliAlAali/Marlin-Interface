using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharpNet.Communication.Exception
{
    public class CommunicationException : System.Exception
    {
        public CommunicationException(string v) : base(v)
        {
        }
    }
}
