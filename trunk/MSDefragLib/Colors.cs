﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSDefragLib
{
    public class Colors
    {
        public Colors(CLUSTER_COLORS color)
        {
            m_color = color;
            m_numColors = 0;
        }

        public CLUSTER_COLORS m_color;
        public Int64 m_numColors;
    }
}