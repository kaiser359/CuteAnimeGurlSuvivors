using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Necromancy : MonoBehaviour
{
    public static Necromancy Instance { get; private set; }

    public GameObject allyPrefab;

    public int maxAllies = 5;

    public float spawnRadius = 1.2f;

    
    public float allyLifetime = 30f;

    private PlayerStats statsPlayer;

    private readonly List<GameObject> _allies = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        statsPlayer = GetComponent<PlayerStats>();

    }

   
    public void CreateAllys(float amount)
    {
        if (allyPrefab == null)
        {
           
            return;
        }

        int toSpawn = Mathf.FloorToInt(amount);
        for (int i = 0; i < toSpawn; i++)
        {
            if (_allies.Count >= maxAllies) break;

            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);
            GameObject allyGO = Instantiate(allyPrefab, spawnPos, Quaternion.identity);

            
            Ally allyComp = allyGO.GetComponent<Ally>();
            if (allyComp != null)
            {
                allyComp.OwnerNecromancy = this;
               
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

    
    public void ClearAllies()
    {
        foreach (var a in _allies.ToArray())
        {
            if (a != null) Destroy(a);
        }
        _allies.Clear();
    }

    public void Update()
    {
        maxAllies = (int)statsPlayer.necromancyAmount;
    }
}
