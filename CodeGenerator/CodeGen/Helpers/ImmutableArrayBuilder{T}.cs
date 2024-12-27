using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CodeGen.Helpers;

/// <summary>
/// A helper type to build sequences of values with pooled buffers.
/// </summary>
/// <typeparam name="T">The type of items to create sequences for.</typeparam>
internal struct ImmutableArrayBuilder<T>
{
  /// <summary>
  /// The rented <see cref="Writer"/> instance to use.
  /// </summary>
  private Writer? _writer;

  /// <summary>
  /// Creates a new <see cref="ImmutableArrayBuilder{T}"/> object with the specified parameters.
  /// </summary>
  /// <param name="writer">The target data writer to use.</param>
  private ImmutableArrayBuilder(Writer writer)
  {
    _writer = writer;
  }

  /// <inheritdoc cref="ImmutableArray{T}.Builder.Count"/>
  public readonly int Count
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _writer!.Count;
  }

  /// <summary>
  /// Gets the data written to the underlying buffer so far, as a <see cref="ReadOnlySpan{T}"/>.
  /// </summary>
  [UnscopedRef]
  public readonly ReadOnlySpan<T> WrittenSpan
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get => _writer!.WrittenSpan;
  }

  /// <summary>
  /// Creates a <see cref="ImmutableArrayBuilder{T}"/> value with a pooled underlying data writer.
  /// </summary>
  /// <returns>A <see cref="ImmutableArrayBuilder{T}"/> instance to write data to.</returns>
  public static ImmutableArrayBuilder<T> Rent()
  {
    return new(new Writer());
  }

  /// <inheritdoc cref="ImmutableArray{T}.Builder.Add(T)"/>
  public readonly void Add(T item)
  {
    _writer!.Add(item);
  }

  /// <summary>
  /// Adds the specified items to the end of the array.
  /// </summary>
  /// <param name="items">The items to add at the end of the array.</param>
  public readonly void AddRange(scoped ReadOnlySpan<T> items)
  {
    _writer!.AddRange(items);
  }

  /// <summary>
  /// Gets an <see cref="IEnumerable{T}"/> instance for the current builder.
  /// </summary>
  /// <returns>An <see cref="IEnumerable{T}"/> instance for the current builder.</returns>
  /// <remarks>
  /// The builder should not be mutated while an enumerator is in use.
  /// </remarks>
  public readonly IEnumerable<T> AsEnumerable()
  {
    return _writer!;
  }

  /// <inheritdoc cref="IDisposable.Dispose"/>
  public void Dispose()
  {
    Writer? writer = _writer;

    _writer = null;

    writer?.Dispose();
  }

  /// <inheritdoc cref="ImmutableArray{T}.Builder.ToArray"/>
  public readonly T[] ToArray()
  {
    return _writer!.WrittenSpan.ToArray();
  }

  /// <inheritdoc cref="ImmutableArray{T}.Builder.ToImmutable"/>
  public readonly ImmutableArray<T> ToImmutable()
  {
    T[] array = _writer!.WrittenSpan.ToArray();

    return Unsafe.As<T[], ImmutableArray<T>>(ref array);
  }

  /// <inheritdoc/>
  public override readonly string ToString()
  {
    return _writer!.WrittenSpan.ToString();
  }

  /// <summary>
  /// A class handling the actual buffer writing.
  /// </summary>
  private sealed class Writer : ICollection<T>, IDisposable
  {
    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    private T?[]? _array;

    /// <summary>
    /// The starting offset within <see cref="_array"/>.
    /// </summary>
    private int _index;

    /// <summary>
    /// Creates a new <see cref="Writer"/> instance with the specified parameters.
    /// </summary>
    public Writer()
    {
      _array = ArrayPool<T?>.Shared.Rent(typeof(T) == typeof(char) ? 1024 : 8);
      _index = 0;
    }

    /// <inheritdoc cref="ImmutableArrayBuilder{T}.Count"/>
    public int Count
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _index;
    }

    /// <inheritdoc cref="ImmutableArrayBuilder{T}.WrittenSpan"/>
    public ReadOnlySpan<T> WrittenSpan
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => new(_array!, 0, _index);
    }

    /// <inheritdoc/>
    bool ICollection<T>.IsReadOnly => true;

    /// <inheritdoc cref="ImmutableArrayBuilder{T}.Add"/>
    public void Add(T value)
    {
      EnsureCapacity(1);

      _array![_index++] = value;
    }

    /// <inheritdoc cref="ImmutableArrayBuilder{T}.AddRange"/>
    public void AddRange(ReadOnlySpan<T> items)
    {
      EnsureCapacity(items.Length);

      items.CopyTo(_array.AsSpan(_index)!);

      _index += items.Length;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      T?[]? array = _array;

      _array = null;

      if (array is not null)
      {
        ArrayPool<T?>.Shared.Return(array, clearArray: typeof(T) != typeof(char));
      }
    }

    /// <inheritdoc/>
    void ICollection<T>.Clear()
    {
      throw new NotSupportedException();
    }

    /// <inheritdoc/>
    bool ICollection<T>.Contains(T item)
    {
      throw new NotSupportedException();
    }

    /// <inheritdoc/>
    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
      Array.Copy(_array!, 0, array, arrayIndex, _index);
    }

    /// <inheritdoc/>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      T?[] array = _array!;
      int length = _index;

      for (int i = 0; i < length; i++)
      {
        yield return array[i]!;
      }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<T>)this).GetEnumerator();
    }

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Ensures that <see cref="_array"/> has enough free space to contain a given number of new items.
    /// </summary>
    /// <param name="requestedSize">The minimum number of items to ensure space for in <see cref="_array"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity(int requestedSize)
    {
      if (requestedSize > _array!.Length - _index)
      {
        ResizeBuffer(requestedSize);
      }
    }

    /// <summary>
    /// Resizes <see cref="_array"/> to ensure it can fit the specified number of new items.
    /// </summary>
    /// <param name="sizeHint">The minimum number of items to ensure space for in <see cref="_array"/>.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ResizeBuffer(int sizeHint)
    {
      int minimumSize = _index + sizeHint;

      T?[] oldArray = _array!;
      T?[] newArray = ArrayPool<T?>.Shared.Rent(minimumSize);

      Array.Copy(oldArray, newArray, _index);

      _array = newArray;

      ArrayPool<T?>.Shared.Return(oldArray, clearArray: typeof(T) != typeof(char));
    }
  }
}
