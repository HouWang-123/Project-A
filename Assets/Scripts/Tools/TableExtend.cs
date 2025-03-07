using System.Collections.Generic;
using UnityEngine;


namespace cfg
{
    public static class TableExtend
    {
        #region DropRule
        public static List<int> GetID(this func.DropRule dropRule)
        {
            List<int> ids = new List<int>();
            switch(dropRule.DropType)
            {
                case 1:
                    int ID = RangeIDType1(dropRule);
                    if(ID != 0)
                    {
                        ids.Add(ID);
                    }
                    break;
                case 2:
                    RangeIDType2(dropRule, ids);
                    break;
                case 3:
                    ids.Add(RangeIDType3(dropRule));
                    break;
                default:
                    Debug.LogError("DropRole  Table 数据有误，请检查数据；DropRoleID:" + dropRule.ID);
                    break;
            }

            return ids;
        }

        private static int RangeIDType1(func.DropRule dropRule)
        {
            float value = Random.Range(0.0f, 1.0f);
            float r = 0;
            for(int i = 0; i < dropRule.DropRate.Count; i++)
            {
                r += dropRule.DropRate[i];
                if(r > value)
                {
                    return dropRule.DropIDlist[i];
                }
            }
            return 0;
        }

        private static void RangeIDType2(func.DropRule dropRule, List<int> ids)
        {
            for(int i = 0; i < dropRule.DropRate.Count; i++)
            {
                float value = Random.Range(0.0f, 1.0f);
                if(value < dropRule.DropRate[i])
                {
                    ids.Add(dropRule.DropIDlist[i]);
                }
            }
        }

        private static int RangeIDType3(func.DropRule dropRule)
        {
            int startID = dropRule.DropIDlist[0], endID = dropRule.DropIDlist[1];
            float value = Random.Range(0.0f, 1.0f);
            float r = 1f / (endID - startID + 1);
            int tmp = (int)(value / r);
            return startID + tmp;
        }

        #endregion


        public static ELanguage GameLanguage = ELanguage.CN;
        /// <summary>
        /// 获取文本
        /// </summary>
        public static string GetText(this text.Localization obj)
        {
            switch(GameLanguage)
            {
                case ELanguage.CN:
                    return obj.Zh;
                case ELanguage.EN:
                    return obj.En;
                case ELanguage.JAP:
                    return obj.Ja;
            }
            return "NULL";
        }

    }

    public enum ELanguage
    {
        CN,
        EN,
        JAP,
    }
}

