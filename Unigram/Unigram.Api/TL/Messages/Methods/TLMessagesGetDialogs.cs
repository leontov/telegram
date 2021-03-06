// <auto-generated/>
using System;
using Telegram.Api.Native.TL;

namespace Telegram.Api.TL.Messages.Methods
{
	/// <summary>
	/// RCP method messages.getDialogs.
	/// Returns <see cref="Telegram.Api.TL.TLMessagesDialogs"/>
	/// </summary>
	public partial class TLMessagesGetDialogs : TLObject
	{
		[Flags]
		public enum Flag : Int32
		{
			ExcludePinned = (1 << 0),
		}

		public bool IsExcludePinned { get { return Flags.HasFlag(Flag.ExcludePinned); } set { Flags = value ? (Flags | Flag.ExcludePinned) : (Flags & ~Flag.ExcludePinned); } }

		public Flag Flags { get; set; }
		public Int32 OffsetDate { get; set; }
		public Int32 OffsetId { get; set; }
		public TLInputPeerBase OffsetPeer { get; set; }
		public Int32 Limit { get; set; }

		public TLMessagesGetDialogs() { }
		public TLMessagesGetDialogs(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.MessagesGetDialogs; } }

		public override void Read(TLBinaryReader from)
		{
			Flags = (Flag)from.ReadInt32();
			OffsetDate = from.ReadInt32();
			OffsetId = from.ReadInt32();
			OffsetPeer = TLFactory.Read<TLInputPeerBase>(from);
			Limit = from.ReadInt32();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.WriteInt32((Int32)Flags);
			to.WriteInt32(OffsetDate);
			to.WriteInt32(OffsetId);
			to.WriteObject(OffsetPeer);
			to.WriteInt32(Limit);
		}
	}
}