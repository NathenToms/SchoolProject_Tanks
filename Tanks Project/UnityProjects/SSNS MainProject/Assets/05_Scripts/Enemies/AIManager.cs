using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIManager : MonoBehaviour
{
    public static AIManager aiManager;
    public enum AITypes { Null, Swarmer, Charger, Fighter };
    [Header("Do not modify any values while running please")]
    readonly float maxAttack = 4;
    [SerializeField] private float curAttack = 0;
    readonly int[] countsPer1Point = { 0, 6, 2, 1 };
    [Header("NULL, Swarmer, Charger, Fighter")]
    [SerializeField] int[] countsOfAI = { 0, 0, 0, 0 };

    float MaxAttack { get { return maxAttack; } } // set { maxAttack = value; } }
    float CurAttack { get { return curAttack; } set { curAttack = value; } }

    private float TestTotal(AITypes whichAI, int numOfAI)
    {
        int[] counts = new int[countsOfAI.Length];
        float total = 0;

        for (int i = 1; i < counts.Length; i++)
        {
            counts[i] = countsOfAI[i];
        }

        counts[(int)whichAI] += numOfAI;
        
        for (int i = 1; i < counts.Length; i++)
        {
            total += (float)counts[i] / countsPer1Point[i];
        }

        return total;
    }

    private void SetTotal(AITypes whichAI, int numOfAI)
    {
        countsOfAI[(int)whichAI] += numOfAI;
        if(countsOfAI[(int)whichAI] < 0)
        {
            countsOfAI[(int)whichAI] = 0;
            Debug.Log("Either kill all was pressed, or you f-ed up");
        }
        float total = 0;
        for (int i = 1; i < countsOfAI.Length; i++)
        {
            total += (float)countsOfAI[i] / countsPer1Point[i];
        }

        aiManager.CurAttack = total;
    }


    /// <summary>
    /// For if n enemies are attacking
    /// </summary>
    public bool CanAttack(AITypes whichAI, int numOfAI = 1)//(int numAttacking = 1)
    {
        float points = TestTotal(whichAI,numOfAI);
        bool willAttack = points <= aiManager.MaxAttack;
        if (willAttack) SetTotal(whichAI, numOfAI);
        return willAttack;
    }

    /// <summary>
    /// For if n enemies are attacking
    /// </summary>
    public void StopAttack(AITypes whichAI, int numOfAI = 1)//(int numStopAttacking = 1)
    {

        SetTotal(whichAI, -numOfAI);
        //int points = numOfAI / countsPer1Point[(int)whichAI];
        //aiManager.CurAttack -= points;
        if(aiManager.CurAttack < 0)
        {
            Debug.LogError("Y'all fudged up.  AICount < 0");
            aiManager.CurAttack = 0;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (aiManager == null)
        {
            aiManager = this;
        }
        for (int i = 0; i < countsOfAI.Length; i++)
        {
            countsOfAI[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
