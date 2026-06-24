using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DeminingAcademy.Features.UI.GlobalLoading
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _fadeRenderer;
        [SerializeField] private GameObject _loadingSpinner;
        [SerializeField] private float _fadeDuration = 0.5f;

        private Material _fadeMaterial;
        private Transform _targetCamera;
        
        // Захист від дублювання об'єкта при перезавантаженні сцени
        private static ScreenFader _instance;
        
        private void Awake()
        {
            // Якщо об'єкт вже існує з попередньої сцени — знищуємо зайвого клона
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            // Відв'язуємо від батьківських об'єктів, інакше DontDestroyOnLoad видасть помилку
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            if (_fadeRenderer != null)
            {
                // Клонуємо матеріал, щоб не змінювати файл ассету напряму
                _fadeMaterial = _fadeRenderer.material;
            }

            SetAlpha(0f);
            ShowSpinner(false);
        }

        private void Update()
        {
            // Жорстко синхронізуємо позицію з камерою кожного кадру
            if (_targetCamera != null)
            {
                transform.position = _targetCamera.position;
                transform.rotation = _targetCamera.rotation;
            }
        }

        public void AttachToCamera(Camera camera)
        {
            if (camera == null) return;
            
            _targetCamera = camera.transform;
            transform.position = _targetCamera.position;
            transform.rotation = _targetCamera.rotation;
        }
        
        public void Detach()
        {
            _targetCamera = null;
        }

        public void ShowSpinner(bool show)
        {
            if (_loadingSpinner != null)
                _loadingSpinner.SetActive(show);
        }

        public async UniTask FadeOutAsync() => await AnimateAlpha(0f, 1f);
        public async UniTask FadeInAsync()  => await AnimateAlpha(1f, 0f);

        private async UniTask AnimateAlpha(float from, float to)
        {
            if (_fadeMaterial == null) return;

            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(Mathf.Lerp(from, to, elapsed / _fadeDuration));
                await UniTask.Yield();
            }
            SetAlpha(to);
        }

        private void SetAlpha(float alpha)
        {
            if (_fadeMaterial != null)
            {
                // Підтримка URP (використовує _BaseColor)
                if (_fadeMaterial.HasProperty("_BaseColor"))
                {
                    Color c = _fadeMaterial.GetColor("_BaseColor");
                    c.a = alpha;
                    _fadeMaterial.SetColor("_BaseColor", c);
                }
                // Підтримка стандартних шейдерів Unity
                else if (_fadeMaterial.HasProperty("_Color"))
                {
                    Color c = _fadeMaterial.color;
                    c.a = alpha;
                    _fadeMaterial.color = c;
                }
            }
        }
    }
}