using UnityEngine;

public class WrestlerController : MonoBehaviour
{
    [Header("Wrestler")]
    public bool isPlayer;
    public float moveSpeed = 4.2f;
    public Transform opponent;

    [Header("Runtime input")]
    [Range(-1f, 1f)] public float mobileHorizontal;
    [Range(-1f, 1f)] public float mobileVertical;

    private float attackCooldown;
    private float stunTimer;
    private float blockTimer;
    private const float RingLimit = 3.25f;

    private void Update()
    {
        attackCooldown = Mathf.Max(0f, attackCooldown - Time.deltaTime);
        stunTimer = Mathf.Max(0f, stunTimer - Time.deltaTime);
        blockTimer = Mathf.Max(0f, blockTimer - Time.deltaTime);

        var health = GetComponent<WrestlerHealth>();
        if (health != null)
            health.guard = blockTimer > 0f;

        if (stunTimer > 0f || (health != null && health.isKnockedOut))
            return;

        if (isPlayer)
        {
            Vector3 direction = new Vector3(mobileHorizontal, 0f, mobileVertical);
            Vector3 keyboard = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            if (keyboard.sqrMagnitude > 0.01f)
                direction = keyboard;

            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            float speed = Input.GetKey(KeyCode.LeftShift) ? 6f : moveSpeed;
            Move(direction, speed);

            if (Input.GetKeyDown(KeyCode.J)) Punch();
            if (Input.GetKeyDown(KeyCode.K)) Kick();
            if (Input.GetKeyDown(KeyCode.L)) Grapple();
            if (Input.GetKey(KeyCode.I)) Block();
        }
        else if (opponent != null)
        {
            Vector3 direction = opponent.position - transform.position;
            direction.y = 0f;
            if (direction.magnitude > 1.65f)
                Move(direction.normalized * 0.72f, moveSpeed);
            else if (attackCooldown <= 0f)
                Punch();
        }

        FaceOpponent();
    }

    private void Move(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        transform.position += direction * speed * Time.deltaTime;
        Vector3 p = transform.position;
        transform.position = new Vector3(
            Mathf.Clamp(p.x, -RingLimit, RingLimit),
            0f,
            Mathf.Clamp(p.z, -RingLimit, RingLimit));
    }

    private void FaceOpponent()
    {
        if (opponent == null)
            return;

        Vector3 direction = opponent.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 9f);
        }
    }

    public void SetMobileMove(Vector2 input)
    {
        input = Vector2.ClampMagnitude(input, 1f);
        mobileHorizontal = input.x;
        mobileVertical = input.y;
    }

    public void Punch() { TryAttack(8f, 1.15f); }
    public void Kick() { TryAttack(12f, 1.45f); }
    public void Grapple() { TryAttack(18f, 1.8f); }

    private void TryAttack(float damage, float range)
    {
        if (attackCooldown > 0f || opponent == null)
            return;

        attackCooldown = 0.65f;

        Vector3 toOpponent = opponent.position - transform.position;
        toOpponent.y = 0f;
        if (toOpponent.magnitude > range)
            return;

        if (toOpponent.sqrMagnitude > 0.001f &&
            Vector3.Dot(transform.forward, toOpponent.normalized) < 0.15f)
            return;

        WrestlerHealth targetHealth = opponent.GetComponent<WrestlerHealth>();
        if (targetHealth != null)
            targetHealth.Damage(damage);
    }

    public void Block()
    {
        blockTimer = 0.45f;
    }

    public void Stun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
    }
}

public class WrestlerHealth : MonoBehaviour
{
    public float value = 100f;
    public bool guard;
    public bool isKnockedOut;

    public void Damage(float amount)
    {
        if (isKnockedOut)
            return;

        value = Mathf.Max(0f, value - (guard ? amount * 0.2f : amount));

        WrestlerController controller = GetComponent<WrestlerController>();
        if (controller != null)
            controller.Stun(0.35f);

        if (value <= 0f)
        {
            isKnockedOut = true;
            if (controller != null)
                controller.SetMobileMove(Vector2.zero);
        }
    }
}
