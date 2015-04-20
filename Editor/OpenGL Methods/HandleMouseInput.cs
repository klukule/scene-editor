using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Figures;
using Editor.Lighting;
using Editor.Scene;
using Tao.OpenGl;
using Tao.FreeGlut;
using Utilities;
using Editor.Menues;

namespace Editor.OpenGL_Methods
{
    class HandleMouseInputClass
    {
        static public void MouseInput(int button, int state, int x, int y, Scene.Scene scene, List<BaseMenu> menuList)
        {

            if (button == Glut.GLUT_LEFT_BUTTON)
            {
                if (state == Glut.GLUT_DOWN)
                {

                    if (x > menuList[0].WIDTH)
                    {

                        ClickScene(scene, menuList, x, y);
                    }
                    else
                    {
                        foreach (BaseMenu menu in menuList)
                            menu.MouseInput(scene, button, state, x, y);
                    }



                }
            }
        }

        protected static void ClickScene(Scene.Scene scene, List<BaseMenu> menuList, int x, int y)
        {

            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            DrawClass.Run(scene, true, menuList);

            byte[] pixel = new byte[3];

            Gl.glReadPixels(x, scene.HEIGHT + menuList[1].HEIGHT - y, 1, 1, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixel);

            if (pixel[0] != 0 || pixel[1] != 0 || pixel[2] != 0)
            {
                Figure selectedFigure = scene.FindFigure(pixel);
                if (selectedFigure != null)
                {
                    scene.SelectFigure(selectedFigure);
                }
            }

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_TEXTURE_2D);

        }

        public static void MouseWheelHandle(Scene.Scene scene, List<BaseMenu> menuList, int wheel, int dir, int x, int y)
        {
            scene.SceneCamera.HandleMouseWheel(wheel, dir, x, y);
        }
    }
}
