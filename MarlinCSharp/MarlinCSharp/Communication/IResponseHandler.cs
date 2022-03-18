using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharp.Communication
{
    public interface IResponseHandler
    {
        void Handle(string handle);
    }
}
