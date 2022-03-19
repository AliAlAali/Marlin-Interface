using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharpNet.Communication.Exception
{
    public class ConnectionException : CommunicationException
    {

        public ConnectionException(string v) : base(v)
        {
        }
    }
}
