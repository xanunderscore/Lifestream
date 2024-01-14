﻿using Lifestream.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonRelicNoteBook;

namespace Lifestream.Tasks
{
    internal static class TaskTryTpToAethernetDestination
    {
        public static void Enqueue(string targetName)
        {
            if (P.ActiveAetheryte != null)
            {
                P.TaskManager.Enqueue(Process);
            }
            else
            {
                P.TaskManager.Enqueue(() =>
                {
                    if (P.ActiveAetheryte == null && Util.GetReachableWorldChangeAetheryte() != null)
                    {
                        P.TaskManager.DelayNextImmediate(10, true);
                        P.TaskManager.EnqueueImmediate(WorldChange.TargetReachableAetheryte);
                        P.TaskManager.EnqueueImmediate(WorldChange.LockOn);
                        P.TaskManager.EnqueueImmediate(WorldChange.EnableAutomove);
                        P.TaskManager.EnqueueImmediate(WorldChange.WaitUntilWorldChangeAetheryteExists);
                        P.TaskManager.EnqueueImmediate(WorldChange.DisableAutomove);
                    }
                }, "ConditionalLockonTask");
                P.TaskManager.Enqueue(WorldChange.WaitUntilWorldChangeAetheryteExists);
                P.TaskManager.DelayNext(10, true);
                P.TaskManager.Enqueue(Process);
            }

            void Process()
            {
                var master = Util.GetMaster();
                {
                    if (P.ActiveAetheryte != master)
                    {
                        var name = (master.Name);
                        if (name.ContainsAny(StringComparison.OrdinalIgnoreCase, targetName) || (P.Config.Renames.TryGetValue(master.ID, out var value) && value.ContainsAny(StringComparison.OrdinalIgnoreCase, targetName)))
                        {
                            TaskRemoveAfkStatus.Enqueue();
                            TaskAethernetTeleport.Enqueue(master);
                            return;
                        }
                    }
                }

                foreach (var x in P.DataStore.AetheryteGroups[master.Group])
                {
                    if (P.ActiveAetheryte != x)
                    {
                        var name = x.Name;
                        if (name.ContainsAny(StringComparison.OrdinalIgnoreCase, targetName) || (P.Config.Renames.TryGetValue(x.ID, out var value) && value.ContainsAny(StringComparison.OrdinalIgnoreCase, targetName)))
                        {
                            TaskRemoveAfkStatus.Enqueue();
                            TaskAethernetTeleport.Enqueue(x);
                            return;
                        }
                    }
                }

                if (P.ActiveAetheryte.Value.ID == 70 && P.Config.Firmament)
                {
                    var name = "Firmament";
                    if (name.ContainsAny(StringComparison.OrdinalIgnoreCase, targetName))
                    {
                        TaskRemoveAfkStatus.Enqueue();
                        TaskFirmanentTeleport.Enqueue();
                        return;
                    }
                }
                Notify.Error($"No destination {targetName} found");
                return;
            }
        }
    }
}
