﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator3 : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 4f;

    float movementFactor; // 0 for not moved, 1 for fully moved.
    Vector3 startingPos;

	// Use this for initialization
	void Start () {
        startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        
        if (period <= Mathf.Epsilon)
            return;

        float cycles = Time.time / period; // grows continually from 0

        const float tau = Mathf.PI; // about 6.28
        float rawSinWave = Mathf.Cos(cycles * tau); // goes from -1 to +1

        movementFactor = rawSinWave / 2f + 0.4f;

        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPos + offset;
	}
}
