///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------

using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    [BlackboardPropertyType(typeof(Vector3BlackboardProperty), "Vector3")]
    public class BlackboardVector3PropertyView : BlackboardFieldView
    {
        public override void CreateField(BlackboardField field)
        {
            var localProperty = (Vector3BlackboardProperty)property;
            CreatePropertyField<Vector3, Vector3Field>(field, localProperty);
        }
    }
}