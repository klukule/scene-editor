using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Tao.OpenGl;
using Editor.Scene;

namespace Editor.Menues
{
    class InfoMenu : BaseMenu
    {
        public InfoMenu(int width, int height, int offsetX, int offsetY)
            : base(width, height, offsetX, offsetY, 3)
        {
            outerColor = new Vector(0.2, 0.2, 0.2);
            innerColor = new Vector(0.9, 0.9, 0.9);
        }
         
        public override void Draw(Scene.Scene scene)
        {
            DrawMenuBox();

            if (scene.SelectedFigure != null)
                scene.SelectedFigure.Print(scene, 5 * Margin, 20 * Margin);
        }

        public override void Update(int width, int height, int offsetX, int offsetY)
        {
            base.WIDTH = width;
            base.HEIGHT = height;
            base.OffsetX = offsetX;
            base.OffsetY = offsetY;
            //throw new NotImplementedException();
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
			
		}
		
		public override void MouseInput(Scene.Scene scene, int button, int state, int x, int y)
		{
			if(scene.SelectedFigure != null)
			{
				scene.SelectedFigure.MouseMenuInput(scene, button, state, x, y);
				
			}
			
		}
    }
}
