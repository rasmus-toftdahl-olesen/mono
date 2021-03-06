//
// support.cs: Support routines to work around the fact that System.Reflection.Emit
// can not introspect types that are being constructed
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2001 Ximian, Inc (http://www.ximian.com)
// Copyright 2003-2009 Novell, Inc
//

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;
using System.Collections.Generic;

namespace Mono.CSharp {

	sealed class ReferenceEquality<T> : IEqualityComparer<T> where T : class
	{
		public static readonly IEqualityComparer<T> Default = new ReferenceEquality<T> ();

		private ReferenceEquality ()
		{
		}

		public bool Equals (T x, T y)
		{
			return ReferenceEquals (x, y);
		}

		public int GetHashCode (T obj)
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode (obj);
		}
	}

	public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
	{
		public Tuple (T1 item1, T2 item2)
		{
			Item1 = item1;
			Item2 = item2;
		}

		public T1 Item1 { get; private set; }
		public T2 Item2 { get; private set; }

		public override int GetHashCode ()
		{
			return Item1.GetHashCode () ^ Item2.GetHashCode ();
		}

		#region IEquatable<Tuple<T1,T2>> Members

		public bool Equals (Tuple<T1, T2> other)
		{
			return EqualityComparer<T1>.Default.Equals (Item1, other.Item1) &&
				EqualityComparer<T2>.Default.Equals (Item2, other.Item2);
		}

		#endregion
	}

	static class Tuple
	{
		public static Tuple<T1, T2> Create<T1, T2> (T1 item1, T2 item2)
		{
			return new Tuple<T1, T2> (item1, item2);
		}
	}

	/// <summary>
	///   This is an arbitrarily seekable StreamReader wrapper.
	///
	///   It uses a self-tuning buffer to cache the seekable data,
	///   but if the seek is too far, it may read the underly
	///   stream all over from the beginning.
	/// </summary>
	public class SeekableStreamReader : IDisposable
	{
		const int buffer_read_length_spans = 3;

		TextReader reader;
		Stream stream;

		static char[] buffer;
		int average_read_length;
		int buffer_start;       // in chars
		int char_count;         // count buffer[] valid characters
		int pos;                // index into buffer[]

		public SeekableStreamReader (Stream stream, Encoding encoding)
		{
			this.stream = stream;

			const int default_average_read_length = 1024;
			InitializeStream (default_average_read_length);
			reader = new StreamReader (stream, encoding, true);
		}

		public void Dispose ()
		{
			// Needed to release stream reader buffers
			reader.Dispose ();
		}

		void InitializeStream (int read_length_inc)
		{
			average_read_length += read_length_inc;

			int required_buffer_size = average_read_length * buffer_read_length_spans;
			if (buffer == null || buffer.Length < required_buffer_size)
				buffer = new char [required_buffer_size];

			stream.Position = 0;			
			buffer_start = char_count = pos = 0;
		}

		/// <remarks>
		///   This value corresponds to the current position in a stream of characters.
		///   The StreamReader hides its manipulation of the underlying byte stream and all
		///   character set/decoding issues.  Thus, we cannot use this position to guess at
		///   the corresponding position in the underlying byte stream even though there is
		///   a correlation between them.
		/// </remarks>
		public int Position {
			get { return buffer_start + pos; }

			set {
				// If the lookahead was too small, re-read from the beginning.  Increase the buffer size while we're at it
				if (value < buffer_start)
					InitializeStream (average_read_length / 2);

				while (value > buffer_start + char_count) {
					pos = char_count;
					if (!ReadBuffer ())
						throw new InternalErrorException ("Seek beyond end of file: " + (buffer_start + char_count - value));
				}

				pos = value - buffer_start;
			}
		}

		private bool ReadBuffer ()
		{
			int slack = buffer.Length - char_count;
			if (slack <= average_read_length / 2) {
				// shift the buffer to make room for average_read_length number of characters
				int shift = average_read_length - slack;
				Array.Copy (buffer, shift, buffer, 0, char_count - shift);
				pos -= shift;
				char_count -= shift;
				buffer_start += shift;
				slack += shift;		// slack == average_read_length
			}

			char_count += reader.Read (buffer, char_count, slack);

			return pos < char_count;
		}

		public int Peek ()
		{
			if ((pos >= char_count) && !ReadBuffer ())
				return -1;

			return buffer [pos];
		}

		public int Read ()
		{
			if ((pos >= char_count) && !ReadBuffer ())
				return -1;

			return buffer [pos++];
		}
	}

	public class UnixUtils {
		[System.Runtime.InteropServices.DllImport ("libc", EntryPoint="isatty")]
		extern static int _isatty (int fd);
			
		public static bool isatty (int fd)
		{
			try {
				return _isatty (fd) == 1;
			} catch {
				return false;
			}
		}
	}

	/// <summary>
	///   An exception used to terminate the compiler resolution phase and provide completions
	/// </summary>
	/// <remarks>
	///   This is thrown when we want to return the completions or
	///   terminate the completion process by AST nodes used in
	///   the completion process.
	/// </remarks>
	public class CompletionResult : Exception {
		string [] result;
		string base_text;
		
		public CompletionResult (string base_text, string [] res)
		{
			if (base_text == null)
				throw new ArgumentNullException ("base_text");
			this.base_text = base_text;

			result = res;
			Array.Sort (result);
		}

		public string [] Result {
			get {
				return result;
			}
		}

		public string BaseText {
			get {
				return base_text;
			}
		}
	}
}
