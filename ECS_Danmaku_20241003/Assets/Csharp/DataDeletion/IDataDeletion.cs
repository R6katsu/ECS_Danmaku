#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endif

/// <summary>
/// Dataを削除するか
/// </summary>
public interface IDataDeletion
{
    /// <summary>
    /// Dataを削除するか
    /// </summary>
    public bool IsDataDeletion { get; set; }
}
