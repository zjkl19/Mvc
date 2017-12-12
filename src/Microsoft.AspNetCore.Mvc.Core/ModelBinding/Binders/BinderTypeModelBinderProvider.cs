// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// An <see cref="IModelBinderProvider"/> for models which specify an <see cref="IModelBinder"/>
    /// using <see cref="BindingInfo.BinderType"/>.
    /// </summary>
    public class BinderTypeModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public BinderTypeModelBinderProvider()
            : this(new NullLoggerFactory())
        {
        }

        public BinderTypeModelBinderProvider(ILoggerFactory loggerFactory)
        {
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

            if (context.BindingInfo.BinderType != null)
            {
                return new BinderTypeModelBinder(context.BindingInfo.BinderType, _loggerFactory);
            }

            return null;
        }
    }
}
