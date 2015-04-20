using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tao.OpenGl;
using Utilities;
using System.Xml.Linq;
using Tao.FreeGlut;
using Editor.Menues;

namespace Editor.Figures
{
    public enum FigureEnum { None, Triangle, Sphere };

    public delegate void EditPropertyDelegate(string property, Object values);

    public abstract class Figure
    {

        public string Name { get; set; }
        public Vector Traslation { get; set; }
        public Vector Rotation { get; set; }
        public Vector Scaling { get; set; }
        public Vector Program { get; set; }
        public virtual Vector Color { get; set; }
        public byte[] ColorByte {get; set;}
        public virtual bool isSelected { get; set; }
        public Dictionary<string, Button> buttonDictionary { get; private set; }


        #region Constructors

        public Figure(string name = "", Vector translation = null, Vector rotation = null, Vector scaling = null)
        {
            this.Name = name;
            this.Traslation = translation != null ? translation : new Vector();
            this.Rotation = rotation != null ? rotation : new Vector();
            this.Scaling = scaling != null ? scaling : new Vector(1, 1, 1);
            buttonDictionary = new Dictionary<string, Button>();
        }


        #endregion

        public abstract void Draw(bool select = false);

        public abstract void Print(Scene.Scene scene, int x, int y);

        protected virtual void SetUpDelegates()
        {
            Button button;

            List<string> propList = new List<string>();
            propList.Add("Name");
            button = new Button("", 200, 30, 1, propList, false,false); //NO BORDER HERE
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Name", button);
        }

        public abstract void Update(int value);

        public abstract void Save(XElement parentNode);
		
		public abstract void MouseMenuInput(Scene.Scene scene, int button, int state, int x, int y);

        public static void WriteText(IntPtr font, string text, int x, int y, int r = 0, int g = 0, int b = 0)
        {
            Gl.glRasterPos2d(x, y);
            Gl.glColor3d(r, g, b);
            Glut.glutBitmapString(font, text);
        }

        protected Vector InputMask(Vector values)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = values[i] == Scene.Scene.UNDEFINED ? 0 : 1;

            return result;
        }

        protected virtual void CallbackMethod(string property, Object values)
        {
            if (values is string)
            {
                string value = values as string;

                if (property == "")
                    Name = value;
            }
        }
    }
}
