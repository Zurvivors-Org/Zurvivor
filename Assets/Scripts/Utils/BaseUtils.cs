using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

abstract class BaseUtils : MonoBehaviour
{
    public static IEnumerator WaitForSecondsThenAction(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
}
