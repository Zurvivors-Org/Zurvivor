using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

abstract class BaseUtils : MonoBehaviour
{
    public static IEnumerator WaitForSecondsThenAction(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }

    public static float EnforceRadius(float min, float max)
    {
        float num = Random.Range(-max, max);
        while (Mathf.Abs(num) <= min)
        {
            num = Random.Range(-max, max);
        }

        return num;
    }
}
