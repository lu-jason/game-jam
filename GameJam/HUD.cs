using Godot;
using System;

public partial class HUD : CanvasLayer {

    [Signal]
    public delegate void StartGameEventHandler();

    public override void _Ready()
    {
        GetNode<CanvasLayer>("YouWin").Hide();
    }
    public void ClearMainMenu()
    {
        GetNode<CanvasLayer>("YouWin").Hide();
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

    public void DisplayGameFinished()
    {
        GD.Print("Game Finsihed");
        GetNode<CanvasLayer>("YouWin").Show();
    }
}