using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class BranchManager: MonoBehaviour
    {
        private static BranchManager _instance;

        public static BranchManager Instance
        {
            get { return _instance; }
        }

        public List<BranchSkin> skins;

        private void Awake()
        {
            _instance = this;
        }
        public Branch Current()
        {
            return skins[DataManager.Instance.SaveGameData.selectedBranch].prefab;
        }
    }
}