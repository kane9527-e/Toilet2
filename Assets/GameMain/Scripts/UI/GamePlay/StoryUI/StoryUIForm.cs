using System.Collections;
using System.Collections.Generic;
using Narrative.Runtime.Scripts.MonoBehaviour;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UI.GamePlay.StoryUI;
using UnityEngine;
using UnityEngine.UI;
using VisualGraphRuntime;

public class StoryUIForm : MonoBehaviour
{
    [SerializeField] private Transform optionRoot;

    [SerializeField] private GameObject optionPrefab;

    // [SerializeField] private Text textDisplayUI;
    // [SerializeField] private RawImage imageDisplayUI;
    // [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private List<DisplayUI> displayUis;

    [SerializeField] private Image textureUI;
    private Coroutine _updateStoryUICoroutine;
    private readonly List<OptionUI> m_Options = new List<OptionUI>();
    private List<OptionNode> optionNodes;
    public bool CanOperate { get; private set; }

    private void Awake()
    {
        NarrativeManager.Instance.OnNarrativeNodeChangeEvent += UpdateNarrativeUI;
        foreach (var displayUi in displayUis) displayUi.ShowDisplayFinishCallBack += UpdateOptionButton;
    }

    private void UpdateNarrativeUI(DisplayNode node)
    {
        if (_updateStoryUICoroutine != null)
            StopCoroutine(_updateStoryUICoroutine);
        _updateStoryUICoroutine = StartCoroutine(UpdateStoryUI(node));
    }

    private IEnumerator UpdateStoryUI(DisplayNode node)
    {
        yield return new WaitForEndOfFrame();
        var manager = NarrativeManager.Instance;
        if (optionNodes != null)
            optionNodes.Clear();
        //Debug.Log(manager.DataBase.GetInt("掏卫生纸盒次数"));
        optionNodes = manager.GetPortNodesWithCondition<OptionNode>(node, VisualGraphPort.PortDirection.Output);
        //var triggerNodes =manager. GetPortNodesWithCondition<TriggerNode>(node, VisualGraphPort.PortDirection.Output);
        var branchNodes = manager.GetPortNodesWithCondition<BranchNode>(node, VisualGraphPort.PortDirection.Output);
        if (branchNodes != null)
            foreach (var branchNode in branchNodes)
            {
                var resultNodes = branchNode.GetResultNodes();
                foreach (var resultNode in resultNodes)
                    if (resultNode.GetType().BaseType == typeof(OptionNode))
                        optionNodes.Add((OptionNode)resultNode);
                // if (resultNode.GetType().BaseType == typeof(TriggerNode))
                //     triggerNodes.Add((TriggerNode)resultNode);
            }

        yield return new WaitForEndOfFrame();

        if (optionNodes != null && optionNodes.Count > 0)
            optionNodes.RemoveAll(optionNode => optionNode.Once && optionNode.Triggered);


        CanOperate = false;
        HideAllOptionButton();
        yield return new WaitForEndOfFrame();
        UpdateDisplay(node);
    }


    public void SwitchNextNarrativeNode()
    {
        if (!CanOperate) return;
        if (optionNodes == null || optionNodes.Count <= 0)
            NarrativeManager.Instance.SwitchNextDefaultNode();
    }

    private void UpdateDisplay(DisplayNode displayNode)
    {
        if (!displayNode) return;
        foreach (var displayUi in displayUis)
        {
            if (!displayUi) continue;
            displayUi.gameObject.SetActive(false);
            if (displayUi.DisplayNodeType.IsInstanceOfType(displayNode))
            {
                displayUi.gameObject.SetActive(true);
                displayUi.Display(displayNode);
            }
        }
    }

    private void HideAllOptionButton()
    {
        foreach (var optionUI in m_Options)
        {
            optionUI.UpdateOption(null);
            optionUI.gameObject.SetActive(false);
        }
    }

    private void UpdateOptionButton()
    {
        CanOperate = true;

        if (optionNodes == null) return;

        while (m_Options.Count < optionNodes.Count)
        {
            var optionObj = Instantiate(optionPrefab, optionRoot);
            var optionUI = optionObj.GetComponent<OptionUI>();
            if (optionUI) m_Options.Add(optionUI);
        }

        for (var i = 0; i < m_Options.Count; i++)
        {
            if (i < optionNodes.Count)
            {
                var optionNode = optionNodes[i];
                if (!optionNode)
                {
                    m_Options[i].gameObject.SetActive(false);
                    continue;
                }

                m_Options[i].UpdateOption(optionNode);
            }

            m_Options[i].gameObject.SetActive(i < optionNodes.Count);
        }
    }
}