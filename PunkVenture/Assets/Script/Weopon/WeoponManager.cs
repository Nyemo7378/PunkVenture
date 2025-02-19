using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeoponManager : MonoBehaviour
{
    Dictionary<string,GameObject> m_weoponList;
    GameObject m_curWeopon = null;
    void Start()
    {
        m_weoponList = new Dictionary<string, GameObject>();
        Transform group = transform.Find("WeoponGroup");
        foreach (Transform child in group)
        {
            GameObject weopon = child.gameObject;
            m_weoponList.Add(weopon.name, weopon);
        }
        SetWeopon("Staff");
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void SetWeopon(string name)
    {
        if(false == m_weoponList.ContainsKey(name))
            Debug.DebugBreak();

        if(m_curWeopon != null)
        {
            m_curWeopon.SetActive(false);
        }

        m_curWeopon = m_weoponList[name];
        m_curWeopon.SetActive(true);
    }
}
