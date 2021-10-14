//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameMain.Scripts.Definition.Constant;
using UnityEditor;
using UnityEngine;

namespace GameMain.Scripts.Editor.DataTableGenerator
{
    public sealed class DataTableGeneratorMenu
    {
        [MenuItem("Game Framework/Generate ConstantTableNames")]
        private static void GenerateConstantTableNames()
        {
            DataTableGenerator.GenerateConstantCodeFile();
            AssetDatabase.Refresh();
        }

        [MenuItem("Game Framework/Generate DataTables")]
        private static void GenerateDataTables()
        {
            foreach (var dataTableName in Constant.DataTable.DataTableNames)
            {
                var dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(dataTableName);
                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
                {
                    Debug.LogError(GameFramework.Utility.Text.Format("Check raw data failure. DataTableName='{0}'",
                        dataTableName));
                    break;
                }

                DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName);
                DataTableGenerator.GenerateCodeFile(dataTableProcessor, dataTableName);
            }

            AssetDatabase.Refresh();
        }
    }
}