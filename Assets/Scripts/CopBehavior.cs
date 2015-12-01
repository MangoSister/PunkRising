using UnityEngine;
using System.Collections;

public class CopBehavior : MonoBehaviour
{
    public GameObject _target;
    private CopAnimHandler _animHandler
    { get { return GetComponent<CopAnimHandler>(); } }
    // Use this for initialization

	// Update is called once per frame
	private void Update ()
    {
        if (_target != null)
            transform.LookAt(_target.transform);
    }

    public void Shoot(Vector3 fromOffset,Vector3 toOffset)
    {
        _animHandler.SetShootAnim();
        var bullet = BulletFire.LaunchBulletFire(gameObject.transform.position + fromOffset, 
            _target.transform.position + toOffset) as BulletFire;
        bullet.transform.parent = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        //should be only bullet
        BulletFire bullet = other.gameObject.GetComponent<BulletFire>();
        if (bullet == null)
            //throw new UnityException("unexpected colliding object");
            return;
        if (bullet.Reflected)
        {
            Debug.Log("ahhhh");
            //hit, maybe fx
            Destroy(bullet.gameObject);
            StartCoroutine(DeathCoroutine());
        }
    }

    private IEnumerator DeathCoroutine()
    {
        SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
                   Play(Random.Range(7, 9), transform.position, AudioType.GameSFX);

        _animHandler.TriggerDeathAnim();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}
