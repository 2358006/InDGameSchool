using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour
{
    public AudioClip deathClip; // 사망시 재생할 오디오 클립
    public float jumpForce = 700f; // 점프 힘

    int jumpCount = 0; // 누적 점프 횟수
    bool isGrounded = false; // 바닥에 닿았는지 나타냄
    bool isDead = false; // 사망 상태

    [Header("GetComponet<>();")]
    [SerializeField] Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
    [SerializeField] Animator animator; // 사용할 애니메이터 컴포넌트
    [SerializeField] AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

    void Update()
    {
        // 사용자 입력을 감지하고 점프하는 처리
        if (isDead)
        {
            // 사망시 처리 하지 않고 종료
            return;
        }

        // 마우스 왼쪽 버튼 0, 오른 쪽 1, 휠 버튼 2
        if (Input.GetMouseButtonDown(0) && jumpCount < 2)
        {
            jumpCount++;
            playerRigidbody.velocity = Vector2.zero;

            // 리지드바디 위쪽을 힘 주기
            playerRigidbody.AddForce(Vector2.up * jumpForce);

            // 오디오
            playerAudio.Play();
        }
        else if (Input.GetMouseButtonUp(0) && playerRigidbody.velocity.y > 0)
        {
            playerRigidbody.velocity = playerRigidbody.velocity * 0.5f;
        }

        animator.SetBool("IsGrounded", isGrounded);
    }

    void Die()
    {
        // 사망 처리
        animator.SetTrigger("Die");

        // 오디오 소스에서 사망 오디오로 교체하여 플레이
        playerAudio.clip = deathClip;
        playerAudio.Play();

        playerRigidbody.velocity = Vector2.zero;
        isDead = true;

        // 게임 매니저의 게임 오버 처리 실행
        GameManager.instance.OnPlayerDead();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
        if (other.CompareTag("Dead") && !isDead)
        {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥에 닿았음을 감지하는 처리
        if (collision.contacts[0].normal.y > 0.7f)
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 바닥에서 벗어났음을 감지하는 처리
        isGrounded = false;
    }
}