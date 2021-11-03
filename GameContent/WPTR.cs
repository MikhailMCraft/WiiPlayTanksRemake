﻿using WiiPlayTanksRemake.Internals;
using WiiPlayTanksRemake.Internals.UI;
using WiiPlayTanksRemake.Internals.Common;
using WiiPlayTanksRemake.Internals.Common.Utilities;
using WiiPlayTanksRemake.Internals.Common.GameInput;
using WiiPlayTanksRemake.Internals.Common.GameUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using WiiPlayTanksRemake.Enums;
using System;
using Microsoft.Xna.Framework.Audio;
using WiiPlayTanksRemake.GameContent.Systems;
using System.Collections.Generic;
using WiiPlayTanksRemake.Internals.Core.Interfaces;

namespace WiiPlayTanksRemake.GameContent
{
    public class WPTR
    {
        public static List<AITank> AllAITanks { get; } = new();

        public static List<PlayerTank> AllPlayerTanks { get; } = new();

        public static float FloatForTesting;

        public static Logger BaseLogger { get; } = new($"{TankGame.ExePath}", "client_logger");

        private static UIElement lastElementClicked;

        public static bool WindowBorderless { get; set; }

        public static TankMusicSystem tankMusicHandler;

        public delegate void MissionStartEvent(List<PlayerTank> players, List<AITank> aiTanks);

        /// <summary>
        /// Fired when a mission is started.
        /// </summary>
        public static MissionStartEvent OnMissionStart;

        internal static void Update()
        {
            tankMusicHandler.Update();

            foreach (var bind in Keybind.AllKeybinds)
                bind?.Update();

            foreach (var tank in AllPlayerTanks)
                tank.Update();
            foreach (var tank in AllAITanks)
                tank.Update();

            foreach (var mine in Mine.AllMines)
                mine?.Update();

            foreach (var bullet in Bullet.AllBullets)
                bullet?.Update();

            foreach (var cube in Cube.cubes)
                cube?.Update();

            FloatForTesting = MathHelper.Clamp(FloatForTesting, -1, 1);

            if (Input.MouseLeft)
            {
                if (TankGame.GameUpdateTime % 5 == 0)
                {
                    var treadPlace = Resources.GetGameResource<SoundEffect>($"Assets/sounds/tnk_tread_place_{new Random().Next(1, 5)}");
                    var treadPlaceSfx = treadPlace.CreateInstance();
                    treadPlaceSfx.Play();
                    treadPlaceSfx.Volume = 0.2f;
                    treadPlaceSfx.Pitch = FloatForTesting;
                }
            }

            if (Input.AreKeysJustPressed(Keys.RightAlt, Keys.Enter))
            {
                WindowBorderless = !WindowBorderless;
            }

            if (Input.CurrentKeySnapshot.IsKeyDown(Keys.Left))
            {
                FloatForTesting -= 0.01f;
            }
            if (Input.CurrentKeySnapshot.IsKeyDown(Keys.Right))
            {
                FloatForTesting += 0.01f;
            }
        }

        internal static void Draw()
        {
            foreach (var tank in AllPlayerTanks)
               tank.DrawBody();
            foreach (var tank in AllAITanks)
                tank.DrawBody();

            foreach (var cube in Cube.cubes)
                cube?.Draw();

            foreach (var mine in Mine.AllMines)
                mine?.Draw();

            foreach (var bullet in Bullet.AllBullets)
                bullet?.Draw();

            foreach (var parent in UIParent.TotalParents)
                parent?.DrawElements();

            TankGame.spriteBatch.DrawString(TankGame.Fonts.Default, $"TestFloat: {FloatForTesting}" +
                $"\nHighestTier: {AITank.GetHighestTierActive()}" +
                $"\n", new(10, GameUtils.WindowHeight / 3), Color.White);

            for (int i = 0; i < Enum.GetNames<TankTier>().Length; i++)
            {
                TankGame.spriteBatch.DrawString(TankGame.Fonts.Default, $"{Enum.GetNames<TankTier>()[i]}: {AITank.GetTankCountOfType((TankTier)i)}", new(10, GameUtils.WindowHeight * 0.6f + (i * 20)), Color.White);
            }

            TankGame.spriteBatch.DrawString(TankGame.Fonts.Default, $"TankWeight: {tankMusicHandler.totalSpike}", new(10, GameUtils.WindowHeight - 20), Color.White);

            if (TankGame.Instance.IsActive) {
                foreach (var parent in UIParent.TotalParents.ToList()) {
                    foreach (var element in parent.Elements) {
                        if (!element.MouseHovering && element.InteractionBox.ToRectangle().Contains(GameUtils.MousePosition)) {
                            element?.MouseOver();
                            element.MouseHovering = true;
                        }
                        else if (element.MouseHovering && !element.InteractionBox.ToRectangle().Contains(GameUtils.MousePosition)) {
                            element?.MouseLeave();
                            element.MouseHovering = false;
                        }
                        if (Input.MouseLeft && GameUtils.MouseOnScreenProtected && element != lastElementClicked) {
                            element?.MouseClick();
                            lastElementClicked = element;
                        }
                        if (Input.MouseRight && GameUtils.MouseOnScreenProtected && element != lastElementClicked) {
                            element?.MouseRightClick();
                            lastElementClicked = element;
                        }
                        if (Input.MouseMiddle && GameUtils.MouseOnScreenProtected && element != lastElementClicked) {
                            element?.MouseMiddleClick();
                            lastElementClicked = element;
                        }
                    }
                }
                if (!Input.MouseLeft && !Input.MouseRight && !Input.MouseMiddle) {
                    lastElementClicked = null;
                }
            }
        }
        public static void Initialize()
        {
            //             OnMissionStart.Invoke(AllPlayerTanks, AllAITanks);
            tankMusicHandler = new();
            new PlayerTank(new Vector3(0, 0, 0), playerType: PlayerType.Red)
            {
            };

            for (int i = 0; i < 4; i++)
            {
                var enemy = new AITank(new Vector3(new Random().Next(-200, 201), new Random().Next(-200, 201), 0), (TankTier)new Random().Next(1, 10))
                {
                    TankRotation = (float)new Random().NextDouble() * new Random().Next(1, 10)
                };
            }

            new Cube(new Vector3(100, 100, 0));

            // UI.PauseMenu.Initialize();
            tankMusicHandler.LoadMusic();
            //MusicContent.green1.Play();
        }
    }
}
