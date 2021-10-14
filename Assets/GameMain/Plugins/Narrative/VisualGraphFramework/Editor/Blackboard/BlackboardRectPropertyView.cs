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
    [BlackboardPropertyType(typeof(RectBlackboardProperty), "Rect")]
    public class BlackboardRectPropertyView : BlackboardFieldView
    {
        public override void CreateField(BlackboardField field)
        {
            var localProperty = (RectBlackboardProperty)property;
            CreatePropertyField<Rect, RectField>(field, localProperty);
        }
    }
}