﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFITarget
{
    namespace A
    {
        public class NamespaceResolutionTargetTest
        {
            public NamespaceResolutionTargetTest()
            {
                Prop = 0; 
            }

            public NamespaceResolutionTargetTest(int prop)
            {
                Prop = prop;
            }

            public static NamespaceResolutionTargetTest Foo(int prop)
            {
                var p = new NamespaceResolutionTargetTest(prop);
                return p;
            }

            public int Prop { get; set; }



        }
    }
}
