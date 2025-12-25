// "Company © 2025. All rights reserved."

using System;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;

using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

using Sixnet.Web.Extensions;

namespace Sixnet.Web.Swagger.Processors
{
    internal class SixnetOperationProcessor : IOperationProcessor
    {
        bool _removeTagFromOpId = false;
        ApiDocGroupItem _groupItem;

        internal SixnetOperationProcessor(bool removeTagFromOpId, ApiDocGroupItem groupItem)
        {
            _removeTagFromOpId = removeTagFromOpId;
            _groupItem = groupItem;
        }

        public bool Process(OperationProcessorContext context)
        {
            if (context.Settings is AspNetCoreOpenApiDocumentGeneratorSettings apiSettings)
            {
                var version = _groupItem.Version;
                var area = _groupItem.Area;

                var op = context.OperationDescription;
                if (!string.IsNullOrWhiteSpace(version) && !(apiSettings.ApiGroupNames?.Contains(version) ?? false))
                {
                    return false;
                }
                var operationAreaName = context.ControllerType.GetCustomAttribute<AreaAttribute>()?.RouteValue?.Trim();
                if ((!string.IsNullOrWhiteSpace(area) || !string.IsNullOrWhiteSpace(operationAreaName))
                    && (!string.Equals(operationAreaName, area, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
                // default operationId = TagName_ActionName
                var operationId = op.Operation.OperationId;
                if (!string.IsNullOrEmpty(operationId) && operationId.Contains('_'))
                {
                    var newOperationId = operationId[(operationId.IndexOf('_') + 1)..];
                    op.Operation.OperationId = newOperationId;
                }
                context.OperationDescription.Operation.ExtensionData["TargetDocGroup"] = _groupItem.GroupName;
                return true;
            }
            return false;
        }
    }
}
