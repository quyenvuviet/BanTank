using UnityEngine;

public interface IDamageable : IDieable
{
    /// <summary>
    /// Máu của xe tăng
    /// </summary>
    public float Health { get; set; }
    /// <summary>
    /// Dame của xe tăng
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="damageDealerID"></param>
    /// <param name="damageSourcePos"></param>
    public void TakeDamage(float damage, byte damageDealerID, Vector3? damageSourcePos = null);
}
