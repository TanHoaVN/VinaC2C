﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VinaC2C.Data.Models
{
    public class FeatureRole : ObjectBaseModel
    {
        public int FeatureID { get; set; }

        public int UserID { get; set; }

        public bool IsAllow { get; set; }
    }
}