using UnityEngine;

public class MaterialPropBlock : MonoBehaviour
{
    private Renderer m_renderer;
    private MaterialPropertyBlock m_propBlock;

    public Texture m_tex;
    public float m_value;

    void Start()
    {
        m_renderer = GetComponent<Renderer>();
        m_propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        m_propBlock.SetTexture("_MainTex", m_tex);
        m_propBlock.SetFloat("_Speed", m_value);
        m_renderer.SetPropertyBlock(m_propBlock);
    }
}
