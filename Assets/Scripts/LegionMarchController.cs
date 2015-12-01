using UnityEngine;
using System.Collections;

public class LegionMarchController : MonoBehaviour
{
    public MoveAgent _hero;
    public MoveAgent _crowd;
    public float _speed;
    public float _marchStartOffset;
    public float _crowdOffset;

    public float _speedTransitionTime;
    private bool _speedTransition = false;

    private PathFollower _heroPathFollower;
    private PathFollower _crowdPathFollower;

    private static PathManager _path { get { return LevelController.Instance._path; } }
    private static PathEventManager _eventManager
    { get { return LevelController.Instance._eventManger; } }

    public void Init()
    {
        //Debug.Assert(_marchStartOffset - _crowdOffset > 0f);

        _heroPathFollower = new PathFollower(_path);
        _heroPathFollower.Speed = _speed;
        _heroPathFollower._offset = Vector3.up * 0.5f;
        _heroPathFollower._FollowerMove += _eventManager.OnFollowerMove;
        _heroPathFollower._currNodeIdx = 0;
        _heroPathFollower._currTngOffset = _marchStartOffset;

        _crowdPathFollower = new PathFollower(_path);
        _crowdPathFollower.Speed = _speed;
        _crowdPathFollower._currNodeIdx = 0;
        _crowdPathFollower._currTngOffset = _marchStartOffset - _crowdOffset;
    }

    public void StartControlHero()
    {
        _hero.ChangeController(_heroPathFollower);
        _hero._enableMoveCtrl = true;
    }

    public void StartControlCrowd()
    {
        _crowd.ChangeController(_crowdPathFollower);
        _crowd._enableMoveCtrl = true;
    }

    public void SlowDown()
    {
        if (!_speedTransition)
            StartCoroutine(SlowDownCoroutine());
    }

    private IEnumerator SlowDownCoroutine()
    {
        _speedTransition = true;
        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < _speedTransitionTime)
        {
            float currSpeed = Mathf.Lerp(_speed, 0, Mathf.Clamp01((currTime - startTime) / _speedTransitionTime));
            _heroPathFollower.Speed = currSpeed;
            _crowdPathFollower.Speed = currSpeed;
            currTime = Time.time;
            yield return null;
        }
        _speedTransition = false;
    }

    public void MoveOn()
    {
        if(!_speedTransition)
            StartCoroutine(MoveOnCoroutine());
    }

    private IEnumerator MoveOnCoroutine()
    {
        _speedTransition = true;
        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < _speedTransitionTime)
        {
            float currSpeed = Mathf.Lerp(0, _speed, Mathf.Clamp01((currTime - startTime) / _speedTransitionTime));
            _heroPathFollower.Speed = currSpeed;
            _crowdPathFollower.Speed = currSpeed;
            currTime = Time.time;
            yield return null;
        }
        _speedTransition = false;
    }
}
