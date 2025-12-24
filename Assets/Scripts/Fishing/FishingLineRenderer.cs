using UnityEngine;

public class FishingLineRenderer : MonoBehaviour
{
    [SerializeField]
    private Transform fishingRodTip;
    private Transform bait;
    [SerializeField]
    private LineRenderer line;

    [Range(2, 40)]
    public int segments = 20;

    public float sagAmount = 1.5f;

    private void Update()
    {
        if (fishingRodTip == null || line == null)
            return;

        if (bait == null)
        {
            line.enabled = false;
            return;
        }

        DrawCurvedLine();
    }

    private void DrawCurvedLine()
    {
        line.enabled = true;
        line.positionCount = segments;

        Vector3 start = fishingRodTip.position;
        Vector3 end = bait.position;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

            Vector3 point = Vector3.Lerp(start, end, t);

            // tạo độ cong (parabolic sag)
            float sag = Mathf.Sin(t * Mathf.PI);
            point.y -= sag * sagAmount;            

            line.SetPosition(i, point);
        }
    }

    public void SetBait(Transform newBait)
    {
        bait = newBait;
    }
}
