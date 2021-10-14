//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    internal sealed class WWWFormInfo : IReference
    {
        public WWWFormInfo()
        {
            WWWForm = null;
            UserData = null;
        }

        public WWWForm WWWForm { get; private set; }

        public object UserData { get; private set; }

        public void Clear()
        {
            WWWForm = null;
            UserData = null;
        }

        public static WWWFormInfo Create(WWWForm wwwForm, object userData)
        {
            var wwwFormInfo = ReferencePool.Acquire<WWWFormInfo>();
            wwwFormInfo.WWWForm = wwwForm;
            wwwFormInfo.UserData = userData;
            return wwwFormInfo;
        }
    }
}