using UnityEngine;

public class WorldCursor : MonoBehaviour
{
    public float followSpeed = 50f;

    private Vector3 targetPosition;

    public void SetTargetPosition(Vector3 worldPos)
    {
        targetPosition = worldPos;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
