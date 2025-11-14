// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Sixnet.Web.Swagger.Processors
{
    internal class RemoveTagFromOperationIdProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var op = context.OperationDescription;
            // default operationId = TagName_ActionName
            var operationId = op.Operation.OperationId;
            if (!string.IsNullOrEmpty(operationId) && operationId.Contains('_'))
            {
                var newOperationId = operationId[(operationId.IndexOf('_') + 1)..];
                op.Operation.OperationId = newOperationId;
            }
            return true;
        }
    }
}
