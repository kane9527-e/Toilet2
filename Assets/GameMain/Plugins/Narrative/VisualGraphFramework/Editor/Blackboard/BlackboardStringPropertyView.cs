///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    [BlackboardPropertyType(typeof(StringBlackboardProperty), "string")]
    public class BlackboardStringPropertyView : BlackboardFieldView
    {
        public override void CreateField(BlackboardField field)
        {
            var localProperty = (StringBlackboardProperty)property;
            CreatePropertyField<string, TextField>(field, localProperty);
        }
    }
}