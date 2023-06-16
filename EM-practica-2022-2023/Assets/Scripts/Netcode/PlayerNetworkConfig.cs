using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;


namespace Netcode
{
    public class PlayerNetworkConfig : NetworkBehaviour
    {
        public GameObject characterPrefab;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            InstantiateCharacterServerRpc(OwnerClientId);
        }
    
        [ServerRpc]
        public void InstantiateCharacterServerRpc(ulong id)
        {
            GameObject characterGameObject = Instantiate(characterPrefab);
            characterGameObject.GetComponent<NetworkObject>().SpawnWithOwnership(id);
            characterGameObject.transform.SetParent(transform, false);
        }
    }
}
