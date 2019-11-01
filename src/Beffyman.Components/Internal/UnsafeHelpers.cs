using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Beffyman.Components.Internal
{
	internal static unsafe class UnsafeHelpers
	{
		public static ReadOnlySpan<byte> AsReadOnlySpan<T>(ref T val) where T : unmanaged
		{
			ReadOnlySpan<T> valSpan = MemoryMarshal.CreateReadOnlySpan(ref val, sizeof(T));
			return MemoryMarshal.Cast<T, byte>(valSpan);
		}

		public static Span<byte> AsSpan<T>(ref T val) where T : unmanaged
		{
			Span<T> valSpan = MemoryMarshal.CreateSpan(ref val, sizeof(T));
			return MemoryMarshal.Cast<T, byte>(valSpan);
		}
	}
}
