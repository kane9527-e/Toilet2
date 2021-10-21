using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Scripts.Component.Mono.AssetLibrary
{
    [CreateAssetMenu(menuName = "AssetLibrary/Library", fileName = "New AssetLibrary")]
    public class AssetLibrary : ScriptableObject
    {
        [SerializeField] private string typeName;
    
        [SerializeField] private List<Object> library = new List<Object>();

        public string TypeName => typeName;
        public List<Object> Library => library;


    }
}