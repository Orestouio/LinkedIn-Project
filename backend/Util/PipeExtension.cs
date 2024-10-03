using System.Collections;
using System;
using System.Collections.Generic;


namespace Utilities{
    public static class PipeExtension{
        public delegate V PipelineMember<T,V>(T arg);
        public static V Pipe<T,V>(this T piped, PipelineMember<T,V> pipeFun) =>
            pipeFun(piped);
    }
}