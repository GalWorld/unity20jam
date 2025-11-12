using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class RespawnPlayer : MonoBehaviour
    {
        [Tooltip("The Y position threshold at which the player will respawn.")]
        public float yThreshold = -5f;

        private Vector3 _startingPosition;

        private Quaternion _startingRotation;

        private CharacterController _characterController;

        public CinemachineCamera vCam;

        private ThirdPersonController _thirdPersonController;
        public AudioClip respawnSound;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private SceneManagerController sceneManagerController;


        private void Start()
        {
            // Save the starting position and rotation
            _startingPosition = transform.position;
            _startingRotation = transform.rotation;
            sceneManagerController= FindFirstObjectByType<SceneManagerController>();

            // Get the CharacterController reference
            _characterController = GetComponent<CharacterController>();
            if (_characterController == null)
            {
                Debug.LogError("CharacterController component is required for RespawnPlayer script!");
            }

            // Get ThirdPersonController reference
            _thirdPersonController = GetComponent<ThirdPersonController>();
            if (_thirdPersonController == null)
            {
                Debug.LogError("ThirdPersonController component is required for RespawnPlayer!");
            }


        }

        private void Update()
        {
            // Check if the player's Y position has fallen below the threshold
            if (transform.position.y < yThreshold)
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            // Disable the CharacterController so we can manually adjust position
            if (_characterController != null)
            {
                _characterController.enabled = false; // Disable to reset position/rotation correctly
            }

            
            // Defer the rest for 2 seconds (unaffected by Time.timeScale)
            StartCoroutine(RespawnAfterDelay());
        }

        private IEnumerator RespawnAfterDelay()
        {
           
            _gameManager.EndGame();
            // If EndGame() pauses the game (Time.timeScale = 0), use realtime wait
            yield return new WaitForSecondsRealtime(2f);

            // Reset the player's position and rotation
            transform.position = _startingPosition;
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            // Re-enable CharacterController and clear vertical velocity
            if (_characterController != null)
            {
                _characterController.enabled = true;
                ResetVerticalVelocity();
            }

            // Reset the camera's rotation
            var thirdPersonController = GetComponent<ThirdPersonController>();
            if (thirdPersonController != null)
                thirdPersonController.ResetCameraRotation(90f);

            // Play SFX
                //AudioSource.PlayClipAtPoint(respawnSound, transform.position);
    
                
        }

        private void ResetVerticalVelocity()
        {
            // Ensures no residual vertical velocity after respawning
            if (TryGetComponent<ThirdPersonController>(out ThirdPersonController controller))
            {
                // Access the private _verticalVelocity via the public interface, if exposed
                var verticalVelocityField = typeof(ThirdPersonController).GetField("_verticalVelocity",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (verticalVelocityField != null)
                {
                    verticalVelocityField.SetValue(controller, 0f);
                }
            }
        }
    }
}
