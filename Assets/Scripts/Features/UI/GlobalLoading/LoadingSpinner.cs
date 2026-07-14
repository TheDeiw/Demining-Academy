using UnityEngine;

namespace DeminingAcademy.Features.UI.GlobalLoading
{
    public class LoadingSpinner : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = -200f;

        private void Update()
        {
            transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
        }
    }
}