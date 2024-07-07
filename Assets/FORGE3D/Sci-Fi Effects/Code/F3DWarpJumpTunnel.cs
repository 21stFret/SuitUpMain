using UnityEngine;
using System.Collections;

namespace FORGE3D
{
    public class F3DWarpJumpTunnel : MonoBehaviour
    {
        private new Transform transform;
        public MeshRenderer meshRenderer;

        public float StartDelay, FadeDelay;
        public Vector3 ScaleTo;
        public float ScaleTime;
        public float ColorTime, ColorFadeTime;
        public float RotationSpeed;

        private bool grow;

        private float alpha;

        private int alphaID;

        public bool infinite;

        private void Awake()
        {
            transform = GetComponent<Transform>();
            meshRenderer = GetComponent<MeshRenderer>();

            alphaID = Shader.PropertyToID("_Alpha");
        }

        public void OnSpawned()
        {
            grow = false;
            meshRenderer.material.SetFloat(alphaID, 0);
            F3DTime.time.AddTimer(StartDelay, 1, ToggleGrow);
            if(!infinite)
            {
                F3DTime.time.AddTimer(FadeDelay, 1, ToggleGrow);
            }
            //transform.localScale = new Vector3(1f, 1f, 1f);
            transform.localRotation = transform.localRotation * Quaternion.Euler(0, 0, Random.Range(-360, 360));
        }

        public void ToggleGrow()
        {
            grow = !grow;
        }

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
            if (grow)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, ScaleTo, Time.deltaTime * ScaleTime);

                alpha = Mathf.Lerp(alpha, 0.5f, Time.deltaTime * ColorTime);
                meshRenderer.material.SetFloat(alphaID, alpha);
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * ScaleTime);

                alpha = Mathf.Lerp(alpha, 0, Time.deltaTime * ColorFadeTime);
                meshRenderer.material.SetFloat(alphaID, alpha);
            }
        }
    }
}