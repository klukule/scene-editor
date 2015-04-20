using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Tao.OpenGl;
using Editor.Scene;
using Tao.FreeGlut;

namespace Editor.Menues
{
    class BottomMenu:BaseMenu
    {


        public BottomMenu(int width, int height, int offsetX, int offsetY)
            : base(width, height, offsetX, offsetY, 3)
        {
            outerColor = new Vector(0.2, 0.2, 0.2);
            innerColor = new Vector(0.9, 0.9, 0.9);
        }

        public override void Draw(Scene.Scene scene)
        {
            DrawMenuBox();

            DrawMenuInfo(scene);
        }
        public override void Update(int width, int height, int offsetX, int offsetY)
        {
            base.WIDTH = width;
            base.HEIGHT = height;
            base.OffsetX = offsetX;
            base.OffsetY = offsetY;
            //throw new NotImplementedException();
        }
        protected void DrawMenuInfo(Scene.Scene scene)
        {
            IntPtr font = Glut.GLUT_BITMAP_HELVETICA_12;
            Gl.glColor3d(0, 0, 0);
            Gl.glRasterPos2d(OffsetX + 20, OffsetY + HEIGHT / 2 + 4);
            string text = "";
            if (scene.sceneState == SceneState.Edit)
                text = "Edit " + scene.SelectedButton.CurrentProperty + ": " + scene.InputString;
            if (scene.sceneState == SceneState.LoadObj)
            {
                text = "Write filename (without extension)" + ": " + scene.InputString;
            }
            else if (scene.sceneState == SceneState.Error)
                text = "Error.";
            else if (scene.sceneState == SceneState.Create)
            {
                text = "Create ";
                if (scene.FigureToCreate == Figures.FigureEnum.Triangle)
                    text += " triangle";
                else if (scene.FigureToCreate == Figures.FigureEnum.Sphere)
                    text += " sphere";

            }
            else if (scene.sceneState == SceneState.Save)
                text = "Scene was successfully saved";
            else if (scene.sceneState == SceneState.Free)
                text = "Commands: 'L' load model, 'C + T + ENTER' create triangle, 'C + S + ENTER' create sphere, 'DEL' delete selected object, 'SPACE' save";
            
            Glut.glutBitmapString(font, text);

        }

        protected void DrawMenuBox()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, WIDTH, HEIGHT, 0, 0, 1);

            Gl.glViewport(OffsetX, OffsetY, WIDTH, HEIGHT);

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glColor3fv(MathUtils.GetVector3Array(outerColor));
            Gl.glVertex2d(0, 0);
            Gl.glVertex2d(WIDTH, 0);
            Gl.glVertex2d(WIDTH, HEIGHT);
            Gl.glVertex2d(0, HEIGHT);

            Gl.glColor3fv(MathUtils.GetVector3Array(innerColor));
            Gl.glVertex2d(Margin, Margin);
            Gl.glVertex2d(WIDTH - Margin, Margin);
            Gl.glVertex2d(WIDTH - Margin, HEIGHT - Margin);
            Gl.glVertex2d(Margin, HEIGHT - Margin);

            Gl.glEnd();
        }

        public override void KeyboardInput(Scene.Scene scene, byte key, int x, int y)
        {
            //throw new NotImplementedException();
        }

        public override void MouseInput(Scene.Scene scene, int button, int state, int x, int y)
        {
            //throw new NotImplementedException();
        }
    }
}
