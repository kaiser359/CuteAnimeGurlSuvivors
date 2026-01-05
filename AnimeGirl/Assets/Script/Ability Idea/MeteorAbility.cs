using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class MeteorAbility : MonoBehaviour
{
    public GameObject meteorPrefab;     
    public LayerMask groundLayers;        
    public KeyCode throwKey = KeyCode.Mouse1; 
    public Camera mainCamera;
    public TextMeshProUGUI cooldownText;

    private PlayerStats stats;

    [SerializeField]  private float cooldownTimer = 0f;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void Update()
    {
        
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownText.text = "Meteor CD: " + Mathf.Ceil(cooldownTimer).ToString();
        }
        else
        {
            cooldownText.text = "Meteor Ready!";
        }
    }


    public void Meteor(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            return;

        if (cooldownTimer <= 0f && meteorPrefab != null)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(GetComponent<TeleportOnFireHover>().aimPosition);
            mouseWorldPos.z = 0f;   // IMPORTANT
            Vector2 targetPos = mouseWorldPos;


            if (groundLayers != 0)
            {
                Collider2D col = Physics2D.OverlapPoint(targetPos, groundLayers);

            }

            SpawnMeteor(targetPos);
            cooldownTimer = stats.meteorCooldown;
        }
    }

    private void SpawnMeteor(Vector2 targetPosition)
    {
        GameObject go = Instantiate(meteorPrefab, (Vector3)targetPosition + Vector3.up * 10f, Quaternion.identity);
        
        MeteorAOE aoe = go.GetComponent<MeteorAOE>();
        if (aoe != null)
        {
            aoe.Initialize(targetPosition, stats.meteorDamage, stats.meteorRadius);
        }
        else
        {
            Debug.LogWarning("MeteorAbility: meteorPrefab missing MeteorAOE component.");
        }
    }

    
    public bool TryTriggerMeteorAt(Vector2 worldPos)
    {
        if (cooldownTimer <= 0f && meteorPrefab != null)
        {
            if (CameraShake.Instance != null)
                CameraShake.Instance.Shake(0.2f, 0.25f);

            SpawnMeteor(worldPos);
            cooldownTimer = stats.meteorCooldown;
            return true;
        }
        return false;
    }

   
    public float GetCooldownRemaining() => cooldownTimer;
}
