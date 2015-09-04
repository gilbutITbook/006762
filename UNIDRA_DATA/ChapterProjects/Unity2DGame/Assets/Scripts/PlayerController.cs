using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveStartDistance = 10f;
    public float moveForce = 150f;
    public float maxSpeed = 100f;
    float targetPointX;
    bool facingRight = true;

    void Start()
    {
        Vector3 screen_point = Camera.main.WorldToScreenPoint(transform.position);
        targetPointX = screen_point.x;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        targetPointX = Input.mousePosition.x;
    }

    void FixedUpdate()
    {
        // 3D 좌표를 스크린 좌표로 변환한다.
        Vector3 screen_point = Camera.main.WorldToScreenPoint(transform.position);

        // 이동할 거리가 일정 거리 이하라면 이동 처리를 하지 않는다.
        if (Mathf.Abs(targetPointX - screen_point.x) <= moveStartDistance)
            return;

        // 이동할 곳이 왼쪽인지 오른쪽인지 계산해서 그 방향으로 이동하는 힘을 더한다.
        float horizontal = Mathf.Sign(targetPointX - screen_point.x);
        rigidbody2D.AddForce(Vector2.right * horizontal * moveForce);

        // 이동 속도를 제한한다.
        if (Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

        // 플레이어의 방향을 조정한다.
        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 local_scale = transform.localScale;
            local_scale.x *= -1;
            transform.localScale = local_scale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Fire")
        {
            Animator myAnimator = GetComponent<Animator>();
            myAnimator.SetTrigger("Damage");
        }
    }
}