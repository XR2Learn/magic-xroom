using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ResetCubes : MonoBehaviour
{
    public GameObject position;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
