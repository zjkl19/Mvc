// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// An <see cref="IModelBinder"/> for models which specify an <see cref="IModelBinder"/> using
    /// <see cref="BindingInfo.BinderType"/>.
    /// </summary>
    public class BinderTypeModelBinder : IModelBinder
    {
        private readonly ObjectFactory _factory;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="BinderTypeModelBinder"/>.
        /// </summary>
        /// <param name="binderType">The <see cref="Type"/> of the <see cref="IModelBinder"/>.</param>
        public BinderTypeModelBinder(Type binderType)
            : this(binderType, new NullLoggerFactory())
        {
        }

        /// <summary>
        /// Creates a new <see cref="BinderTypeModelBinder"/>.
        /// </summary>
        /// <param name="binderType">The <see cref="Type"/> of the <see cref="IModelBinder"/>.</param>
        /// <param name="loggerFactory"></param>
        public BinderTypeModelBinder(Type binderType, ILoggerFactory loggerFactory)
        {
            if (binderType == null)
            {
                throw new ArgumentNullException(nameof(binderType));
            }

            if (!typeof(IModelBinder).GetTypeInfo().IsAssignableFrom(binderType.GetTypeInfo()))
            {
                throw new ArgumentException(
                    Resources.FormatBinderType_MustBeIModelBinder(
                        binderType.FullName,
                        typeof(IModelBinder).FullName),
                    nameof(binderType));
            }

            _factory = ActivatorUtilities.CreateFactory(binderType, Type.EmptyTypes);
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc />
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var requestServices = bindingContext.HttpContext.RequestServices;
            var binder = (IModelBinder)_factory(requestServices, arguments: null);

            await binder.BindModelAsync(bindingContext);
        }
    }
}
