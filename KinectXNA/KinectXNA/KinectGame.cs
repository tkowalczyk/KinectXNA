using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Microsoft.Kinect;
using KinectXNA.Primitives;

namespace KinectXNA
{
    public class KinectGame : Microsoft.Xna.Framework.Game
    {
        #region Properties

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KinectSensor _sensor;
        SkeletonFrame _skeletonFrame;
        Skeleton[] _skeletonData;

        List<GeometricPrimitive> primitives = new List<GeometricPrimitive>();

        List<Color> colors = new List<Color>
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.White,
            Color.Black,
        };

        int currentColorIndex = 1;
        int currentPrimitiveIndex = 2;

        #endregion

        #region Ctor
        public KinectGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialize
        protected override void Initialize()
        {
            base.Initialize();

            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
                if (_sensor.Status == KinectStatus.Connected)
                {
                    _sensor.SkeletonStream.Enable();
                    _sensor.Start();
                }
            }
        }
        #endregion

        #region LoadContent
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            primitives.Add(new CubePrimitive(GraphicsDevice));
            primitives.Add(new SpherePrimitive(GraphicsDevice));
            primitives.Add(new CylinderPrimitive(GraphicsDevice));
            primitives.Add(new TorusPrimitive(GraphicsDevice));
            primitives.Add(new TeapotPrimitive(GraphicsDevice));
        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            using (_skeletonFrame = _sensor.SkeletonStream.OpenNextFrame(0))
            {
                if (_skeletonFrame != null)
                {
                    _skeletonData = new Skeleton[_skeletonFrame.SkeletonArrayLength];
                    
                    _skeletonFrame.CopySkeletonDataTo(_skeletonData);
                }
            }

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, -20), new Vector3(0, 0, 100), Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                        GraphicsDevice.Viewport.AspectRatio,
                                                        1.0f,
                                                        100);

            GeometricPrimitive currentPrimitive = primitives[currentPrimitiveIndex];
            Color color = colors[currentColorIndex];

            DrawPrimitveSkeleton(currentPrimitive, view, projection, color);

            base.Draw(gameTime);
        }
        #endregion

        #region Private Methods

        private void DrawPrimitveSkeleton(GeometricPrimitive primitive, Matrix view, Matrix projection, Color color)
        {
            if (_skeletonData != null)
            {
                foreach (Skeleton s in _skeletonData)
                {
                    if (s.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        foreach (Joint joint in s.Joints)
                        {
                            var position = ConvertRealWorldPoint(joint.Position);
                            Matrix world = new Matrix();
                            world = Matrix.CreateTranslation(position);
                            primitive.Draw(world, view, projection, color);
                        }
                    }
                }
            }
        }

        private Vector3 ConvertRealWorldPoint(SkeletonPoint skeletonPoint)
        {
            var returnVector = new Vector3();
            returnVector.X = skeletonPoint.X * 10;
            returnVector.Y = skeletonPoint.Y * 10;
            returnVector.Z = skeletonPoint.Z;
            return returnVector;
        }

        #endregion
    }
}
