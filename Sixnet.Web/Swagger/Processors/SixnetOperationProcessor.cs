// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Sixnet.Web.Swagger.Processors
{
    internal class SixnetOperationProcessor : IOperationProcessor
    {
        bool _removeTagFromOpId = false;
        string _areaName = string.Empty;
        string _groupName = string.Empty;

        internal SixnetOperationProcessor(bool removeTagFromOpId, string areaName, string groupName)
        {
            _removeTagFromOpId = removeTagFromOpId;
            _areaName = areaName;
            _groupName = groupName;
        }

        public bool Process(OperationProcessorContext context)
        {
            if (context.Settings is AspNetCoreOpenApiDocumentGeneratorSettings apiSettings)
            {
                if (!string.IsNullOrWhiteSpace(_groupName) && !(apiSettings.ApiGroupNames?.Contains(_groupName) ?? false))
                {
                    return false;
                }
                var operationAreaName = context.ControllerType.GetCustomAttribute<AreaAttribute>()?.RouteValue?.Trim();
                if ((!string.IsNullOrWhiteSpace(_areaName) || !string.IsNullOrWhiteSpace(operationAreaName))
                    && (!string.Equals(operationAreaName, _areaName, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
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
            return false;
        }
    }
}
