using Unity.Netcode;
using UnityEngine;

public class PlayerCameraSetup : NetworkBehaviour
{
    [Header("Camera & Audio References")]
    [SerializeField] private GameObject playerCameraObject; // 자식 카메라 오브젝트
    [SerializeField] private AudioListener audioListener;    // 자식 오디오 리스너

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // 씬에 기본배치되어 있는 개발용/관전용 메인 카메라가 있다면 끄기
            if (Camera.main != null && Camera.main.gameObject != playerCameraObject)
            {
                Camera.main.gameObject.SetActive(false);
            }

            // 내 소유 플레이어의 카메라와 오디오 리스너만 활성화
            if (playerCameraObject != null) playerCameraObject.SetActive(true);
            if (audioListener != null) audioListener.enabled = true;
        }
        else
        {
            // 다른 사람의 플레이어 프리팹에 달린 카메라는 비활성화 유지
            if (playerCameraObject != null) playerCameraObject.SetActive(false);
            if (audioListener != null) audioListener.enabled = false;
        }
    }
}