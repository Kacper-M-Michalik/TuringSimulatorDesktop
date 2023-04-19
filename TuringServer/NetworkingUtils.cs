using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;

namespace TuringServer.ServerSide
{
    public static class NetworkingUtils
    {
        public static bool PortInUse(int Port)
        {
            //Get all active TCP connections
            IPGlobalProperties IpProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] IpEndPoints = IpProperties.GetActiveTcpListeners();

            //Check if any of them match our port ID
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
