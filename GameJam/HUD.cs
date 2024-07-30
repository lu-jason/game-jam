using Godot;
using System;

public partial class HUD : CanvasLayer {

    [Signal]
    public delegate void StartGameEventHandler();
    public void ClearMainMenu()
    {
        var mainMenu = GetNode<CanvasLayer>("MainMenu");
        mainMenu.GetNode<ColorRect>("Background").Hide();
        mainMenu.GetNode<Button>("StartButton").Hide();
        mainMenu.GetNode<Label>("Title").Hide();
        mainMenu.Hide();
    }

    public void OnStartButtonPressed()
    {
        EmitSignal(SignalName.StartGame);
    }
}