using UnityEngine;
using System.Collections;

public class BulletFire : MonoBehaviour
{
    public const int layerNum = 21;
    public const float selfDestroyWaitTime = 5f;
    private BarrageFXCtrl _fx;
    public static BulletFire LaunchBulletFire(Vector3 from, Vector3 to)
    {
        var bullet = Instantiate(PrefabContainer.Instance._bulletFirePrefab,
            from, Quaternion.identity) as BulletFire;
        bullet._from = from;
        bullet._to = to;

        bullet._fx = Instantiate
            (VFXManager.Instance._barrageFXPrefab, 
            bullet.transform.position, bullet.transform.rotation) 
            as BarrageFXCtrl;

        bullet._fx.transform.parent = bullet.transform;

        SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
                    Play(9, bullet.transform.position, AudioType.GameSFX);

        return bullet;
    }

    public Vector3 _from;
    public Vector3 _to;

    private Vector3 _currDir;

    //public bool _track;
    public float _hitDist = 0.1f;
    public float _speed = 5f;
    private bool _reflected = false;
    public bool Reflected { get { return _reflected; } }

    private void UpdateDir()
    {
        if (_reflected)
            _currDir = (_from - transform.position).normalized;
        else
            _currDir = (_to - transform.position).normalized;
    }

    private void Start()
    {
        StartCoroutine(FireCoroutine());
    }

    private IEnumerator FireCoroutine()
    {
        Vector3 target = _reflected ? _from : _to;
        while (Vector3.Distance(transform.position, target) > _hitDist)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);
            target = _reflected ? _from : _to;
            yield return null;
        }

        transform.position = target;
        //self destory ?
        yield return new WaitForSeconds(selfDestroyWaitTime);
        if (gameObject != null)
            Destroy(gameObject);
        //while (true)
        //{
        //    Vector3 targetPos = Vector3.zero;
        //    UpdateDir();
        //    if (GetTargetPos(ref targetPos))
        //    {
        //        if (Vector3.Distance(targetPos, transform.position) > _hitDist)
        //            transform.position += _currDir * _speed * Time.deltaTime;
        //        else
        //        {
        //            transform.position = targetPos;
        //            break;
        //        }
        //    }
        //    else
        //    {
        //        transform.position += _currDir * _speed * Time.deltaTime;
        //        if (transform.position.sqrMagnitude > maxBound * maxBound)
        //            Destroy(gameObject);
        //    }
        //    yield return null;
        //}
    }

    //reflection will be ignored if from has been already destroyed
    public void Reflect()
    {
        if (gameObject != null)
        {
            _reflected = true;
            _fx.SwitchColor();
            SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
            Play(10, transform.position, AudioType.GameSFX);
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //        Reflect();
    //}
}
