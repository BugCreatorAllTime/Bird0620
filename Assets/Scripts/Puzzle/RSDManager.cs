using System;
using System.Collections.Generic;
using System.Text;
using Lib.RapidSheetData;
using UnityEngine;

namespace Puzzle
{
    public class RSDManager
    {
        private static RSDManager _instance;

        public static RSDManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RSDManager();
                }

                return _instance;
            }
        }

        public RSDAsset DataAsset = null;

        private List<LevelDOB> _levelDB;

        public RSDManager()
        {
            ImportGameDataAsset();
        }

        void ImportGameDataAsset()
        {
            DataAsset = Resources.Load<RSDAsset>(DATA_RESOURCES.RSDASSET);

            DataAsset.Init(null, new RSDSerializerDefaultLit(new RSDConverterAOT()));
            
            _levelDB = DataAsset.GetSheet<LevelDOB>("Level");
        }

        public void ValidateData()
        {
            for (int i = 0; i < _levelDB.Count; i++)
            {
                var data = GetLevelData(i + 1);
                    
                var result = ValidateData(data);
                if (!result)
                {
                    StringBuilder sb = new StringBuilder();
                    var separator = " ";
                    foreach (var ints in data)
                    {
                        sb.Append("[");
                        sb.Append(string.Join(",", ints));
                        sb.Append("]");
                        sb.Append(separator);
                    }
                    Debug.LogWarning($"Level {i + 1} contain invalid data: {sb}");
                }
            }
        }

        public void PullData(Action<bool> onCompleted)
        {
            DataAsset.PullDataCacheAndDeserialize(onCompleted);
        }
        
        public List<List<int>> GetLevelData(int level)
        {
            var levelData = LevelData(level);
            var data = levelData.Data();
            return data;
        }
        
        private LevelDOB LevelData(int level)
        {
            foreach (var dob in _levelDB)
            {
                if (dob.Lv == level)
                {
                    return dob;
                }
            }

            return null;
        }

        private bool ValidateData(List<List<int>> data)
        {
            Dictionary<int, int> counts = new Dictionary<int, int>();

            foreach (var ints in data)
            {
                if (ints.Count == 0) continue;
                foreach (var i in ints)
                {
                    if (counts.ContainsKey(i))
                    {
                        counts[i]++;
                    }
                    else
                    {
                        counts[i] = 1;
                    }
                }
            }

            bool flag = true;
            foreach (var keyValuePair in counts)
            {
                if (keyValuePair.Value != 4)
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }
    }
}