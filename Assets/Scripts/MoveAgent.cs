using UnityEngine;
using System.Collections;

public class MoveAgent : MonoBehaviour
{
    private MoveController _currMoveController;
    public bool _enableMoveCtrl;
    public MoveController CurrMoveController { get { return _currMoveController; } }

    public MoveController ChangeController(MoveController ctrl)
    {
        MoveController oldCtrl = _currMoveController;
        _currMoveController = ctrl;
        Vector3 initPos;
        Quaternion initRot;
        _currMoveController.Init(out initPos, out initRot);
        transform.position = initPos;
        transform.rotation = initRot;
        return oldCtrl;
    }

    private void Update()
    {
        if (_enableMoveCtrl)
        {
            Vector3 nextPos;
            Quaternion nextRot;
            if (_currMoveController.Step(out nextPos, out nextRot))
            {
                transform.position = nextPos;
                transform.rotation = nextRot;
            }
            else _enableMoveCtrl = false;
        }
    }
}
