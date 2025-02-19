using System;
using R3;
using UnityEngine;

namespace Project.Scripts.MapRenderer
{
    public class MapChunk : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;

        public readonly ReactiveProperty<bool> IsVisible = new();
        
        private Texture2D _bordersTexture;
        private MaterialPropertyBlock _block;
        
        private void Start()
        {
            IsVisible.Value = _renderer.isVisible;
        }

        private void OnBecameVisible()
        {
            IsVisible.Value = true;
        }

        private void OnBecameInvisible()
        {
            IsVisible.Value = false;
            Destroy(_bordersTexture);
        }

        public void SetBorders(Texture2D texture)
        {
            Destroy(_bordersTexture);
            _bordersTexture = texture;
            _block ??= new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_block);
            _block.SetTexture("_BorderTexture", texture);
            _renderer.SetPropertyBlock(_block);
        }


        public void SetTilingAndOffset(Vector4 tilingOffset)
        {
            _block ??= new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_block);
            _block.SetVector("_Tilling", tilingOffset);
            _renderer.SetPropertyBlock(_block);
        }
    }
}