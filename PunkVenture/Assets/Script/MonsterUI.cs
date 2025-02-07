using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterUI : MonoBehaviour
{
    private SpriteRenderer m_renderer;
    private MaterialPropertyBlock m_block;
    public Texture m_tex;
    public float m_value;
    private void Awake()
    {
        m_block = new MaterialPropertyBlock();
        m_renderer = GetComponent<SpriteRenderer>();
        m_tex = m_renderer.sharedMaterial.mainTexture;
        m_block.SetFloat("_CutOff", 0);
        m_block.SetTexture("_MainTex", m_tex);
        m_renderer.SetPropertyBlock(m_block);
    }

    public void UpdateUI(float cutOffValue)
    {
        m_block.SetFloat("_CutOff", cutOffValue);
        m_renderer.SetPropertyBlock(m_block);
    }
}
