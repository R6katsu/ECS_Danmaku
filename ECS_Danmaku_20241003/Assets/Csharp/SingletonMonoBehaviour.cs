using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
#endif

// リファクタリング済み

/// <summary>
/// シングルトン用のMonoBehaviour
/// </summary>
/// <typeparam name="T">シングルトン化する型</typeparam>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);

                if (instance == null)
                {
                    Debug.LogError($"現在のシーンに{t}をアタッチしているGameObjectはありません");
                }
            }
            return instance;
        }
    }
    virtual protected void Awake()
    {
        // 他のゲームオブジェクトにアタッチされているか調べる
        // アタッチされている場合は破棄する
        CheckInstance();
    }

    /// <summary>
    /// 他のゲームオブジェクトにアタッチされているか調べる。<br/>
    /// アタッチされている場合は破棄する。
    /// </summary>
    /// <returns>アタッチされていなかったらtrue</returns>
    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        else if (instance == this)
        {
            return true;
        }
        Destroy(this);
        return false;
    }
}
