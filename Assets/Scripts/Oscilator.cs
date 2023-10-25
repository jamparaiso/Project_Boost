using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent] //disables multiple script on object
public class Oscilator : MonoBehaviour
{
    const float tau = Mathf.PI * 2; // about 6.28

    Vector3 startingPos;

    //the amount of units the object will move from starting position in x y z coordinate
    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f); //default value | values stated on component will overide this

    //how long it will take to complete 1 cycle 
    [SerializeField] float period = 2f;

    [Range(0,1)] [SerializeField] float movementFactor;


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position; // stores start position of the object
    }

    // Update is called once per frame
    void Update()
    {
        OscillateObject();
    }

    private void OscillateObject()
    {
        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period;

        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
