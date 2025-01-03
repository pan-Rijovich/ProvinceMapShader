using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPoint : MonoBehaviour
{
    private MeshRenderer _renderer;
    private TMP_Text _label;
    
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _label = GetComponentInChildren<TMP_Text>();
    }

    public void SetColor(Color color)
    {
        _renderer.material.color = color;
    }
    
    public void SetTextColor(Color color)
    {
        _label.color = color;
    }

    public void SetText(string text)
    {
        _label.text = text;
    }
}
