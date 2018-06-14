using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Singularity.Utils;

namespace Singularity.Libraries
{
    /// <summary>
    /// Author: John McDonald and Gary Texmo
    /// Website: http://jcpmcdonald.com/2d-xna-primitives
    /// Copyright (c) 2012 John McDonald and Gary Texmo
    ///
    /// This library enables the drawing of 2D primitives in XNA using a fast 
    /// </summary>
    public static class Primitives2D
	{


		#region Private Members

		private static readonly Dictionary<String, List<Vector2>> sCircleCache = new Dictionary<string, List<Vector2>>();
	    private static readonly Dictionary<Rectangle, List<Vector2>> sEllipseCache = new Dictionary<Rectangle, List<Vector2>>();
        //private static readonly Dictionary<String, List<Vector2>> arcCache = new Dictionary<string, List<Vector2>>();
        private static Texture2D sPixel;

		#endregion


		#region Private Methods

		private static void CreateThePixel(SpriteBatch spriteBatch)
		{
			sPixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			sPixel.SetData(new[]{ Color.White });
		}


	    /// <summary>
	    /// Draws a list of connecting points
	    /// </summary>
	    /// <param name="spriteBatch">The destination drawing surface</param>
	    /// /// <param name="position">Where to position the points</param>
	    /// <param name="points">The points to connect with lines</param>
	    /// <param name="color">The color to use</param>
	    /// <param name="thickness">The thickness of the lines</param>
	    private static void DrawPoints(SpriteBatch spriteBatch, Vector2 position, List<Vector2> points, Color color, float thickness, float layer = 0f)
	    {
	        if (points.Count < 2)
	        {
	            return;
	        }

	        for (int i = 1; i < points.Count; i++)
	        {
                DrawLine(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness, layer);
	        }
            // connect the last and first again
	        DrawLine(spriteBatch, points[points.Count - 1] + position, points[0] + position, color, thickness, layer);
        }


        /// <summary>
        /// Creates a list of vectors that represents a circle
        /// </summary>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <returns>A list of vectors that, if connected, will create a circle</returns>
        private static List<Vector2> CreateCircle(double radius, int sides)
		{
			// Look for a cached version of this circle
			String circleKey = radius + "x" + sides;
			if (sCircleCache.ContainsKey(circleKey))
			{
				return sCircleCache[circleKey];
			}

			List<Vector2> vectors = new List<Vector2>();

			const double max = 2.0 * Math.PI;
			double step = max / sides;

			for (double theta = 0.0; theta < max; theta += step)
			{
				vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta))));
			}

			// then add the first vector again so it's a complete loop
			vectors.Add(new Vector2((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0))));

			// Cache this circle so that it can be quickly drawn next time
			sCircleCache.Add(circleKey, vectors);

			return vectors;
		}

	    private static List<Vector2> CreateEllipse(Rectangle rect)
	    {
	        if (sEllipseCache.ContainsKey(rect))
	        {
	            return sEllipseCache[rect];
	        }

            var vectors = new List<Vector2>();

	        var a = rect.Width / 2;
	        var b = rect.Height / 2;

	        const double precision = 0.001;

            // The reason for the double the two different for loops is so the points are "sorted" correctly. We want to draw lines
            // between all of the vectors we add, so we need to make sure that "neighbours" are added next to each other. 

            // this is EXTREMELY inefficient. This is the most naive way to calculate and has the highest resolution (according to precision) that can
            // be shown on screen. This will definitely need some kind of rework.
            for (var x = rect.X; x <= rect.X + rect.Width; x++)
	        {
	            for (var y = rect.Y; y <= rect.Y + rect.Height / 2; y++)
	            {

	                var distanceToFocuses = Math.Pow(x - rect.Center.X, 2) / Math.Pow(a, 2) + Math.Pow(y - rect.Center.Y, 2) / Math.Pow(b, 2);

                    if (!(distanceToFocuses < 1 && distanceToFocuses + precision > 1 ||
	                      distanceToFocuses > 1 && distanceToFocuses - precision < 1))
	                {
	                    continue;

	                }
                    //System.Diagnostics.Debug.Write(x + ", "  + y);
	                vectors.Add(new Vector2(x, y));

                }


            }
	        for (var x = rect.X + rect.Width; x >= rect.X; x--)
	        {
	            for (var y = rect.Y + rect.Height / 2; y <= rect.Y + rect.Height; y++)
	            {

	                var distanceToFocuses = Math.Pow(x - rect.Center.X, 2) / Math.Pow(a, 2) +
	                                        Math.Pow(y - rect.Center.Y, 2) / Math.Pow(b, 2);

	                if (!(distanceToFocuses < 1 && distanceToFocuses + precision > 1 ||
	                      distanceToFocuses > 1 && distanceToFocuses - precision < 1))
	                {
	                    continue;

	                }

	                //System.Diagnostics.Debug.Write(x + ", "  + y);
	                vectors.Add(new Vector2(x, y));

	            }
	        }
	        sEllipseCache.Add(rect, vectors);
            return vectors;
	        
	    }


		/// <summary>
		/// Creates a list of vectors that represents an arc
		/// </summary>
		/// <param name="radius">The radius of the arc</param>
		/// <param name="sides">The number of sides to generate in the circle that this will cut out from</param>
		/// <param name="startingAngle">The starting angle of arc, 0 being to the east, increasing as you go clockwise</param>
		/// <param name="radians">The radians to draw, clockwise from the starting angle</param>
		/// <returns>A list of vectors that, if connected, will create an arc</returns>
		private static List<Vector2> CreateArc(float radius, int sides, float startingAngle, float radians)
		{
			List<Vector2> points = new List<Vector2>();
			points.AddRange(CreateCircle(radius, sides));
			points.RemoveAt(points.Count - 1); // remove the last point because it's a duplicate of the first

			// The circle starts at (radius, 0)
			double curAngle = 0.0;
			double anglePerSide = MathHelper.TwoPi / sides;

			// "Rotate" to the starting point
			while ((curAngle + (anglePerSide / 2.0)) < startingAngle)
			{
				curAngle += anglePerSide;

				// move the first point to the end
				points.Add(points[0]);
				points.RemoveAt(0);
			}

			// Add the first point, just in case we make a full circle
			points.Add(points[0]);

			// Now remove the points at the end of the circle to create the arc
			int sidesInArc = (int)((radians / anglePerSide) + 0.5);
			points.RemoveRange(sidesInArc + 1, points.Count - sidesInArc - 1);

			return points;
		}

		#endregion


		#region FillRectangle

		/// <summary>
		/// Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rect">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color)
		{
			if (sPixel == null)
			{
				CreateThePixel(spriteBatch);
			}

			// Simply use the function already there
			spriteBatch.Draw(sPixel, rect, color);
		}


		/// <summary>
		/// Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rect">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		/// <param name="angle">The angle in radians to draw the rectangle at</param>
		/// <param name="layer">The depth of the layer this gets drawn on</param>
		public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float angle, float layer)
		{
			if (sPixel == null)
			{
				CreateThePixel(spriteBatch);
			}

			spriteBatch.Draw(sPixel, rect, null, color, angle, Vector2.Zero, SpriteEffects.None, layer);
		}


		/// <summary>
		/// Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="location">Where to draw</param>
		/// <param name="size">The size of the rectangle</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color)
		{
			FillRectangle(spriteBatch, location, size, color, 0.0f);
		}


		/// <summary>
		/// Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="location">Where to draw</param>
		/// <param name="size">The size of the rectangle</param>
		/// <param name="angle">The angle in radians to draw the rectangle at</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public static void FillRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float angle)
		{
			if (sPixel == null)
			{
				CreateThePixel(spriteBatch);
			}

			// stretch the pixel between the two vectors
			spriteBatch.Draw(sPixel,
			                 location,
			                 null,
			                 color,
			                 angle,
			                 Vector2.Zero,
			                 size,
			                 SpriteEffects.None,
			                 0);
		}


		/// <summary>
		/// Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="x">The X coord of the left side</param>
		/// <param name="y">The Y coord of the upper side</param>
		/// <param name="w">Width</param>
		/// <param name="h">Height</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float w, float h, Color color)
		{
			FillRectangle(spriteBatch, new Vector2(x, y), new Vector2(w, h), color, 0.0f);
		}


		/// <summary>
		/// Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="x">The X coord of the left side</param>
		/// <param name="y">The Y coord of the upper side</param>
		/// <param name="w">Width</param>
		/// <param name="h">Height</param>
		/// <param name="color">The color to draw the rectangle in</param>
		/// <param name="angle">The angle of the rectangle in radians</param>
		public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float w, float h, Color color, float angle)
		{
			FillRectangle(spriteBatch, new Vector2(x, y), new Vector2(w, h), color, angle);
		}

        /// <summary>
        /// Draws a filled rectangle
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="angle"></param>
        /// <param name="opacity"></param>
	    public static void FillRectangle(this SpriteBatch spriteBatch,
	        Vector2 location,
	        Vector2 size,
	        Color color,
	        float angle, float opacity)
	    {
	        if (sPixel == null)
	        {
	            CreateThePixel(spriteBatch);
	        }

	        // stretch the pixel between the two vectors
	        spriteBatch.Draw(sPixel,
	            location,
	            null,
	            color * opacity,
	            angle,
	            Vector2.Zero,
	            size,
	            SpriteEffects.None,
	            0);
        }

		#endregion


		#region DrawRectangle

		/// <summary>
		/// Draws a rectangle with the thickness provided
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rect">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color)
		{
			DrawRectangle(spriteBatch, rect, color, 1.0f);
		}


		/// <summary>
		/// Draws a rectangle with the thickness provided
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rect">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		/// <param name="thickness">The thickness of the lines</param>
		public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness)
		{

			// TODO: Handle rotations
			// TODO: Figure out the pattern for the offsets required and then handle it in the line instead of here

			DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), color, thickness); // top
			DrawLine(spriteBatch, new Vector2(rect.X + 1f, rect.Y), new Vector2(rect.X + 1f, rect.Bottom + thickness), color, thickness); // left
			DrawLine(spriteBatch, new Vector2(rect.X, rect.Bottom), new Vector2(rect.Right, rect.Bottom), color, thickness); // bottom
			DrawLine(spriteBatch, new Vector2(rect.Right + 1f, rect.Y), new Vector2(rect.Right + 1f, rect.Bottom + thickness), color, thickness); // right
		}


		/// <summary>
		/// Draws a rectangle with the thickness provided
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="location">Where to draw</param>
		/// <param name="size">The size of the rectangle</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color)
		{
			DrawRectangle(spriteBatch, new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color, 1.0f);
		}


        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="location">Where to draw</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the line</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float thickness)
		{
			DrawRectangle(spriteBatch, new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color, thickness);
		}

        /// <summary>
        /// Draws rectangle with adjustable opaque borders
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="opacity"></param>
	    public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color color, float thickness, float opacity)
	    {
	        DrawRectangle(spriteBatch, new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color*opacity, thickness);
	    }

        #endregion


        #region DrawLine

        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x1">The X coord of the first point</param>
        /// <param name="y1">The Y coord of the first point</param>
        /// <param name="x2">The X coord of the second point</param>
        /// <param name="y2">The Y coord of the second point</param>
        /// <param name="color">The color to use</param>
        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color)
		{
			DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, 1.0f);
		}


		/// <summary>
		/// Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="x1">The X coord of the first point</param>
		/// <param name="y1">The Y coord of the first point</param>
		/// <param name="x2">The X coord of the second point</param>
		/// <param name="y2">The Y coord of the second point</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the line</param>
		public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness)
		{
			DrawLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
		}


		/// <summary>
		/// Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="point1">The first point</param>
		/// <param name="point2">The second point</param>
		/// <param name="color">The color to use</param>
		public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color)
		{
			DrawLine(spriteBatch, point1, point2, color, 1.0f);
		}


		/// <summary>
		/// Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="point1">The first point</param>
		/// <param name="point2">The second point</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the line</param>
		/// <param name="layerDepth">The depth of the layer this gets drawn to</param>
		public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth = 0)
		{
			// calculate the distance between the two vectors
			float distance = Vector2.Distance(point1, point2);

			// calculate the angle between the two vectors
			float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

			DrawLine(spriteBatch, point1, distance, angle, color, thickness, layerDepth);
		}


		/// <summary>
		/// Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="point">The starting point</param>
		/// <param name="length">The length of the line</param>
		/// <param name="angle">The angle of this line from the starting point in radians</param>
		/// <param name="color">The color to use</param>
		public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color)
		{
			DrawLine(spriteBatch, point, length, angle, color, 1.0f);
		}


        /// <summary>
        /// Draws a line from point1 to point2 with an offset
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="point">The starting point</param>
        /// <param name="length">The length of the line</param>
        /// <param name="angle">The angle of this line from the starting point</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the line</param>
        /// <param name="layerDepth">The depth of the layer this gets drawn to</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness, float layerDepth = 0)
		{
			if (sPixel == null)
			{
				CreateThePixel(spriteBatch);
			}

			// stretch the pixel between the two vectors
			spriteBatch.Draw(sPixel,
			                 point,
			                 null,
			                 color,
			                 angle,
			                 Vector2.Zero,
			                 new Vector2(length, thickness),
			                 SpriteEffects.None,
			                 layerDepth);
		}

		#endregion


		#region PutPixel

		public static void PutPixel(this SpriteBatch spriteBatch, float x, float y, Color color)
		{
			PutPixel(spriteBatch, new Vector2(x, y), color);
		}


		public static void PutPixel(this SpriteBatch spriteBatch, Vector2 position, Color color)
		{
			if (sPixel == null)
			{
				CreateThePixel(spriteBatch);
			}

			spriteBatch.Draw(sPixel, position, color);
		}

		#endregion


		#region DrawEllipse

		/// <summary>
		/// Draw an ellipse
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rect">The rectangle which desribes the ellipse</param>
		/// <param name="color">The color of the ellipse</param>
		public static void DrawEllipse(this SpriteBatch spriteBatch, Rectangle rect, Color color, float layer)
		{
			DrawPoints(spriteBatch, Vector2.Zero, CreateEllipse(rect), color, 1.0f, layer);
		}


	    /// <summary>
	    /// Draw an ellipse
	    /// </summary>
	    /// <param name="spriteBatch">The destination drawing surface</param>
	    /// <param name="rect">The rectangle which desribes the ellipse</param>
	    /// <param name="color">The color of the ellipse</param>
        /// <param name="thickness">The thickness of the lines used</param>
        public static void DrawEllipse(this SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness, float layer)
		{
		    DrawPoints(spriteBatch, Vector2.Zero, CreateEllipse(rect), color, thickness, layer);
        }

        #endregion


        #region DrawCircle

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color)
        {
            DrawPoints(spriteBatch, center, CreateCircle(radius, sides), color, 1.0f);
        }


        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness, float layerDepth = 0)
        {
            DrawPoints(spriteBatch, center, CreateCircle(radius, sides), color, thickness, layerDepth);
        }


        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The center X of the circle</param>
        /// <param name="y">The center Y of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color)
        {
            DrawPoints(spriteBatch, new Vector2(x, y), CreateCircle(radius, sides), color, 1.0f);
        }


        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="x">The center X of the circle</param>
        /// <param name="y">The center Y of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color, float thickness)
        {
            DrawPoints(spriteBatch, new Vector2(x, y), CreateCircle(radius, sides), color, thickness);
        }

        #endregion


        #region DrawArc

        /// <summary>
        /// Draw a arc
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the arc</param>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="startingAngle">The starting angle of arc, 0 being to the east, increasing as you go clockwise</param>
        /// <param name="radians">The number of radians to draw, clockwise from the starting angle</param>
        /// <param name="color">The color of the arc</param>
        public static void DrawArc(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, float startingAngle, float radians, Color color)
		{
			DrawArc(spriteBatch, center, radius, sides, startingAngle, radians, color, 1.0f);
		}


		/// <summary>
		/// Draw a arc
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="center">The center of the arc</param>
		/// <param name="radius">The radius of the arc</param>
		/// <param name="sides">The number of sides to generate</param>
		/// <param name="startingAngle">The starting angle of arc, 0 being to the east, increasing as you go clockwise</param>
		/// <param name="radians">The number of radians to draw, clockwise from the starting angle</param>
		/// <param name="color">The color of the arc</param>
		/// <param name="thickness">The thickness of the arc</param>
		public static void DrawArc(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, float startingAngle, float radians, Color color, float thickness)
		{
			List<Vector2> arc = CreateArc(radius, sides, startingAngle, radians);
			//List<Vector2> arc = CreateArc2(radius, sides, startingAngle, degrees);
			DrawPoints(spriteBatch, center, arc, color, thickness);
		}

        #endregion


        #region StrokedRectangle
        /// <summary>
        /// Draws stroked rectangle
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="colorBorder"></param>
        /// <param name="colorCenter"></param>
        /// <param name="opacityBorder"></param>
        /// <param name="opacityCenter"></param>
	    public static void StrokedRectangle(this SpriteBatch spriteBatch, Vector2 location, Vector2 size, Color colorBorder, Color colorCenter, float opacityBorder, float opacityCenter)
	    {

	        FillRectangle(spriteBatch, location, size, colorCenter, 0, opacityCenter);
            DrawRectangle(spriteBatch, location, size, colorBorder, 1, opacityBorder);
	    }

        #endregion
    }
}