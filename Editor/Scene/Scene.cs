using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Figures;
using Editor.Lighting;
using Editor.Menues;
using System.Xml.Linq;
using System.Globalization;
using Utilities;
using System.Xml;
using System.Threading;
using System.Drawing;
using System.IO;

namespace Editor.Scene
{

	
    public class Scene
    {
        public static float UNDEFINED = -9999.8875f;
        public static float TRIANGLE_FACES = 3;

        public CultureInfo SceneCultureInfo { get; set; }
        public Camera SceneCamera { get; private set; }
        public List<Figure> SceneFigures { get; private set; }
        public List<Light> SceneLights { get; private set; }
        public Dictionary<string, Material> MaterialDictionary { get; set; }
        public int WIDTH { get; set; }
        public int HEIGHT { get; set; }
		
		public SceneState sceneState {get; set;}
		public Button SelectedButton {get; set;}
		public string InputString {get; set;}
        public FigureEnum FigureToCreate { get; set; }

        public Figure SelectedFigure { get; private set; }

        protected int lightLimit = 8;

        public Scene(int width, int height)
        {
            this.SceneCamera = new Camera();
            this.SceneFigures = new List<Figure>();
            this.SceneLights = new List<Light>();
            this.MaterialDictionary = new Dictionary<string, Material>();

            this.SceneCultureInfo = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = this.SceneCultureInfo;
            Thread.CurrentThread.CurrentUICulture = this.SceneCultureInfo;
			
			this.sceneState = SceneState.Free;
            this.FigureToCreate = FigureEnum.None;

            this.WIDTH = width;
            this.HEIGHT = height;
            //AddFigure(new Axis());
            AddFigure(new Grid(30));
        }


        protected byte redId = 1;
        protected byte blueId = 0;
        protected byte greenId = 0;
        protected int figureCount = 0;
        public void AddFigure(Figure figure)
        {
            figure.Color = new Vector(redId, blueId, greenId) / (float)255;
            figure.ColorByte = new byte[3] { (byte)redId, (byte)blueId, (byte)greenId };

            redId++;
            if (redId == 250)
            {
                redId = 0;
                greenId++;
                if (greenId == 250)
                {
                    greenId = 0;
                    blueId++;
                }
            }

            if (figure.Name == "")
                figure.Name = figure.ToString();
            figure.Name = figure.Name + "_" + figureCount++;
            SceneFigures.Add(figure);
        }

        public void DeleteSelectedFigure()
        {
            if (SelectedFigure != null)
            {
                SceneFigures.Remove(SelectedFigure);
                SelectedFigure = null;
            }
        }

        public Figure FindFigure(byte[] color)
        {
            foreach (Figure figure in SceneFigures)
            {
                byte[] figureColor = figure.ColorByte;
                bool match = true;
                for (int i = 0; match && i < 3; i++)
                    if (color[i] != figureColor[i])
                        match = false;
                if (match)
                    return figure;
            }

            return null;
        }

        public void SelectFigure(Figure figure)
        {
            if(SelectedFigure != null)
                SelectedFigure.isSelected = false;

            SelectedFigure = figure;
            SelectedFigure.isSelected = true;

            if (SelectedButton != null)
            {
                SelectedButton.COMPLETED = false;
                SelectedButton.RestartInput();
                sceneState = SceneState.Free;
            }
        }

        #region SCENE LOAD

        #region LOAD HELPERS
        
        protected float LoadFloat(XElement node, string attribute)
        {
            if (node == null)
                return 0;
            float value = 0;
            float.TryParse(node.Attribute(attribute).Value, NumberStyles.Number, SceneCultureInfo, out value);
            return value;
        }

        private double LoadDouble(XElement node, string attribute)
        {
            if (node == null)
                return 0;
            double value = 0;
            double.TryParse(node.Attribute(attribute).Value, NumberStyles.Number, SceneCultureInfo, out value);
            return value;
        }

        protected Vector LoadXYZFloat(XElement node)
        {
            if (node == null)
                return null;
            return new Vector(LoadFloat(node, "x"), LoadFloat(node, "y"), LoadFloat(node, "z"));
        }

        protected Vector LoadXYZDouble(XElement node)
        {
            if (node == null)
                return null;
            return new Vector(LoadDouble(node, "x"), LoadDouble(node, "y"), LoadDouble(node, "z"));
        }

        protected Vector LoadColor(XElement node)
        {
            if(node == null)
                return null;
            return new Vector(LoadFloat(node, "red"), LoadFloat(node, "green"), LoadFloat(node, "blue"));
        }

        private Vector LoadSpecular(XElement node)
        {
            if (node == null)
                return null;
            return new Vector(LoadFloat(node, "red"), LoadFloat(node, "green"), LoadFloat(node, "blue"), LoadFloat(node, "shininess"));
        }

        protected Object LoadAttribute(XElement node, string attribute)
        {
            if (node == null)
                return 0;
            Object value = node.Attribute(attribute).Value;
            return value;
        }

        #endregion LOAD HELPERS

        public void Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                XDocument xmlDoc = XDocument.Load(fileName);
                XElement xmlScene = xmlDoc.Elements("scene").First();

                LoadCamera(xmlScene);

                LoadFigures(xmlScene);

                LoadLights(xmlScene);

                LoadMaterials(xmlScene);
            }
            else
            {
                LoadCamera();
                LoadLights();
            }
        }

        protected void LoadCamera(XElement xmlScene = null)
        {
            if (xmlScene != null && xmlScene.Elements("camera").Any())
            {
                XElement xmlCamera = xmlScene.Elements("camera").First();

                SceneCamera.FOV = LoadFloat(xmlCamera, "fieldOfView");
                SceneCamera.NearClip = LoadFloat(xmlCamera, "nearClip");
                SceneCamera.FarClip = LoadFloat(xmlCamera, "farClip");

                Vector Target = LoadXYZFloat(xmlCamera.Elements("target").First());
                Vector Position = LoadXYZFloat(xmlCamera.Elements("position").First());
                SceneCamera.Target = Target;
                SceneCamera.Radius = (Target - Position).Magnitude3();
                SceneCamera.Up = LoadXYZFloat(xmlCamera.Elements("up").First());
                SceneCamera.CalculatePosition();
            }
            else
            {
                SceneCamera.FOV = 45.0f;
                SceneCamera.NearClip = 0.1f;
                SceneCamera.FarClip = 200.0f;
                Vector Target = new Vector();
                SceneCamera.Target = Target;
                Vector Position = new Vector(25, 0, 0);
                SceneCamera.Radius = (Target - Position).Magnitude3();
                SceneCamera.Up = new Vector(0, 1, 0);
                SceneCamera.CalculatePosition();
            }
        }

        protected void LoadFigures(XElement xmlScene = null)
        {
            if (xmlScene != null && xmlScene.Elements("object_list").Any())
            {
                XElement xmlObjects = xmlScene.Elements("object_list").First();

                foreach (XElement xmlTriangle in xmlObjects.Elements("triangle"))
                    LoadTriangle(xmlTriangle);

                foreach (XElement xmlSphere in xmlObjects.Elements("sphere"))
                    LoadSphere(xmlSphere);

            }
        }

        #region FIGURE LOAD HELPERS

        protected void LoadTriangle(XElement xmlTriangle)
        {
            string name = xmlTriangle.Attribute("name").Value;
            if (name == "sphere_triangle_26")
                name = "sphere_!";
            Vector scale = LoadXYZDouble(xmlTriangle.Elements("scale").First());
            Vector rotation = LoadXYZDouble(xmlTriangle.Elements("rotation").First());
            Vector position = LoadXYZDouble(xmlTriangle.Elements("position").First());
            List<Vector> vertexList = new List<Vector>(3);
            List<Vector> normalList = new List<Vector>(3);

            foreach (XElement vertexNode in xmlTriangle.Elements("vertex"))
            {
                // TODO: Material
                string hola = "";
                Vector vertex = LoadXYZDouble(vertexNode.Elements("position").First());
                if(vertex.x > 100 || vertex.y > 100 || vertex.z > 100)
                    hola = "hola";
                vertexList.Add(vertex);
                normalList.Add(LoadXYZDouble(vertexNode.Elements("normal").First()));

                // TODO: UV, Texture
            }

            Triangle triangle = new Triangle(name, vertexList, normalList);
            triangle.Scaling = scale;
            triangle.Rotation = rotation;
            triangle.Traslation = position;

            AddFigure(triangle);
        }

        protected void LoadSphere(XElement xmlSphere)
        {
            float radius = LoadFloat(xmlSphere, "radius");
            // TODO: Material
            Vector position = LoadXYZDouble(xmlSphere.Elements("position").First());
            Vector rotation = LoadXYZDouble(xmlSphere.Elements("rotation").First());
            Vector scale = LoadXYZDouble(xmlSphere.Elements("scale").First());
            Vector center = LoadXYZDouble(xmlSphere.Elements("center").First());

            Sphere sphere = new Sphere(center: center, Radius: radius);
            sphere.Traslation = position;
            sphere.Rotation = rotation;
            sphere.Scaling = scale;

            AddFigure(sphere);
        }

        #endregion FIGURE LOAD HELPERS

        private void LoadLights(XElement xmlScene = null)
        {
            if (xmlScene != null && xmlScene.Elements("light_list").Any())
            {
                XElement xmlLights = xmlScene.Elements("light_list").First();

                foreach (XElement xmlLight in xmlLights.Elements("light"))
                {
                    Vector position = LoadXYZDouble(xmlLight.Elements("position").First());
                    Vector color = LoadColor(xmlLight.Elements("color").First());
                    XElement attenuation = xmlLight.Elements("attenuation").First();
                    float quadratic = LoadFloat(attenuation, "quadratic");
                    float linear = LoadFloat(attenuation, "linear");
                    float constant = LoadFloat(attenuation, "constant");

                    if (SceneLights.Count <= lightLimit)
                    {
                        Light light = new Light(position,
                                                diffuse: color,
                                                constantAttenuation: new Vector(constant, constant, constant),
                                                linearAttenuation: new Vector(linear, linear, linear),
                                                quadraticAttenuation: new Vector(quadratic, quadratic, quadratic));
                        SceneLights.Add(light);
                    }
                }
            }
            else
            {
                Vector position = new Vector(0, 5, 0);
                Vector color = new Vector(1, 1, 1);
                float quadratic = 0;
                float linear = 0;
                float constant = 1;

                Light light = new Light(position,
                                        diffuse: color,
                                        constantAttenuation: new Vector(constant, constant, constant),
                                        linearAttenuation: new Vector(linear, linear, linear),
                                        quadraticAttenuation: new Vector(quadratic, quadratic, quadratic));
                SceneLights.Add(light);
            }
        }

        private void LoadMaterials(XElement xmlScene)
        {
            if (xmlScene != null && xmlScene.Elements("material_list").Any())
            {
                XElement xmlMaterials = xmlScene.Elements("material_list").First();
                foreach (XElement materialNode in xmlMaterials.Elements("material"))
                {
                    string name = materialNode.Attribute("name").Value;
                    Material material = new Material(name);
                    if (materialNode.Elements("texture").Any())
                        material.FileName = materialNode.Elements("texture").First().Attribute("filename").Value;
                    if (material.FileName != null && material.FileName != String.Empty && File.Exists(material.FileName))
                        material.TextureImage = (Bitmap)Bitmap.FromFile(material.FileName);

                    if (materialNode.Elements("specular").Any())
                        material.Specular = LoadSpecular(materialNode.Elements("specular").First());

                    if (materialNode.Elements("diffuse").Any())
                        material.Diffuse = LoadColor(materialNode.Elements("diffuse").First());

                    if (materialNode.Elements("transparent").Any())
                        material.Transparent = LoadColor(materialNode.Elements("transparent").First());

                    if (materialNode.Elements("reflective").Any())
                        material.Reflective = LoadColor(materialNode.Elements("reflective").First());

                    if (materialNode.Elements("refraction_index").Any())
                        material.RefractionIndex = LoadColor(materialNode.Elements("refraction_index").First());

                    MaterialDictionary.Add(name, material);
                }
            }
            else
            {

                Material materialDefault = new Material("Yellow");
                materialDefault.FileName = "";
                materialDefault.Diffuse = new Vector(1, 1, 0);
                materialDefault.Specular = new Vector(0, 0, 0, 5);
                if (!MaterialDictionary.ContainsKey("Yellow"))
                {
                    MaterialDictionary.Add("Yellow", materialDefault);
                }
                
            }


        }

        public bool LoadObjModel(string fileName)
        {
            string name = "OBJ-Load: " + fileName + " ";
            int nameIndex = 0;
            string fileNameFull = "Scene/" + fileName + ".obj";

            if (System.IO.File.Exists(fileNameFull))
            {
                using (System.IO.StreamReader sr = System.IO.File.OpenText(fileNameFull))
                {
                    Model model = new Model(fileName, fileName+".xml");

                    Dictionary<Vector, List<Vector>> vertexToNormalMap = new Dictionary<Vector, List<Vector>>();
                    List<Vector> vertexBuffer = new List<Vector>();	
                    List<Vector> Faces_Triangles = new List<Vector>();
                    List<Vector> normals = new List<Vector>();
                    List<Vector> textCoordsBuffer = new List<Vector>();	


                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length < 1)
                        {
                            continue;
                        }
                        if (line.StartsWith("vt"))										
                        {
                            string cleanLine = line.Remove(0, 2).Trim();
                            string[] splittedLine = cleanLine.Split(' ');
                            float x = Math.Max(0, Math.Min(1, float.Parse(splittedLine[0])));
                            float y = Math.Max(0, Math.Min(1, float.Parse(splittedLine[1])));
                            float z = Math.Max(0, Math.Min(1, float.Parse(splittedLine[2])));
                            Vector texCoords = new Vector(x, y, z);
                            textCoordsBuffer.Add(texCoords);
                        }
                        else if (line.StartsWith("vn"))								   
                        {
                            string cleanLine = line.Remove(0, 2).Trim();
                            string[] splittedLine = cleanLine.Split(' ');
                            float x = float.Parse(splittedLine[0]);
                            float y = float.Parse(splittedLine[1]);
                            float z = float.Parse(splittedLine[2]);
                            Vector normal = new Vector(x, y, z);
                            normals.Add(normal);
                        }
                        else if (line[0] == 'v')										
                        {
                            string cleanLine = line.Remove(0, 1).Trim();
                            string[] splittedLine = cleanLine.Split(' ');
                            
                            float x = float.Parse(splittedLine[0]);
                            float y = float.Parse(splittedLine[1]);
                            float z = float.Parse(splittedLine[2]);
                            Vector v = new Vector(x, y, z);
                            vertexBuffer.Add(v);
                            vertexToNormalMap.Add(v, new List<Vector>());
                        }
                        else if (line[0] == 'f')										
                        {
                            string cleanLine = line.Remove(0, 1).Trim();
                            string[] splittedLine = cleanLine.Split(' ');
                            int[] vertexNumber = new int[4];

                            string[] splittedVertex1Info = splittedLine[0].Split('/');
                            string[] splittedVertex2Info = splittedLine[1].Split('/');
                            string[] splittedVertex3Info = splittedLine[2].Split('/');
                            string[] splittedVertex4Info = null;
                            if (splittedLine.Length > TRIANGLE_FACES)
                                splittedVertex4Info = splittedLine[3].Split('/');

                            vertexNumber[0] = int.Parse(splittedVertex1Info[0].Trim('-')) - 1;
                            vertexNumber[1] = int.Parse(splittedVertex2Info[0].Trim('-')) - 1;
                            vertexNumber[2] = int.Parse(splittedVertex3Info[0].Trim('-')) - 1;
                            if (splittedLine.Length > TRIANGLE_FACES)
                                vertexNumber[3] = int.Parse(splittedVertex4Info[0].Trim('-')) - 1;

                            Vector vector1 = vertexBuffer[vertexNumber[0]];
                            Vector vector2 = vertexBuffer[vertexNumber[1]];
                            Vector vector3 = vertexBuffer[vertexNumber[2]];

                            List<Vector> vertexList = new List<Vector>(3);
                            vertexList.Add(vector1);
                            vertexList.Add(vector2);
                            vertexList.Add(vector3);

                            List<Vector> vertexList2 = null;
                            Vector vector4 = null;
                            if (splittedLine.Length > TRIANGLE_FACES)
                            {
                                vertexList2 = new List<Vector>(3);
                                vector4 = vertexBuffer[vertexNumber[3]];
                                vertexList2.Add(vector1);
                                vertexList2.Add(vector3);
                                vertexList2.Add(vector4);
                            }

                            List<Vector> textCoordsList = null;
                            List<Vector> textCoordsList2 = null;
                            
                            if (splittedVertex1Info.Length > 1 && splittedVertex2Info.Length > 1 && splittedVertex3Info.Length > 1) 
                            {
                                Vector textCoords1 = textCoordsBuffer[int.Parse(splittedVertex1Info[1].Trim('-')) - 1];
                                Vector textCoords2 = textCoordsBuffer[int.Parse(splittedVertex2Info[1].Trim('-')) - 1];
                                Vector textCoords3 = textCoordsBuffer[int.Parse(splittedVertex3Info[1].Trim('-')) - 1];
                                textCoordsList = new List<Vector>(3);
                                textCoordsList.Add(textCoords1);
                                textCoordsList.Add(textCoords2);
                                textCoordsList.Add(textCoords3);
                                if (splittedLine.Length > TRIANGLE_FACES)
                                {
                                    Vector textCoords4 = textCoordsBuffer[int.Parse(splittedVertex4Info[1].Trim('-')) - 1];
                                    textCoordsList2 = new List<Vector>(3);
                                    textCoordsList2.Add(textCoords1);
                                    textCoordsList2.Add(textCoords3);
                                    textCoordsList2.Add(textCoords4);
                                }
                            }

                            List<Vector> normalList = new List<Vector>(3);
                            List<Vector> normalList2 = new List<Vector>(3);
                            bool useFileNormals = false;

                            if (splittedVertex1Info.Length > 2 && splittedVertex2Info.Length > 2 && splittedVertex3Info.Length > 2)
                            {
                                Vector n1 = normals[int.Parse(splittedVertex1Info[2].Trim('-')) - 1];
                                Vector n2 = normals[int.Parse(splittedVertex2Info[2].Trim('-')) - 1];
                                Vector n3 = normals[int.Parse(splittedVertex3Info[2].Trim('-')) - 1];
                                normalList.Add(n1);
                                normalList.Add(n2);
                                normalList.Add(n3);
                                if (splittedLine.Length > TRIANGLE_FACES)
                                {
                                    Vector n4 = textCoordsBuffer[int.Parse(splittedVertex4Info[2].Trim('-')) - 1];
                                    normalList2.Add(n1);
                                    normalList2.Add(n3);
                                    normalList2.Add(n4);
                                }
                                useFileNormals = true;
                            }
                            else
                            {
                                Vector normal = calculateNormal(vector1, vector2, vector3);
                                vertexToNormalMap[vector1].Add(normal);
                                vertexToNormalMap[vector2].Add(normal);
                                vertexToNormalMap[vector3].Add(normal);
                                if (splittedLine.Length > TRIANGLE_FACES)
                                    vertexToNormalMap[vector4].Add(normal);

                                normalList.Add(normal);
                                normalList.Add(normal);
                                normalList.Add(normal);

                                normalList2.Add(normal);
                                normalList2.Add(normal);
                                normalList2.Add(normal);
                            }

                            Triangle triangle = new Triangle(name + nameIndex++, vertexList, normalList, textCoordsList);
                            triangle.GenerateNormal = !useFileNormals;
                            model.addTriangle(triangle);

                            if (splittedLine.Length > TRIANGLE_FACES)
                            {
                                triangle = new Triangle(name + nameIndex++, vertexList2, normalList2, textCoordsList2);
                                triangle.GenerateNormal = !useFileNormals;
                                model.addTriangle(triangle);
                            }
                        }

                    }
                    foreach(Triangle t in model.triangleList)
                    {
                        if (t.GenerateNormal)
                        {
                            t.NormalList.Clear();
                            foreach (Vector v in t.VertexList)
                            {
                                List<Vector> normalsPerVertex = vertexToNormalMap[v];
                                Vector finalNormal = new Vector();
                                foreach (Vector n in normalsPerVertex)
                                    finalNormal += n;
                                finalNormal = finalNormal / normalsPerVertex.Count;
                                finalNormal = finalNormal.Normalize3();
                                t.NormalList.Add(finalNormal);
                            }
                        }
                    }
                    AddFigure(model);                    
                }
            }
            else
            {
                Console.WriteLine("file not found");
                return false;
            }
            Console.WriteLine("loaded!");
            return true;
        }


        #endregion SCENE LOAD

        public void Save(string fileName)
        {
            XDocument xmlDoc = new XDocument();
            xmlDoc.Declaration = new XDeclaration("1.0", "UTF-8", "yes");

            XElement xmlScene = new XElement("scene");
            xmlScene.SetAttributeValue("desc", "Scene generated by kEditor");
            xmlScene.SetAttributeValue("author", "Lukas Jech");

            XElement background = new XElement("background");
            background.Add(MathUtils.GetRGBElement("color", new Vector()));
            background.Add(MathUtils.GetRGBElement("ambientLight", new Vector(0.1, 0.1, 0.1)));
            xmlScene.Add(background);

            SceneCamera.Save(xmlScene);

            SaveLights(xmlScene);

            SaveMaterials(xmlScene);

            SaveFigures(xmlScene);

            xmlDoc.Add(xmlScene);
            xmlDoc.Save(fileName);
        }

        

        private void SaveFigures(XElement xmlScene)
        {
            XElement xmlObjects = new XElement("object_list");

            foreach (Figure figure in SceneFigures)
                figure.Save(xmlObjects);

            xmlScene.Add(xmlObjects);
        }

        protected void SaveLights(XElement xmlScene)
        {
            XElement xmlLightList = new XElement("light_list");

            foreach (Light light in SceneLights)
                light.Save(xmlLightList);

            xmlScene.Add(xmlLightList);
        }

        private void SaveMaterials(XElement xmlScene)
        {
            XElement xmlMaterialList = new XElement("material_list");

            foreach (KeyValuePair<string, Material> pair in MaterialDictionary)
                pair.Value.Save(xmlMaterialList);

            xmlScene.Add(xmlMaterialList);
        }



        protected Vector calculateNormal( Vector coord1, Vector coord2, Vector coord3 )
        {
           /* calculate Vector1 and Vector2 */
           /* float[] va = new float[3], vb = new float[3], vr = new float[3];
           double val;

           va[0] = coord1[0] - coord2[0];
           va[1] = coord1[1] - coord2[1];
           va[2] = coord1[2] - coord2[2];
 
           vb[0] = coord1[0] - coord3[0];
           vb[1] = coord1[1] - coord3[1];
           vb[2] = coord1[2] - coord3[2];
 
           /* cross product * /
           vr[0] = va[1] * vb[2] - vb[1] * va[2];
           vr[1] = vb[0] * va[2] - va[0] * vb[2];
           vr[2] = va[0] * vb[1] - vb[0] * va[1];
 
           /* normalization factor * /
           val = Math.Sqrt( vr[0]*vr[0] + vr[1]*vr[1] + vr[2]*vr[2] );
 
	        Vector norm = new Vector(vr[0]/val, vr[1]/val, vr[2]/val);
 	        return norm;*/
            Vector va = coord1 - coord2;
            Vector vb = coord1 - coord3;

            return Vector.Cross3(va, vb).Normalize3();
        }
    }
}
