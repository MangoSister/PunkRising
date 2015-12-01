using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VFXManager : GenericSingleton<VFXManager>
{
    public InsipirationFXCtrl _inspirationFXPrefab;
    public BarrageFXCtrl _barrageFXPrefab;
    public AirDistortionCtrl _airDistortionPrefab;
    public FiringCtrl _tankFiringParticleSysPrefab;
    public GameObject _tankExplosionPrefab;

    public List<Texture2D> _RiotTex;

    public Texture2D RandomRiotTex()
    {
        return _RiotTex[Random.Range(0, _RiotTex.Count)];
    }
}
