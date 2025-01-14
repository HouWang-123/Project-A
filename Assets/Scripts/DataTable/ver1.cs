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
/// ver1
/// </summary>
public class ver1 : DataRowBase
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
        /// 物品名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 物品描述
        /// </summary>
        public string DESCRIBE
        {
            get;
            private set;
        }

        /// <summary>
        /// 10002
        /// </summary>
        public int InteractEffectID
        {
            get;
            private set;
        }

        /// <summary>
        /// 武器无法被攻击破坏，但这个几种物品可能会，且武器是在背包里的，而这里的物品是举起来的
        /// </summary>
        public int CanDestory
        {
            get;
            private set;
        }

        /// <summary>
        /// 耐久值
        /// </summary>
        public int Durability
        {
            get;
            private set;
        }

        /// <summary>
        /// 重量
        /// </summary>
        public int Weight
        {
            get;
            private set;
        }

        /// <summary>
        /// 攻击力
        /// </summary>
        public int Attack
        {
            get;
            private set;
        }

        /// <summary>
        /// 鼠标左键功能
        /// </summary>
        public string UseEffect
        {
            get;
            private set;
        }

        /// <summary>
        /// 落地/击中时执行
        /// </summary>
        public string FallEffect
        {
            get;
            private set;
        }

        /// <summary>
        /// 0
        /// </summary>
        public int AttackCD
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
            Name = columnStrings[index++];
            DESCRIBE = columnStrings[index++];
            InteractEffectID = int.Parse(columnStrings[index++]);
            CanDestory = int.Parse(columnStrings[index++]);
            Durability = int.Parse(columnStrings[index++]);
            Weight = int.Parse(columnStrings[index++]);
            Attack = int.Parse(columnStrings[index++]);
            UseEffect = columnStrings[index++];
            FallEffect = columnStrings[index++];
            AttackCD = int.Parse(columnStrings[index++]);

            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    Name = binaryReader.ReadString();
                    DESCRIBE = binaryReader.ReadString();
                    InteractEffectID = binaryReader.Read7BitEncodedInt32();
                    CanDestory = binaryReader.Read7BitEncodedInt32();
                    Durability = binaryReader.Read7BitEncodedInt32();
                    Weight = binaryReader.Read7BitEncodedInt32();
                    Attack = binaryReader.Read7BitEncodedInt32();
                    UseEffect = binaryReader.ReadString();
                    FallEffect = binaryReader.ReadString();
                    AttackCD = binaryReader.Read7BitEncodedInt32();
                }
            }

            return true;
        }

//__DATA_TABLE_PROPERTY_ARRAY__
}
