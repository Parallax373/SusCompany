using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LC_API.GameInterfaceAPI.Features;

namespace TestMod.Patches
{
    class EnemyAiPatch
    {
        //Powoduje brak damagu przez single targetuj�ce moby ale nadal si� path finduj� do impostora :(

        [HarmonyPatch((typeof(EnemyAI)),(nameof(EnemyAI.PlayerIsTargetable)))]
        [HarmonyPostfix]
        static public void MonstersDontTarget(PlayerControllerB playerScript, ref bool __result)
        {
            if (TestModBase.impostorsIDs.Contains((int)playerScript.actualClientId))
            {
                __result = false;
                //output to console
                TestModBase.mls.LogInfo("Player " + playerScript.actualClientId + " is impostor and is not targetable");

            }
        }


        //Z jakiego� powodu nie dzia�a
        [HarmonyPatch((typeof(Turret)),(nameof(Turret.CheckForPlayersInLineOfSight)))]
        [HarmonyPostfix]
        static public void TurretsDontTarget(ref PlayerControllerB __result)
        {
            if (TestModBase.impostorsIDs.Contains((int)__result.actualClientId))
            {
                __result = null;
                TestModBase.mls.LogInfo("Player " + __result.actualClientId + " is impostor and is not targetable by turret");
}
        }
    }
}