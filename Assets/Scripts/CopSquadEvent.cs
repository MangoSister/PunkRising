using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CopSquadEvent : PathEvent
{
    public int _copNum = 3;
    public float _copSpace = 1f;
    public float _speed;
    public float _attackTimerOffset = 0f;
    public float _attackInteval = 2f;
    private List<CopBehavior> _cops;

    private static PathManager _path
    { get { return LevelController.Instance._path; } }

    private CopBehavior _copPrefab
    { get { return PrefabContainer.Instance._copPrefab; } }

    private PathSurfaceWalker _walker;

    public static CopSquadEvent CreateNewEvent(int triggerNodeIdx, float triggerTngOffset,
        int startNodeIdx, float startTngOffset, float startBinrmOffset)
    {      
        if (_path == null)
            throw new UnityException("Cannot find path");
        GameObject obj = new GameObject();
        obj.name = "CopSquad";

        CopSquadEvent squadEvent = obj.AddComponent<CopSquadEvent>();
        squadEvent._triggerNodeIdx = triggerNodeIdx;
        squadEvent._triggerTngOffset = triggerTngOffset;
        squadEvent._startNodeIdx = startNodeIdx;
        squadEvent._startTngOffset = startTngOffset;
        squadEvent._startBinrmOffset = startBinrmOffset;

        obj.transform.position = Bezier.GetPoint3d(_path.Nodes[startNodeIdx]._ctrlPts, startTngOffset) +
                      Bezier.GetBinormal3d(_path.Nodes[startNodeIdx]._ctrlPts, startTngOffset, _path.transform.up) * startBinrmOffset;
        return squadEvent;
    }

    public override void StartEvent()
    {
        LevelController.Instance._legionCtrl.SlowDown();
        //attach walker
        //suble bug about rotation- -
        _walker = PathSurfaceWalker.AttachPathSurfaceWalker(gameObject, _path,
                                    _startNodeIdx, _startTngOffset, _startBinrmOffset, 0f, _speed);
        _walker.WalkFinish += OnWalkFinish;
        _walker.MoveOnPathSurface(_startNodeIdx, _startTngOffset, 0f);

        Vector3 tng = Bezier.GetTangent3d(_path.Nodes[_startNodeIdx]._ctrlPts,
                _startTngOffset);

        Vector3 binrm = Bezier.GetBinormal3d(_path.Nodes[_startNodeIdx]._ctrlPts,
                        _startTngOffset, transform.up);

        //instantiate cop squad
        _cops = new List<CopBehavior>();
        for (int i = 0; i < _copNum; i++)
        {
            CopBehavior cop = Instantiate(_copPrefab, transform.position, transform.rotation) as CopBehavior;

            cop.transform.parent = transform;
            cop.transform.position += binrm * _copSpace *
                (i > 0 ? ((float)((i - 1) / 2) + 1f) : 0f) * (i % 2 == 0 ? 1f : -1f);
            //cop.transform.position += binrm * _copSpace * i;
            _cops.Add(cop);
        }

        SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
                        Play(UnityEngine.Random.Range(11, 14), transform, AudioType.GameSFX);

        _start = true;
    }

    private void OnWalkFinish(object sender, EventArgs e)
    {
        foreach (var cop in _cops)
            cop._target = LevelController.Instance._heroObj;
        StartCoroutine(AttackCoroutine());
        StartCoroutine(CheckEndCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(_attackTimerOffset);
        while (true)
        {
            var heroFollower = LevelController.Instance._legionCtrl._hero.CurrMoveController as PathFollower;
            Vector3 tanget, binrm, nrm;
            Bezier.GetTangentSpaceAxes3d(
                _path.Nodes[heroFollower._currNodeIdx]._ctrlPts,
                heroFollower._currTngOffset, Vector3.up, out tanget, out binrm, out nrm);

            foreach (var cop in _cops)
                if (cop != null)
                {
                    cop.Shoot(Vector3.up * 0.5f, Vector3.up * 0.5f +
                        UnityEngine.Random.Range(-3f, 3f) * binrm + UnityEngine.Random.Range(-1f, 1f) * nrm);
                }

            yield return new WaitForSeconds(_attackInteval);
        }
    }

    private IEnumerator CheckEndCoroutine()
    {
        while (true)
        {
            bool exist = false;
            foreach (var cop in _cops)
                if (cop != null)
                    exist = true;
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
        Destroy(gameObject);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //        EndEvent();
    //}

}
