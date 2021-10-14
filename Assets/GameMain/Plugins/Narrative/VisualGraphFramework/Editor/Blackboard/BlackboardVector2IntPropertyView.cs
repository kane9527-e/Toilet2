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
    [BlackboardPropertyType(typeof(Vector2IntBlackboardProperty), "Vector2Int")]
    public class BlackboardVector2IntPropertyView : BlackboardFieldView
    {
        public override void CreateField(BlackboardField field)
        {
            var localProperty = (Vector2IntBlackboardProperty)property;
            CreatePropertyField<Vector2Int, Vector2IntField>(field, localProperty);
        }
    }
}