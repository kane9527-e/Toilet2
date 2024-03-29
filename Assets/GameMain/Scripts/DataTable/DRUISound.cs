﻿//------------------------------------------------------------
// DRUISound数据表
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2021-06-20 16:40:18.505
//------------------------------------------------------------

using System.IO;
using System.Text;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.DataTable
{
    /// <summary>
    ///     声音配置表。
    /// </summary>
    public class DRUISound : DataRowBase
    {
        private int m_Id;

        /// <summary>
        ///     获取声音编号。
        /// </summary>
        public override int Id => m_Id;

        /// <summary>
        ///     获取资源名称。
        /// </summary>
        public string AssetName { get; private set; }

        /// <summary>
        ///     获取优先级（默认0，128最高，-128最低）。
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        ///     获取音量（0~1）。
        /// </summary>
        public float Volume { get; private set; }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            var columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
            for (var i = 0; i < columnStrings.Length; i++)
                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);

            var index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
            AssetName = columnStrings[index++];
            Priority = int.Parse(columnStrings[index++]);
            Volume = float.Parse(columnStrings[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (var memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (var binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    AssetName = binaryReader.ReadString();
                    Priority = binaryReader.Read7BitEncodedInt32();
                    Volume = binaryReader.ReadSingle();
                }
            }

            GeneratePropertyArray();
            return true;
        }

        private void GeneratePropertyArray()
        {
        }
    }
}