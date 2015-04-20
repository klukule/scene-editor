using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using System.Xml.Linq;

namespace Editor.Scene
{
    public enum CameraMode { X, Y, Z, Free };

    public class Camera
    {
        public double FOV { get; set; }
        public double NearClip { get; set; }
        public double FarClip { get; set; }

        public Vector Position { get; set; }
        public Vector Target { get; set; }
        public Vector Up { get; set; }
        public Vector Right { get; private set; }

        public float Radius { get; set; }
        public double Alfa { get; private set; }
		public double AlfaRad{ get{ return Math.PI*Alfa/180; } }
        public double Beta { get; private set; }
		public double BetaRad{ get{ return Math.PI*Beta/180; } }
        public double AlfaDelta = 2;
        public double BetaDelta = 2;

        public CameraMode cameraMode { get;  set; }

        public Camera()
        {
            cameraMode = CameraMode.Free;
        }

        public Camera(double fov, double near, double far,
            Vector target = null, float radius = 10.0f, double alfa = 0, double beta = 0)
        {
            this.FOV = fov;
            this.NearClip = near;
            this.FarClip = far;

            this.Target = target != null ? target : new Vector();

            this.Radius = radius;
            this.Alfa = alfa;
            this.Beta = beta;


            cameraMode = CameraMode.Free;

            CalculatePosition();

        }

        public void CalculatePosition()
        {
            float z = Target.z + Radius * (float)Math.Cos(BetaRad) * (float)Math.Cos(AlfaRad);
            float x = Target.x - Radius * (float)Math.Cos(BetaRad) * (float)Math.Sin(AlfaRad);
            float y = Target.y + Radius * (float)Math.Sin(BetaRad);

            Position = new Vector(x, y, z);

            Vector relPosition = Position - Target;

            if (Beta != 90 && Beta != 270)
            {
                Right = Vector.Cross3(new Vector(relPosition.x, 0, relPosition.z), new Vector(0, 1, 0));
                this.Up = Vector.Cross3(Right, Position).Normalize3();
            }
            else
            {
                double alfaUp = Alfa;

                this.Up = new Vector(Math.Sin(Math.PI*alfaUp/180), 0, -Math.Cos(Math.PI*alfaUp/180));

                Right = Vector.Cross3(relPosition, Up).Normalize3();
            }
        }

        private double angleDelta = (double)360 / (double)30;
        double finalAngleA = 0, finalAngleB = 0, divisorA = 0, divisorB = 0;

        public void Update(int value)
        {
            if (cameraMode != CameraMode.Free)
                UpdateCameraMode();
        }

        protected void UpdateCameraMode()
        {
            if (cameraMode == CameraMode.Z)
            {
                finalAngleA = 0; divisorA = 180;
                finalAngleB = 0; divisorB = 180;
            }
            else if (cameraMode == CameraMode.X)
            {
                finalAngleA = 270; divisorA = 90;
                finalAngleB = 0; divisorB = 180;
            }
            else if (cameraMode == CameraMode.Y)
            {
                finalAngleA = Alfa;
                finalAngleB = 90; divisorB = 270;
            }

            bool alfaFinal = false;
            if (Alfa != finalAngleA)
            {
                if (cameraMode != CameraMode.X)
                    Alfa += (Alfa >= divisorA) ? angleDelta : -angleDelta;
                else
                    Alfa += (Alfa >= 90 && Alfa <= 270) ? angleDelta : -angleDelta;

                if (Alfa > 360) Alfa -= 360;
                if (Alfa < 0) Alfa += 360;

                if (Math.Abs(Alfa - finalAngleA) <= angleDelta)
                {
                    Alfa = finalAngleA;
                }
            }
            else
                alfaFinal = true;

            bool betaFinal = false;
            if (Beta != finalAngleB)
            {
                if (cameraMode != CameraMode.Y)
                    Beta += (Beta >= divisorB) ? angleDelta : -angleDelta;
                else
                    Beta += (Beta >= 90 && Beta <= 270) ? -angleDelta : angleDelta;

                if (Beta > 360) Beta -= 360;
                if (Beta < 0) Beta += 360;


                if (Math.Abs(Beta - finalAngleB) <= angleDelta)
                {
                    Beta = finalAngleB;
                }
            }
            else
                betaFinal = true;

            if (alfaFinal && betaFinal)
                cameraMode = CameraMode.Free;

            CalculatePosition();
        }

        public void HandleInput(byte key)
        {
            TargetMovement(key);
            AngleMovement(key);
            Zoom(key);

            CalculatePosition();                
        }

        float moveRightDelta = 0.01f;
        float moveUpDelta = 0.05f;

        protected void TargetMovement(byte key)
        {
            if (key == 'W')
                Target = Target + Up * moveUpDelta;
            else if (key == 'S')
                Target = Target - Up * moveUpDelta;
            else if (key == 'A')
                Target = Target + Right * moveRightDelta;
            else if (key == 'D')
                Target = Target - Right * moveRightDelta;
        }
        protected void AngleMovement(byte key)
        {
            if (key == 'w')
                Beta += BetaDelta;
            if (key == 's')
                Beta -= BetaDelta;
            if (key == 'a')
                Alfa += AlfaDelta;
            if (key == 'd')
                Alfa -= AlfaDelta;

            if (Alfa >= 360) Alfa -= 360;
            if (Alfa < 0) Alfa += 360;
            if (Beta >= 360) Beta -= 360;
            if (Beta < 0) Beta += 360;
            if (Beta >= 90 && Beta < 100) Beta = 90;
            if (Beta <= 274 && Beta > 260) Beta = 274;
        }

        public void Zoom(byte key)
        {
            if (key == 'i')
            {
                Radius -= zoomDelta;
                if (Radius < 1) Radius = 1;
            }
            else if (key == 'k')
            {
                Radius += zoomDelta;
            }

        }

        float zoomDelta = 1;

        public void HandleMouseWheel(int wheel, int direction, int x, int y)
        {
            if (direction > 0)
            {
                Radius -= zoomDelta;
                if (Radius < 1) Radius = 1;
            }
            else
            {
                Radius += zoomDelta;
            }

            CalculatePosition();
        }

        public void Save(XContainer node)
        {
            CalculatePosition();

            XElement position = new XElement("position");
            position.SetAttributeValue("x", Position.x);
            position.SetAttributeValue("y", Position.y);
            position.SetAttributeValue("z", Position.z);

            XElement target = new XElement("target");
            target.SetAttributeValue("x", Target.x);
            target.SetAttributeValue("y", Target.y);
            target.SetAttributeValue("z", Target.z);

            XElement up = new XElement("up");
            up.SetAttributeValue("x", Up.x);
            up.SetAttributeValue("y", Up.y);
            up.SetAttributeValue("z", Up.z);

            XElement camera = new XElement("camera");
            camera.SetAttributeValue("farClip", FarClip);
            camera.SetAttributeValue("nearClip", NearClip);
            camera.SetAttributeValue("fieldOfView", FOV);

            camera.Add(position);
            camera.Add(target);
            camera.Add(up);

            node.Add(camera);
        }



    }
}
