using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Project.Scripts.MapRenderer
{
    public class BorderRenderCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Camera _mainCamera;
        
        private RenderTexture _renderTexture;
        private Queue<RenderRequest> _renderQueue = new ();
        private bool _isBusy = false;

        private void Awake()
        {
            CreateNewRenderTexture();
        }

        public void RenderAsync(Vector2 position, Action<Texture2D> onSuccess, float worldSize = 64f)
        {
            var request = new RenderRequest()
            {
                Position = position,
                OnSuccess = onSuccess,
                WorldSize = worldSize
            };
            _renderQueue.Enqueue(request);
            
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            RenderNextRequest();
        }

        private void RenderNextRequest()
        {
            if (_renderQueue.Count == 0) return;
            
            var renderRequest = _renderQueue.Dequeue();
            
            int textureSize = 1024;
            
            this.Activate();
            transform.position = new Vector3(renderRequest.Position.x, renderRequest.Position.y, -10f);
            _camera.orthographicSize = renderRequest.WorldSize;
            _camera.targetTexture = _renderTexture;
            _camera.Render();
            
            this.Disactivate(); 
            
            AsyncGPUReadback.Request(_renderTexture, 0, TextureFormat.R8, request =>
            {
                if (request.hasError)
                {
                    Debug.LogError("GPU Readback Error!");
                    return;
                }

                Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.R8, false);
                texture.filterMode = FilterMode.Bilinear;
                texture.LoadRawTextureData(request.GetData<byte>());
                texture.Apply();
            
                renderRequest.OnSuccess?.Invoke(texture);

                _isBusy = false;
                RenderNextRequest();
            });
        }
        
        private void CreateNewRenderTexture()
        {
            _renderTexture = new RenderTexture(1024, 1024, 0);
            _renderTexture.enableRandomWrite = true;
            _renderTexture.filterMode = FilterMode.Bilinear;
            _renderTexture.format = RenderTextureFormat.R8;
            _renderTexture.depthStencilFormat = GraphicsFormat.None;
            _renderTexture.Create();
        }
    }

    public struct RenderRequest
    {
        public Vector2 Position;
        public Action<Texture2D> OnSuccess;
        public float WorldSize;
    }
}