namespace ControlProgram.Remote
{
    interface IRemoteControl
    {
        int GetData();
        int Open();
        int SetData();
    }
}