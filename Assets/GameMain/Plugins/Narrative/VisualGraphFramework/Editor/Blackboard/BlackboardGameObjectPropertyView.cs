///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------

using UnityEditor.Experimental.GraphView;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    [BlackboardPropertyType(typeof(GameObjectBlackboardProperty), "GameObject")]
    public class BlackboardGameObjectPropertyView : BlackboardFieldView
    {
        private GameObjectBlackboardProperty localProperty;

        public override void CreateField(BlackboardField field)
        {
            localProperty = (GameObjectBlackboardProperty)property;
            CreateObjectPropertyField(field, localProperty);
        }
    }
}