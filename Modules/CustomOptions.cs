using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using System;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnhollowerBaseLib;
using TownOfHost;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;
using Il2CppSystem;
using System.Threading;
using System.Threading.Tasks;

namespace TownOfHost
{
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    public static class GameSettingMenuPatch
    {
        public static void Prefix(GameSettingMenu __instance)
        {
            // Unlocks map/impostor amount changing in online (for testing on your custom servers)
            // オンラインモードで部屋を立て直さなくてもマップを変更できるように変更
            __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    [HarmonyPriority(Priority.First)]
    public static class GameOptionsMenuPatch
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            foreach (var ob in __instance.Children)
            {
                if (ob.Title == StringNames.GameShortTasks ||
                ob.Title == StringNames.GameLongTasks ||
                ob.Title == StringNames.GameCommonTasks)
                {
                    ob.Cast<NumberOption>().ValidRange = new FloatRange(0, 99);
                }
                if (ob.Title == StringNames.GameKillCooldown)
                {
                    ob.Cast<NumberOption>().ValidRange = new FloatRange(0, 180);
                }
                if(ob.Title == StringNames.GameRecommendedSettings) {
                    ob.enabled = false;
                    ob.gameObject.SetActive(false);
                }
            }
        }
    }
}