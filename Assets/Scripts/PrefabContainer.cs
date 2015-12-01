using UnityEngine;
using System.Collections;

public class PrefabContainer : GenericSingleton<PrefabContainer>
{
    public GameObject _copMotorcadePrefab;
    public CopBehavior _copPrefab;
    public TankBehavior _tankPrefab;
    public BulletFire _bulletFirePrefab;
    public Cannon _cannonPrefab;

    public Canvas _chargingCanvasPrefab;
    public Canvas _inspirationCanvasPrefab;
    public Canvas _reflectionCanvasPrefab;

}
