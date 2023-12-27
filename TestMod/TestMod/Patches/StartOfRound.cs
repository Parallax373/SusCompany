﻿using HarmonyLib;
using System;
using System.Linq;
using LC_API.GameInterfaceAPI.Features;

namespace TestMod.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    class StartOfRoundPatch
    {
        [HarmonyPatch("OnShipLandedMiscEvents")]
        //[HarmonyPatch("SceneManager_OnLoad")]
        [HarmonyPostfix]
        public static void ImpostorStartGame(ref int ___randomMapSeed,ref int ___currentLevelID)
        {
            if (___currentLevelID != 3)
            {
                TestModBase.mls.LogInfo("Seed is : " + ___randomMapSeed);
                TestModBase.impostorsIDs.Clear();

                Random random = new Random(___randomMapSeed);
                int currentImpostorID;
                int impostorsToSpawn;

                //Customizable sprawn rate
                float impostorSpawnRate = 1f;
                bool isImposterCountRandom = false;

                impostorsToSpawn = (int)(Player.ActiveList.Count * impostorSpawnRate);

                TestModBase.mls.LogInfo("Player.ActiveList is : " + Player.ActiveList);
                TestModBase.mls.LogInfo("Player.ActiveList.Count is : " + Player.ActiveList.Count);
                TestModBase.mls.LogInfo("impostorsToSpawn is : " + impostorsToSpawn);

                if (isImposterCountRandom)
                {
                    impostorsToSpawn = random.Next(0, impostorsToSpawn + 1);
                    TestModBase.mls.LogInfo("Spawn rate is randomized");
                }

                while (TestModBase.impostorsIDs.Count < impostorsToSpawn)
                {
                    currentImpostorID = (int)Player.ActiveList.ElementAt(random.Next(0, Player.ActiveList.Count)).ClientId;

                    if (!TestModBase.impostorsIDs.Contains(currentImpostorID))
                    {
                        TestModBase.impostorsIDs.Add(currentImpostorID);
                       
                        if (Player.LocalPlayer.IsHost)
                        {
                            OtherFunctions.GetImpostorStartingItem(random.Next(1, 6), Player.ActiveList.FirstOrDefault(p => (int)p.ClientId == currentImpostorID));

                        }
                        TestModBase.mls.LogInfo("Client ID " + currentImpostorID + " is impostor");
                    }
                }

                if (TestModBase.impostorsIDs.Contains((int)Player.LocalPlayer.ClientId))
                {
                    HUDManager.Instance.DisplayTip("Alert", "You Are The Impostor!", true, false, "");
                    Player.LocalPlayer.PlayerController.nightVision.intensity = 3000;
                    Player.LocalPlayer.PlayerController.nightVision.range = 5000;
                }
            }
            
        }

        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPrefix]
        static public void ShipHasLeftPatch()
        {
            OtherFunctions.RemoveImposter();
        }

    }

}
