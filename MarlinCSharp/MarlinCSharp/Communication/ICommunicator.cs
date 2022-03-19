﻿using MarlinCSharp.GCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarlinCSharp.Communication
{
    public interface ICommunicator
    {
        /// <summary>
        /// Reset communication
        /// </summary>
        void Reset();

        void Connect();

        void Disconnect();

        void DispatchCommand(GCodeCommand command);

        void SendCommand(GCodeCommand command);

        void SendPrioityCommand(string command);
      
    }
}