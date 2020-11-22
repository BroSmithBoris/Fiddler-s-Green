using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SlowScale());
    }

    IEnumerator SlowScale()
    {
        for (float q = 1f; q < 10f; q += .1f)
        {
            transform.localScale = new Vector3(q, q, q);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }
}
