﻿using ClickLib.Clicks;
using ECommons.GameFunctions;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Havok;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lifestream
{
    internal unsafe static class Scheduler
    {        
        internal static bool? TargetValidAetheryte()
        {
            if (IsOccupied()) return false;
            var a = Util.GetValidAetheryte();
            if(a != null && a.Address != Svc.Targets.Target?.Address)
            {
                if (EzThrottler.Throttle("TargetValidAetheryte", 500))
                {
                    Svc.Targets.SetTarget(a);
                    return true;
                }
            }
            return false;
        }

        internal static bool? InteractWithTargetedAetheryte()
        {
            if (IsOccupied()) return false;
            var a = Util.GetValidAetheryte();
            if(a != null && Svc.Targets.Target?.Address == a.Address)
            {
                if(EzThrottler.Throttle("InteractWithTargetedAetheryte", 500))
                {
                    TargetSystem.Instance()->InteractWithObject(a.Struct(), false);
                    return true;
                }
            }
            return false;
        }

        internal static bool? SelectAethernet()
        {
            return Util.TrySelectSpecificEntry("Aethernet.", () => EzThrottler.Throttle("SelectString"));
        }

        internal static bool? SelectVisitAnotherWorld()
        {
            return Util.TrySelectSpecificEntry("Visit Another World Server.", () => EzThrottler.Throttle("SelectString"));
        }

        internal static bool? ConfirmWorldVisit(string s)
        {
            var x = (AddonSelectYesno*)Util.GetSpecificYesno($"Travel to {s}?");
            if (x != null)
            {
                if (x->YesButton->IsEnabled && EzThrottler.Throttle("ConfirmWorldVisit"))
                {
                    ClickSelectYesNo.Using((nint)x).Yes();
                    return true;
                }
            }
            return false;
        }

        internal static bool? SelectWorldToVisit(string world)
        {
            var worlds = Util.GetAvailableWorldDestinations();
            var index = Array.IndexOf(worlds, world);
            if (index != -1)
            {
                if (TryGetAddonByName<AtkUnitBase>("WorldTravelSelect", out var addon) && IsAddonReady(addon))
                {
                    if (EzThrottler.Throttle("SelectWorldToVisit", 5000))
                    {
                        Callback(addon, index + 2);
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool? TeleportToAethernetDestination(TinyAetheryte t)
        {
            if (TryGetAddonByName<AtkUnitBase>("TelepotTown", out var telep) && IsAddonReady(telep))
            {
                if (P.DataStore.CallbackData.Data.TryGetValue(t.ID, out var callback))
                {
                    if (Util.GetAvailableAethernetDestinations().Contains(t.Name))
                    {
                        if (EzThrottler.Throttle("TeleportToAethernetDestination", 2000))
                        {
                            Callback(telep, (int)11, (uint)callback);
                            Callback(telep, (int)11, (uint)callback);
                            return true;
                        }
                    }
                    else
                    {
                        if(EzThrottler.Throttle("TeleportToAethernetDestinationLog", 5000))
                        {
                            PluginLog.Warning($"GetAvailableAethernetDestinations does not contains {t.Name}, contains {Util.GetAvailableAethernetDestinations().Print()}");
                        }
                    }
                }
                else
                {
                    PluginLog.Error($"Callback data absent for {t.Name}");
                    return null;
                }
            }
            return false;
        }
    }
}
