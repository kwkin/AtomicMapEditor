﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ame.Infrastructure.Attributes;

namespace Ame.Infrastructure.BaseTypes
{
    public class NodeViewBuilder
    {
        #region fields
        
        #endregion fields


        #region constructor
        
        public NodeViewBuilder(string name, object value)
        {
            ParseObjectTree(name, value, value.GetType());
        }

        public NodeViewBuilder(string name, object value, Type objectType)
        {
            ParseObjectTree(name, value, objectType);
        }

        #endregion constructor


        #region properties

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        private object value;
        public object Value
        {
            get
            {
                return value;
            }
        }

        private Type type;
        public Type Type
        {
            get
            {
                return type;
            }
        }

        public List<NodeViewBuilder> Children { get; set; }

        #endregion properties


        #region methods

        private void ParseObjectTree(string name, object value, Type type)
        {
            Children = new List<NodeViewBuilder>();

            this.type = type;
            this.name = name;

            SetValue(value);

            PropertyInfo[] properties = type.GetProperties();
            if (properties.Length == 0 && type.IsClass && value is IEnumerable && !(value is string))
            {
                IEnumerable arr = value as IEnumerable;
                if (arr != null)
                { 
                    int index = 0;
                    foreach (object element in arr)
                    {
                        string elementName = string.Format("[{0}]", index);
                        this.Children.Add(new NodeViewBuilder(elementName, element, element.GetType()));
                        index++;
                    }
                }
            }

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<IgnoreNodeBuilderAttribute>() == null && property.PropertyType.IsPublic)
                {
                    if (property.PropertyType.IsArray)
                    {
                        try
                        {
                            object childValue = property.GetValue(value, null);
                            IEnumerable array = childValue as IEnumerable;
                            CreateChildPropertyList(array, property);
                        }
                        catch
                        {
                        }
                    }
                    else if (property.PropertyType.IsClass)
                    {
                        try
                        {
                            object childValue = property.GetValue(value, null);
                            if (childValue != null)
                            {
                                this.Children.Add(new NodeViewBuilder(property.Name, childValue, property.PropertyType));
                            }
                        }
                        catch
                        {
                        }
                    }
                    else if (property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null)
                    {
                        try
                        {
                            object childValue = property.GetValue(value, null);
                            IEnumerable enumerable = childValue as IEnumerable;
                            CreateChildPropertyList(enumerable, property);
                        }
                        catch
                        {
                        }
                    }
                    else if (property.PropertyType.IsValueType && !(value is string))
                    {
                        try
                        {
                            object childValue = property.GetValue(value, null);
                            if (childValue != null)
                            {
                                this.Children.Add(new NodeViewBuilder(property.Name, childValue, property.PropertyType));
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void SetValue(object value)
        {
            if (value != null)
            {
                if (value is string && type != typeof(object))
                {
                    this.value = string.Format("\"{0}\"", value);
                }
                else if (value is double || value is bool || value is int || value is float || value is long || value is decimal)
                {
                    this.value = value;
                }
                else
                {
                    this.value = "{" + value.ToString() + "}";
                }
            }
        }

        private void CreateChildPropertyList(IEnumerable enumerable, PropertyInfo property)
        {
            NodeViewBuilder arrayNode = new NodeViewBuilder(property.Name, enumerable.ToString(), typeof(object));
            if (enumerable != null)
            {
                int index = 0;
                int subIndex = 0;
                NodeViewBuilder arrayNode2;

                foreach (object element in enumerable)
                {
                    if (element is IEnumerable && !(element is string))
                    {
                        arrayNode2 = new NodeViewBuilder("[" + index + "]", element.ToString(), typeof(object));

                        IEnumerable subElement = element as IEnumerable;
                        subIndex = 0;

                        foreach (object e in subElement)
                        {
                            arrayNode2.Children.Add(new NodeViewBuilder("[" + subIndex + "]", e, e.GetType()));
                            subIndex++;
                        }
                        arrayNode.Children.Add(arrayNode2);
                    }
                    else
                    {
                        arrayNode.Children.Add(new NodeViewBuilder("[" + index + "]", element, element.GetType()));
                    }
                    index++;
                }
            }
            this.Children.Add(arrayNode);
        }

        #endregion methods
    }
}
