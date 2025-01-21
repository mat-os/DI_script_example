using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.UI.Widgets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Initialization
{
    public class Bootstrap : MonoBehaviour
    {
        /*[SerializeField] private Image _background;
        [SerializeField] private Image _logo;*/
        [SerializeField] private AppCore _appCorePrefab;
        [SerializeField] private PrivacyPolicy _privacyPolicyPrefab;
        [SerializeField] private int _targetFramerate = 60;

        private PrivacyPolicy _privacyPolicy;
        private async void Start()
        {
            await UniTask.WaitWhile(() => !UnityEngine.Rendering.SplashScreen.isFinished);

            if (PlayerPrefs.GetInt("GDPR_KEY") != 1)
            {
                _privacyPolicy = Instantiate(_privacyPolicyPrefab, transform);
            }
            await UniTask.WaitUntil(() => PlayerPrefs.GetInt("GDPR_KEY") == 1);

            if(_privacyPolicy != null)
                Destroy(_privacyPolicy);

            AppCore appCore = Instantiate(_appCorePrefab, Vector3.zero, Quaternion.identity);
            appCore.InitializeGame();

            //await UniTask.WaitWhile(() => !SDK.IsInitialized);
            
            await SceneManager.LoadSceneAsync(AppConst.GameProd, LoadSceneMode.Single);

            Application.targetFrameRate = _targetFramerate;

            appCore.EnterGame();
        }
    }
}