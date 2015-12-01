using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TankEvent : PathEvent
{
    public float _speed;
    public float _attackTimerOffset = 0f;
    public float _attackInteval = 100f;
    
    private static TankBehavior _tankPrefab
    { get { return PrefabContainer.Instance._tankPrefab; } }

    private PathSurfaceWalker _walker;
    private TankBehavior _tank;

    public static TankEvent CreateNewEvent(int triggerNodeIdx, float triggerTngOffset,
        int startNodeIdx, float startTngOffset, float startBinrmOffset)
    {        
        PathManager path = LevelController.Instance._path;
        if (path == null)
            throw new UnityException("Cannot find path");
        GameObject obj = new GameObject();
        obj.name = "TankEvent";

        TankEvent tankEvent = obj.AddComponent<TankEvent>();
        tankEvent._triggerNodeIdx = triggerNodeIdx;
        tankEvent._triggerTngOffset = triggerTngOffset;
        tankEvent._startNodeIdx = startNodeIdx;
        tankEvent._startTngOffset = startTngOffset;
        tankEvent._startBinrmOffset = startBinrmOffset;

        obj.transform.position = Bezier.GetPoint3d(path.Nodes[startNodeIdx]._ctrlPts, startTngOffset) +
                      Bezier.GetBinormal3d(path.Nodes[startNodeIdx]._ctrlPts, startTngOffset, path.transform.up) * startBinrmOffset;
        return tankEvent;
    }

    public override void StartEvent()
    {
        LevelController.Instance._legionCtrl.SlowDown();
        //instantiate tank
        _tank = Instantiate(_tankPrefab, transform.position, transform.rotation) as TankBehavior;
        _tank.transform.parent = transform;

        PathManager path = LevelController.Instance._path;
        _walker = PathSurfaceWalker.AttachPathSurfaceWalker(gameObject, path,
            _startNodeIdx, _startTngOffset, _startBinrmOffset, 0f, _speed);
        _walker.WalkFinish += OnWalkFinish;
        _walker.MoveOnPathSurface(_startNodeIdx, _startTngOffset, 0f);

        SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
                       Play(16, transform, AudioType.GameSFX);

        _start = true;
    }

    private void OnWalkFinish(object sender, EventArgs e)
    {
        //rotate to target

        //attack
        StartCoroutine(AttackCoroutine());
        StartCoroutine(CheckEndCoroutine());

    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(_attackTimerOffset);
        while (true)
        {
            Debug.Log("tank fire");
            _tank.Fire(LevelController.Instance._heroObj);
            yield return new WaitForSeconds(_attackInteval);
        }
    }

    private IEnumerator CheckEndCoroutine()
    {
        while (true)
        {
            bool exist = (_tank != null);
            if (!exist)
                break;
            yield return null;
        }
        EndEvent();
    }

    public override void EndEvent()
    {
        _end = true;
        LevelController.Instance._legionCtrl.MoveOn();
        //destroy fx?
        Destroy(gameObject);
    }
}
