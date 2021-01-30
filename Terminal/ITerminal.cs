namespace ControlProgram.Terminal
{
    interface ITerminal
    {
        void CommandLine();
        void Update(ObjectDataRecords currentObject);
        void DrawDisplay();
        void Open();
    }
}