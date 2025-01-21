using UnityEngine;

namespace Game.Scripts.Utils
{
    public class FPSMeter : MonoBehaviour
    {
        private static FPSMeter _instance;

        public static FPSMeter Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var gameObject = new GameObject($"[Singleton]{nameof(FPSMeter)}");
                DontDestroyOnLoad(gameObject);
                _instance = gameObject.AddComponent<FPSMeter>();

                return _instance;
            }
        }

        public Color FontColor = Color.red;

#if !PRODUCTION_BUILD
        private bool _fast;
#endif

        private bool _show;
        private float _deltaTime;
        private GUIStyle _guiStyle;
        private int _screenHeight;
        private float _fps;
        private Rect _rect;

        public static bool Show
        {
            set => Instance._show = value;
            get => Instance._show;
        }

        private void Awake()
        {
            _guiStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = Screen.height * 3 / 100,
                normal = { textColor = FontColor }
            };
        }

        private void Start()
        {
            _screenHeight = Screen.height;
            _rect = new Rect(30, 20, Screen.width, Screen.height * 2 / 100.0f);
        }

        private void Update()
        {
#if !PRODUCTION_BUILD
            for (var i = 0; i < Input.touchCount && Input.touchCount == 3; i++)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Ended)
                    break;

                _fast = !_fast;
            }

            if (Input.touchCount == 3)
            {
                _fast = !_fast;
            }
#endif

            for (var i = 0; i < Input.touchCount && Input.touchCount == 5; i++)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Ended)
                    break;

                _show = !_show;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F))
                _show = !_show;
#endif

            if (!_show)
                return;

            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;

            if (_screenHeight == Screen.height)
                return;

            _screenHeight = Screen.height;
            _guiStyle.fontSize = _screenHeight * 3 / 100;
            _rect = new Rect(10, 10, Screen.width, Screen.height * 2 / 100.0f);
            _rect = new Rect(100, 100, 200, Screen.height * 2 / 100.0f);
        }

        private void OnGUI()
        {
            if (!_show)
                return;

            _fps = 1.0f / _deltaTime;
            GUI.Label(_rect, ((int)_fps).ToString(), _guiStyle);
        }
    }
}