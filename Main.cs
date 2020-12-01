// HotDogVendors 1.0 - Kubanetz
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotDogVendors
{
    public class Main : Script
    {
        string ModName = "HotDogVendors";
        string Developer = "Kubanetz";
        string Version = "1.0";
        //We set the message line about the possibility of buying a hot dog
        string MsgKey = "Press ~INPUT_CONTEXT~ to buy a hot dog for $5.";
        //We set a message string that you need to reset the wanted level before using
        string MsgWanted = "Unavailable. Lose your wanted level.";
        //Set a message string that there is not enough money to use
        string MsgMoney = "Unavailable. Not enough money.";
        //The vendor model that the script uses
        readonly Model[] VendorModel = { "S_M_M_StrVend_01" };
        //The hot dog model
        private Prop Hotdog;

        public Main()
        {
            //The script runs continuously with an interval of 1 ms.
            Tick += onTick;
            KeyDown += onKeyDown;
            Interval = 1;
        }

        //We set the display of an information message with the text in the upper left corner of the screen
        void DisplayHelpTextThisFrame(string text)
        {
            Function.Call(Hash._SET_TEXT_COMPONENT_FORMAT, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._0x238FFE5C7B0498A6, 0, 0, 1, -1);
        }
        //
        void onKeyDown(object sender, KeyEventArgs e)
        {
          //Debug message
          //if (e.KeyCode == Keys.L) { UI.ShowSubtitle("Mister Kubanetz send his regards!", 5000); }
        }
        private void onTick(object sender, EventArgs e)
        {
            //Game.Player.Character.IsInvincible = true;
            //Player character must not be in vehicle
            if (Game.Player.Character.IsOnFoot)
            {
                //Displaying a "wanted" message
                if (Game.Player.WantedLevel > 0)
                {
                    DisplayHelpTextThisFrame(MsgWanted);
                    return;
                }
                //Displaying a "money" message (if less 5)
                if (Game.Player.Money < 5)
                {
                    DisplayHelpTextThisFrame(MsgMoney);
                    return;
                }
                //If everything is OK
                else
                {
                    //Select an NPC from an area around the player with a 2f radius
                    foreach (var Vendor in World.GetNearbyPeds(Game.Player.Character.Position, 2f))
                    {
                        //Existing
                        if (Vendor.Exists())
                        {
                            //NPC must be alive
                            if (Vendor.IsAlive)
                            {
                                //NPC must be a vendor    
                                if (VendorModel.Contains(Vendor.Model))
                                {
                                        //Show message with buy button (action button)
                                        DisplayHelpTextThisFrame(MsgKey);
                                        if(Game.IsControlJustPressed(2, GTA.Control.Context))
                                        {
                                          //Loading animation dictionary #1
                                          Function.Call(Hash.REQUEST_ANIM_DICT, "gestures@m@standing@casual");
                                          Wait(5);
                                          //Loading animation dictionary #2
                                          Function.Call(Hash.REQUEST_ANIM_DICT, "mp_player_inteat@burger");
                                          Wait(5);
                                          //Playing conversation animation
                                          Function.Call(Hash.TASK_PLAY_ANIM, Game.Player.Character, "gestures@m@standing@casual", "gesture_you_soft", 8.0, 8.0 * -1, -1, 0, 0, false, false, false);
                                          //Waiting for animation #1 to end playback
                                          Wait(1000);
                                          //Stopping the animation task #1
                                          Function.Call(Hash.STOP_ANIM_TASK, Game.Player.Character, "gestures@m@standing@casual", "gesture_you_soft", 1.0);
                                          //The player is charged $5 to cook a hot dog
                                          Game.Player.Money = Game.Player.Money - 5;
                                          //Vendor cooks hot dog
                                          Wait(500);
                                          //Hot dog appears
                                          Hotdog = World.CreateProp("prop_cs_hotdog_01", Game.Player.Character.Position, true, true);
                                          //Current player character' fixation (1 of 3)
                                          Ped CurrentCharacter = Game.Player.Character;
                                          //The hot dog moves to the current character's hand
                                          Hotdog.AttachTo(CurrentCharacter, CurrentCharacter.GetBoneIndex(Bone.SKEL_L_Hand), new Vector3(0.2089f, 0.0662f, 0.0675f), new Vector3(151.4672f, 37.1995f, -145.3044f));
                                          //Playing eating animation
                                          Function.Call(Hash.TASK_PLAY_ANIM, Game.Player.Character, "mp_player_inteat@burger", "mp_player_int_eat_burger", 1.0, 1.0 * -1, -1, 0, 0, false, false, false);
                                          //Waiting for animation #2 to end playback
                                          Wait(2000);
                                          //Stopping the animation task #2
                                          Function.Call(Hash.STOP_ANIM_TASK, Game.Player.Character, "mp_player_inteat@burger", "mp_player_int_eat_burger", 1.0);
                                          Wait(5);
                                          //The hot dog disappears
                                          Hotdog.Delete();
                                          //The current player character' fixation ends
                                          CurrentCharacter.Delete();
                                          //Adding +5HP to player's character
                                          Game.Player.Character.Health = Game.Player.Character.Health + 5;
                                          //Pause before the purchase/wanted/money message appears (the cycle will start again)
                                          Wait(1000);
                                        }
                                }
                            
                            }
                        }

                    }
                }
            }

        }
    }
}

// Useful Links
// All Vehicles - https://pastebin.com/uTxZnhaN
// All Player Models - https://pastebin.com/i5c1zA0W
// All Weapons - https://pastebin.com/M3kD9pnJ
// GTA V ScriptHook V Dot Net - https://www.gta5-mods.com/tools/scripthookv-net