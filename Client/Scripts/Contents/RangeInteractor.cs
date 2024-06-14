using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class RangeInteractor : MonoBehaviour
{
    bool _canInteraction = false;
    GameObject _interactionUI = null;
    public InteractionType IType;
    private void Update()
    {
        if (_canInteraction)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                if(IType==InteractionType.Shop)
                {
                    if (Managers.UI.Root.GetComponentInChildren<UI_Shop>() != null) return;
                    Managers.UI.ShowPopupUI<UI_Shop>();
                }
                else if(IType==InteractionType.Boss) 
                {
                    if (Managers.UI.Root.GetComponentInChildren<UI_EnterBoss>() != null) return;
                    Managers.UI.ShowPopupUI<UI_EnterBoss>();
                }
                else if (IType == InteractionType.Village)
                {
                    if (Managers.Party.OwnerId == -1)
                    {
                        UI_MessageWindow messageWindow = Managers.UI.Root.GetComponent<UI_MessageWindow>();
                        if (messageWindow == null)
                        {
                            messageWindow = Managers.UI.ShowPopupUI<UI_MessageWindow>();
                            messageWindow.Init();
                        }
                        messageWindow.SetMessage(PartyWarning.PartyNotJoined);
                    }
                    else if (Managers.Party.OwnerId != Managers.Object.MyPlayer.ObjectInfo.ObjectId)
                    {
                        UI_MessageWindow messageWindow = Managers.UI.Root.GetComponent<UI_MessageWindow>();
                        if (messageWindow == null)
                        {
                            messageWindow = Managers.UI.ShowPopupUI<UI_MessageWindow>();
                            messageWindow.Init();
                        }
                        messageWindow.SetMessage(PartyWarning.PartyNotPartyOwner);
                    }
                    else
                    {
                        C_Portal portalPacket = new C_Portal();
                        portalPacket.RoomType = RoomType.Village;
                        Managers.Network.Send(portalPacket);
                    }
                }
                Managers.Sound.Play("Interaction");
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HeroPlayerController>() == null) return;
        if (_interactionUI == null)
        {
            _interactionUI = Managers.Resource.Instantiate("UI/UI_Interaction");
        }
        _canInteraction = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<HeroPlayerController>() == null) return;
        if (_interactionUI != null)
        {
            Destroy(_interactionUI);
            _interactionUI = null;
        }
        _canInteraction = false;
    }
}
