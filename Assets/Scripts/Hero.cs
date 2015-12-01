using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
    public bool _turnOnOculus;
    public GameObject _normalCamObj;
    public GameObject _oculusCamObj;

    private void Start()
    {
        if (_turnOnOculus)
        {
            _oculusCamObj.SetActive(true);
            _normalCamObj.SetActive(false);
        }
        else
        {
            _oculusCamObj.SetActive(false);
            _normalCamObj.SetActive(true);
        }
    }
}
