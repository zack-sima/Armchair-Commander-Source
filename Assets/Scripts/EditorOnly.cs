using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//anything with this script is destroyed if not using unity editor
public class EditorOnly : MonoBehaviour
{
    public bool disable;
    // Start is called before the first frame update
    void Awake() {
        if (!disable) {
#if !UNITY_EDITOR
Destroy(gameObject);
#endif
        }
    }

}
