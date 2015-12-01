using UnityEngine;
using System.Collections;

public interface MoveController
{
    float Speed { get; set; }
    void Init(out Vector3 initPos, out Quaternion initRot);
    bool Step(out Vector3 nextPos, out Quaternion nextRot);
    
}