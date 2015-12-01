using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathEventManager : MonoBehaviour
{
    public List<PathEvent> _events;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnFollowerMove(object sender, PathFollowerEventArg arg)
    {
        foreach (PathEvent e in _events)
        {
            if (e == null || e._end)
                continue;
            else if (e._start)
            {
                //force end
                if (arg._nodeIdx > e._triggerNodeIdx)
                    e.EndEvent();
            }
            else
            {
                if (arg._nodeIdx == e._triggerNodeIdx && arg._tngOffset > e._triggerTngOffset)
                    e.StartEvent();
            }
        }
    }

}
