using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
        StartCoroutine(Flying());
    }
    IEnumerator Flying() {
        for (float i = 0f; i < 0.5f; i += Time.deltaTime) {
            yield return null;
            GetComponent<SpriteRenderer>().color = new Color(1f, 1, 1.3f, 1 - i * 2f);
            

        }
        Destroy(gameObject);

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * 3.5f);
    }
}
