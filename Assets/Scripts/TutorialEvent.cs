using UnityEngine;
using System;
using System.Collections;

public class TutorialEvent : PathEvent
{
    public Canvas _chargingCanvas;
    public Canvas _inspirationCanvas;
    public Canvas _reflectionCanvas;
    public float _copSpeed = 3f;
    public int _copNodeIdx;
    public float _copTngOffset;
    public float _copBinrmOffset = 5f;
    public float _copAttackInterval = 2f;

    private bool _chargingFinish;
    private bool _inspirationFinish;
    private bool _reflectionFinish;

    private CopBehavior _cop;
    private PathSurfaceWalker _copWalker;

    private CopBehavior _copPrefab
    { get { return PrefabContainer.Instance._copPrefab; } }

    //private static InputManager _inputManager
    //{ get { return LevelController.Instance._heroObj.GetComponent<InputManager>(); } }

    private static Canvas _chargingCanvasPrefab
    { get { return PrefabContainer.Instance._chargingCanvasPrefab; } }

    private static Canvas _inspirationCanvasPrefab
    { get { return PrefabContainer.Instance._inspirationCanvasPrefab; } }

    private static Canvas _reflectionCanvasPrefab
    { get { return PrefabContainer.Instance._reflectionCanvasPrefab; } }

    private static PathManager _path
    { get { return LevelController.Instance._path; } }

    public static TutorialEvent CreateNewEvent(int triggerNodeIdx, float triggerTngOffset,
        int startNodeIdx, float startTngOffset)
    {
        if (_path == null)
            throw new UnityException("Cannot find path");
        GameObject obj = new GameObject();
        obj.name = "TutorialEvent";

        TutorialEvent tutorialEvent = obj.AddComponent<TutorialEvent>();
        tutorialEvent._triggerNodeIdx = triggerNodeIdx;
        tutorialEvent._triggerTngOffset = triggerTngOffset;
        tutorialEvent._startNodeIdx = startNodeIdx;
        tutorialEvent._startTngOffset = startTngOffset;
        tutorialEvent._startBinrmOffset = 0f;

        obj.transform.position = Bezier.GetPoint3d(_path.Nodes[startNodeIdx]._ctrlPts, startTngOffset);

        Vector3 tng = Bezier.GetTangent3d(_path.Nodes[startNodeIdx]._ctrlPts, startTngOffset);

        tutorialEvent._chargingCanvas = Instantiate(_chargingCanvasPrefab,
            obj.transform.position + Vector3.up * 2.5f, Quaternion.LookRotation(tng)) as Canvas;
        tutorialEvent._chargingCanvas.transform.parent = tutorialEvent.transform;
        tutorialEvent._chargingCanvas.gameObject.SetActive(false);

        tutorialEvent._inspirationCanvas = Instantiate(_inspirationCanvasPrefab,
            obj.transform.position + Vector3.up * 2f, Quaternion.LookRotation(tng)) as Canvas;
        tutorialEvent._inspirationCanvas.transform.parent = tutorialEvent.transform;
        tutorialEvent._inspirationCanvas.gameObject.SetActive(false);

        tutorialEvent._reflectionCanvas = Instantiate(_reflectionCanvasPrefab,
            obj.transform.position + Vector3.up * 2f, Quaternion.LookRotation(tng)) as Canvas;
        tutorialEvent._reflectionCanvas.transform.parent = tutorialEvent.transform;
        tutorialEvent._reflectionCanvas.gameObject.SetActive(false);

        return tutorialEvent;
    }

    public override void StartEvent()
    {
        _start = true;
        _chargingFinish = false;
        _inspirationFinish = false;
        _reflectionFinish = false;

        LevelController.Instance._legionCtrl.SlowDown();

        StartCoroutine(TutorialCoroutine());
    }

    public override void EndEvent()
    {
        LevelController.Instance._legionCtrl.MoveOn();
        Destroy(gameObject);
        _end = true;
    }

    private IEnumerator TutorialCoroutine()
    {
        //charging
        _chargingCanvas.gameObject.SetActive(true);
        _inspirationCanvas.gameObject.SetActive(false);
        _reflectionCanvas.gameObject.SetActive(false);
        StartCoroutine(CheckChargeFinish());
        while (!_chargingFinish)
            yield return null;

        //inspiration
        _chargingCanvas.gameObject.SetActive(false);
        _inspirationCanvas.gameObject.SetActive(true);
        _reflectionCanvas.gameObject.SetActive(false);
        StartCoroutine(CheckInspirationFinish());
        while (!_inspirationFinish)
            yield return null;


        //reflection
        _chargingCanvas.gameObject.SetActive(false);
        _inspirationCanvas.gameObject.SetActive(false);
        _reflectionCanvas.gameObject.SetActive(true);
        LevelController.Instance._legionCtrl._crowd.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(3f);
        _cop = Instantiate(_copPrefab, transform.position, transform.rotation) as CopBehavior;
        _cop.transform.parent = transform;
        _copWalker = PathSurfaceWalker.AttachPathSurfaceWalker(_cop.gameObject, _path,
                                    _copNodeIdx, _copTngOffset, _copBinrmOffset, 0f, _copSpeed);
        _copWalker.WalkFinish += OnWalkFinish;
        _copWalker.MoveOnPathSurface(_copNodeIdx, _copTngOffset, 0f);

        StartCoroutine(CheckReflectionFinish());
        while (!_reflectionFinish)
            yield return null;
        LevelController.Instance._legionCtrl._crowd.GetComponent<BoxCollider>().enabled = true;
        EndEvent();
    }

    private void OnWalkFinish(object sender, EventArgs e)
    {
        _cop._target = LevelController.Instance._heroObj;
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        while (_cop != null)
        {
            var heroFollower = LevelController.Instance._legionCtrl._hero.CurrMoveController as PathFollower;
            Vector3 tanget, binrm, nrm;
            Bezier.GetTangentSpaceAxes3d(
                _path.Nodes[heroFollower._currNodeIdx]._ctrlPts,
                heroFollower._currTngOffset, Vector3.up, out tanget, out binrm, out nrm);

            _cop.Shoot(Vector3.up * 0.5f, Vector3.up * 0.5f +
                UnityEngine.Random.Range(-3f, 3f) * binrm + UnityEngine.Random.Range(-1f, 1f) * nrm);

            yield return new WaitForSeconds(_copAttackInterval);
        }
    }

    private IEnumerator CheckChargeFinish()
    {
        var refill = LevelController.Instance._heroObj.GetComponent<EnergyRefill>();
        while (refill.tutorialFlag)
        {
            yield return null;
        }
        _chargingFinish = true;
    }

    private IEnumerator CheckInspirationFinish()
    {
        var crowd = LevelController.Instance._legionCtrl._crowd.gameObject.GetComponent<CrowdManager>();
        while (crowd.tutFlag)
        {
            yield return null;
        }
        _inspirationFinish = true;
    }

    private IEnumerator CheckReflectionFinish()
    {
        while (_cop != null)
        {
            yield return null;
        }
        _reflectionFinish = true;
    }
}
