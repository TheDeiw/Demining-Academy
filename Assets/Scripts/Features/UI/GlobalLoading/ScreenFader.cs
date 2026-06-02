using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace DeminingAcademy.Features.UI.GlobalLoading
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeDuration = 0.5f;

        private Canvas _canvas;
        private Transform _originalParent;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            // Для VR обов'язково World Space
            if (_canvas != null)
                _canvas.renderMode = RenderMode.WorldSpace;

            SetAlpha(0f);
            
            _originalParent = transform.parent;
        }

        public void AttachToCamera(Camera camera)
        {
            if (camera == null)
            {
                Debug.LogWarning("[ScreenFader] Camera.main is null");
                return;
            }

            transform.SetParent(camera.transform, false);

            // Розміщуємо одразу за near clip plane
            float distance = camera.nearClipPlane + 0.01f;
            transform.localPosition = new Vector3(0f, 0f, distance);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            // Розраховуємо розмір canvas щоб точно перекрити весь FOV
            float halfFov = camera.fieldOfView * Mathf.Deg2Rad * 0.5f;
            float height = 2f * distance * Mathf.Tan(halfFov);
            float width = height * camera.aspect;

            var rect = GetComponent<RectTransform>();
            if (rect != null)
                rect.sizeDelta = new Vector2(width, height);

            if (_canvas != null)
                _canvas.worldCamera = camera;
        }
        
        
        public void Detach()
        {
            transform.SetParent(_originalParent, false);
        }

        public async UniTask FadeOutAsync() => await AnimateAlpha(0f, 1f);
        public async UniTask FadeInAsync()  => await AnimateAlpha(1f, 0f);

        private async UniTask AnimateAlpha(float from, float to)
        {
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
            Color c = _fadeImage.color;
            c.a = alpha;
            _fadeImage.color = c;
        }
    }
}