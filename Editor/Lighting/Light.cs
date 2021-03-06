﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Tao.OpenGl;
using System.Xml.Linq;

namespace Editor.Lighting
{
    public class Light
    {
        public Vector Ambient { get; set; }                     // No exportable
        public Vector Diffuse { get; set; }                     // Att.: Color
        public Vector Specular { get; set; }                    // No exportable
        public Vector Position { get; set; }
        public Vector SpotDirection { get; set; }               // No exportable
        public Vector SpotExponent { get; set; }                // No exportable
        public Vector SpotCutOff { get; set; }                  // No exportable
        public Vector ConstantAttenuation { get; set; }
        public Vector LinearAttenuation { get; set; }
        public Vector QuadraticAttenuation { get; set; }

        public Light(Vector position,
            Vector ambient = null, Vector diffuse = null, Vector specular = null,
            Vector spotDirection = null, Vector spotExponent = null, Vector spotCutOff = null,
            Vector constantAttenuation = null, Vector linearAttenuation = null, Vector quadraticAttenuation = null)
        {
            this.Position = position;
            this.Ambient = ambient;
            this.Diffuse = diffuse;
            this.Specular = specular;
            this.SpotDirection = spotDirection;
            this.SpotExponent = spotExponent;
            this.SpotCutOff = spotCutOff;
            this.ConstantAttenuation = constantAttenuation;
            this.LinearAttenuation = linearAttenuation;
            this.QuadraticAttenuation = quadraticAttenuation;
        }
        public void RegisterLight(int GL_ID)
        {
            Gl.glLightfv(GL_ID, Gl.GL_POSITION, MathUtils.GetVector3Array(Position));
            if (Ambient != null) Gl.glLightfv(GL_ID, Gl.GL_AMBIENT, MathUtils.GetVector4Array(Ambient));
            if (Diffuse != null) Gl.glLightfv(GL_ID, Gl.GL_DIFFUSE, MathUtils.GetVector4Array(Diffuse));
            if (Specular != null) Gl.glLightfv(GL_ID, Gl.GL_SPECULAR, MathUtils.GetVector4Array(Specular));
            if (SpotDirection != null) Gl.glLightfv(GL_ID, Gl.GL_SPOT_DIRECTION, MathUtils.GetVector4Array(SpotDirection));
            if (SpotExponent != null) Gl.glLightfv(GL_ID, Gl.GL_SPOT_EXPONENT, MathUtils.GetVector4Array(SpotExponent));
            if (SpotCutOff != null) Gl.glLightfv(GL_ID, Gl.GL_SPOT_CUTOFF, MathUtils.GetVector4Array(SpotCutOff));
            if (ConstantAttenuation != null) Gl.glLightfv(GL_ID, Gl.GL_CONSTANT_ATTENUATION, MathUtils.GetVector4Array(ConstantAttenuation));
            if (LinearAttenuation != null) Gl.glLightfv(GL_ID, Gl.GL_LINEAR_ATTENUATION, MathUtils.GetVector4Array(LinearAttenuation));
            if (QuadraticAttenuation != null) Gl.glLightfv(GL_ID, Gl.GL_QUADRATIC_ATTENUATION, MathUtils.GetVector4Array(QuadraticAttenuation));
        }

        public void Save(XElement parentNode)
        {
            XElement color = new XElement("color");
            color.SetAttributeValue("red", Diffuse.x);
            color.SetAttributeValue("green", Diffuse.y);
            color.SetAttributeValue("blue", Diffuse.z);

            XElement position = new XElement("position");
            position.SetAttributeValue("x", Position.x);
            position.SetAttributeValue("y", Position.y);
            position.SetAttributeValue("z", Position.z);

            XElement attenuation = new XElement("attenuation");
            attenuation.SetAttributeValue("quadratic", QuadraticAttenuation.x);
            attenuation.SetAttributeValue("linear", LinearAttenuation.x);
            attenuation.SetAttributeValue("constant", ConstantAttenuation.x);

            XElement xmlLight = new XElement("light");
            xmlLight.Add(color);
            xmlLight.Add(position);
            xmlLight.Add(attenuation);

            parentNode.Add(xmlLight);
        }

    }
}
