// ReSharper disable once CheckNamespace

using UnityEngine;
using UnityEngine.Serialization;

namespace Characteristic.Runtime.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "New Characteristic UI Config", menuName = "Characteristic/UIConfig")]
    public class CharacteristicUnitUIConfig : UnityEngine.ScriptableObject
    {
        [SerializeField] private Texture2D icon;
        [SerializeField] private Color iconColor=Color.white;
        [SerializeField] private Texture2D valueTex;
        [SerializeField] private Color valueColor=Color.white;
        [SerializeField] private Texture2D backTex;
        [SerializeField] private Color backColor=Color.white;

        public Texture2D Icon => icon;
        public Texture2D ValueTexture2D => valueTex;
        public Texture2D BackTexture2D => backTex;
        
        public Color IconColor => iconColor;
        public Color ValueColor => valueColor;
        public Color BackColor => backColor;
    }
}