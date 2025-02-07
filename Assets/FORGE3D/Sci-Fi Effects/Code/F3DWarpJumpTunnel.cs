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
            grow = true;
            meshRenderer.material.SetFloat(alphaID, 0);
            transform.localScale = Vector3.zero;
            transform.localRotation = transform.localRotation * Quaternion.Euler(0, 0, Random.Range(-360, 360));
        }

        public void ToggleGrow(bool value)
        {
            grow = value;
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