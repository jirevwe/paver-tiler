using UnityEngine;

public class PoolObject : MonoBehaviour
{
    [HideInInspector]
    protected float timeToDie = 0f;

    /// <summary>
    /// Disable after a certain time has passed
    /// </summary>
    public virtual void Disable()
    {
        SetInactive(timeToDie);
    }

    /// <summary>
    /// Set inactive with delay
    /// </summary>
    /// <param name="time"></param>
    void SetInactive(float time)
    {
        Invoke("SetInactive", time);
    }

    /// <summary>
    /// set inactive immediately
    /// </summary>
    protected void SetInactive()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resue this object
    /// </summary>
    /// <param name="args"> data to be passed to new instance </param>
    public virtual void OnObjectReuse<T>(T args)
    {

    }
}
