using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingCamara : MonoBehaviour
{
    public Transform target;
    public Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.transform.position.x + initPos.x, 
            target.transform.position.y + initPos.y,
            initPos.z);
    }
}
