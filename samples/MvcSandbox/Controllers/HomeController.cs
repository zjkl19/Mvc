// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MvcSandbox.Models;

namespace MvcSandbox.Controllers
{
    public class HomeController : Controller
    {
        [ModelBinder]
        public string Id { get; set; }

        public IActionResult Index()
        {
            var model = new TestViewModel
            {
                Dict = new Dictionary<string, int>
                {
                    ["Key1"] = 1,
                    ["Key2"] = 2,
                    ["Key3"] = 3,
                    ["Key4"] = 4,
                    ["index"] = 5,
                }
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(TestViewModel model)
        {
            return Ok();
        }
    }
}
