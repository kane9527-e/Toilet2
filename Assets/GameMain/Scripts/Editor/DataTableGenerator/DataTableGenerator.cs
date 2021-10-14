﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using GameFramework;
using UnityEngine;

namespace GameMain.Scripts.Editor.DataTableGenerator
{
    public sealed class DataTableGenerator
    {
        private const string NameSpace = "GameMain.Scripts.DataTable";
        private const string DataTablePath = "Assets/GameMain/DataTables";
        private const string CSharpCodePath = "Assets/GameMain/Scripts/DataTable";
        private const string CSharpConstantCodePath = "Assets/GameMain/Scripts/Definition/Constant";
        private const string CSharpCodeTemplateFileName = "Assets/GameMain/Configs/DataTableCodeTemplate.txt";

        private const string CSharpConstantCodeTemplateFileName =
            "Assets/GameMain/Configs/Constant.DataTableTemplate.txt";

        private static readonly Regex EndWithNumberRegex = new Regex(@"\d+$");
        private static readonly Regex NameRegex = new Regex(@"^[A-Z][A-Za-z0-9_]*$");

        public static DataTableProcessor CreateDataTableProcessor(string dataTableName)
        {
            return new DataTableProcessor(
                GameFramework.Utility.Path.GetRegularPath(Path.Combine(DataTablePath, dataTableName + ".txt")),
                Encoding.GetEncoding("GB2312"), 1, 2, null, 3, 4, 1);
        }

        public static bool CheckRawData(DataTableProcessor dataTableProcessor, string dataTableName)
        {
            for (var i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                var name = dataTableProcessor.GetName(i);
                if (string.IsNullOrEmpty(name) || name == "#") continue;

                if (!NameRegex.IsMatch(name))
                {
                    Debug.LogWarning(GameFramework.Utility.Text.Format(
                        "Check raw data failure. DataTableName='{0}' Name='{1}'",
                        dataTableName, name));
                    return false;
                }
            }

            return true;
        }

        public static void GenerateDataFile(DataTableProcessor dataTableProcessor, string dataTableName)
        {
            var binaryDataFileName =
                GameFramework.Utility.Path.GetRegularPath(Path.Combine(DataTablePath, dataTableName + ".bytes"));
            if (!dataTableProcessor.GenerateDataFile(binaryDataFileName) && File.Exists(binaryDataFileName))
                File.Delete(binaryDataFileName);
        }

        public static void GenerateCodeFile(DataTableProcessor dataTableProcessor, string dataTableName)
        {
            dataTableProcessor.SetCodeTemplate(CSharpCodeTemplateFileName, Encoding.UTF8);
            dataTableProcessor.SetCodeGenerator(DataTableCodeGenerator);

            var csharpCodeFileName =
                GameFramework.Utility.Path.GetRegularPath(Path.Combine(CSharpCodePath, "DR" + dataTableName + ".cs"));
            if (!dataTableProcessor.GenerateCodeFile(csharpCodeFileName, Encoding.UTF8, dataTableName) &&
                File.Exists(csharpCodeFileName))
                File.Delete(csharpCodeFileName);
        }

        /// <summary>
        ///     生成数据表静态代码文件
        /// </summary>
        public static void GenerateConstantCodeFile()
        {
            var codeTemplate = File.ReadAllText(CSharpConstantCodeTemplateFileName, Encoding.UTF8);
            codeTemplate = DataTableConstantCodeGenerator(codeTemplate);
            var csharpCodeFileName =
                GameFramework.Utility.Path.GetRegularPath(Path.Combine(CSharpConstantCodePath,
                    "Constant.DataTable.cs"));

            if (!GenerateCodeFile(codeTemplate, csharpCodeFileName, Encoding.UTF8) &&
                File.Exists(csharpCodeFileName))
                File.Delete(csharpCodeFileName);
        }

        private static bool GenerateCodeFile(string codeTemplate, string outputFileName, Encoding encoding,
            object userData = null)
        {
            if (string.IsNullOrEmpty(codeTemplate))
                throw new GameFrameworkException("You must set code template first.");

            if (string.IsNullOrEmpty(outputFileName)) throw new GameFrameworkException("Output file name is invalid.");

            try
            {
                var stringBuilder = new StringBuilder(codeTemplate);
                using (var fileStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {
                    using (var stream = new StreamWriter(fileStream, encoding))
                    {
                        stream.Write(stringBuilder.ToString());
                    }
                }

                Debug.Log(
                    GameFramework.Utility.Text.Format("Generate ConstantCode file '{0}' success.", outputFileName));
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(GameFramework.Utility.Text.Format(
                    "Generate ConstantCode file '{0}' failure, exception is '{1}'.",
                    outputFileName, exception));
                return false;
            }
        }

        private static void DataTableCodeGenerator(DataTableProcessor dataTableProcessor, StringBuilder codeContent,
            object userData)
        {
            var dataTableName = (string)userData;

            codeContent.Replace("__DATA_TABLE_CREATE_TIME__",
                DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            codeContent.Replace("__DATA_TABLE_NAME_SPACE__", NameSpace);
            codeContent.Replace("__DATA_TABLE_CLASS_NAME__", "DR" + dataTableName);
            codeContent.Replace("__DATA_TABLE_COMMENT__", dataTableProcessor.GetValue(0, 1) + "。");
            codeContent.Replace("__DATA_TABLE_ID_COMMENT__",
                "获取" + dataTableProcessor.GetComment(dataTableProcessor.IdColumn) + "。");
            codeContent.Replace("__DATA_TABLE_PROPERTIES__", GenerateDataTableProperties(dataTableProcessor));
            codeContent.Replace("__DATA_TABLE_PARSER__", GenerateDataTableParser(dataTableProcessor));
            codeContent.Replace("__DATA_TABLE_PROPERTY_ARRAY__", GenerateDataTablePropertyArray(dataTableProcessor));
        }

        private static string DataTableConstantCodeGenerator(string codeContent)
        {
            var builder = new StringBuilder(codeContent);
            builder.Replace("__DATA_TABLE_CREATE_TIME__",
                DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
            builder.Replace("__DATA_TABLE_CONSTANT_NAME__", DataTableNameProperties());
            return builder.ToString();
        }

        private static string DataTableNameProperties()
        {
            var stringBuilder = new StringBuilder();
            var files = Directory.GetFiles(DataTablePath, "*.txt");
            foreach (var fileName in files)
            {
                var tableName = Path.GetFileNameWithoutExtension(fileName);
                stringBuilder.AppendLine(string.Format("                \"{0}\",", tableName));
            }

            return stringBuilder.ToString();
        }

        private static string GenerateDataTableProperties(DataTableProcessor dataTableProcessor)
        {
            var stringBuilder = new StringBuilder();
            var firstProperty = true;
            for (var i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                    // 注释列
                    continue;

                if (dataTableProcessor.IsIdColumn(i))
                    // 编号列
                    continue;

                if (firstProperty)
                    firstProperty = false;
                else
                    stringBuilder.AppendLine().AppendLine();

                stringBuilder
                    .AppendLine("        /// <summary>")
                    .AppendFormat("        /// 获取{0}。", dataTableProcessor.GetComment(i)).AppendLine()
                    .AppendLine("        /// </summary>")
                    .AppendFormat("        public {0} {1}", dataTableProcessor.GetLanguageKeyword(i),
                        dataTableProcessor.GetName(i)).AppendLine()
                    .AppendLine("        {")
                    .AppendLine("            get;")
                    .AppendLine("            private set;")
                    .Append("        }");
            }

            return stringBuilder.ToString();
        }

        private static string GenerateDataTableParser(DataTableProcessor dataTableProcessor)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder
                .AppendLine("        public override bool ParseDataRow(string dataRowString, object userData)")
                .AppendLine("        {")
                .AppendLine(
                    "            string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);")
                .AppendLine("            for (int i = 0; i < columnStrings.Length; i++)")
                .AppendLine("            {")
                .AppendLine(
                    "                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);")
                .AppendLine("            }")
                .AppendLine()
                .AppendLine("            int index = 0;");

            for (var i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                {
                    // 注释列
                    stringBuilder.AppendLine("            index++;");
                    continue;
                }

                if (dataTableProcessor.IsIdColumn(i))
                {
                    // 编号列
                    stringBuilder.AppendLine("            m_Id = int.Parse(columnStrings[index++]);");
                    continue;
                }

                if (dataTableProcessor.IsSystem(i))
                {
                    var languageKeyword = dataTableProcessor.GetLanguageKeyword(i);
                    if (languageKeyword == "string")
                        stringBuilder.AppendFormat("            {0} = columnStrings[index++];",
                            dataTableProcessor.GetName(i)).AppendLine();
                    else
                        stringBuilder.AppendFormat("            {0} = {1}.Parse(columnStrings[index++]);",
                            dataTableProcessor.GetName(i), languageKeyword).AppendLine();
                }
                else
                {
                    stringBuilder.AppendFormat("            {0} = DataTableExtension.Parse{1}(columnStrings[index++]);",
                        dataTableProcessor.GetName(i), dataTableProcessor.GetType(i).Name).AppendLine();
                }
            }

            stringBuilder.AppendLine()
                .AppendLine("            GeneratePropertyArray();")
                .AppendLine("            return true;")
                .AppendLine("        }")
                .AppendLine()
                .AppendLine(
                    "        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)")
                .AppendLine("        {")
                .AppendLine(
                    "            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))")
                .AppendLine("            {")
                .AppendLine(
                    "                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))")
                .AppendLine("                {");

            for (var i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                    // 注释列
                    continue;

                if (dataTableProcessor.IsIdColumn(i))
                {
                    // 编号列
                    stringBuilder.AppendLine("                    m_Id = binaryReader.Read7BitEncodedInt32();");
                    continue;
                }

                var languageKeyword = dataTableProcessor.GetLanguageKeyword(i);
                if (languageKeyword == "int" || languageKeyword == "uint" || languageKeyword == "long" ||
                    languageKeyword == "ulong")
                    stringBuilder.AppendFormat("                    {0} = binaryReader.Read7BitEncoded{1}();",
                        dataTableProcessor.GetName(i), dataTableProcessor.GetType(i).Name).AppendLine();
                else
                    stringBuilder.AppendFormat("                    {0} = binaryReader.Read{1}();",
                        dataTableProcessor.GetName(i), dataTableProcessor.GetType(i).Name).AppendLine();
            }

            stringBuilder
                .AppendLine("                }")
                .AppendLine("            }")
                .AppendLine()
                .AppendLine("            GeneratePropertyArray();")
                .AppendLine("            return true;")
                .Append("        }");

            return stringBuilder.ToString();
        }

        private static string GenerateDataTablePropertyArray(DataTableProcessor dataTableProcessor)
        {
            var propertyCollections = new List<PropertyCollection>();
            for (var i = 0; i < dataTableProcessor.RawColumnCount; i++)
            {
                if (dataTableProcessor.IsCommentColumn(i))
                    // 注释列
                    continue;

                if (dataTableProcessor.IsIdColumn(i))
                    // 编号列
                    continue;

                var name = dataTableProcessor.GetName(i);
                if (!EndWithNumberRegex.IsMatch(name)) continue;

                var propertyCollectionName = EndWithNumberRegex.Replace(name, string.Empty);
                var id = int.Parse(EndWithNumberRegex.Match(name).Value);

                PropertyCollection propertyCollection = null;
                foreach (var pc in propertyCollections)
                    if (pc.Name == propertyCollectionName)
                    {
                        propertyCollection = pc;
                        break;
                    }

                if (propertyCollection == null)
                {
                    propertyCollection =
                        new PropertyCollection(propertyCollectionName, dataTableProcessor.GetLanguageKeyword(i));
                    propertyCollections.Add(propertyCollection);
                }

                propertyCollection.AddItem(id, name);
            }

            var stringBuilder = new StringBuilder();
            var firstProperty = true;
            foreach (var propertyCollection in propertyCollections)
            {
                if (firstProperty)
                    firstProperty = false;
                else
                    stringBuilder.AppendLine().AppendLine();

                stringBuilder
                    .AppendFormat("        private KeyValuePair<int, {1}>[] m_{0} = null;", propertyCollection.Name,
                        propertyCollection.LanguageKeyword).AppendLine()
                    .AppendLine()
                    .AppendFormat("        public int {0}Count", propertyCollection.Name).AppendLine()
                    .AppendLine("        {")
                    .AppendLine("            get")
                    .AppendLine("            {")
                    .AppendFormat("                return m_{0}.Length;", propertyCollection.Name).AppendLine()
                    .AppendLine("            }")
                    .AppendLine("        }")
                    .AppendLine()
                    .AppendFormat("        public {1} Get{0}(int id)", propertyCollection.Name,
                        propertyCollection.LanguageKeyword).AppendLine()
                    .AppendLine("        {")
                    .AppendFormat("            foreach (KeyValuePair<int, {1}> i in m_{0})", propertyCollection.Name,
                        propertyCollection.LanguageKeyword).AppendLine()
                    .AppendLine("            {")
                    .AppendLine("                if (i.Key == id)")
                    .AppendLine("                {")
                    .AppendLine("                    return i.Value;")
                    .AppendLine("                }")
                    .AppendLine("            }")
                    .AppendLine()
                    .AppendFormat(
                        "            throw new GameFrameworkException(Utility.Text.Format(\"Get{0} with invalid id '{{0}}'.\", id));",
                        propertyCollection.Name).AppendLine()
                    .AppendLine("        }")
                    .AppendLine()
                    .AppendFormat("        public {1} Get{0}At(int index)", propertyCollection.Name,
                        propertyCollection.LanguageKeyword).AppendLine()
                    .AppendLine("        {")
                    .AppendFormat("            if (index < 0 || index >= m_{0}.Length)", propertyCollection.Name)
                    .AppendLine()
                    .AppendLine("            {")
                    .AppendFormat(
                        "                throw new GameFrameworkException(Utility.Text.Format(\"Get{0}At with invalid index '{{0}}'.\", index));",
                        propertyCollection.Name).AppendLine()
                    .AppendLine("            }")
                    .AppendLine()
                    .AppendFormat("            return m_{0}[index].Value;", propertyCollection.Name).AppendLine()
                    .Append("        }");
            }

            if (propertyCollections.Count > 0) stringBuilder.AppendLine().AppendLine();

            stringBuilder
                .AppendLine("        private void GeneratePropertyArray()")
                .AppendLine("        {");

            firstProperty = true;
            foreach (var propertyCollection in propertyCollections)
            {
                if (firstProperty)
                    firstProperty = false;
                else
                    stringBuilder.AppendLine().AppendLine();

                stringBuilder
                    .AppendFormat("            m_{0} = new KeyValuePair<int, {1}>[]", propertyCollection.Name,
                        propertyCollection.LanguageKeyword).AppendLine()
                    .AppendLine("            {");

                var itemCount = propertyCollection.ItemCount;
                for (var i = 0; i < itemCount; i++)
                {
                    var item = propertyCollection.GetItem(i);
                    stringBuilder.AppendFormat("                new KeyValuePair<int, {0}>({1}, {2}),",
                        propertyCollection.LanguageKeyword, item.Key.ToString(), item.Value).AppendLine();
                }

                stringBuilder.Append("            };");
            }

            stringBuilder
                .AppendLine()
                .Append("        }");

            return stringBuilder.ToString();
        }

        private sealed class PropertyCollection
        {
            private readonly List<KeyValuePair<int, string>> m_Items;

            public PropertyCollection(string name, string languageKeyword)
            {
                Name = name;
                LanguageKeyword = languageKeyword;
                m_Items = new List<KeyValuePair<int, string>>();
            }

            public string Name { get; }

            public string LanguageKeyword { get; }

            public int ItemCount => m_Items.Count;

            public KeyValuePair<int, string> GetItem(int index)
            {
                if (index < 0 || index >= m_Items.Count)
                    throw new GameFrameworkException(
                        GameFramework.Utility.Text.Format("GetItem with invalid index '{0}'.", index));

                return m_Items[index];
            }

            public void AddItem(int id, string propertyName)
            {
                m_Items.Add(new KeyValuePair<int, string>(id, propertyName));
            }
        }
    }
}