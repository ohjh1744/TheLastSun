using UnityEngine;

public class GodProjectile : MonoBehaviour
{
    private UnitModel _unitModel;

    private void Awake()
    {
        _unitModel = GetComponentInParent<UnitModel>();
    }

    private void Start()
    {
        Destroy(gameObject, 1.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            other.GetComponent<MonsterController>()?.TakeDamage(_unitModel.Damage);
        }
    }
}
