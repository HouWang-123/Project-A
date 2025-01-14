//------------------------------------------------------------
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：__DATA_TABLE_CREATE_TIME__
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

[System.Reflection.Obfuscation(Feature = "renaming", ApplyToMembers = false)]
/// <summary>
/// 关卡表
/// </summary>
public class LevelTable : DataRowBase
{
	private int m_Id = 0;
	/// <summary>
    /// 
    /// </summary>
    public override int Id
    {
        get { return m_Id; }
    }

        /// <summary>
        /// 关卡prefab名，代表几乘几
        /// </summary>
        public string LvPfbName
        {
            get;
            private set;
        }

        /// <summary>
        /// 难度，1~5代表简单到专家，0代表没有难度选项
        /// </summary>
        public int Difficulty
        {
            get;
            private set;
        }

        /// <summary>
        /// 棋盘大小
        /// </summary>
        public int BoardSize
        {
            get;
            private set;
        }

        /// <summary>
        /// 棋子分布，n*n，用逗号隔开,每一行之间有回车
        /// </summary>
        public string BoardList
        {
            get;
            private set;
        }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
            LvPfbName = columnStrings[index++];
            Difficulty = int.Parse(columnStrings[index++]);
            BoardSize = int.Parse(columnStrings[index++]);
            BoardList = columnStrings[index++];
            Answer = columnStrings[index++];

            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    LvPfbName = binaryReader.ReadString();
                    Difficulty = binaryReader.Read7BitEncodedInt32();
                    BoardSize = binaryReader.Read7BitEncodedInt32();
                    BoardList = binaryReader.ReadString();
                    Answer = binaryReader.ReadString();
                }
            }

            return true;
        }

//__DATA_TABLE_PROPERTY_ARRAY__
}
