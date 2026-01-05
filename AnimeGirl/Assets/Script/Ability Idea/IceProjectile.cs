using UnityEngine;
using UnityEngine.InputSystem;

public class IceProjectile : MonoBehaviour
{
   
    public float speed = 8f;
    public float lifetime = 6f;

    [HideInInspector] public Camera followCamera;
    [HideInInspector] public TeleportOnFireHover teleportHover;

    float _timer;

    void Start()
    {
        _timer = 0f;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetWorld;
       
        if (teleportHover != null && teleportHover.playerCamera != null)
        {
            Camera pc = teleportHover.playerCamera;
            float zDistance = Mathf.Abs(pc.transform.position.z - transform.position.z);
            Vector3 screenPoint = new Vector3(teleportHover.aimPosition.x, teleportHover.aimPosition.y, zDistance);
            targetWorld = pc.ScreenToWorldPoint(screenPoint);
            targetWorld.z = transform.position.z;
        }
        else if (followCamera != null)
        {
            float zDistance = Mathf.Abs(followCamera.transform.position.z - transform.position.z);
            Vector3 screenPoint = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), zDistance);
            targetWorld = followCamera.ScreenToWorldPoint(screenPoint);
            targetWorld.z = transform.position.z;
        }
        else
        {
            return;
        }

        Vector3 dir = targetWorld - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
        {
           //direction from curson in firre rover
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetWorld, step);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
