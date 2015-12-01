using UnityEngine;
using System.Collections;

public class KeyboardMove : MonoBehaviour
{
    public float speed = 1f;
	void Update ()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }
}