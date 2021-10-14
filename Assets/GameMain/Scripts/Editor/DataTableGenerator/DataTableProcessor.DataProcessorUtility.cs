//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using GameFramework;

namespace GameMain.Scripts.Editor.DataTableGenerator
{
    public sealed partial class DataTableProcessor
    {
        private static class DataProcessorUtility
        {
            private static readonly IDictionary<string, DataProcessor> s_DataProcessors =
                new SortedDictionary<string, DataProcessor>(StringComparer.Ordinal);

            static DataProcessorUtility()
            {
                var dataProcessorBaseType = typeof(DataProcessor);
                var assembly = Assembly.GetExecutingAssembly();
                var types = assembly.GetTypes();
                for (var i = 0; i < types.Length; i++)
                {
                    if (!types[i].IsClass || types[i].IsAbstract) continue;

                    if (dataProcessorBaseType.IsAssignableFrom(types[i]))
                    {
                        var dataProcessor = (DataProcessor)Activator.CreateInstance(types[i]);
                        foreach (var typeString in dataProcessor.GetTypeStrings())
                            s_DataProcessors.Add(typeString.ToLowerInvariant(), dataProcessor);
                    }
                }
            }

            public static DataProcessor GetDataProcessor(string type)
            {
                if (type == null) type = string.Empty;

                DataProcessor dataProcessor = null;
                if (s_DataProcessors.TryGetValue(type.ToLowerInvariant(), out dataProcessor)) return dataProcessor;

                throw new GameFrameworkException(
                    GameFramework.Utility.Text.Format("Not supported data processor type '{0}'.", type));
            }
        }
    }
}