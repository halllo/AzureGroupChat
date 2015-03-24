// Guids.cs
// MUST match guids.h
using System;

namespace ManuelNaujoks.VSChat
{
    static class GuidList
    {
        public const string guidVSChatPkgString = "c2968bf7-a01e-4049-bce6-331ac5042e64";
        public const string guidVSChatCmdSetString = "eecf0ba1-3640-47f7-a9f4-f85f20610204";
        public const string guidToolWindowPersistanceString = "173cbcde-e728-442c-82ee-1c29ae3e00af";

        public static readonly Guid guidVSChatCmdSet = new Guid(guidVSChatCmdSetString);
    };
}