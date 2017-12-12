// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// An <see cref="IModelBinderProvider"/> for <see cref="IFormFile"/>, collections
    /// of <see cref="IFormFile"/>, and <see cref="IFormFileCollection"/>.
    /// </summary>
    public class FormFileModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public FormFileModelBinderProvider()
            : this(new NullLoggerFactory())
        {
        }

        public FormFileModelBinderProvider(ILoggerFactory loggerFactory)
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

            // Note: This condition needs to be kept in sync with ApiBehaviorApplicationModelProvider.
            var modelType = context.Metadata.ModelType;
            if (modelType == typeof(IFormFile) ||
                modelType == typeof(IFormFileCollection) ||
                typeof(IEnumerable<IFormFile>).IsAssignableFrom(modelType))
            {
                return new FormFileModelBinder(_loggerFactory);
            }
            else
            {
                _logger.LogDebug("Cannot bind to model of type {ModelType}. Supported types are ....");
            }

            return null;
        }
    }
}
