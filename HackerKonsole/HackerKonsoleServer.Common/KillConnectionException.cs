/*
 */
using System;
using System.Runtime.Serialization;

namespace HackerKonsoleServer.Common
{
	/// <summary>
	/// An interrupt to kill the connection
	/// </summary>
	public class KillConnectionException : Exception, ISerializable
	{
		public KillConnectionException()
		{
		}

	 	public KillConnectionException(string message) : base(message)
		{
		}

		public KillConnectionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected KillConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}