using System;
using System.Linq;
using Godot;

public partial class SignalManager : Node2D
{
    [Signal]
    public delegate void TriggerChannelEventHandler(int channel, int state);

    private Node2D[] _emitters;

    private Node2D[] _listeners;

    public void AddEmitter(Node2D node)
    {
        _emitters.Append (node);
    }

    public void AddListener(Node2D node)
    {
        _listeners.Append (node);
    }

    private void Link()
    {
    }
}
