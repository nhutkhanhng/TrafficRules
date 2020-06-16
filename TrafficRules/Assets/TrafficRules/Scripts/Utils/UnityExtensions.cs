using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
{
    /// <summary>
    /// This function check null Component and Ensure gameobject be atteched exsist
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public static bool Available(this MonoBehaviour component)
    {
        if (component)
            if (component.gameObject)
                return true;

        return false;
    }

    public static void ResetLocal(this Transform Target)
    {
        Target.transform.localPosition = Vector3.zero;
        Target.transform.localRotation = new Quaternion(0, 0, 0, 0);
        Target.transform.localScale = Vector3.one;
    }
    public static void SetLossyScale(this Transform go, Vector3 lossyScale)
    {
        var par = go.parent;
        go.SetParent(null);
        go.localScale = lossyScale;
        go.SetParent(par);
    }

    public static void IncrPlayerPref(string key, int quantity = 1)
    {
        int val = PlayerPrefs.GetInt(key);
        val += quantity;
        PlayerPrefs.SetInt(key, val);
        PlayerPrefs.Save();
    }
}
