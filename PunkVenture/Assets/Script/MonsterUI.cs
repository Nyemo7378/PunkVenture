using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_gaugeRenderer;
    [SerializeField] SpriteRenderer m_coverRenderer;
    private MaterialPropertyBlock m_block;
    private void Awake()
    {
        m_block = new MaterialPropertyBlock();
    }
    private void OnEnable()
    {
        UpdateUI(0);
    }
    public void UpdateUI(float cutOffValue)
    {
        m_block.SetFloat("_CutOff", cutOffValue);
        m_gaugeRenderer.SetPropertyBlock(m_block);
    }
    public void SetGaugeRenderOrder(int order)
    {
        m_gaugeRenderer.sortingOrder = order;
    }

    public void SetCoverRenderOrder(int order)
    {
        m_coverRenderer.sortingOrder = order;
    }
}
