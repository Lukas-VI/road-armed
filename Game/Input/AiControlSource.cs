namespace RoadArmed.Game.Input;

public partial class AiControlSource : ControlSourceNode
{
    private readonly ControlFrame _buffer = new();

    public void SetFrame(ControlFrame frame)
    {
        _buffer.CopyFrom(frame);
    }

    public override void Capture(ControlMode currentMode, ControlFrame frame, double delta)
    {
        frame.Clear(currentMode);
        frame.CopyFrom(_buffer);
        _buffer.ClearTransientSignals();
    }
}
