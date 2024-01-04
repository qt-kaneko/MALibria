using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MALibria.Extensions;

static class IEnumerableExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source,
                                                                   Func<TSource, TResult?> selector)
  where TResult: struct
  {
    return source.Select(selector).Where(result => result != null)!.Cast<TResult>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source,
                                                                   Func<TSource, TResult?> selector)
  where TResult: class
  {
    return source.Select(selector).Where(result => result != null)!;
  }
}