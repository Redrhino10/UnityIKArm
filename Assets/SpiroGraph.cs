using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiroGraph : MonoBehaviour
{
    private float Timer = 0.0f;

    void Update()
    {
        Timer += Time.deltaTime;
        if(Timer > 0.2)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            marker.transform.position = gameObject.transform.position;
            Timer -= 0.2f;
        }
    }
}
