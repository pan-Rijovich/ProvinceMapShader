using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.CameraController;
using MapBorderRenderer;
using MapRenderer;
using Project.Scripts.Configs;
using Project.Scripts.MapBorderRenderer;
using Project.Scripts.MapRenderer;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private Camera _borderRenderCamera;
    [SerializeField] private Material _mapMaterial;

    [Header("Configs")]
    [InlineEditor, SerializeField] private MapConfig _mapConfig;
    [InlineEditor, SerializeField] private MapBordersGeneratorConfig _mapBordersGeneratorConfig;

    private MapInput _input;
    private ReactiveProperty<int> _tst;
    private BorderBaker _borderBaker;
    
    private async void Awake()
    {
        var mapBordersGenerator = new MapBordersGenerator(_mapConfig);
        var remapBaker = new RemapBaker(_mapConfig);
        var mapShower = new MapShower(_mapConfig, _mapMaterial);
        _input = new MapInput(mapShower, _mapConfig);
        
        var cameraController = Camera.main.GetComponent<CameraController>();
        //var chunkRenderSettingsProvider = new DefaultChunkRenderSettingsProvider(cameraController);
        var chunkRenderSettingsProvider = new ResolutionTestChunkRenderSettingsProvider(cameraController);
        
        try
        {
            var borderGenHandle = mapBordersGenerator.Generate();

            await borderGenHandle;

            _borderBaker = new BorderBaker(_borderRenderCamera, chunkRenderSettingsProvider);
            var chunkSystem = new ChunkSystem(_borderBaker, _mapConfig, chunkRenderSettingsProvider);
        }
        catch (OperationCanceledException){}
    }

    private void Update()
    {
        _input.Update();
    }

    [Button]
    private void Save()
    {
        _borderBaker.Save();
    }
}