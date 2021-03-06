// <auto-generated/>
using System;
using Telegram.Api.Native.TL;

namespace Telegram.Api.TL
{
	public partial class TLWallPaperSolid : TLWallPaperBase 
	{
		public Int32 BgColor { get; set; }

		public TLWallPaperSolid() { }
		public TLWallPaperSolid(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.WallPaperSolid; } }

		public override void Read(TLBinaryReader from)
		{
			Id = from.ReadInt32();
			Title = from.ReadString();
			BgColor = from.ReadInt32();
			Color = from.ReadInt32();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.WriteInt32(Id);
			to.WriteString(Title ?? string.Empty);
			to.WriteInt32(BgColor);
			to.WriteInt32(Color);
		}
	}
}