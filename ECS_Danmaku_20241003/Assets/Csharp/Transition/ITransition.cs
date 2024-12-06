using System.Collections;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
#endif

/// <summary>
/// トランジションの実装
/// </summary>
public interface ITransition
{
    /// <summary>
    /// トランジションの開始
    /// </summary>
    public void StartTransition(int sceneNumber);

    /// <summary>
    /// トランジションの実装
    /// </summary>
    IEnumerator Transition(int sceneNumber);
}
