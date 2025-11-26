using UnityEngine;

public class DungeonCamera : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;

    private Bounds currentRoomBounds;

    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        // tamaño de la cámara
        Camera cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        if (!player)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void LateUpdate()
    {
        if (!player) return;

        // Posición deseada sin clamp
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, transform.position.z);

        // Aplicar restricciones de la room
        if (currentRoomBounds.size != Vector3.zero)
        {
            float minX = currentRoomBounds.min.x + camHalfWidth;
            float maxX = currentRoomBounds.max.x - camHalfWidth;

            float minY = currentRoomBounds.min.y + camHalfHeight;
            float maxY = currentRoomBounds.max.y - camHalfHeight;

            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        // suavizado
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }

    public void SetCurrentRoom(Bounds b)
    {
        currentRoomBounds = b;
    }
}
