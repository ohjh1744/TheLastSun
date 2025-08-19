using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int _moveSpeed = 4;

    private Rigidbody2D _rigid;

    private Transform _target;

    private void Awake()
    {
        _target = GetComponentInParent<UnitController>().Target.transform;
    }

    public void MoveToTarget(Vector2 targetPos)
    {
        transform.Translate(targetPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            MonsterController monsterController = collision.GetComponent<MonsterController>();
            if (monsterController != null)
            {
                monsterController.TakeDamage(1); 
            }
            Destroy(gameObject); 
        }
    }
}
