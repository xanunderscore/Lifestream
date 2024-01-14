﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lifestream.Schedulers;

namespace Lifestream.Tasks
{
    internal static class TaskAethernetTeleport
    {
        internal static void Enqueue(TinyAetheryte a)
        {
            P.TaskManager.Enqueue(WorldChange.TargetValidAetheryte);
            P.TaskManager.Enqueue(WorldChange.InteractWithTargetedAetheryte);
            if(P.ActiveAetheryte.Value.IsAetheryte) P.TaskManager.Enqueue(WorldChange.SelectAethernet);
            P.TaskManager.Enqueue(() => WorldChange.TeleportToAethernetDestination(a), nameof(WorldChange.TeleportToAethernetDestination));
        }
    }
}
