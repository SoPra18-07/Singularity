using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Input;
using Singularity.Libraries;
using Singularity.Manager;
using Singularity.Map;
using Singularity.Property;
using Singularity.Screen;
using Singularity.Utils;

namespace Singularity.Units
{
    class Settler: ICollider, IRevealing, IMouseClickListener, IMousePositionListener
    {
        public Vector2 RelativePosition { get; set; }
        public Vector2 RelativeSize { get; set; }
        public Vector2 AbsolutePosition { get; set; }
        public Vector2 AbsoluteSize { get; set; }
        public Rectangle AbsBounds { get; }
        public bool Moved { get; }
        public int Id { get; }
        public int RevelationRadius { get; }
        public Vector2 Center { get; }
        public EScreen Screen { get; }
        public Rectangle Bounds { get; }


        private readonly Camera mCamera;
        private Director mDirector;
        private Map.Map mMap;
        private MilitaryPathfinder mPathfinder;
      

        // constructor for settler (position)
        public Settler(Vector2 position, Camera camera, ref Director director, ref Map.Map map)
        {
            Id = IdGenerator.NextiD(); // id for the specific unit.

            AbsolutePosition = position;
            AbsoluteSize = new Vector2(30, 30);

            RevelationRadius = (int)AbsoluteSize.X * 3;
            Center = new Vector2(AbsolutePosition.X + AbsoluteSize.X / 2, AbsolutePosition.Y + AbsoluteSize.Y / 2);

            Moved = false;
            mCamera = camera;

            mDirector = director;

            mDirector.GetInputManager.AddMouseClickListener(this, EClickType.Both, EClickType.Both);
            mDirector.GetInputManager.AddMousePositionListener(this);

            mMap = map;

            mPathfinder = new MilitaryPathfinder();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.StrokedRectangle(AbsolutePosition, AbsoluteSize, Color.White, Color.Black, 0.8f, 0.8f);

        }


        // movement

        // update

        // draw

        // life?

        // create a control center function if b pressed 

        public void Update(GameTime gametime)
        {
            
        }


        public bool MouseButtonClicked(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public bool MouseButtonPressed(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public bool MouseButtonReleased(EMouseAction mouseAction, bool withinBounds)
        {
            return true;
        }

        public void MousePositionChanged(float newX, float newY)
        {
            
        }
    }
}
