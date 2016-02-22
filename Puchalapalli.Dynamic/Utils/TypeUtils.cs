/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/
 
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System;

namespace Puchalapalli.Dynamic.Utils
{
    
    internal static class TypeUtils
    {
#if CLR2 || SILVERLIGHT
            
        internal static bool AreEquivalent(Type t1, Type t2) => t1 == t2;
#else
        internal static bool AreEquivalent(Type t1, Type t2) => t1 == t2 || t1.IsEquivalentTo(t2);
#endif
    }
}
