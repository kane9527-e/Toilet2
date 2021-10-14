///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------

using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    [BlackboardPropertyType(typeof(IntBlackboardProperty), "int")]
    public class BlackboardIntPropertyView : BlackboardFieldView
    {
        public override void CreateField(BlackboardField field)
        {
            var localProperty = (IntBlackboardProperty)property;
            CreatePropertyField<int, IntegerField>(field, localProperty);
        }
    }
}