using System;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Extensions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAnimation : MonoBehaviour
{
    [SerializeField] public int FrameOffset;
    [SerializeField] public float Delay;
    [SerializeField] public float Rate;

    [SerializeField] public Sprite[] Sprites;

    private float playTime;

    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (Sprites == null || Sprites.Length == 0)
        {
            _image.enabled = false;
        }
        else
        {
            var cur = Time.timeSinceLevelLoad - playTime;
            if (cur < Delay)
            {
                _image.enabled = false;
            }
            else
            {
                _image.enabled = true;
                int idx = ((cur * Rate).FloorToInt() + FrameOffset) % Sprites.Length;
                _image.sprite = Sprites[idx];
            }
        }
    }

    private void OnEnable()
    {
        playTime = Time.timeSinceLevelLoad;
    }

    private void OnDisable()
    {
        _image.enabled = false;
    }
}