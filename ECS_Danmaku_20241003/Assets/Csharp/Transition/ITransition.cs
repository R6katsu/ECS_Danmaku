using System.Collections;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
#endif

/// <summary>
/// �g�����W�V�����̎���
/// </summary>
public interface ITransition
{
    /// <summary>
    /// �g�����W�V�����̊J�n
    /// </summary>
    public void StartTransition();

    /// <summary>
    /// �g�����W�V�����̎���
    /// </summary>
    IEnumerator Transition();
}
