using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SparkLine : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField] private float noiseScale = 0.1f;   
    [SerializeField] private float noiseSpeed = 2f;     
    [SerializeField] private int pointCount = 10;       
    [SerializeField] private float rough = 0.1f; 
    private Vector3[] originalPositions;              
    private Vector3[] currentPositions;              

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCount;
        originalPositions = new Vector3[pointCount];
        currentPositions = new Vector3[pointCount];
    }

    public void SetPositions(Vector2 startPos, Vector2 endPos)
    {
        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            originalPositions[i] = Vector3.Lerp(startPos, endPos, t);
            currentPositions[i] = originalPositions[i];
        }

        lineRenderer.SetPositions(originalPositions);
    }

    void Update()
    {
        float quantizedTime = Mathf.Floor(Time.time / rough) * rough;
        for (int i = 1; i < pointCount - 1; i++)
        {
            float noiseX = Mathf.PerlinNoise(quantizedTime * noiseSpeed + i * 0.1f, 0f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(quantizedTime * noiseSpeed + i * 0.1f, 10f) - 0.5f;

            Vector3 noiseOffset = new Vector3(noiseX, noiseY, 0f) * noiseScale;
            currentPositions[i] = originalPositions[i] + noiseOffset;
        }
        lineRenderer.SetPositions(currentPositions);
    }
}