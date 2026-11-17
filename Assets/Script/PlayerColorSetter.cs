using Unity.Netcode;
using UnityEngine;

public class PlayerColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer characterRenderer; // 캐릭터의 MeshRenderer 또는 SkinnedMeshRenderer
    [SerializeField] private Color myColor = Color.green;
    [SerializeField] private Color otherPlayerColor = Color.white;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // 렌더러가 인스펙터에서 할당되지 않았다면 자식 오브젝트에서 탐색
        if (characterRenderer == null)
        {
            characterRenderer = GetComponentInChildren<Renderer>();
        }

        if (characterRenderer != null)
        {
            // IsOwner가 true면 내 컴퓨터에서 조종하는 로컬 플레이어 캐릭터
            if (IsOwner)
            {
                characterRenderer.material.color = myColor;
            }
            else
            {
                characterRenderer.material.color = otherPlayerColor;
            }
        }
    }
}