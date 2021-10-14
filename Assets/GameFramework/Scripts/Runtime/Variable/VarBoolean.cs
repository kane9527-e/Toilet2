//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    ///     System.Boolean 变量类。
    /// </summary>
    public sealed class VarBoolean : Variable<bool>
    {
        /// <summary>
        ///     从 System.Boolean 到 System.Boolean 变量类的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator VarBoolean(bool value)
        {
            var varValue = ReferencePool.Acquire<VarBoolean>();
            varValue.Value = value;
            return varValue;
        }

        /// <summary>
        ///     从 System.Boolean 变量类到 System.Boolean 的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator bool(VarBoolean value)
        {
            return value.Value;
        }
    }
}