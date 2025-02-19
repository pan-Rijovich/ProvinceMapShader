using System;
using System.Collections;
using Assets.Scripts;
using MapBorderRenderer;
using MapRenderer;
using Project.Scripts.Configs;
using Project.Scripts.MapRenderer;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private BorderRenderCamera _borderRenderCamera;
    [SerializeField] private Material _mapMaterial;

    [Header("Configs")]
    [InlineEditor, SerializeField] private MapConfig _mapConfig;
    [InlineEditor, SerializeField] private MapBordersGeneratorConfig _mapBordersGeneratorConfig;

    private MapInput _input;
    private ReactiveProperty<int> _tst;
    
    private async void Awake()
    {
        var mapBordersGenerator = new MapBordersGenerator(_mapConfig);
        var remapBaker = new RemapBaker(_mapConfig);
        var mapShower = new MapShower(_mapConfig, _mapMaterial);
        _input = new MapInput(mapShower, _mapConfig);
        
        try
        {
            var borderGenHandle = mapBordersGenerator.Generate();

            await borderGenHandle;
            
            var chunkSystem = new ChunkSystem(_borderRenderCamera, _mapConfig);
        }
        catch (OperationCanceledException){}
    }

    private void Update()
    {
        _input.Update();
    }
}