namespace RNUIExamples
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Rage;
    using Rage.Attributes;
    using Rage.Native;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class TimerBarsExample
    {
        private static TimerBarPool pool;

        private static void Main()
        {
            // create the pool that handles drawing the timer bars
            pool = new TimerBarPool();

            // timer bar that shows the player health as a progress bar and is highlighted in red when the player is damaged
            BarTimerBar healthTB = new BarTimerBar("HEALTH");

            // timer bar that shows the player speed
            TextTimerBar speedTB = new TextTimerBar("SPEED", "0 km/h");

            // timer bar that shows the time of day
            TextTimerBar clockTB = new TextTimerBar("CLOCK", "00:00:00");
            // use monospace font for the time text
            clockTB.TextStyle = clockTB.TextStyle.With(font: TextFont.ChaletLondonFixedWidthNumbers);

            // timer bar that shows the player equipped weapon as an icon
            IconsTimerBar weaponTB = new IconsTimerBar("WEAPON");
            weaponTB.Icons.Add(new TimerBarIcon("commonmenu", "arrowleft")
                                {
                                    Size = new SizeF(TimerBarIcon.DefaultSize.Width * 0.825f, TimerBarIcon.DefaultSize.Height* 0.825f),
                                    Spacing = TimerBarIcon.DefaultSpacing * 2.75f
                                });
            weaponTB.Icons.Add(new TimerBarIcon("mpkillquota", "weapon_unarmed"));

            // timer bar that shows the wanted level as checkpoints
            Color checkpointsColor = HudColor.Gold.GetColor();
            CheckpointsTimerBar wantedLevelTB = new CheckpointsTimerBar("WANTED", 5) { Accent = checkpointsColor };
            foreach (TimerBarCheckpoint cp in wantedLevelTB.Checkpoints)
            {
                cp.Color = checkpointsColor;
            }

            // add all the timer bars to the pool
            pool.Add(healthTB, speedTB, clockTB, weaponTB, wantedLevelTB);

            // start the fiber which will handle drawing the timer bars
            GameFiber.StartNew(ProcessTimerBars);

            // continue with the plugin...
            float lastHealth = 0.0f;
            float healthDamage = 0.0f;
            const float HealthDamageIndicatorDuration = 2.0f; // seconds
            while (true)
            {
                GameFiber.Yield();

                Ped playerPed = Game.LocalPlayer.Character;

                // update speedometer timer bar
                {
                    int speed = MathHelper.ConvertMetersPerSecondToKilometersPerHourRounded(playerPed.Speed);
                    speedTB.Text = $"{speed} km/h";
                }

                // update clock timer bar
                {
                    clockTB.Text = World.TimeOfDay.ToString();
                }

                // update health timer bar
                {
                    float health = (float)(playerPed.Health - 100) / (playerPed.MaxHealth - 100);
                    bool hasReceivedDamaged = health < lastHealth;
                    lastHealth = health;

                    healthTB.Percentage = health;

                    healthTB.ForegroundColor = (health < 0.25 ? HudColor.RadarDamage : HudColor.RadarHealth).GetColor();
                    healthTB.BackgroundColor = Color.FromArgb(120, healthTB.ForegroundColor);


                    if (healthDamage > 0.0f)
                    {
                        // set highlight and fade it out
                        healthTB.Highlight = Color.FromArgb((int)(healthDamage * 255), HudColor.RadarDamage.GetColor());
                        healthDamage -= (1.0f / HealthDamageIndicatorDuration) * Game.FrameTime;
                    }
                    else
                    {
                        healthTB.Highlight = null;

                        if (hasReceivedDamaged)
                        {
                            healthDamage = 1.0f;
                        }
                    }
                }

                // update weapon timer bar
                {
                    // choose a new texture based on the equipped weapon
                    var tex = playerPed.CurrentVehicle.Exists() ?
                                ("mpcarhud", "transport_car_icon", 1.1f, 1.1f) :
                                playerPed.Inventory.EquippedWeapon switch
                                {
                                    null => ("mpkillquota", "weapon_unarmed", 2.75f, 1.25f),
                                    var w => NativeFunction.Natives.GetWeapontypeGroup<uint>(w.Asset.Hash) switch
                                    {
                                        0x18D5FA97u /* group_pistol  */ => ("mpkillquota", "weapon_pistol", 2.75f, 1.25f),
                                        0x33431399u /* group_shotgun */ => ("mpkillquota", "weapon_shotgun_pump", 2.7f, 1.3f),
                                        _ => ("mpkillquota", "weapon_rifle_carbine", 2.75f, 1.25f)
                                    }
                                };

                    var icon = weaponTB.Icons[1];
                    icon.TextureDictionary = tex.Item1;
                    icon.TextureName = tex.Item2;
                    var s = TimerBarIcon.DefaultSize;
                    icon.Size = new SizeF(s.Width * tex.Item3, s.Height * tex.Item4);
                }

                // update wanted level timer bar
                {
                    int level = Game.LocalPlayer.WantedLevel;
                    for (int i = 0; i < wantedLevelTB.Checkpoints.Count; i++)
                    {
                        wantedLevelTB.Checkpoints[i].State = i < level ? TimerBarCheckpointState.Completed : TimerBarCheckpointState.InProgress;
                        wantedLevelTB.Checkpoints[i].IsCrossedOut = i < level - 1;
                    }
                }
            }
        }

        private static void ProcessTimerBars()
        {
            while (true)
            {
                GameFiber.Yield();

                pool.Draw();
            }
        }


        // a command that simulates loading the plugin
        [ConsoleCommand]
        private static void RunTimerBarsExample() => GameFiber.StartNew(Main);
    }
}
