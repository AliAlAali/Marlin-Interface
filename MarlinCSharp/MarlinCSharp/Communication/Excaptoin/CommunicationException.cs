using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharp.Communication.Excaptoin
{
    public class CommunicationException : Exception
    {
        public CommunicationException(string v) : base(v)
        {
        }
    }
}
