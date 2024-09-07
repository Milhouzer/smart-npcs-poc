using System.Collections;
using System.Collections.Generic;
using Unity.NetCode;
using UnityEngine;

namespace Milhouzer.Netcode
{
    public class AutoConnectBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            return false;
        }
    }
}