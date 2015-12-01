using UnityEngine;
using System.Collections;

public class HaloSin : MonoBehaviour {

    private Light _halo;

    // Private const
    private const float k_F = 1.0f * Mathf.PI;
    private const float k_Intensity = 0.5f;
    private const float k_Range = 30.0f;

	// Use this for initialization
	void Start () {
        _halo = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        float _lightflag = Mathf.Clamp01((100.0f * (Mathf.Sin(k_F * Time.time) - 0.99f)));
        //Debug.Log(_lightflag);
        _halo.intensity = _lightflag * k_Intensity;
        _halo.range = _lightflag * k_Range;
        
    }
}
