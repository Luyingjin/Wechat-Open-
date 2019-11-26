using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Config;

namespace Formula
{
    public class WorkflowService : Formula.IWorkflowService
    {
        public string GetFlowCurrentStepCode()
        {
            return Config.Logic.WorkflowService.GetFlowCurrentStepCode();
        }
    }
}
