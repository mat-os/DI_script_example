using DG.Tweening;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Widgets
{
    public class ToggleSwitch : MonoBehaviour
    {
        public Transform leftPosition;
        public GameObject leftPositionText;
        public Transform rightPosition;
        public GameObject rightPositionText;
        public float moveDuration = 0.2f;

        public RectTransform rectTransform;
        public Image ButtonImage;
        public Sprite ActiveSprite;
        public Sprite InactiveSprite;
        
        [Header("Icon Img")]
        public Image IconActiveSprite;
        public Image IconInactiveSprite;
        
        [Header("Bg")]
        public Image BgButtonImage;
        public Sprite BgActiveSprite;
        public Sprite BgInactiveSprite;
        
        private bool _isOn = false;

        public void Init(bool isSwitchActive)
        {
            CustomDebugLog.Log("is Toggle Switch Active " + isSwitchActive);
            _isOn = isSwitchActive;
            ChangeButtonState(isSwitchActive, 0);
        }
        public void OnClick()
        {
            _isOn = !_isOn;
            ChangeButtonState(_isOn, moveDuration);
        }

        private void ChangeButtonState(bool isOnState, float moveDur)
        {
            var pos = isOnState ?  rightPosition.position : leftPosition.position;
            rectTransform.DOMove(pos, moveDur);
            leftPositionText.gameObject.SetActive(isOnState);
            rightPositionText.gameObject.SetActive(!isOnState);
            
            ButtonImage.sprite = isOnState ? ActiveSprite : InactiveSprite;
            
            BgButtonImage.sprite = isOnState ? BgActiveSprite : BgInactiveSprite;
            
            IconActiveSprite.gameObject.SetActive(isOnState); 
            IconInactiveSprite.gameObject.SetActive(!isOnState); 
            
            
        }
    }
}