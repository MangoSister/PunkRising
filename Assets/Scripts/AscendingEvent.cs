using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class AscendingEvent : PathEvent
{
    public FinalAscendingController _ascendingCtrl;
    public float _thresholdHeight = 15f;
    public float _shaftAnimLength = 2f;
    public float _maxShaftIntensity = 3f;
    public float _fadeTime = 1f;

    public float _maxSpeed;
    public float _acc;
    private static LegionMarchController legion
    {
        get
        {
            return LevelController.Instance._legionCtrl;
        }
    }

    public static AscendingEvent CreateNewEvent(int triggerNodeIdx, float triggerTngOffset)
    {
        GameObject obj = new GameObject();
        obj.name = "FinalAscendingEvent";

        AscendingEvent ascendingEvent = obj.AddComponent<AscendingEvent>();
        ascendingEvent._ascendingCtrl = new FinalAscendingController();
        ascendingEvent._triggerNodeIdx = triggerNodeIdx;
        ascendingEvent._triggerTngOffset = triggerTngOffset;
        ascendingEvent._startNodeIdx = triggerNodeIdx;
        ascendingEvent._startTngOffset = triggerTngOffset;
        ascendingEvent._startBinrmOffset = 0f;
        return ascendingEvent;
    }

    public override void StartEvent()
    {
        LevelController.Instance._uiCanvas.gameObject.SetActive(false);
        var currFollower = (legion._hero.CurrMoveController as PathFollower);
        _ascendingCtrl._forwardAxis = Bezier.GetTangent3d
            (LevelController.Instance._path.Nodes[currFollower._currNodeIdx]._ctrlPts,
            currFollower._currTngOffset);
        _ascendingCtrl._up = Vector3.up;
        _ascendingCtrl._initPos = legion._hero.transform.position;
        _ascendingCtrl.Speed = legion._hero.CurrMoveController.Speed;

        legion._hero.ChangeController(_ascendingCtrl);
        legion._crowd._enableMoveCtrl = false;
        _start = true;

        StartCoroutine(SpeedModifyCoroutine());
        StartCoroutine(CheckEndCoroutine());
    }

    private IEnumerator SpeedModifyCoroutine()
    {
        float startTime = Time.time;
        float currTime = startTime;
        float totalTime = (_maxSpeed - legion._hero.CurrMoveController.Speed) / _acc;
        while (currTime - startTime < totalTime)
        {
            legion._hero.CurrMoveController.Speed += _acc * Time.deltaTime;
            currTime = Time.time;
            yield return null;
        }
    }

    private IEnumerator CheckEndCoroutine()
    {
        while(legion._hero.gameObject.transform.position.y < _thresholdHeight)
            yield return null;
        EndEvent();
    }

    public override void EndEvent()
    {
        _end = true;
        StartCoroutine(EndCoroutine());
        
    }

    private IEnumerator EndCoroutine()
    {
        StartCoroutine(LightShaftCoroutine());
        yield return new WaitForSeconds(_shaftAnimLength);
        SceneManager.Instance.GetComponent<ScreenFader>().fadeTime = _fadeTime;
        SoundManagerSingletonWrapper.Instance.
            GetComponent<SoundManager>().StopMusic(_fadeTime);
        SceneManager.Instance.TransitScene(SceneManager.SceneType.End);
        //LevelController.Instance._legionCtrl._hero._enableMoveCtrl = false;
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator LightShaftCoroutine()
    {
        var lightShaft = Camera.main.GetComponent<SunShafts>();
        lightShaft.enabled = true;
        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < _shaftAnimLength)
        {
            lightShaft.sunShaftIntensity = Mathf.Lerp(0f, _maxShaftIntensity,
                Mathf.Clamp01((currTime - startTime) / _shaftAnimLength));
            currTime = Time.time;
            yield return null;
        }
    }

}
