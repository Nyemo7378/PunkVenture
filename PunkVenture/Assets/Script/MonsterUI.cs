using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterUI : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_cover;
    [SerializeField] SpriteRenderer m_gauge;
    [SerializeField] SpriteRenderer m_back;
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
        m_gauge.SetPropertyBlock(m_block);
    }
    public void SetUIRenderOrder(int order)
    {
        m_cover.sortingOrder = order;
        m_gauge.sortingOrder = order;
        m_back.sortingOrder = order - 1;
    }
}
