using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CopMotorcadeEvent : PathEvent
{
    public float _spawnTngOffset = 3f;
    public int _copNum = 3;
    public float _copSpace = 1f;
    public float _speed;
    public float _attackTimerOffset = 5f;
    public float _attackInteval = 7f;
    private List<CopBehavior> _cops;

    private static PathManager _path
    { get { return LevelController.Instance._path; } }

    private GameObject _copMotorcadePrefab
    {  get { return PrefabContainer.Instance._copMotorcadePrefab; } }

    private CopBehavior _copPrefab
    { get { return PrefabContainer.Instance._copPrefab; } }

    private PathSurfaceWalker _walker;

    public static CopMotorcadeEvent CreateNewEvent(int triggerNodeIdx, float triggerTngOffset,
        int startNodeIdx, float startTngOffset, float startBinrmOffset)
    {
        PathManager path = LevelController.Instance._path;
        if (path == null)
            throw new UnityException("Cannot find path");
        GameObject obj = new GameObject();
        obj.name = "CopMotorcade";

        CopMotorcadeEvent motorcadeEvent = obj.AddComponent<CopMotorcadeEvent>();
        motorcadeEvent._triggerNodeIdx = triggerNodeIdx;
        motorcadeEvent._triggerTngOffset = triggerTngOffset;
        motorcadeEvent._startNodeIdx = startNodeIdx;
        motorcadeEvent._startTngOffset = startTngOffset;
        motorcadeEvent._startBinrmOffset = startBinrmOffset;

        obj.transform.position = Bezier.GetPoint3d(path.Nodes[startNodeIdx]._ctrlPts, startTngOffset) +
                      Bezier.GetBinormal3d(path.Nodes[startNodeIdx]._ctrlPts, startTngOffset, path.transform.up) * startBinrmOffset;

        return motorcadeEvent;
    }

    public override void StartEvent()
    {
        LevelController.Instance._legionCtrl.SlowDown();
        //instantiate cop motorcade
        GameObject motorcade = Instantiate(_copMotorcadePrefab, transform.position, transform.rotation) as GameObject;
        motorcade.transform.parent = transform;

        PathManager path = LevelController.Instance._path;
        _walker = PathSurfaceWalker.AttachPathSurfaceWalker(gameObject, path,
            _startNodeIdx, _startTngOffset, _startBinrmOffset, 0f, _speed);
        _walker.WalkFinish += OnWalkFinish;
        _walker.MoveOnPathSurface(_startNodeIdx, _startTngOffset, 0f);

        SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
                   Play(15, transform, AudioType.GameSFX);

        SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
           Play(UnityEngine.Random.Range(11, 14), transform, AudioType.GameSFX);

        _start = true;
    }

    private void OnWalkFinish(object sender, EventArgs e)
    {
        //begin attack and other bullshit

        //spawn cops
        Vector3 tng = Bezier.GetTangent3d(LevelController.Instance._path.Nodes[_walker.CurrNodeIdx]._ctrlPts,
                        _walker.CurrTngOffset);
        Vector3 binrm = Bezier.GetBinormal3d(LevelController.Instance._path.Nodes[_walker.CurrNodeIdx]._ctrlPts,
                        _walker.CurrTngOffset, transform.up);

        _cops = new List<CopBehavior>();
        for (int i = 0; i < _copNum; i++)
        {
            CopBehavior cop = Instantiate(_copPrefab, transform.position, transform.rotation) as CopBehavior;

            cop.transform.parent = transform;
            cop.transform.position -= tng * _spawnTngOffset;
            cop.transform.position += binrm * _copSpace * 
                (i > 0 ? ((float)((i - 1) / 2) + 1f) : 0f) * (i % 2 == 0 ? 1f : -1f);
            cop._target = LevelController.Instance._heroObj;
            _cops.Add(cop);
        }
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
            {
                if (cop != null)
                {
                    cop.Shoot(Vector3.up * 0.5f, Vector3.up * 0.5f +
                        UnityEngine.Random.Range(-3f, 3f) * binrm + UnityEngine.Random.Range(-1f, 1f) * nrm);
                }
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
        //destroy fx?
        Destroy(gameObject);
    }

}
