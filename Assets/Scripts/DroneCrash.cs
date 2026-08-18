    using UnityEngine;

    public class DroneCrash : MonoBehaviour
    {
        [Header("Effects")]
        public GameObject explosionPrefab;
        public GameObject blackScreen;
        public GameObject fpvOverlay;

        [Header("Disable On Crash")]
        public MonoBehaviour droneMovement;
        public MonoBehaviour propellerMovement;
        public MonoBehaviour audioController;
        public MonoBehaviour sparkCollisionDetection;
        public DroneCameraToggle cameraToggleScript;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip boomSound;

        [Header("Player")]
        public SimplePlayer player;

        private bool crashed = false;

        private void Start()
        {
            if (blackScreen != null)
                blackScreen.SetActive(false);

            if (fpvOverlay != null)
                fpvOverlay.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (crashed) return;
            crashed = true;

            bool hitPlayer = collision.gameObject.CompareTag("Player");

            if (hitPlayer && player != null)
                player.Die();
            else if (GameManager.instance != null)
                GameManager.instance.DroneCrashed();

            CrashNow();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (crashed) return;
            crashed = true;

            bool hitPlayer = other.CompareTag("Player");

            if (hitPlayer && player != null)
                player.Die();
            else if (GameManager.instance != null)
                GameManager.instance.DroneCrashed();

            CrashNow();
        }

        private void CrashNow()
        {
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            if (audioSource != null && boomSound != null)
                audioSource.PlayOneShot(boomSound);

            if (droneMovement != null) droneMovement.enabled = false;
            if (propellerMovement != null) propellerMovement.enabled = false;
            if (audioController != null) audioController.enabled = false;
            if (sparkCollisionDetection != null) sparkCollisionDetection.enabled = false;

            // HUD va crosshairni majburan o'chir
            if (cameraToggleScript != null)
                cameraToggleScript.ForceHideHUD();

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            if (fpvOverlay != null)
                fpvOverlay.SetActive(false);

            if (blackScreen != null)
                blackScreen.SetActive(true);
        }

        public void ResetCrashState()
        {
            crashed = false;

            if (blackScreen != null)
                blackScreen.SetActive(false);

            // Reset paytida HUD avtomatik chiqib ketmasin
            if (fpvOverlay != null)
                fpvOverlay.SetActive(false);

            if (cameraToggleScript != null)
                cameraToggleScript.ForceHideHUD();

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (droneMovement != null) droneMovement.enabled = true;
            if (propellerMovement != null) propellerMovement.enabled = true;
            if (audioController != null) audioController.enabled = true;
            if (sparkCollisionDetection != null) sparkCollisionDetection.enabled = true;
        }

        public bool IsCrashed()
        {
            return crashed;
        }
    }