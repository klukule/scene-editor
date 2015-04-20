using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Tao.OpenGl;
using System.Xml.Linq;
using Tao.FreeGlut;
using Editor.Menues;

namespace Editor.Figures
{
    class Triangle : Figure
    {
        public List<Vector> VertexList { get; private set; }
        public List<Vector> NormalList { get; private set; }
        public List<Vector> TexCoordsList { get; private set; }
        public List<Vector> VertexColor {get; set;}

        public bool GenerateNormal { get; set; }

        public Triangle(string name = "", List<Vector> vertexList = null, List<Vector> normalList = null, List<Vector> texCoordsList = null, List<Vector> vertexColor = null)
            : base(name: name)
        {
            GenerateNormal = true;
            if (vertexList == null)
            {
                vertexList = new List<Vector>();
                vertexList.Add(new Vector(1, 0, 0));
                vertexList.Add(new Vector(0, 1, 0));
                vertexList.Add(new Vector(0, 0, 1));
            }
            if (normalList == null)
            {
                normalList = new List<Vector>();
                normalList.Add(new Vector());
                normalList.Add(new Vector());
                normalList.Add(new Vector());
            }
            if (texCoordsList == null)
            {
                texCoordsList = new List<Vector>();
                texCoordsList.Add(new Vector());
                texCoordsList.Add(new Vector());
                texCoordsList.Add(new Vector());
            }
            if (vertexColor == null)
            {
                vertexColor = new List<Vector>();
                vertexColor.Add(new Vector(255, 0, 0));
                vertexColor.Add(new Vector(0, 255, 0));
                vertexColor.Add(new Vector(0, 0, 255));
            }
            this.VertexColor = vertexColor;
            this.VertexList = vertexList;
            this.NormalList = normalList;
            this.TexCoordsList = texCoordsList;



            SetUpDelegates();
        }

        protected override void SetUpDelegates()
        {
            Button button;

            List<string> xyzList = new List<string>();
            xyzList.Add("X"); xyzList.Add("Y"); xyzList.Add("Z");

            List<string> colorList = new List<string>();
            colorList.Add("R"); colorList.Add("G"); colorList.Add("B");

            button = new Button("Position", 60, 20, 3, xyzList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Position", button);

            button = new Button("Rotation", 60, 20, 3, xyzList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Rotation", button);

            button = new Button("Scale", 60, 20, 3, xyzList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Scale", button);

            button = new Button("Vertex 0", 60, 20, 3, xyzList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Vertex 0", button);

            button = new Button("Vertex 1", 60, 20, 3, xyzList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Vertex 1", button);

            button = new Button("Vertex 2", 60, 20, 3, xyzList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Vertex 2", button);

            button = new Button("Vertex 0 color", 90, 20, 3, colorList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Vertex 0 color", button);

            button = new Button("Vertex 1 color", 90, 20, 3, colorList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Vertex 1 color", button);

            button = new Button("Vertex 2 color", 90, 20, 3, colorList);
            button.callback = new EditPropertyDelegate(CallbackMethod);
            buttonDictionary.Add("Vertex 2 color", button);

            base.SetUpDelegates();

        }

        public override void Draw(bool select = false)
        {
            List<Vector> colors = new List<Vector>();

            Gl.glPushMatrix();

            if (select)
            {
                Gl.glColor3fv(MathUtils.GetVector3Array(Color));
            }
            else if (isSelected)
            {
                Gl.glColor3fv(MathUtils.GetVector3Array(new Vector(1, 1, 1)));
            }
            else
            {
                colors.Add(VertexColor[0] / 255);
                colors.Add(VertexColor[1] / 255);
                colors.Add(VertexColor[2] / 255);
            }
            

            Gl.glBegin(Gl.GL_TRIANGLES);
            for (int i = 0; i < 3; i++)
            {
                if(!select && !isSelected)
                    Gl.glColor3fv(MathUtils.GetVector3Array(colors[i]));
                Gl.glNormal3fv(MathUtils.GetVector3Array(NormalList[i]));
                Gl.glVertex3fv(MathUtils.GetVector3Array(VertexList[i]));
            }
            Gl.glEnd();
            Gl.glPopMatrix();
        }

        public override void Print(Scene.Scene scene, int x, int y)
        {
            IntPtr normalFont = Glut.GLUT_BITMAP_HELVETICA_10;
            IntPtr titleFont = Glut.GLUT_BITMAP_HELVETICA_18;
            IntPtr middleFont = Glut.GLUT_BITMAP_HELVETICA_12;
            Button button;
            int postButtonDelta = 35;
            int interButtonDelta = 20;

            int X = x; int Y = y;
            Gl.glColor3d(0, 0, 0);

            button = buttonDictionary["Name"];
            button.X = X-5; button.Y = Y-10;
            button.Draw(scene);
            Y += 10;
            WriteText(titleFont, Name, X, Y);

            #region UNUSED

            
           /* Y += interButtonDelta;
            button = buttonDictionary["Position"];
            button.X = X; button.Y = Y;
            button.Draw(scene);
            Y += postButtonDelta;
            WriteText(normalFont, Traslation.ToString(), X + 10, Y);

            Y += interButtonDelta;
            button = buttonDictionary["Rotation"];
            button.X = X; button.Y = Y;
            button.Draw(scene);
            Y += postButtonDelta;
            WriteText(normalFont, Rotation.ToString(), X+10, Y);

            Y += interButtonDelta;
            button = buttonDictionary["Scale"];
            button.X = X; button.Y = Y;
            button.Draw(scene);
            Y += postButtonDelta;
            WriteText(normalFont, Scaling.ToString(), X+10, Y);*/
             

            #endregion

            for (int i = 0; i < 3; i++)
            {
                Y += interButtonDelta;
                button = buttonDictionary["Vertex "+i];
                button.X = X; button.Y = Y;
                button.Draw(scene);
                Y += postButtonDelta;
                WriteText(normalFont, VertexList[i].ToString(), X+10, Y);
            }

            for (int i = 0; i < 3; i++)
            {
                Y += interButtonDelta;
                button = buttonDictionary["Vertex " + i + " color"];
                button.X = X; button.Y = Y;
                button.Draw(scene);
                Y += postButtonDelta;
                WriteText(normalFont, VertexColor[i].ToString(), X + 10, Y);
            }




        }

        public override void Update(int value)
        {
            //throw new NotImplementedException();
        }
		
		public override void MouseMenuInput(Scene.Scene scene, int button, int state, int x, int y)
		{
			foreach(KeyValuePair<string, Button> entry in buttonDictionary)
			{
				entry.Value.MouseInput(scene, button, state, x, y);	
			}
		}
		
        public override void Save(XElement parentNode)
        {
            List<XElement> elementList = new List<XElement>();

            XElement scale = new XElement("scale");
            scale.SetAttributeValue("x", Scaling.x);
            scale.SetAttributeValue("y", Scaling.y);
            scale.SetAttributeValue("z", Scaling.z);
            elementList.Add(scale);

            XElement rotation = new XElement("rotation");
            rotation.SetAttributeValue("x", Rotation.x);
            rotation.SetAttributeValue("y", Rotation.y);
            rotation.SetAttributeValue("z", Rotation.z);
            elementList.Add(rotation);

            XElement position = new XElement("position");
            position.SetAttributeValue("x", Traslation.x);
            position.SetAttributeValue("y", Traslation.y);
            position.SetAttributeValue("z", Traslation.z);
            elementList.Add(position);

            for (int i = 0; i < 3; i++)
            {
                XElement vertexNode = new XElement("vertex");
                vertexNode.SetAttributeValue("index", i);
                vertexNode.SetAttributeValue("material", "Yellow");

                vertexNode.Add(MathUtils.GetXYZElement("position", VertexList[i]));
                vertexNode.Add(MathUtils.GetXYZElement("normal", NormalList[i]));
                XElement textNode = new XElement("texture");
                float u = 0;
                float v = 0;
                if (TexCoordsList != null)
                {
                    u = TexCoordsList[i].x;
                    v = TexCoordsList[i].y;
                }
                textNode.SetAttributeValue("u", u);
                textNode.SetAttributeValue("v", v);
                vertexNode.Add(textNode);
                //TODO: UV Mapping

                elementList.Add(vertexNode);
            }

            XElement triangleNode = new XElement("triangle");
            triangleNode.SetAttributeValue("name", Name);
            foreach (XElement element in elementList)
                triangleNode.Add(element);

            parentNode.Add(triangleNode);
        }

        #region Delegate Methods

        protected override void CallbackMethod(string property, Object values)
        {


            Vector mask = new Vector();
            if(values is Vector)
                mask = InputMask((Vector)values);

            switch (property)
            {
                case "Position":
                    EditPosition((Vector)values, mask); break;
                case "Rotation":
                    EditRotation((Vector)values, mask); break;
                case "Scale":
                    EditScaling((Vector)values, mask); break;
                case "Vertex 0":
                    EditVertex0((Vector)values, mask); break;
                case "Vertex 1":
                    EditVertex1((Vector)values, mask); break;
                case "Vertex 2":
                    EditVertex2((Vector)values, mask); break;
                case "Vertex 0 color":
                    EditVertex0Color((Vector)values, mask); break;
                case "Vertex 1 color":
                    EditVertex1Color((Vector)values, mask); break;
                case "Vertex 2 color":
                    EditVertex2Color((Vector)values, mask); break;
                default:
                    base.CallbackMethod(property, values); break;
            }

        }

        protected void EditPosition(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? Traslation[i] : values[i];

            this.Traslation = result;
        }

        protected void EditRotation(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? Rotation[i] : values[i];

            this.Rotation = result;
        }

        protected void EditScaling(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? Scaling[i] : values[i];

            this.Scaling = result;
        }

        protected void EditVertex0(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? VertexList[0][i] : values[i];

            this.VertexList[0] = result;
        }

        protected void EditVertex1(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? VertexList[1][i] : values[i];

            this.VertexList[1] = result;
        }

        protected void EditVertex2(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? VertexList[2][i] : values[i];

            this.VertexList[2] = result;
        }

        protected void EditVertex0Color(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? VertexList[0][i] : values[i];

            this.VertexColor[0] = result;
        }

        protected void EditVertex1Color(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? VertexList[1][i] : values[i];

            this.VertexColor[1] = result;
        }

        protected void EditVertex2Color(Vector values, Vector mask)
        {
            Vector result = new Vector();
            for (int i = 0; i < 4; i++)
                result[i] = mask[i] == 0 ? VertexList[2][i] : values[i];

            this.VertexColor[2] = result;
        }

        #endregion Delegate Methods

        public override string ToString()
        {
            return "TRIANGLE";
        }
    }
}
