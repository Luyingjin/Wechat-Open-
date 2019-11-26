using System;
namespace Formula
{
    public interface IWorkflowService : ISingleton
    {
        string GetFlowCurrentStepCode();
    }
}
