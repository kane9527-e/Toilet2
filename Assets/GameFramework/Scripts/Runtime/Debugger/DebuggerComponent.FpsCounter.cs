//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed class FpsCounter
        {
            private float m_Accumulator;
            private int m_Frames;
            private float m_TimeLeft;
            private float m_UpdateInterval;

            public FpsCounter(float updateInterval)
            {
                if (updateInterval <= 0f)
                {
                    Log.Error("Update interval is invalid.");
                    return;
                }

                m_UpdateInterval = updateInterval;
                Reset();
            }

            public float UpdateInterval
            {
                get => m_UpdateInterval;
                set
                {
                    if (value <= 0f)
                    {
                        Log.Error("Update interval is invalid.");
                        return;
                    }

                    m_UpdateInterval = value;
                    Reset();
                }
            }

            public float CurrentFps { get; private set; }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                m_Frames++;
                m_Accumulator += realElapseSeconds;
                m_TimeLeft -= realElapseSeconds;

                if (m_TimeLeft <= 0f)
                {
                    CurrentFps = m_Accumulator > 0f ? m_Frames / m_Accumulator : 0f;
                    m_Frames = 0;
                    m_Accumulator = 0f;
                    m_TimeLeft += m_UpdateInterval;
                }
            }

            private void Reset()
            {
                CurrentFps = 0f;
                m_Frames = 0;
                m_Accumulator = 0f;
                m_TimeLeft = 0f;
            }
        }
    }
}