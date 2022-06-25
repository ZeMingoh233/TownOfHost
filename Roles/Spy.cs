using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using UnityEngine;

namespace TownOfHost
{
    public class Spy
    {
        #region static
        public static Spy Instance;
        public static readonly CustomRoles ThisRole = CustomRoles.Spy;
        static readonly int optionId = 21000;
        #region CustomOptions
        private static CustomOption opt_SpyCanVent;
        private static CustomOption opt_ImpostorCanKillSpy;
        private static CustomOption opt_SheriffCanKillSpy;
        #endregion
        #region OptionGetter
        public static bool IsRoleEnabled => ThisRole.IsEnable();
        public static float RoleChance => ThisRole.GetChance();
        public static int RoleCount => ThisRole.GetCount();
        public static bool SpyCanVent => opt_SpyCanVent.GetBool();
        public static bool ImpostorCanKillSpy => opt_ImpostorCanKillSpy.GetBool();
        public static bool SheriffCanKillSpy => opt_SheriffCanKillSpy.GetBool();
        #endregion
        public static void SetupCustomOption()
        {
            //21000~21001
            Options.SetupSingleRoleOptions(optionId, ThisRole, 1);
            CustomOption parent = Options.CustomRoleSpawnChances[ThisRole];
            int i = 10; //以降、21010から1ずつ上がっていく。
            opt_SpyCanVent = CustomOption.Create(optionId + i++, Color.white, "SpyCanVent", true, parent);
            opt_ImpostorCanKillSpy = CustomOption.Create(optionId + i++, Color.white, "ImpostorCanKillSpy", true, parent);
            opt_SheriffCanKillSpy = CustomOption.Create(optionId + i++, Color.white, "SheriffCanKillSpy", false, parent);
        }
        #region 役職割り当て完了後の視点ごとの役職表
        /*
        |  PC  | Imp1 | Imp2 | Spy  |
        | Imp1 | Imp  | Imp  | Sci  |
        | Imp2 |  GA  | Imp  | Sci  |
        | Spy  | Crew | Crew | Cr/E |
        GA = 守護天使, Sci = 科学者, Cr/E = クルーまたはエンジニア
        */
        #endregion
        public static void AssignRoleForRandomPlayer(ref List<PlayerControl> AllPlayers, CustomRpcSender sender)
        {
            System.Random rand = new();
            if (IsRoleEnabled)
            {
                for (int i = 0; i < RoleCount; i++)
                {
                    if (AllPlayers.Count <= 0) break;
                    var target = AllPlayers[rand.Next(0, AllPlayers.Count)];
                    AllPlayers.Remove(target);
                    Spy.Assign(target);
                    //Desync開始 そういえば最近ホストもDesyncするようになったんだってね。
                    if (target.PlayerId != 0)
                    {
                        int clientId = target.GetClientId();
                        //target視点: target = クルー or エンジニア [TODO]
                        sender.RpcSetRole(target, RoleTypes.Engineer, clientId);
                    }
                    else
                    {
                        //ホストは代わりに自視点エンジニア, 他視点インポスターにする
                        target.SetRole(RoleTypes.Engineer); //ホスト視点用
                        sender.RpcSetRole(target, RoleTypes.Impostor);
                    }
                    target.Data.IsDead = true;
                }
            }
        }
        #endregion

        public PlayerControl player;
        private Spy(PlayerControl player)
        {
            this.player = player;
        }
        public static Spy Assign(PlayerControl player)
        {
            var role = new Spy(player);
            Main.AllPlayerCustomRoles[player.PlayerId] = Spy.ThisRole;
            return role;
        }
        public void Init()
        {
        }
        public void FixedUpdate()
        {

        }
        public void OnStartMeeting()
        {

        }
    }
}