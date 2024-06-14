using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Village,
        Boss,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
        BeginDrag,
        EndDrag,
        PointerEnter,
        PointerExit,
        Drop,
    }
    public enum InteractionType
    {
        Shop,
        Boss,
        Village,
    }
}
