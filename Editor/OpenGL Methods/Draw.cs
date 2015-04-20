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

namespace Editor
{
    class DrawClass
    {
        static public void Run(Scene.Scene scene, bool select, List<BaseMenu> menuList)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glClearColor(0, 0, 0, 1);        //TODO: Handle this from XML
            DrawScene(scene, select, leftMenu: menuList[0]);

            DrawMenu(scene, select, menuList);

            if (!select)
                Glut.glutSwapBuffers();
            
        }

        

        #region DRAW SCENE

        private static void DrawScene(Scene.Scene scene, bool select, BaseMenu leftMenu)
        {
            Program.Resize(scene.WIDTH, scene.HEIGHT);

            Camera camera = scene.SceneCamera;
            List<Figure> objects = scene.SceneFigures;
            List<Light> lights = scene.SceneLights;

            Gl.glMatrixMode(Gl.GL_MODELVIEW); 
            Gl.glLoadIdentity();

            Glu.gluLookAt(camera.Position.x, camera.Position.y, camera.Position.z,
                          camera.Target.x, camera.Target.y, camera.Target.z,
                          camera.Up.x, camera.Up.y, camera.Up.z);

            foreach (Figure figure in objects)
                figure.Draw(select);

            SetLighting(lights);

            
        }

        private static void SetLighting(List<Light> sceneLights)
        {
            Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, new float[] { 0.7f, 0.7f, 0.7f, 1f });         //TODO: handle real value from xml

            int lightCount = 0;
            foreach (Light light in sceneLights)
                light.RegisterLight(Gl.GL_LIGHT0 + lightCount++);

        }

        #endregion

        #region DRAW MENU

        private static void DrawMenu(Scene.Scene scene, bool select, List<BaseMenu> menuList)
        {



            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glDisable(Gl.GL_CULL_FACE);

            foreach (BaseMenu menu in menuList)
                menu.Draw(scene);

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_CULL_FACE);


        }

        #endregion
    }
}
