//------------------------------------------------------------
// DRScene数据表
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2021-06-20 16:40:18.497
//------------------------------------------------------------

using System.IO;
using System.Text;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.DataTable
{
    /// <summary>
    ///     鍦烘櫙閰嶇疆琛?。
    /// </summary>
    public class DRScene : DataRowBase
    {
        private int m_Id;

        /// <summary>
        ///     获取鍦烘櫙缂栧彿。
        /// </summary>
        public override int Id => m_Id;

        /// <summary>
        ///     获取璧勬簮鍚嶇О。
        /// </summary>
        public string AssetName { get; private set; }

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