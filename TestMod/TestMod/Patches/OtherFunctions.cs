﻿using GameNetcodeStuff;
using System.Collections.Generic;
using LC_API.GameInterfaceAPI.Features;

namespace TestMod.Patches
{
    class OtherFunctions
    {
        static public void CheckForImpostorVictory()
        {
            TestModBase.mls.LogInfo("Checking for Impostor Victory");
            int aliveCrewMates = 0;
            IEnumerator<Player> activePlayers = Player.ActiveList.GetEnumerator();
            while (activePlayers.MoveNext())
            {
                if (!activePlayers.Current.IsDead && !TestModBase.impostorsIDs.Contains((int)activePlayers.Current.ClientId))
                {
                    aliveCrewMates++;
                }

            }
            TestModBase.mls.LogInfo("aliveCrewMates is : " + aliveCrewMates);
            if (aliveCrewMates == 0)
            {
                TestModBase.mls.LogInfo("Impostors Won");
                StartOfRound.Instance.ShipLeaveAutomatically();
            }
        }

        // Ten system ssie pento, ale nie potrafie tego przepisac w lepszy sposób
        static public void OnDiedCheckForImpostorVictory(LC_API.GameInterfaceAPI.Events.EventArgs.Player.DyingEventArgs dyingEventArgs)
        {
            CheckForImpostorVictory();
        }
        static public void OnLeftCheckForImpostorVictory(LC_API.GameInterfaceAPI.Events.EventArgs.Player.LeftEventArgs leftEventArgs)
        {
            if (leftEventArgs.Player.IsLocalPlayer)
            {
                TestModBase.mls.LogInfo("Local player has left, Clearing Player.Dictionary");
                Player.Dictionary.Clear();
            }
            CheckForImpostorVictory();
        }
        static public void OnJoinedAddOtherClients(LC_API.GameInterfaceAPI.Events.EventArgs.Player.JoinedEventArgs joinedEventArgs)
        {

            if (joinedEventArgs.Player.IsLocalPlayer && !joinedEventArgs.Player.IsHost)
            {
                TestModBase.mls.LogInfo("Local player joined");

                for (int i = 0; i <= StartOfRound.Instance.allPlayerObjects.Length; i++)
                {
                    if (StartOfRound.Instance.allPlayerObjects[i].GetComponent<PlayerControllerB>().isPlayerControlled)
                    {
                        Player.GetOrAdd(StartOfRound.Instance.allPlayerObjects[i].GetComponent<PlayerControllerB>());
                        TestModBase.mls.LogInfo("Adding other player" + StartOfRound.Instance.allPlayerObjects[i].GetComponent<PlayerControllerB>().name +
                            " [" + StartOfRound.Instance.allPlayerObjects[i].GetComponent<PlayerControllerB>().actualClientId + "]");
                    }

                }
            }
        }
        public static void RemoveImposter()
        {
            TestModBase.impostorsIDs.Clear();
            Player.LocalPlayer.PlayerController.nightVision.intensity = 1000;
            Player.LocalPlayer.PlayerController.nightVision.range = 2000;
            Player.LocalPlayer.PlayerController.nightVision.enabled = false;
            TestModBase.mls.LogInfo("Removing Impostors");

        }

        public static void GetImpostorStartingItem(int ItemNumber, Player player)
        {
            string itemNameIm;

            switch (ItemNumber)
            {
                case 1:
                    itemNameIm = "Shovel";//każdy item spawniony jako pierwszy jest zepsuty
                    break;
                case 2:
                    itemNameIm = "Tragedy";
                    break;
                case 3:
                    itemNameIm = "Extension ladder";
                    break;
                case 4:
                    itemNameIm = "Zap gun";//Sprawdzić czy da zapować sojuszników
                    break;
                case 5:
                    itemNameIm = "Stun grenade";
                    break;
                default:
                    itemNameIm = "";
                    break;
            }
            TestModBase.mls.LogInfo("itemNameIm is:" + itemNameIm);
            TestModBase.mls.LogInfo("ItemNumber is:" + ItemNumber);
            LC_API.GameInterfaceAPI.Features.Item item = LC_API.GameInterfaceAPI.Features.Item.CreateAndGiveItem(itemNameIm, player, default, false);
            TestModBase.mls.LogInfo("item is:" + item.name);

            item.ItemProperties.itemName = "Impostor's "+ itemNameIm;
            item.ItemProperties.twoHanded = false;
            item.ItemProperties.isConductiveMetal = false;
            item.ItemProperties.isScrap = false;
            if (itemNameIm == "Tragedy")
            {
                item.ScanNodeProperties.maxRange = 1;
            }
            TestModBase.mls.LogInfo("Adding item to slot result is:" + player.Inventory.TryAddItemToSlot(item, 3, false));
        }
        public static void LocalPlayerDC(LC_API.GameInterfaceAPI.Events.EventArgs.Player.LeftEventArgs leftEventArgs)
        {
            if (leftEventArgs.Player.IsLocalPlayer)
            {
                Player.Dictionary.Clear();
                OtherFunctions.RemoveImposter();
            }
        }
    }
}

