using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("이동할 다음 포탈 오브젝트")]
    [SerializeField] private Transform nextPortal;

    // 3D 환경에서 Is Trigger가 켜진 Collider와 닿았을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        // 닿은 오브젝트의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            Teleport(other.transform);
        }
    }


    private void Teleport(Transform player)
    {
        if (nextPortal == null)
        {
            Debug.LogWarning("연결된 다음 포탈이 없습니다!");
            return;
        }

        // CharacterController를 사용하는 경우, 일시적으로 꺼주어야 위치 이동이 정상 적용됨
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.position = nextPortal.position;
            cc.enabled = true;
        }
        else
        {
            player.position = nextPortal.position;
        }
    }
}