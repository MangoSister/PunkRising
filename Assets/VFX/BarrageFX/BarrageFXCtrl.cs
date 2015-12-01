using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarrageFXCtrl : MonoBehaviour
{
    public TrailRenderer _beforeTrailRenderer;
    public TrailRenderer _afterTrailRenderer;

    private void Start()
    {
        _beforeTrailRenderer.gameObject.SetActive(true);
        _afterTrailRenderer.gameObject.SetActive(false);
    }

    public void SwitchColor()
    {
        _beforeTrailRenderer.gameObject.SetActive(false);
        _afterTrailRenderer.gameObject.SetActive(true);
        _afterTrailRenderer.material.mainTexture = VFXManager.Instance.RandomRiotTex();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            SwitchColor();
    }
}
