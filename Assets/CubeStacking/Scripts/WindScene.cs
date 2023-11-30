using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindScene : MonoBehaviour
{
    public List<GameObject> stackingCubes;
    public float strength;
    public Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Apply wind to the stacking cubes.
        foreach (GameObject cube in stackingCubes)
        {
            cube.GetComponent<Rigidbody>().AddForce(direction * strength);
        }
    }
}