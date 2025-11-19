using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class enemygeneration : MonoBehaviour
{

    public GameObject cursenun;
    public GameObject attacknun;

    public float minstep = 3f;
    public float maxstep = 7f;

    public float hs;
    public float vs;

    public bool DrawRangeWhenSelectedGizmo;
    public int[] rounds;

    private void Start()
    {

    }
    public void Generate(float hspan, float vspan, int enemyLimit)
    {
        float enemies = enemyLimit;
        float atckenemies = Mathf.Floor(enemies * UnityEngine.Random.Range(0.5f, 0.8f));
        Vector3 spawn = new Vector3(transform.position.x - hspan / 2, transform.position.y + vspan / 2, 0);

        GameObject newenemy;
        while (spawn.y > transform.position.y - vspan / 2 && enemies > 0)
        {
            spawn += new Vector3(UnityEngine.Random.Range(minstep, maxstep), 0, 0);
            if (atckenemies == 0)
            {

                newenemy = Instantiate(cursenun);
            }
            else
            {

                newenemy = Instantiate(attacknun);
            }
            newenemy.transform.position = spawn;
            enemies -= 1;
            if (atckenemies > 0)
            {
                Debug.Log("atckenemeies: " + atckenemies);
                atckenemies -= 1;
            }
            if (spawn.x >= transform.position.x + hspan / 2)
            {
                spawn = new Vector3(transform.position.x - hspan / 2, spawn.y, 0);
                spawn -= new Vector3(0, UnityEngine.Random.Range(minstep, maxstep), 0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, 2.5f);

        if (DrawRangeWhenSelectedGizmo == false)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(transform.position, new Vector3(hs, vs, 0));
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (DrawRangeWhenSelectedGizmo == true)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(transform.position, new Vector3(hs, vs, 0));
        }
    }

    //#if UNITY_EDITOR
    //    [CustomEditor(typeof(enemygeneration))]
    //    public class EnemyGenerationEditor : Editor
    //    {
    //        private int numberOfRounds;

    //        public override void OnInspectorGUI()
    //        {
    //            enemygeneration script = (enemygeneration)target;

    //            DrawPropertiesExcluding(serializedObject, "rounds");

    //            EditorGUILayout.Space();
    //            EditorGUILayout.LabelField("Rounds Configuration", EditorStyles.boldLabel);

    //            numberOfRounds = EditorGUILayout.IntField("Number of Rounds", numberOfRounds);

    //            if (GUILayout.Button("Set Number of Rounds"))
    //            {
    //                AdjustRoundsArray(script, numberOfRounds);
    //            }

    //            SerializedProperty roundsProperty = serializedObject.FindProperty("rounds");
    //            for (int i = 0; i < roundsProperty.arraySize; i++)
    //            {
    //                SerializedProperty round = roundsProperty.GetArrayElementAtIndex(i);
    //                EditorGUILayout.PropertyField(round, new GUIContent($"Round {i + 1}"));
    //            }

    //            serializedObject.ApplyModifiedProperties();
    //        }

    //        private void AdjustRoundsArray(enemygeneration script, int newSize)
    //        {
    //            SerializedProperty roundsProperty = serializedObject.FindProperty("rounds");
    //            roundsProperty.arraySize = newSize;

    //            for (int i = 0; i < newSize; i++)
    //            {
    //                if (roundsProperty.GetArrayElementAtIndex(i).intValue == 0)
    //                {
    //                    roundsProperty.GetArrayElementAtIndex(i).intValue = 0;
    //                }
    //            }

    //            serializedObject.ApplyModifiedProperties();
    //        }
    //    }
    //#endif
}