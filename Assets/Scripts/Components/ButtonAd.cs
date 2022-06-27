using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class ButtonAd : MonoBehaviour
    {
        public Button button;

        [SerializeField] Animator _animator;
        protected bool _isLoading = false;
        protected static readonly int Loading = Animator.StringToHash("Loading");

        protected void OnEnable()
        {
            button.interactable = true;
            _isLoading = false;
        }

        public void SetLoading()
        {
            Debug.Log("Set Loading: " + _isLoading + " " + button.interactable);
            if (_animator != null)
            {
                _animator.SetBool(Loading, true);
            }

            _isLoading = true;

            button.interactable = false;
        }

        public void SetLoadDone()
        {
            if (gameObject.activeInHierarchy)
            {
                LoadDone();
            }
        }

        IEnumerator DelayLoadDone()
        {
            yield return new WaitForSeconds(0.25f);

            LoadDone();
        }

        public void LoadDone()
        {
            Debug.Log("Set Load Done: " + _isLoading + " " + button.interactable);

            if (!_isLoading) return;

            if (_animator != null)
            {
                _animator.SetBool(Loading, false);
            }

            if (button != null)
            {
                button.interactable = true;
            }

            _isLoading = false;
        }
    }
}