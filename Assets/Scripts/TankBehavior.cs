using UnityEngine;
using System.Collections;

public class TankBehavior : MonoBehaviour
{
    public GameObject _topModel;
    public float _topRotateSpeed;
    public Vector2 launchPosOffset;
    private TankAnimHandler _animHandler
    { get { return GetComponent<TankAnimHandler>(); } }

    public void Fire(GameObject target)
    {
        StartCoroutine(FireCoroutine(target));
        SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
           Play(17, transform, AudioType.GameSFX);
    }

    private IEnumerator FireCoroutine(GameObject target)
    {
        //first rotate
        Vector3 targetDir = (target.transform.position - _topModel.transform.position).normalized;
        while (Vector3.Angle(targetDir, _topModel.transform.forward) > 2f)
        {
            Vector3 nextDir = Vector3.RotateTowards(_topModel.transform.forward, targetDir, Time.deltaTime * _topRotateSpeed, 0f);
            _topModel.transform.rotation = Quaternion.LookRotation(nextDir);
            yield return null;
        }
        _topModel.transform.rotation = Quaternion.LookRotation(targetDir);
        //then fire
        _animHandler.SetFireAnim();
        Cannon.LaunchCannon(transform.position + targetDir * launchPosOffset.x + transform.up * launchPosOffset.y,
            LevelController.Instance._heroObj.transform.position,
            3f, 0.5f);
        Instantiate(VFXManager.Instance._tankFiringParticleSysPrefab, transform.position + targetDir * launchPosOffset.x + transform.up * launchPosOffset.y, Quaternion.identity);

    }

    private void OnTriggerEnter(Collider other)
    {
        Cannon cannon = other.gameObject.GetComponent<Cannon>();
        if (cannon == null)
            return;
        if (cannon.Reflected)
        {
            Debug.Log("goodbye");
            Instantiate(VFXManager.Instance._tankExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(cannon.gameObject);
            Destroy(gameObject);
        }
    }
}
