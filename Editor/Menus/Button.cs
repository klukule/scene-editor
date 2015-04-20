using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Tao.OpenGl;
using Editor.Figures;
using Editor.Scene;
using Tao.FreeGlut;

namespace Editor.Menues
{
    public class Button : BaseMenu
    {
        protected Vector selectedColor;
   
        protected Vector UndefinedInputVector()
        {
            float u = Scene.Scene.UNDEFINED;
            return new Vector(u, u, u, u);
        }

        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; private set; }
        public int inputCount { get; private set; }
        public int inputIndex { get; set; }
        public void RestartInput() { inputIndex = 0; }
        public Vector currentInput { get; private set; }
        public bool COMPLETED { get; set; }
        public bool selected { get; set; }

        private bool numberButton;
        private bool hasBorder;

        private List<string> propertyList;
        public string CurrentProperty{
            get{return propertyList[inputIndex];}
        }

        public EditPropertyDelegate callback { get; set; }

        public Button(string text, int width, int height, int inputCount, List<string> propertyList, bool numberButton = true, bool hasBorder = true)
            : base(width, height, 0, 0, 2)
        {
            this.numberButton = numberButton;
            this.hasBorder = hasBorder;
            this.Name = text;
            this.inputCount = inputCount;
            this.inputIndex = 0;
            this.currentInput = new Vector();
            this.propertyList = propertyList;

            outerColor = new Vector(0.2, 0.2, 0.2);
            innerColor = new Vector(0.9, 0.9, 0.9);
            selectedColor = new Vector(0.7, 0.7, 0.7);

            COMPLETED = false;
            selected = false;
        }

        public override void Draw(Scene.Scene scene)
        {
            


            IntPtr middleFont = Glut.GLUT_BITMAP_HELVETICA_12;
            if (hasBorder)
            {
                Gl.glBegin(Gl.GL_QUADS);

                Gl.glColor3fv(MathUtils.GetVector3Array(outerColor));
                Gl.glVertex2d(X, Y);
                Gl.glVertex2d(X + WIDTH, Y);
                Gl.glVertex2d(X + WIDTH, Y + HEIGHT);
                Gl.glVertex2d(X, Y + HEIGHT);

                if (!selected)
                    Gl.glColor3fv(MathUtils.GetVector3Array(innerColor));
                else
                    Gl.glColor3fv(MathUtils.GetVector3Array(selectedColor));
                Gl.glVertex2d(X + Margin, Y + Margin);
                Gl.glVertex2d(X + WIDTH - Margin, Y + Margin);
                Gl.glVertex2d(X + WIDTH - Margin, Y + HEIGHT - Margin);
                Gl.glVertex2d(X + Margin, Y + HEIGHT - Margin);

                Gl.glEnd();
            }
            Gl.glColor3d(0, 0, 0);
            Figure.WriteText(middleFont, Name, X+5, Y+HEIGHT/2+5);

        }
        public override void Update(int width, int height, int offsetX, int offsetY)
        {
            //throw new NotImplementedException();
        }
		public override void KeyboardInput(Scene.Scene scene, byte key, int x, int y)
		{
			throw new NotImplementedException();
		}
		
		public override void MouseInput(Scene.Scene scene, int button, int state, int x, int y)
		{
			if(x > X && x < (X + WIDTH))
			{
				if(y > Y && y < (Y+HEIGHT))
				{
					if(scene.sceneState == SceneState.Free)
					{
                        if (scene.SelectedButton != null)
                            scene.SelectedButton.selected = false;
						scene.sceneState = SceneState.Edit;
						scene.SelectedButton = this;
                        selected = true;
						scene.InputString = "";
                        currentInput = UndefinedInputVector();
					}
				}
			}
		}

        public bool ParseInput(string inputString)
        {
            if (numberButton)
            {
                string input = inputString.Trim(' ');
                input = input.Replace(',', '.');

                float data = 0;
                if (float.TryParse(input, out data))
                {

                    currentInput[inputIndex++] = data;

                    callback(Name, currentInput);

                    if (inputIndex == inputCount)
                    {
                        inputIndex = 0;
                        COMPLETED = true;
                    }

                    return true;
                }

                return false;
            }
            else
            {
                callback(Name, inputString);
                COMPLETED = true;
                return true;
            }
        }
        
    }
}
