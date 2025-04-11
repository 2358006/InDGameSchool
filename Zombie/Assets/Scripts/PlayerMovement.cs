using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 앞뒤 움직임의 속도
    public float rotateSpeed = 180f; // 좌우 회전 속도

    [SerializeField] PlayerInput playerInput; // 플레이어 입력을 알려주는 컴포넌트
    [SerializeField] Rigidbody playerRigidbody; // 플레이어 캐릭터의 리지드바디
    [SerializeField] Animator playerAnimator; // 플레이어 캐릭터의 애니메이터

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    void FixedUpdate()
    {
        // 물리 갱신 주기마다 움직임, 회전, 애니메이션 처리 실행
        Move();
        Rotate();

        // 입력 값에 따라 애니메이터의 Move 파라미터 값 변경
        playerAnimator.SetFloat("Move", playerInput.move);

    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임
    void Move()
    {
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    void Rotate()
    {
        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;

        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0);
    }
}