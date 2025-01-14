using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace FEVM.Data
{
    public interface IDataKeeper
    {
        void Save();
        BaseData Read();
        void Write(object o);
        BaseData FetchData();
        String GetTypeKey();
        bool Loaded();
    }


    [Serializable]
    public class BaseData
    {
        [NonSerialized]
        protected string dataKey;

        public BaseData()
        {
            dataKey = GetType().ToString();
        }

        public string GetKey()
        {
            return dataKey;
        }
    }

    /// <summary>
    /// 存储器
    /// 说明：将需要存储的数据对象存储到 Applicaiton.Persistendata下
    /// 可通过存储器本身进行数据的读取，写入，存储，也可也通过全局对象 FEVMDataManager 进行数据的管理和调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDataKeeperKeeper<T> : IDataKeeper where T : BaseData, new()
    {
        public T Data { get { return baseData; } }
        protected T baseData;
        public bool initialized;
        public List<string> floder_arr;
        public bool loaded;
        public BaseDataKeeperKeeper()
        {
            InitData();
        }
        
        /// <summary>
        /// DataKeeper 在初始化时会注册进TyDataManager中
        /// </summary>
        protected void InitData() {
            baseData = new T();
            FEVMDataManager.Instance.AddDataKeeper(this);
            initialized = true;
        }
        public BaseData FetchData() { return baseData; }
        public bool Loaded() { return loaded; }
        public void Save() { SaveData(); }

        public BaseData Read()
        {
            return ReadData(out loaded);
        }

        public String GetTypeKey()
        {
            return GetKey();
        }

        public void Write(object o)
        {
            if (!initialized) throw new DataException("数据初始化异常");
            String w_type = o.GetType().ToString();
            String m_type = baseData.GetType().ToString();
            if (w_type.Equals(m_type))
            {
                Write(o as T);
            }
            else
            {
                throw new Exception("存储类型错误！！！");
            }
        }

        protected void Write(T data)
        {
            WriteData(data);
        }

        protected string SavePath { get; private set; }

        protected string GetKey()
        {
            return baseData.GetKey();
        }

        protected T GetDataEntity()
        {
            if (!initialized) throw new DataException("数据初始化异常");
            if (baseData == null)
            {
                baseData = new T();
            }

            return baseData;
        }
        /// <summary>
        /// 进行数据读取，成功则返回数据，否则返回null
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        /// <exception cref="DataException"></exception>
        protected T ReadData(out bool success)
        {
            if (!initialized) throw new DataException("数据初始化异常");
            if (File.Exists(SavePath))
            {
                string jsonData = File.ReadAllText(SavePath);
                
                // todo 解密
                
                
                success = true;
                // 新增：如果读取成功则可通过datakeeper实例中basedata直接获取
                baseData = JsonUtility.FromJson<T>(jsonData);
                return JsonUtility.FromJson<T>(jsonData);
            }
            else
            {
                Debug.LogWarning($"Save file not found at {SavePath}");
                success = false;
                return null;
            }
        }

        protected void SaveData()
        {
            if (!initialized) throw new DataException("数据初始化异常");
            string directory = Path.GetDirectoryName(SavePath);
            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
            string jsonData = JsonUtility.ToJson(baseData);
            
            // todo 加密
            
            
            File.WriteAllText(SavePath, jsonData);
            ColorfulDebugger.Debug(Data.GetKey()+"数据保存成功,位置:"+SavePath,ColorfulDebugger.Instance.File);
        }
        protected void LoadData(Action callBack)
        {
            baseData = ReadData(out loaded);
            callBack?.Invoke();
        }
        
        /// <summary>
        /// 传入路径字符串数组，程序将自动进行数据存储路径的设置
        /// 例如 传入 List<string> = {a,b,c}
        /// 则存储路径为
        /// Application.persistentDataPath/a/b/c/T.json
        /// 备注：T为类名。
        /// </summary>
        /// <param name="path"></param>
        public virtual void setSavePath(List<String> path)
        {
            floder_arr = path;
            StringBuilder folder = new StringBuilder();
            foreach (var VARIABLE in floder_arr)
            {
                folder.Append(VARIABLE);
                folder.Append(Path.DirectorySeparatorChar);
            }
            SavePath = Path.Combine(Application.persistentDataPath, folder.ToString());
            SavePath = Path.Combine(SavePath, typeof(T).ToString().Replace('.', Path.DirectorySeparatorChar) + ".thereisnoData");
        }
        protected void WriteData(T data)
        {
            if (!initialized)
                throw new DataException("数据初始化异常");
            baseData = data;
        }
    }
}