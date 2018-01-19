using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

    public bool rotating;

    public Vector3 rotationAxis;
    public float rotSpeed;


	void FixedUpdate()
    {
        if (rotating)
        {
            transform.Rotate(rotationAxis * Time.deltaTime * rotSpeed);
        }
    }

}
