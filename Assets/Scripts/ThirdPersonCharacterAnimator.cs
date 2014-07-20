using UnityEngine;
using System.Collections;

/// <summary>
/// Third person character animator.
/// Handles the animations for the character.
/// </summary>
public class ThirdPersonCharacterAnimator : MonoBehaviour
{
    private static ThirdPersonCharacterAnimator instance;

    private Vector3 moveVector;

    /// <summary>
    /// Gets a reference to this instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ThirdPersonCharacterAnimator Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Gets or sets the move vector.
    /// </summary>
    /// <value>The move vector.</value>
    public Vector3 MoveVector
    {
        get
        {
            return this.moveVector;
        }
        set
        {
            this.moveVector = value;
        }
    }

    #region Unity Events

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
    
    }

    #endregion Unity Events
}
