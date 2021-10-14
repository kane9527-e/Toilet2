//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        /// <summary>
        ///     日志记录结点。
        /// </summary>
        public sealed class LogNode : IReference
        {
            /// <summary>
            ///     初始化日志记录结点的新实例。
            /// </summary>
            public LogNode()
            {
                LogTime = default;
                LogFrameCount = 0;
                LogType = LogType.Error;
                LogMessage = null;
                StackTrack = null;
            }

            /// <summary>
            ///     获取日志时间。
            /// </summary>
            public DateTime LogTime { get; private set; }

            /// <summary>
            ///     获取日志帧计数。
            /// </summary>
            public int LogFrameCount { get; private set; }

            /// <summary>
            ///     获取日志类型。
            /// </summary>
            public LogType LogType { get; private set; }

            /// <summary>
            ///     获取日志内容。
            /// </summary>
            public string LogMessage { get; private set; }

            /// <summary>
            ///     获取日志堆栈信息。
            /// </summary>
            public string StackTrack { get; private set; }

            /// <summary>
            ///     清理日志记录结点。
            /// </summary>
            public void Clear()
            {
                LogTime = default;
                LogFrameCount = 0;
                LogType = LogType.Error;
                LogMessage = null;
                StackTrack = null;
            }

            /// <summary>
            ///     创建日志记录结点。
            /// </summary>
            /// <param name="logType">日志类型。</param>
            /// <param name="logMessage">日志内容。</param>
            /// <param name="stackTrack">日志堆栈信息。</param>
            /// <returns>创建的日志记录结点。</returns>
            public static LogNode Create(LogType logType, string logMessage, string stackTrack)
            {
                var logNode = ReferencePool.Acquire<LogNode>();
                logNode.LogTime = DateTime.UtcNow;
                logNode.LogFrameCount = Time.frameCount;
                logNode.LogType = logType;
                logNode.LogMessage = logMessage;
                logNode.StackTrack = stackTrack;
                return logNode;
            }
        }
    }
}