using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            sPixel = new Texture2D(graphicsDevice: spriteBatch.GraphicsDevice, width: 1, height: 1, mipmap: false, format: SurfaceFormat.Color);
            sPixel.SetData(data: new[] { Color.White });
        }


        /// <summary>
        /// Draws a list of connecting points
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// /// <param name="position">Where to position the points</param>
        /// <param name="points">The points to connect with lines</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layer">The layer the ponits should be drawn at</param>
        private static void DrawPoints(SpriteBatch spriteBatch, Vector2 position, List<Vector2> points, Color color, float thickness, float layer = 0f)
        {
            if (points.Count < 2)
            {
                return;
            }

            for (int i = 1; i < points.Count; i++)
            {
                DrawLine(spriteBatch: spriteBatch, point1: points[index: i - 1] + position, point2: points[index: i] + position, color: color, thickness: thickness, layerDepth: layer);
            }
            // connect the last and first again
            DrawLine(spriteBatch: spriteBatch, point1: points[index: points.Count - 1] + position, point2: points[index: 0] + position, color: color, thickness: thickness, layerDepth: layer);
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
            if (sCircleCache.ContainsKey(key: circleKey))
            {
                return sCircleCache[key: circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(item: new Vector2(x: (float)(radius * Math.Cos(d: theta)), y: (float)(radius * Math.Sin(a: theta))));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(item: new Vector2(x: (float)(radius * Math.Cos(d: 0)), y: (float)(radius * Math.Sin(a: 0))));

            // Cache this circle so that it can be quickly drawn next time
            sCircleCache.Add(key: circleKey, value: vectors);

            return vectors;
        }

        private static List<Vector2> CreateEllipse(Rectangle rect)
        {
            if (sEllipseCache.ContainsKey(key: rect))
            {
                return sEllipseCache[key: rect];
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

                    var distanceToFocuses = Math.Pow(x: x - rect.Center.X, y: 2) / Math.Pow(x: a, y: 2) + Math.Pow(x: y - rect.Center.Y, y: 2) / Math.Pow(x: b, y: 2);

                    if (!(distanceToFocuses < 1 && distanceToFocuses + precision > 1 ||
                          distanceToFocuses > 1 && distanceToFocuses - precision < 1))
                    {
                        continue;

                    }
                    //System.Diagnostics.Debug.Write(x + ", "  + y);
                    vectors.Add(item: new Vector2(x: x, y: y));

                }


            }
            for (var x = rect.X + rect.Width; x >= rect.X; x--)
            {
                for (var y = rect.Y + rect.Height / 2; y <= rect.Y + rect.Height; y++)
                {

                    var distanceToFocuses = Math.Pow(x: x - rect.Center.X, y: 2) / Math.Pow(x: a, y: 2) +
                                            Math.Pow(x: y - rect.Center.Y, y: 2) / Math.Pow(x: b, y: 2);

                    if (!(distanceToFocuses < 1 && distanceToFocuses + precision > 1 ||
                          distanceToFocuses > 1 && distanceToFocuses - precision < 1))
                    {
                        continue;

                    }

                    //System.Diagnostics.Debug.Write(x + ", "  + y);
                    vectors.Add(item: new Vector2(x: x, y: y));

                }
            }
            sEllipseCache.Add(key: rect, value: vectors);
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
            points.AddRange(collection: CreateCircle(radius: radius, sides: sides));
            points.RemoveAt(index: points.Count - 1); // remove the last point because it's a duplicate of the first

            // The circle starts at (radius, 0)
            double curAngle = 0.0;
            double anglePerSide = MathHelper.TwoPi / sides;

            // "Rotate" to the starting point
            while (curAngle + anglePerSide / 2.0 < startingAngle)
            {
                curAngle += anglePerSide;

                // move the first point to the end
                points.Add(item: points[index: 0]);
                points.RemoveAt(index: 0);
            }

            // Add the first point, just in case we make a full circle
            points.Add(item: points[index: 0]);

            // Now remove the points at the end of the circle to create the arc
            int sidesInArc = (int)(radians / anglePerSide + 0.5);
            points.RemoveRange(index: sidesInArc + 1, count: points.Count - sidesInArc - 1);

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
                CreateThePixel(spriteBatch: spriteBatch);
            }

            // Simply use the function already there
            spriteBatch.Draw(texture: sPixel, destinationRectangle: rect, color: color);
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
                CreateThePixel(spriteBatch: spriteBatch);
            }

            spriteBatch.Draw(texture: sPixel, destinationRectangle: rect, sourceRectangle: null, color: color, rotation: angle, origin: Vector2.Zero, effects: SpriteEffects.None, layerDepth: layer);
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
            FillRectangle(spriteBatch: spriteBatch, location: location, size: size, color: color, angle: 0.0f);
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
                CreateThePixel(spriteBatch: spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(texture: sPixel,
                             position: location,
                             sourceRectangle: null,
                             color: color,
                             rotation: angle,
                             origin: Vector2.Zero,
                             scale: size,
                             effects: SpriteEffects.None,
                             layerDepth: 0);
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
            FillRectangle(spriteBatch: spriteBatch, location: new Vector2(x: x, y: y), size: new Vector2(x: w, y: h), color: color, angle: 0.0f);
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
            FillRectangle(spriteBatch: spriteBatch, location: new Vector2(x: x, y: y), size: new Vector2(x: w, y: h), color: color, angle: angle);
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
                CreateThePixel(spriteBatch: spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(texture: sPixel,
                position: location,
                sourceRectangle: null,
                color: color * opacity,
                rotation: angle,
                origin: Vector2.Zero,
                scale: size,
                effects: SpriteEffects.None,
                layerDepth: 0);
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
            DrawRectangle(spriteBatch: spriteBatch, rect: rect, color: color, thickness: 1.0f);
        }


        /// <summary>
        /// Draws a rectangle with the thickness provided
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="color">The color to draw the rectangle in</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layerDepth">The layer at which to draw</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness, float layerDepth = 0)
        {

            // TODO: Handle rotations
            // TODO: Figure out the pattern for the offsets required and then handle it in the line instead of here

            DrawLine(spriteBatch: spriteBatch, point1: new Vector2(x: rect.X, y: rect.Y), point2: new Vector2(x: rect.Right, y: rect.Y), color: color, thickness: thickness, layerDepth: layerDepth); // top
            DrawLine(spriteBatch: spriteBatch, point1: new Vector2(x: rect.X + 1f, y: rect.Y), point2: new Vector2(x: rect.X + 1f, y: rect.Bottom + thickness), color: color, thickness: thickness, layerDepth: layerDepth); // left
            DrawLine(spriteBatch: spriteBatch, point1: new Vector2(x: rect.X, y: rect.Bottom), point2: new Vector2(x: rect.Right, y: rect.Bottom), color: color, thickness: thickness, layerDepth: layerDepth); // bottom
            DrawLine(spriteBatch: spriteBatch, point1: new Vector2(x: rect.Right + 1f, y: rect.Y), point2: new Vector2(x: rect.Right + 1f, y: rect.Bottom + thickness), color: color, thickness: thickness, layerDepth: layerDepth); // right
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
            DrawRectangle(spriteBatch: spriteBatch, rect: new Rectangle(x: (int)location.X, y: (int)location.Y, width: (int)size.X, height: (int)size.Y), color: color, thickness: 1.0f);
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
            DrawRectangle(spriteBatch: spriteBatch, rect: new Rectangle(x: (int)location.X, y: (int)location.Y, width: (int)size.X, height: (int)size.Y), color: color, thickness: thickness);
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
            DrawRectangle(spriteBatch: spriteBatch, rect: new Rectangle(x: (int)location.X, y: (int)location.Y, width: (int)size.X, height: (int)size.Y), color: color * opacity, thickness: thickness);
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
            DrawLine(spriteBatch: spriteBatch, point1: new Vector2(x: x1, y: y1), point2: new Vector2(x: x2, y: y2), color: color, thickness: 1.0f);
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
            DrawLine(spriteBatch: spriteBatch, point1: new Vector2(x: x1, y: y1), point2: new Vector2(x: x2, y: y2), color: color, thickness: thickness);
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
            DrawLine(spriteBatch: spriteBatch, point1: point1, point2: point2, color: color, thickness: 1.0f);
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
            float distance = Vector2.Distance(value1: point1, value2: point2);

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(y: point2.Y - point1.Y, x: point2.X - point1.X);

            DrawLine(spriteBatch: spriteBatch, point: point1, length: distance, angle: angle, color: color, thickness: thickness, layerDepth: layerDepth);
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
            DrawLine(spriteBatch: spriteBatch, point: point, length: length, angle: angle, color: color, thickness: 1.0f);
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
                CreateThePixel(spriteBatch: spriteBatch);
            }

            // stretch the pixel between the two vectors
            spriteBatch.Draw(texture: sPixel,
                             position: point,
                             sourceRectangle: null,
                             color: color,
                             rotation: angle,
                             origin: Vector2.Zero,
                             scale: new Vector2(x: length, y: thickness),
                             effects: SpriteEffects.None,
                             layerDepth: layerDepth);
        }

        #endregion


        #region PutPixel

        public static void PutPixel(this SpriteBatch spriteBatch, float x, float y, Color color)
        {
            PutPixel(spriteBatch: spriteBatch, position: new Vector2(x: x, y: y), color: color);
        }


        public static void PutPixel(this SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            if (sPixel == null)
            {
                CreateThePixel(spriteBatch: spriteBatch);
            }

            spriteBatch.Draw(texture: sPixel, position: position, color: color);
        }

        #endregion


        #region DrawEllipse

        /// <summary>
        /// Draw an ellipse
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle which desribes the ellipse</param>
        /// <param name="color">The color of the ellipse</param>
        /// <param name="layer">The layer to draw the ellipse on</param>
        public static void DrawEllipse(this SpriteBatch spriteBatch, Rectangle rect, Color color, float layer)
        {
            DrawPoints(spriteBatch: spriteBatch, position: Vector2.Zero, points: CreateEllipse(rect: rect), color: color, thickness: 1.0f, layer: layer);
        }


        /// <summary>
        /// Draw an ellipse
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="rect">The rectangle which desribes the ellipse</param>
        /// <param name="color">The color of the ellipse</param>
        /// <param name="thickness">The thickness of the lines used</param>
        /// <param name="layer">todo: @Ativolex </param>
        public static void DrawEllipse(this SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness, float layer)
        {
            DrawPoints(spriteBatch: spriteBatch, position: Vector2.Zero, points: CreateEllipse(rect: rect), color: color, thickness: thickness, layer: layer);
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
            DrawPoints(spriteBatch: spriteBatch, position: center, points: CreateCircle(radius: radius, sides: sides), color: color, thickness: 1.0f);
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
        /// <param name="layerDepth">todo: @Ativolex</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness, float layerDepth = 0)
        {
            DrawPoints(spriteBatch: spriteBatch, position: center, points: CreateCircle(radius: radius, sides: sides), color: color, thickness: thickness, layer: layerDepth);
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
            DrawPoints(spriteBatch: spriteBatch, position: new Vector2(x: x, y: y), points: CreateCircle(radius: radius, sides: sides), color: color, thickness: 1.0f);
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
            DrawPoints(spriteBatch: spriteBatch, position: new Vector2(x: x, y: y), points: CreateCircle(radius: radius, sides: sides), color: color, thickness: thickness);
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
            DrawArc(spriteBatch: spriteBatch, center: center, radius: radius, sides: sides, startingAngle: startingAngle, radians: radians, color: color, thickness: 1.0f);
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
            List<Vector2> arc = CreateArc(radius: radius, sides: sides, startingAngle: startingAngle, radians: radians);
            //List<Vector2> arc = CreateArc2(radius, sides, startingAngle, degrees);
            DrawPoints(spriteBatch: spriteBatch, position: center, points: arc, color: color, thickness: thickness);
        }

        #endregion

        #region FillCircle

        /// <summary>
        /// Draws a filled circle
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="center"> center of the cirlce</param>
        /// <param name="radius"> radius of the circle</param>
        /// <param name="sides"> how many sides the circle is composed of</param>
        /// <param name="color"> color of the cirlce </param>
        /// <param name="layerDepth">The layer at which to draw</param>
        public static void FillCircle(this SpriteBatch spriteBatch, Vector2 center, float radius,int sides, Color color, float layerDepth = 0)
        {
            spriteBatch.DrawCircle(center: center, radius: radius, sides: sides, color: color, thickness: radius, layerDepth: layerDepth);
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

            FillRectangle(spriteBatch: spriteBatch, location: location, size: size, color: colorCenter, angle: 0, opacity: opacityCenter);
            DrawRectangle(spriteBatch: spriteBatch, location: location, size: size, color: colorBorder, thickness: 1, opacity: opacityBorder);
        }

        #endregion

        #region StrokedCircle
        /// <summary>
        /// Draws a stroked circle with a 3 pixel wide radius with opacity adjustments
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="center">center of the circle</param>
        /// <param name="radius">size of the radius</param>
        /// <param name="colorBorder">color of the border of circle (1 pixel)</param>
        /// <param name="colorCenter"> color of the center of circle</param>
        /// <param name="opacityBorder"> opacity of the circle border</param>
        /// <param name="opacityCenter">opacity of the circle center</param>
        public static void StrokedCircle(this SpriteBatch spriteBatch, Vector2 center, int radius, Color colorBorder, Color colorCenter, float opacityBorder, float opacityCenter)
        {
            // 3 pixel wide border of the circle
            DrawCircle(spriteBatch: spriteBatch, center: center, radius: radius, sides: 100, color: colorBorder * opacityBorder, thickness: 1);
            // fills the circle
            DrawCircle(spriteBatch: spriteBatch, center: center, radius: radius - 1, sides: 100, color: colorCenter * opacityCenter, thickness: radius - 1);
        }

        /// <summary>
        /// Draws a stroked circle with a 3 pixel wide radius without opacity adjustments
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="center">center of the circle</param>
        /// <param name="radius">size of the radius</param>
        /// <param name="colorBorder">color of the border of circle (1 pixel)</param>
        /// <param name="colorCenter"> color of the center of circle</param>
        public static void StrokedCircle(this SpriteBatch spriteBatch, Vector2 center, int radius, Color colorBorder, Color colorCenter)
        {
            // 3 pixel wide border of the circle
            DrawCircle(spriteBatch: spriteBatch, center: center, radius: radius, sides: 100, color: colorBorder, thickness: 1);
            // fills the circle
            DrawCircle(spriteBatch: spriteBatch, center: center, radius: radius - 1, sides: 100, color: colorCenter, thickness: radius - 1);
        }

        #endregion

    }
}
