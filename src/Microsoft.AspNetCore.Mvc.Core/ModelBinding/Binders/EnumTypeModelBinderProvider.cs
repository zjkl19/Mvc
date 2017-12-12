// Copyright (c) .NET Foundation. All righ_ts reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// A <see cref="IModelBinderProvider"/> for types deriving from <see cref="Enum"/>.
    /// </summary>
    public class EnumTypeModelBinderProvider : IModelBinderProvider
    {
        private readonly MvcOptions _options;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public EnumTypeModelBinderProvider(MvcOptions options)
            : this(options, new NullLoggerFactory())
        {
            _options = options;
        }

        public EnumTypeModelBinderProvider(MvcOptions options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.IsEnum)
            {
                return new EnumTypeModelBinder(
                    _options.AllowBindingUndefinedValueToEnumType,
                    context.Metadata.UnderlyingOrModelType,
                    _loggerFactory);
            }
            else
            {
                _logger.LogDebug($"Model type {context.Metadata.UnderlyingOrModelType} is not an enum.");
            }

            return null;
        }
    }
}
