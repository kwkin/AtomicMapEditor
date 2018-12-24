﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ame.Infrastructure.Files;

namespace Ame.Infrastructure.Models
{
    public class LayerGroup : ILayer
    {
        #region fields

        #endregion fields


        #region constructor

        public LayerGroup(string layerGroupName)
        {
            this.Name = layerGroupName;
        }

        public LayerGroup(string layerGroupName, IList<ILayer> layers)
        {
            this.Name = layerGroupName;
            this.Layers = layers;
        }

        #endregion constructor


        #region properties

        public string Name { get; set; }
        public bool IsImmutable { get; set; }
        public bool IsVisible { get; set; }
        public IList<ILayer> Layers { get; set; }

        #endregion properties


        #region methods
        
        public void SerializeXML(XmlWriter writer)
        {
            XMLTagMethods.WriteElement(writer, AmeXMLTags.NAME, this.Name);
        }

        #endregion methods
    }
}
