namespace UI.Native
{
    using System;

    [Flags]
    public enum SetWindowPosFlags : uint
    {
        AsynchronousWindowPosition = 0x4000,
        DeferErase = 0x2000,
        DoNotActivate = 0x10,
        DoNotChangeOwnerZOrder = 0x200,
        DoNotCopyBits = 0x100,
        DoNotRedraw = 8,
        DoNotReposition = 0x200,
        DoNotSendChangingEvent = 0x400,
        DrawFrame = 0x20,
        FrameChanged = 0x20,
        HideWindow = 0x80,
        IgnoreMove = 2,
        IgnoreResize = 1,
        IgnoreZOrder = 4,
        ShowWindow = 0x40
    }
}

