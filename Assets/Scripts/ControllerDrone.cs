    using UnityEngine;

    public class DroneController : MonoBehaviour
    {
        private bool controlEnabled = false;

        public void SetControlEnabled(bool value)
        {
            controlEnabled = value;
        }

        void Update()
        {
            if (!controlEnabled) return;

            // dron harakati shu yerda bo'ladi
        }
    }