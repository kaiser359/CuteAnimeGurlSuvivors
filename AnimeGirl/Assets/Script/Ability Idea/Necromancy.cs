using System.Collections.Generic;
using UnityEngine;

public class Necromancy : MonoBehaviour
{
    public static Necromancy Instance { get; private set; }

    [Tooltip("Prefab for the ally. The prefab should include the Ally component.")]
    public GameObject allyPrefab;

    [Tooltip("Maximum number of allies the player can have at once.")]
    public int maxAllies = 5;

    [Tooltip("Radius around the player where allies will spawn.")]
    public float spawnRadius = 1.2f;

    // optional default lifetime (the Ally also has its own lifetime field)
    public float allyLifetime = 30f;

    private readonly List<GameObject> _allies = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Call this when an enemy is killed (amount is number of allies to create, typically 1)
    public void CreateAllys(float amount)
    {
        if (allyPrefab == null)
        {
            Debug.LogWarning("Necromancy: allyPrefab is not assigned.");
            return;
        }

        int toSpawn = Mathf.FloorToInt(amount);
        for (int i = 0; i < toSpawn; i++)
        {
            // enforce max allies
            if (_allies.Count >= maxAllies) break;

            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);
            GameObject allyGO = Instantiate(allyPrefab, spawnPos, Quaternion.identity);

            // Configure ally so it knows its owner
            Ally allyComp = allyGO.GetComponent<Ally>();
            if (allyComp != null)
            {
                allyComp.OwnerNecromancy = this;
                // make sure ally lifetime matches necromancy default if desired
                allyComp.lifeTime = allyLifetime;
            }

            _allies.Add(allyGO);
        }
    }


    public void NotifyAllyDestroyed(GameObject ally)
    {
        if (_allies.Contains(ally))
            _allies.Remove(ally);
    }

    // Optional: debug helper to clear all allies
    public void ClearAllies()
    {
        foreach (var a in _allies.ToArray())
        {
            if (a != null) Destroy(a);
        }
        _allies.Clear();
    }
}
