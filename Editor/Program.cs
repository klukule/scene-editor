using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using Utilities;
using Editor.Figures;
using Editor.Lighting;
using Editor.Scene;
using System.Threading;
using Editor.Menues;
using Editor.OpenGL_Methods;

namespace Editor
{
    class Program
    {
        static Editor.Scene.Scene scene;
        static List<BaseMenu> menuList = new List<BaseMenu>();
		
		enum CameraPerspective { Perspective, Ortho };
		static CameraPerspective cameraPerspective = CameraPerspective.Perspective;
        static int bottomHeight = 50;
        static void Main(string[] args)
        {

            scene = new Scene.Scene(800, 600);
            
            menuList.Add(new InfoMenu(300, scene.HEIGHT - 20, 0, bottomHeight + 20));
            menuList.Add(new BottomMenu(scene.WIDTH, bottomHeight, 0, 0));

            Glut.glutInit();

            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_RGBA | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(scene.WIDTH + menuList[0].WIDTH, scene.HEIGHT + menuList[1].HEIGHT);

            Glut.glutInitWindowPosition(100, 10);
            int id = Glut.glutCreateWindow("Scene Editor");
            Init();
            Glut.glutDisplayFunc(new Glut.DisplayCallback(Draw));
            Glut.glutKeyboardFunc(new Glut.KeyboardCallback(KeyboardHandle));
            Glut.glutMouseFunc(new Glut.MouseCallback(MouseHandle));
            Glut.glutMouseWheelFunc(new Glut.MouseWheelCallback(MouseWheelHandle));
            //Glut.glutReshapeFunc(new Glut.ReshapeCallback(Resize));
            Glut.glutTimerFunc(1000/60,Update, 0);

            Glut.glutMainLoop();
        }

        static void Init()
        {
            scene.Load("output.xml");

            //scene.Save("Scene/output.xml");

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glCullFace(Gl.GL_BACK);


            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_LIGHT1);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glEnable(Gl.GL_SMOOTH);

			Resize(scene.WIDTH, scene.HEIGHT);

            //setShaders("Toon");
        }

        static int program;

        static void setShaders(String shader)
        {
            
            int vs = Gl.glCreateShader(Gl.GL_VERTEX_SHADER);
            String[] vsSource = new String[] { System.IO.File.ReadAllText("Shaders/" + shader + ".vert") };
            Gl.glShaderSource(vs, 1, vsSource, null);
            Gl.glCompileShader(vs);

            int fs = Gl.glCreateShader(Gl.GL_FRAGMENT_SHADER);
            String[] fsSource = new String[] { System.IO.File.ReadAllText("Shaders/" + shader + ".frag") };
            Gl.glShaderSource(fs, 1, fsSource, null);
            Gl.glCompileShader(fs);

            program = Gl.glCreateProgram();
            Gl.glAttachShader(program, vs);
            Gl.glAttachShader(program, fs);
            Gl.glLinkProgram(program);

            Gl.glUseProgram(program);

        }

        static void Update(int value)
        {
            UpdateClass.Run(value, camera: scene.SceneCamera,
                            objects: scene.SceneFigures,
                            lights: scene.SceneLights);
            Glut.glutPostRedisplay();

            Glut.glutTimerFunc(1000 / 60, Update, value);
        }
        static void Draw()
        {
            DrawClass.Run(scene: scene, select: false, menuList: menuList);
        }
        public static void Resize(int w, int h)
        {
            Gl.glViewport(menuList[0].WIDTH, menuList[1].HEIGHT, w, h);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Camera camera = scene.SceneCamera;

			if(cameraPerspective == CameraPerspective.Perspective)
            	Glu.gluPerspective(camera.FOV,
                                (double)(w) / (double)(h),
                                camera.NearClip,
                                camera.FarClip);
			else if(cameraPerspective == CameraPerspective.Ortho)
			{
				double zDif = camera.FarClip - camera.NearClip;
				double div = 50;
				Gl.glOrtho(-w/div, w/div, -h/div, h/div, 1, 200);
			}
            scene.WIDTH = w;
            scene.HEIGHT = h;

            menuList[0].Update(300, scene.HEIGHT , 0, bottomHeight);
            menuList[1].Update(scene.WIDTH + menuList[0].WIDTH, bottomHeight, 0, 0);

			
        }

        static void KeyboardHandle(byte key, int x, int y)
        {
            HandleKeyboardInputClass.KeyboardInput(key, x, y, scene, menuList);
        }

        static void MouseHandle(int button, int state, int x, int y)
        {
            HandleMouseInputClass.MouseInput(button, state, x, y, scene, menuList);
        }

        static void MouseWheelHandle(int wheel, int direction, int x, int y)
        {
            HandleMouseInputClass.MouseWheelHandle(scene, menuList, wheel, direction, x, y);
        }

    }
}
