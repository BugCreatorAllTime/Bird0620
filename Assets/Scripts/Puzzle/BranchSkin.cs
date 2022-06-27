using UnityEngine;

namespace Puzzle
{
    [CreateAssetMenu]
    public class BranchSkin: ScriptableObject
    {
        public Sprite icon;
        public Branch prefab;
    }
}