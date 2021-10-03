using ConditionSetting;
using ConditionSetting.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphEditor;
using VisualGraphRuntime;

[CustomPortView(typeof(NarrativePort))]
public class NarrativePortView : VisualGraphPortView
{
    public override void CreateView(VisualGraphPort port)
    {
        VisualElement port_data = new VisualElement();
        port_data.AddToClassList("port_data");
        Add(port_data);
        NarrativePort narrativePort = (NarrativePort)port;

        ObjectField conditionField = new ObjectField("Condition");
        conditionField.objectType = typeof(ConditionConfig);
        conditionField.labelElement.style.marginRight = -90;
        conditionField.value = narrativePort.conditionConfig;
        conditionField.ElementAt(1).style.flexDirection = FlexDirection.RowReverse;
        conditionField.RegisterValueChangedCallback(evt =>
        {
            narrativePort.conditionConfig = (ConditionConfig)evt.newValue;
        });

        port_data.Add(conditionField);
        // if (narrativePort.conditionConfig)
        // {
        //     UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(narrativePort.conditionConfig,typeof(ConditionConfigInspectorEditor));
        //     IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        //     port_data.Add(inspectorIMGUI);
        // }


        // FSMPort fsmPort = (FSMPort)port;
        //
        // EnumField field = new EnumField(FSMPort.State.None);
        // field.SetValueWithoutNotify(fsmPort.state);
        // field.RegisterValueChangedCallback<System.Enum>(evt=>
        // {
        //     fsmPort.state = (FSMPort.State)evt.newValue;
        // });
        // Add(field);
    }
}