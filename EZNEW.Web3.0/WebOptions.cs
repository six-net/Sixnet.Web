﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Serialization.Json;

namespace EZNEW.Web
{
    public class WebOptions
    {
        /// <summary>
        /// Gets or sets the json serialization options
        /// </summary>
        public JsonSerializationOptions JsonSerializationOptions { get; set; } = new JsonSerializationOptions();
    }
}