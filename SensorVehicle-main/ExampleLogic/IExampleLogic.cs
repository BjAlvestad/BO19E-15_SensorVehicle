namespace ExampleLogic
{
    public interface IExampleLogic
    {
        ExampleLogicDetails Details { get; }

        void Initialize();
        void Run();
    }
}
