using System;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace Puzzle
{
    public class Branches: MonoBehaviour
    {
        public void Sort()
        {
            var branchCount = transform.childCount;
            var height = 1.1f * branchCount;

            int index = 0;
            foreach (Transform child in transform)
            {
                child.localPosition = new Vector3(0, index * 1.1f - height / 2, 0);
                index++;
            }
        }

        public void AddNewBranch(Branch branch)
        {
            var firstChildPos = transform.GetChild(0).transform.position - new Vector3(0, 0.55f, 0);
            foreach (Transform child in transform)
            {
                child.transform.DOLocalMove(child.transform.localPosition + new Vector3(0, 0.55f, 0), 0.25f);
            }

            DOVirtual.DelayedCall(0.25f, () =>
            {
                branch.transform.parent = transform;
                branch.transform.position = firstChildPos;
                branch.transform.SetAsFirstSibling();
                branch.gameObject.SetActive(true);
            });
        }
    }
}