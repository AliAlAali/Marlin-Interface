using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharpNet.Communication
{
    public interface IResponseHandler
    {
        void Handle(string response);
    }
}
