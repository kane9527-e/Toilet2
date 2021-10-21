
using MissionSystem.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityEngine.UI;

public class MissionDisplayUI : MonoBehaviour
{
    [SerializeField] private Text missionText;

    private MissionConfig _config;
    public MissionConfig Config => _config;

    public void Init(MissionConfig config)
    {
        this._config = config;
        UpdateMissionText();
    }

    private void UpdateMissionText()
    {
        if (!missionText || !_config) return;
        missionText.text =
            string.Format("<b><size=15>{0}</size></b>\n{1}", _config.MissionTitle, _config.MissionIntro);
    }
}