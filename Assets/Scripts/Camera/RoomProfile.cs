using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomProfile : MonoBehaviour
{
private BoxCollider2D roomCollider;


void Awake()
{
    roomCollider = GetComponent<BoxCollider2D>();
    roomCollider.isTrigger = true;
}

void Start()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        if (roomCollider.OverlapPoint(player.transform.position))
        {
            SendRoomToCamera();
        }
    }
}

private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player")) 
        return;

    SendRoomToCamera();
}

private void SendRoomToCamera()
{
    if (Camera.main == null)
        return;

    DungeonCamera cam = Camera.main.GetComponent<DungeonCamera>();
    if (cam != null)
    {
        cam.SetCurrentRoom(roomCollider.bounds);
    }
}

void OnDrawGizmos()
{
    BoxCollider2D bc = GetComponent<BoxCollider2D>();
    if (bc == null) return;

    Gizmos.color = new Color(0, 1, 0, 0.15f);
    Gizmos.DrawCube(bc.bounds.center, bc.bounds.size);

    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(bc.bounds.center, bc.bounds.size);
}


}
