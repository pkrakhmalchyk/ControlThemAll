using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AimRenderingService : ILoadingModule, IDisposable
    {
        private readonly Dictionary<string, LineRenderer> aimRenderers;
        private readonly GameObject rootGameObject;
        private Material aimMaterial;
        private AsyncOperationHandle<Material> aimMaterialHandle;


        private readonly ObjectPool<LineRenderer> lineRenderersPool;


        public AimRenderingService()
        {
            aimRenderers = new Dictionary<string, LineRenderer>();
            rootGameObject = new GameObject("AimRenderer");
            lineRenderersPool = new ObjectPool<LineRenderer>(CreateLineRenderer, null, null, DestroyLineRenderer);
        }

        public async UniTask Load()
        {
            aimMaterialHandle = Addressables.LoadAssetAsync<Material>($"{RuntimeConstants.Materials.MaterialsPath}/{RuntimeConstants.Materials.AimMaterial}");
            aimMaterial = await aimMaterialHandle;
        }

        public void AddAimRenderer(string key, AimRenderingSettings settings = null)
        {
            LineRenderer lineRenderer = lineRenderersPool.Get();
            lineRenderer.gameObject.SetActive(false);

            if (settings != null)
            {
                lineRenderer.startWidth = settings.StartWidth;
                lineRenderer.endWidth = settings.EndWidth;
            }

            if (aimRenderers.TryGetValue(key, out LineRenderer renderer))
            {
                RemoveAimRendererInternal(key, renderer);
                aimRenderers.Add(key, lineRenderer);
            }
            else
            {
                aimRenderers.Add(key, lineRenderer);
            }
        }

        public void RenderAim(string key, List<Vector3> points)
        {
            if (rootGameObject == null)
            {
                return;
            }

            if (!aimRenderers.TryGetValue(key, out LineRenderer renderer))
            {
                return;
            }

            UpdateLineRendererPoints(renderer, points);
            renderer.gameObject.SetActive(true);
        }

        public void HideAim(string key)
        {
            if (!aimRenderers.TryGetValue(key, out LineRenderer renderer))
            {
                return;
            }

            renderer.gameObject.SetActive(false);
        }

        public void RemoveAimRenderer(string key)
        {
            if (!aimRenderers.TryGetValue(key, out LineRenderer renderer))
            {
                return;
            }

            RemoveAimRendererInternal(key, renderer);
        }

        public void Dispose()
        {
            aimRenderers.Clear();
            lineRenderersPool.Dispose();
            Object.Destroy(rootGameObject);
        }


        private void RemoveAimRendererInternal(string key, LineRenderer renderer)
        {
            renderer.gameObject.SetActive(false);
            lineRenderersPool.Release(renderer);
            aimRenderers.Remove(key);
        }

        private LineRenderer CreateLineRenderer()
        {
            GameObject gameObject = new GameObject("AimRenderer");
            gameObject.transform.SetParent(rootGameObject.transform, false);
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();

            lineRenderer.material = aimMaterial;

            return lineRenderer;
        }

        private void DestroyLineRenderer(LineRenderer lineRenderer)
        {
            if (lineRenderer != null)
            {
                Object.Destroy(lineRenderer.gameObject);
            }
        }

        private void UpdateLineRendererPoints(LineRenderer lineRenderer, List<Vector3> points)
        {
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }


        public class AimRenderingSettings
        {
            public float StartWidth;
            public float EndWidth;


            public AimRenderingSettings(float startWidth, float endWidth)
            {
                StartWidth = startWidth;
                EndWidth = endWidth;
            }
        }
    }
}