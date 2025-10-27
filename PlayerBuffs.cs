using UnityEngine;
using System.Collections;

public class PlayerBuffs : MonoBehaviour
{
    public float attackMultiplier = 1f;
    public float defenseMultiplier = 1f;
    private Coroutine buffCoroutine;

    public void ApplyBuffs(float attackBuff, float defenseBuff, float duration)
    {
        if (buffCoroutine != null)
            StopCoroutine(buffCoroutine);
        buffCoroutine = StartCoroutine(BuffRoutine(attackBuff, defenseBuff, duration));
    }

    private IEnumerator BuffRoutine(float attackBuff, float defenseBuff, float duration)
    {
        attackMultiplier = attackBuff;
        defenseMultiplier = defenseBuff;
        Debug.Log($"Buffs applied: Attack x{attackBuff}, Defense x{defenseBuff} for {duration} seconds");
        yield return new WaitForSeconds(duration);
        attackMultiplier = 1f;
        defenseMultiplier = 1f;
        Debug.Log("Buffs expired");
    }
}
