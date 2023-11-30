
using System;
using UnityEngine;

namespace UnityEditor
{ 
    public class ListToPopupAttribute : PropertyAttribute
    {
        public Type type;
        public string propertyName;

        public ListToPopupAttribute(Type type, string propertyName)
        {
            this.type = type;
            this.propertyName = propertyName;
        }
    }
}
