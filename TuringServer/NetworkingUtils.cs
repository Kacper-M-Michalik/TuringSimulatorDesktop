﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace TuringServer
{
    public static class NetworkingUtils
    {
        public static bool PortInUse(int Port)
        {
            IPGlobalProperties IpProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] IpEndPoints = IpProperties.GetActiveTcpListeners();

            foreach (IPEndPoint EndPoint in IpEndPoints)
            {
                if (EndPoint.Port == Port)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
