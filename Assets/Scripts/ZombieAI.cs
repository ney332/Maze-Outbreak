using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ZombieAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float detectionDistance = 8f;
    public float attackDistance = 1.2f;
    public float pointReachDistance = 0.35f;

    private CharacterController controller;
    private Transform player;
    private int patrolIndex;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        float playerDistance = Vector3.Distance(transform.position, player.position);
        if (playerDistance <= attackDistance)
        {
            GameManager.Instance.GameOver();
            return;
        }

        Vector3 target = playerDistance <= detectionDistance ? player.position : GetPatrolTarget();
        float speed = playerDistance <= detectionDistance ? chaseSpeed : patrolSpeed;
        MoveToward(target, speed);
    }

    private Vector3 GetPatrolTarget()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return transform.position;
        }

        Transform point = patrolPoints[patrolIndex];
        if (Vector3.Distance(transform.position, point.position) <= pointReachDistance)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            point = patrolPoints[patrolIndex];
        }

        return point.position;
    }

    private void MoveToward(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }

        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
