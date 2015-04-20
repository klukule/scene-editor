using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace Editor.Figures
{
    class Grid : Figure
    {
        public int Size { get; private set; }
        public bool DrawAxis { get; private set; }

        public Grid(int Size = 10, bool DrawAxis = true)
        {
            this.Size = Size;
            this.DrawAxis = DrawAxis;
        }

        public override void Update(int value)
        {
            // Nothing to do here
        }

        public override void Draw(bool select = false)
        {
            if (!select)
            {
                Gl.glDisable(Gl.GL_LIGHTING);

                Gl.glPushMatrix();
                for (int x = -Size/2; x < 1 + Size / 2; x++)
                {
                    if (x == 0 && DrawAxis)
                    {
                        Gl.glColor3f(0, 0, 1);
                    }
                    else
                    {
                        Gl.glColor3f(1, 1, 1);
                    }
                        Gl.glBegin(Gl.GL_LINE_LOOP);
                        Gl.glVertex3f(x, 0, -Size / 2);
                        Gl.glVertex3f(x, 0, Size / 2);
                        Gl.glEnd();
                    
                };

                for (int y = -Size / 2; y < 1 + Size / 2; y++)
                {
                    if (y == 0 && DrawAxis)
                    {
                        Gl.glColor3f(1, 0, 0);
                    }
                    else
                    {
                        Gl.glColor3f(1, 1, 1);
                    }
                    Gl.glBegin(Gl.GL_LINE_LOOP);
                    Gl.glVertex3f(-Size / 2, 0, y);
                    Gl.glVertex3f(Size / 2, 0, y);
                    Gl.glEnd();
                };

                if (DrawAxis)
                {
                    Gl.glBegin(Gl.GL_LINE_LOOP);
                    Gl.glColor3f(0, 1, 0);
                    Gl.glVertex3f(0, 0, 0);
                    Gl.glVertex3f(0, 10, 0);
                    Gl.glEnd();
                }


                Gl.glPopMatrix();
                Gl.glEnable(Gl.GL_LIGHTING);

            }
        }

        protected override void SetUpDelegates() { }

        public override void MouseMenuInput(Scene.Scene scene, int button, int state, int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void Save(System.Xml.Linq.XElement parentNode)
        {
            //throw new NotImplementedException();
        }

        public override void Print(Scene.Scene scene, int x, int y)
        {
            //throw new NotImplementedException();
        }

        protected override void CallbackMethod(string property, object values)
        {
            throw new NotImplementedException();
        }

    }
}
