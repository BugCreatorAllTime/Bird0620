using TMPro;
using UnityEngine;

namespace Components
{
    public class ButtonItem: ButtonAd
    {
        public TextMeshProUGUI itemCount;
        public GameObject adIcon;

        public void ChangeToAdType()
        {
            itemCount.gameObject.SetActive(false);
            adIcon.gameObject.SetActive(true);
        }

        public void ChangeToFreeType()
        {
            itemCount.gameObject.SetActive(true);
            adIcon.gameObject.SetActive(false);
        }
    }
}