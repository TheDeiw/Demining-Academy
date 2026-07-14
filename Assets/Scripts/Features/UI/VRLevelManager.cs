using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using DeminingAcademy.Infrastructure.SceneManagement;

namespace DeminingAcademy.Features.UI
{
    public class VRMenuController : MonoBehaviour
    {
        [Header("UI Settings")]
        [Tooltip("Панель меню (Canvas)")]
        [SerializeField] private GameObject _menuPanel;

        [Header("Input Settings")]
        [Tooltip("Кнопка для виклику меню (наприклад, Menu або PrimaryButton)")]
        [SerializeField] private InputActionProperty _toggleMenuAction;

        [Header("Positioning Settings")]
        [Tooltip("Камера гравця (Main Camera)")]
        [SerializeField] private Transform _headTransform;

        [Tooltip("Відстань від гравця до меню")]
        [SerializeField] private float _distanceFromPlayer = 2.0f;

        [Tooltip("Фіксована висота меню відносно підлоги (Tracking Origin)")]
        [SerializeField] private float _menuHeight = 1.2f;

        [Tooltip("Чи повинно меню плавно плисти за поглядом гравця?")]
        [SerializeField] private bool _smoothFollow = true;

        [Tooltip("Швидкість слідування меню за поглядом")]
        [SerializeField] private float _followSpeed = 5.0f;

        private ISceneLoaderService _sceneLoader;

        [Inject]
        public void Construct(ISceneLoaderService sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void Start()
        {
            if (_menuPanel != null)
                _menuPanel.SetActive(false);
        }

        private void Update()
        {
            // Перевірка натискання кнопки
            if (_toggleMenuAction.action != null && _toggleMenuAction.action.WasPressedThisFrame())
            {
                ToggleMenu();
            }

            // Якщо меню активне і увімкнене плавне слідування – оновлюємо позицію
            if (_menuPanel != null && _menuPanel.activeSelf && _smoothFollow && _headTransform != null)
            {
                UpdateMenuPosition(instant: false);
            }
        }

        public void ToggleMenu()
        {
            if (_menuPanel == null) return;

            bool isActive = !_menuPanel.activeSelf;
            _menuPanel.SetActive(isActive);

            // Коли відкриваємо меню, воно має миттєво з'явитися перед гравцем
            if (isActive && _headTransform != null)
            {
                UpdateMenuPosition(instant: true);
            }
        }

        private void UpdateMenuPosition(bool instant)
        {
            // 1. Отримуємо напрямок погляду, але ігноруємо нахили голови вгору/вниз
            Vector3 flattenedForward = _headTransform.forward;
            flattenedForward.y = 0;
            flattenedForward.Normalize();

            // 2. Розраховуємо цільову позицію (тільки в площині XZ) + фіксована висота Y
            Vector3 targetPosition = _headTransform.position + (flattenedForward * _distanceFromPlayer);
            targetPosition.y = _headTransform.position.y;

            // 3. Меню має дивитися на гравця. Розворот робимо вздовж нашого згладженого вектору
            Quaternion targetRotation = Quaternion.LookRotation(flattenedForward);

            if (instant)
            {
                // Миттєве переміщення (при першому відкритті)
                _menuPanel.transform.position = targetPosition;
                _menuPanel.transform.rotation = targetRotation;
            }
            else
            {
                // Плавне слідування (коли гравець крутить головою з відкритим меню)
                _menuPanel.transform.position = Vector3.Lerp(_menuPanel.transform.position, targetPosition, Time.deltaTime * _followSpeed);
                _menuPanel.transform.rotation = Quaternion.Slerp(_menuPanel.transform.rotation, targetRotation, Time.deltaTime * _followSpeed);
            }
        }

        // Універсальний метод для завантаження будь-якої сцени
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;

            Debug.Log($"[VRMenuController] Завантаження сцени: {sceneName}");
            _sceneLoader.LoadSceneAsync(sceneName);
        }

        public void QuitGame()
        {
            Debug.Log("[VRMenuController] Вихід з гри...");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}