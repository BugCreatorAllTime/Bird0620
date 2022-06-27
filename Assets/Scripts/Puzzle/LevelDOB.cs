using System.Collections.Generic;
using Lib.RapidSheetData;

namespace Puzzle
{
    [RSDObject]
    public class LevelDOB
    {
        public int Lv { get; set; }
        public List<int> B1 { get; set; }
        public List<int> B2 { get; set; }
        public List<int> B3 { get; set; }
        public List<int> B4 { get; set; }
        public List<int> B5 { get; set; }
        public List<int> B6 { get; set; }
        public List<int> B7 { get; set; }
        public List<int> B8 { get; set; }
        public List<int> B9 { get; set; }
        public List<int> B10 { get; set; }
        public List<int> B11 { get; set; }
        public List<int> B12 { get; set; }
        
        public List<List<int>> Data()
        {
            var data = new List<List<int>>();

            if (B1 != null) { data.Add(B1); }
            if (B2 != null) { data.Add(B2); }
            if (B3 != null) { data.Add(B3); }
            if (B4 != null) { data.Add(B4); }
            if (B5 != null) { data.Add(B5); }
            if (B6 != null) { data.Add(B6); }
            if (B7 != null) { data.Add(B7); }
            if (B8 != null) { data.Add(B8); }
            if (B9 != null) { data.Add(B9); }
            if (B10 != null) { data.Add(B10); }
            if (B11 != null) { data.Add(B11); }
            if (B12 != null) { data.Add(B12); }

            return data;
        }
    }
}