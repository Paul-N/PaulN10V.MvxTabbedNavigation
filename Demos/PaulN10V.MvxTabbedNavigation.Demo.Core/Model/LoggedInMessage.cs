using MvvmCross.Plugin.Messenger;
// ReSharper disable ConvertToPrimaryConstructor

namespace PaulN10V.MvxTabbedNavigation.Demo.Core.Model;

internal sealed class LoggedInMessage : MvxMessage
{
    public LoggedInMessage(object sender) : base(sender) { }
}