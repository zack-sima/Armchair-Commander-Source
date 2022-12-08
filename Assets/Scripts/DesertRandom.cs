using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertRandom : MonoBehaviour
{
    SpriteRenderer[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer i in sprites) {
            if (i == GetComponent<SpriteRenderer>())
                continue;
            if (Random.Range(0, 3) == 1)
                i.enabled = false;
        }
    }

}
