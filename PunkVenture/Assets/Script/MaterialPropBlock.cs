using UnityEngine;

public class MaterialPropBlock : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    public Texture customTex;
    public float customValue;

    void Start()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        propBlock.SetTexture("_MainTex", customTex);
        propBlock.SetFloat("_Speed", customValue);
        rend.SetPropertyBlock(propBlock);
    }
}
