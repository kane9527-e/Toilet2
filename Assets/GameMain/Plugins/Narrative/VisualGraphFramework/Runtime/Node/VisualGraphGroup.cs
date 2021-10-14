using System;
using System.Collections.Generic;
using UnityEngine;

namespace VisualGraphRuntime
{
    /// <summary>
    ///     Not supported at this time
    /// </summary>
    [Serializable]
    public class VisualGraphGroup
    {
        public string title;
        public Rect position;

        public List<string> node_guids = new List<string>();
    }
}