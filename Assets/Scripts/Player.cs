using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static Player instance = null;

    private void Awake()
    {
        instance = this;
    }
}
