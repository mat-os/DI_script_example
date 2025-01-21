using UnityEngine;

namespace Game.Scripts.UI.Widgets
{
    public class PrivacyPolicy : MonoBehaviour
    {
        public string PrivacyLink;

        /*private void Awake()
    {
        if (PlayerPrefs.GetInt("GDPR_KEY") == 1)
        {
            gameObject.SetActive(false);
        }
    }*/

        public void AcceptPrivatePolicy()
        {
            SetGdprKey(1);
            gameObject.SetActive(false);
        }

        private void SetGdprKey(int value)
        {
            PlayerPrefs.SetInt("GDPR_KEY", value);
        }

        public void OpenPrivatePolicyLink()
        {
            Application.OpenURL(PrivacyLink);
        }
    }
}