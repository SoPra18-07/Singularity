﻿using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Singularity.AI.Structures;
using Singularity.Manager;
using Singularity.Nature;
using Singularity.Screen;
using Singularity.Screen.ScreenClasses;
using Singularity.Units;

namespace Singularity.Levels
{
    /// <inheritdoc cref="BasicLevel"/>
    [DataContract]
    internal sealed class Skirmish : BasicLevel
    {
        public Skirmish(GraphicsDeviceManager graphics,
            ref Director director,
            ContentManager content,
            IScreenManager screenmanager)
            : base(graphics, ref director, content, screenmanager)
        {

            LoadContent(content);
        }

        public override void LoadContent(ContentManager content)
        {
            var map = Map;

            var settler = new Settler(new Vector2(3000, 3200), Camera, ref mDirector, ref map, GameScreen, Ui);
            GameScreen.AddObject(settler);

            // add a puddle
            GameScreen.AddObject(new Puddle(new Vector2(3300, 2500), ref mDirector));
            GameScreen.AddObject(new Puddle(new Vector2(3300, 2700), ref mDirector, false));

            for (int i = 0; i < 100; i++)
            {
                GameScreen.AddObject(new MilitaryFast(new Vector2(3000, 3000), Camera, ref mDirector, ref map));
            }

            //TESTMETHODS HERE ====================================
        }

        public GameScreen GetGameScreen()
        {
            return GameScreen;
        }
    }
}
