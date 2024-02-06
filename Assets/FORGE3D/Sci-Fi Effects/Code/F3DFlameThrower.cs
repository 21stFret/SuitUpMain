using UnityEngine;
using System.Collections;

namespace FORGE3D
{
    public class F3DFlameThrower : MonoBehaviour
    {
        public Light pLight; // Attached point light
        public ParticleSystem heat; // Heat particles

        int lightState; // Point light state flag (fading in or out)

        // OnSpawned called by pool manager 
        void OnSpawned()
        {
            lightState = 1;
            pLight.intensity = 0f;
        }

        // OnDespawned called by pool manager 
        void OnDespawned()
        {
        }

        // Despawn game object
        void OnDespawn()
        {
            F3DPoolManager.Pools["GeneratedPool"].Despawn(transform);
        }

        void Update()
        {
            // Fade in point light
            if (lightState == 1)
            {
                pLight.intensity = Mathf.Lerp(pLight.intensity, 0.7f, Time.deltaTime*10f);

                if (pLight.intensity >= 0.5f)
                    lightState = 0;
            }
            // Fade out point light
            else if (lightState == -1)
            {
                pLight.intensity = Mathf.Lerp(pLight.intensity, -0.1f, Time.deltaTime*10f);

                if (pLight.intensity <= 0f)
                    lightState = 0;
            }
        }
    }
}