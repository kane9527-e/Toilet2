///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------

using UnityEditor.Experimental.GraphView;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    [BlackboardPropertyType(typeof(MaterialBlackboardProperty), "Material")]
    public class BlackboardMaterialPropertyView : BlackboardFieldView
    {
        public override void CreateField(BlackboardField field)
        {
            var localProperty = (MaterialBlackboardProperty)property;
            CreateObjectPropertyField(field, localProperty);
        }
    }
}