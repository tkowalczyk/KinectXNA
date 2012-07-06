KinectXNA
=========

This simple application shows how to properly use [Kinect for Windows SDK](http://www.microsoft.com/en-us/kinectforwindows/ "Kinect for Windows SDK") with the XNA.

In previous examples I showed how to use Kinect in the simple desktop apps based on WPF where an eventing model makes sense. When we want to use Kinect in the land of XNA we have to use another aproach - pooling model.

In this example we are also using `GeometricPrimitive` class from Microsoft XNA Community Game Platform.

**How does it work?**

First we need to define some properties:

`KinectSensor _sensor;`
`SkeletonFrame _skeletonFrame;`
`Skeleton[] _skeletonData;`

`List<GeometricPrimitive> primitives = new List<GeometricPrimitive>();`

In the `Initialize()` method we need to check Kinect status and enable device.

In the `LoadContent()` method load some primitives brom third party library:

`primitives.Add(new CubePrimitive(GraphicsDevice));`

In the `Update()` method we use pooling method to retrive the data from the sensor as mentioned above:

`using (_skeletonFrame = _sensor.SkeletonStream.OpenNextFrame(0))`

In the `Draw()` method we only need defined primitives on the each of joints:

`foreach (Skeleton s in _skeletonData)
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
                }`

And that`s all.

**More examples**

Feel free to visit my homepage [Tomasz Kowalczyk](http://tomek.kownet.info/ "Tomasz Kowalczyk") to see more complex examples.

