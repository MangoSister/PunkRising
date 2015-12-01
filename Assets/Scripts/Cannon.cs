using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{
    public static Cannon LaunchCannon(Vector3 from, Vector3 to,
        float startJudgeDist, float judgeRange)
    {
        var cannon = Instantiate(PrefabContainer.Instance._cannonPrefab,
            from, Quaternion.LookRotation((to - from).normalized, Vector3.up)) as Cannon;
        cannon._from = from;
        cannon._to = to;
        cannon._startJudgeDist = startJudgeDist;
        cannon._judgeRange = judgeRange;
        cannon._reflected = false;

        AirDistortionCtrl fx = Instantiate(VFXManager.Instance._airDistortionPrefab,
            cannon.transform.position, cannon.transform.rotation) as AirDistortionCtrl;
        fx.transform.parent = cannon.transform;
        fx._size = cannon._fxSize;
        //fx.transform.rotation = fx.transform.parent.rotation;

        SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
           Play(17, cannon.transform.position, AudioType.GameSFX);

        return cannon;
    }
    public float _slowMotionScale = 0.02f;
    public float _normalSpeed = 20f;
    public float _judgeMaxSpeed = 2f;

    public float _startJudgeDist;
    public float _judgeRange;

    public float _hitDist = 0.1f;
    private bool _reflected = false;
    public bool Reflected { get { return _reflected; } }

    public Vector3 _from;
    public Vector3 _to;

    public float _fxSize = 10f;

    private static InputManager _inputManager
    { get { return LevelController.Instance._heroObj.GetComponent<InputManager>(); } }

    private void TriggerSlowMotion(bool enable)
    {
        if (enable)
        {
            Time.timeScale = _slowMotionScale;
            SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().StartSlowMotionSFX();
            SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().
                       Play(18, transform, AudioType.SlowMotionSFX);
        }
        else
        {
            Time.timeScale = 1f;
            SoundManagerSingletonWrapper.Instance.GetComponent<EmitterSoundManager>().EndSlowMotionSFX();
        }
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    
    private void Start()
    {
        StartCoroutine(InteractionCoroutine());
    }

    private IEnumerator InteractionCoroutine()
    {
        //move toward
        while (Vector3.Distance(transform.position, _to) > _startJudgeDist)
        {
            transform.position = Vector3.MoveTowards(transform.position, _to, _normalSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = _to + (_from - _to).normalized * _startJudgeDist;
        TriggerSlowMotion(true);
        _inputManager.tankFlag = true;

        //judges
        Vector3 _nearThreshold = _to + (_from - _to).normalized * (_startJudgeDist - _judgeRange);
        Vector3 _farThreshold = _to +(_from - _to).normalized * (_startJudgeDist + _judgeRange);
        while (true)
        {
            float percent = _inputManager.threshold / 250f;
            transform.position = Vector3.Lerp(_nearThreshold, _farThreshold, 1f - percent);

            if (percent >= 0.99f)
            {
                _reflected = false;
                break;
            }
            else if (percent <= 0.01f)
            {
                _reflected = true;
                break;
            }

            yield return null;
        }

        //after judge
        TriggerSlowMotion(false);
        _inputManager.tankFlag = false;
        Vector3 finalTarget = _reflected ? _from : _to;
        while (Vector3.Distance(transform.position, finalTarget) > _hitDist)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalTarget, _normalSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = finalTarget;
    }
}
