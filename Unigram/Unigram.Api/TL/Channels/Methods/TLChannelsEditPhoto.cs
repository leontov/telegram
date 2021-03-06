// <auto-generated/>
using System;
using Telegram.Api.Native.TL;

namespace Telegram.Api.TL.Channels.Methods
{
	/// <summary>
	/// RCP method channels.editPhoto.
	/// Returns <see cref="Telegram.Api.TL.TLUpdatesBase"/>
	/// </summary>
	public partial class TLChannelsEditPhoto : TLObject
	{
		public TLInputChannelBase Channel { get; set; }
		public TLInputChatPhotoBase Photo { get; set; }

		public TLChannelsEditPhoto() { }
		public TLChannelsEditPhoto(TLBinaryReader from)
		{
			Read(from);
		}

		public override TLType TypeId { get { return TLType.ChannelsEditPhoto; } }

		public override void Read(TLBinaryReader from)
		{
			Channel = TLFactory.Read<TLInputChannelBase>(from);
			Photo = TLFactory.Read<TLInputChatPhotoBase>(from);
		}

		public override void Write(TLBinaryWriter to)
		{
			to.WriteObject(Channel);
			to.WriteObject(Photo);
		}
	}
}