using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int ItemId;
    public int ObjectId;
    public int Money;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HeroPlayerController>() == null) return;
        C_PickUpItem pickUpItemPacket = new C_PickUpItem();
        pickUpItemPacket.ObjectId = ObjectId;
        pickUpItemPacket.ItemId = ItemId;
        pickUpItemPacket.Money = Money;
        Managers.Network.Send(pickUpItemPacket);
    }
}
