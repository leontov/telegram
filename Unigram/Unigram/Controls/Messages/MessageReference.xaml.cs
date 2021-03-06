﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Telegram.Api.Services.Cache;
using Telegram.Api.Helpers;
using Telegram.Api.TL;
using Unigram.Common;
using Unigram.Converters;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Unigram.Controls.Messages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessageReference : HyperlinkButton
    {
        public MessageReference()
        {
            InitializeComponent();
        }

        //public string Title
        //{
        //    get
        //    {
        //        return TitleLabel.Text;
        //    }
        //    set
        //    {
        //        TitleLabel.Text = value;
        //    }
        //}

        public string Title { get; set; }

        #region Message

        public object Message
        {
            get { return (object)GetValue(MessageProperty); }
            //set { SetValue(MessageProperty, value); }
            set
            {
                // TODO: shitty hack!!!
                var oldValue = (object)GetValue(MessageProperty);
                SetValue(MessageProperty, value);

                if (oldValue == value)
                {
                    SetTemplateCore(value);
                }
            }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(object), typeof(MessageReference), new PropertyMetadata(null, OnMessageChanged));

        private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageReference)d).SetTemplateCore((object)e.NewValue);
        }

        #endregion

        private bool SetTemplateCore(object item)
        {
            if (item == null)
            {
                return SetUnsupportedTemplate(null, null);
            }

            var replyInfo = item as ReplyInfo;
            if (replyInfo == null)
            {
                if (item is TLMessageBase)
                {
                    return GetMessageTemplate(item as TLObject);
                }

                return SetUnsupportedTemplate(null, null);
            }
            else
            {
                if (replyInfo.Reply == null)
                {
                    //return ReplyLoadingTemplate;
                }

                var contain = replyInfo.Reply as TLMessagesContainter;
                if (contain != null)
                {
                    return GetMessagesContainerTemplate(contain);
                }

                if (replyInfo.ReplyToMsgId == null || replyInfo.ReplyToMsgId.Value == 0)
                {
                    return SetUnsupportedTemplate(null, null);
                }

                return GetMessageTemplate(replyInfo.Reply);
            }
        }

        #region Container

        private bool GetMessagesContainerTemplate(TLMessagesContainter container)
        {
            //if (container.WebPageMedia != null)
            //{
            //    var webpageMedia = container.WebPageMedia as TLMessageMediaWebPage;
            //    if (webpageMedia != null)
            //    {
            //        var pendingWebpage = webpageMedia.Webpage as TLWebPagePending;
            //        if (pendingWebpage != null)
            //        {
            //            return WebPagePendingTemplate;
            //        }

            //        var webpage = webpageMedia.Webpage as TLWebPage;
            //        if (webpage != null)
            //        {
            //            return WebPageTemplate;
            //        }

            //        var emptyWebpage = webpageMedia.Webpage as TLWebPageEmpty;
            //        if (emptyWebpage != null)
            //        {
            //            return WebPageEmptyTemplate;
            //        }
            //    }
            //}

            if (container.FwdMessages != null)
            {
                if (container.FwdMessages.Count == 1)
                {
                    var forwardMessage = container.FwdMessages[0];
                    if (forwardMessage != null)
                    {
                        if (!string.IsNullOrEmpty(forwardMessage.Message) && (forwardMessage.Media == null || forwardMessage.Media is TLMessageMediaEmpty || forwardMessage.Media is TLMessageMediaWebPage))
                        {
                            return SetTextTemplate(forwardMessage, "forward");
                        }

                        var media = container.FwdMessages[0].Media;
                        if (media != null)
                        {
                            switch (media.TypeId)
                            {
                                case TLType.MessageMediaPhoto:
                                    return SetPhotoTemplate(forwardMessage, "forward");
                                case TLType.MessageMediaGeo:
                                    return SetGeoTemplate(forwardMessage, "forward");
                                case TLType.MessageMediaGeoLive:
                                    return SetGeoLiveTemplate(forwardMessage, "forward");
                                case TLType.MessageMediaVenue:
                                    return SetVenueTemplate(forwardMessage, "forward");
                                case TLType.MessageMediaContact:
                                    return SetContactTemplate(forwardMessage, "forward");
                                case TLType.MessageMediaGame:
                                    return SetGameTemplate(forwardMessage, "forward");
                                case TLType.MessageMediaEmpty:
                                    return SetUnsupportedTemplate(forwardMessage, "forward");
                                case TLType.MessageMediaDocument:
                                    if (forwardMessage.IsSticker())
                                    {
                                        return SetStickerTemplate(forwardMessage, "forward");
                                    }
                                    else if (forwardMessage.IsGif())
                                    {
                                        return SetGifTemplate(forwardMessage, "forward");
                                    }
                                    else if (forwardMessage.IsVoice())
                                    {
                                        return SetVoiceMessageTemplate(forwardMessage, "forward");
                                    }
                                    else if (forwardMessage.IsVideo())
                                    {
                                        return SetVideoTemplate(forwardMessage, "forward");
                                    }
                                    else if (forwardMessage.IsRoundVideo())
                                    {
                                        return SetRoundVideoTemplate(forwardMessage, "forward");
                                    }
                                    else if (forwardMessage.IsAudio())
                                    {
                                        return SetAudioTemplate(forwardMessage, "forward");
                                    }

                                    return SetDocumentTemplate(forwardMessage, "forward");
                                case TLType.MessageMediaUnsupported:
                                    return SetUnsupportedMediaTemplate(forwardMessage, "forward");
                            }
                        }
                    }
                }

                return SetForwardedMessagesTemplate(container.FwdMessages);
            }

            if (container.EditMessage != null)
            {
                var editMessage = container.EditMessage;
                if (editMessage != null)
                {
                    if (!string.IsNullOrEmpty(editMessage.Message) && (editMessage.Media == null || editMessage.Media is TLMessageMediaEmpty || editMessage.Media is TLMessageMediaWebPage))
                    {
                        return SetTextTemplate(editMessage, Strings.Android.Edit);
                    }

                    var media = editMessage.Media;
                    if (media != null)
                    {
                        switch (media.TypeId)
                        {
                            case TLType.MessageMediaPhoto:
                                return SetPhotoTemplate(editMessage, Strings.Android.Edit);
                            case TLType.MessageMediaGeo:
                                return SetGeoTemplate(editMessage, Strings.Android.Edit);
                            case TLType.MessageMediaGeoLive:
                                return SetGeoLiveTemplate(editMessage, Strings.Android.Edit);
                            case TLType.MessageMediaVenue:
                                return SetVenueTemplate(editMessage, Strings.Android.Edit);
                            case TLType.MessageMediaContact:
                                return SetContactTemplate(editMessage, Strings.Android.Edit);
                            case TLType.MessageMediaGame:
                                return SetGameTemplate(editMessage, Strings.Android.Edit);
                            case TLType.MessageMediaEmpty:
                                return SetUnsupportedTemplate(editMessage, Strings.Android.Edit);
                            case TLType.MessageMediaDocument:
                                if (editMessage.IsSticker())
                                {
                                    return SetStickerTemplate(editMessage, Strings.Android.Edit);
                                }
                                else if (editMessage.IsGif())
                                {
                                    return SetGifTemplate(editMessage, Strings.Android.Edit);
                                }
                                else if (editMessage.IsVoice())
                                {
                                    return SetVoiceMessageTemplate(editMessage, Strings.Android.Edit);
                                }
                                else if (editMessage.IsVideo())
                                {
                                    return SetVideoTemplate(editMessage, Strings.Android.Edit);
                                }
                                else if (editMessage.IsRoundVideo())
                                {
                                    return SetRoundVideoTemplate(editMessage, Strings.Android.Edit);
                                }
                                else if (editMessage.IsAudio())
                                {
                                    return SetAudioTemplate(editMessage, Strings.Android.Edit);
                                }

                                return SetDocumentTemplate(editMessage, Strings.Android.Edit);
                            case TLType.MessageMediaUnsupported:
                                return SetUnsupportedMediaTemplate(editMessage, Strings.Android.Edit);
                        }
                    }
                }

                return SetUnsupportedTemplate(editMessage, Strings.Android.Edit);
            }

            return SetUnsupportedTemplate(null, Strings.Android.Edit);
        }

        #endregion

        #region Reply

        private bool GetMessageTemplate(TLObject obj)
        {
            Visibility = Visibility.Collapsed;

            var message = obj as TLMessage;
            if (message != null)
            {
                if (!string.IsNullOrEmpty(message.Message) && (message.Media == null || message.Media is TLMessageMediaEmpty))
                {
                    return SetTextTemplate(message, Title);
                }

                var media = message.Media;
                if (media != null)
                {
                    switch (media.TypeId)
                    {
                        case TLType.MessageMediaPhoto:
                            return SetPhotoTemplate(message, Title);
                        case TLType.MessageMediaGeo:
                            return SetGeoTemplate(message, Title);
                        case TLType.MessageMediaGeoLive:
                            return SetGeoLiveTemplate(message, Title);
                        case TLType.MessageMediaVenue:
                            return SetVenueTemplate(message, Title);
                        case TLType.MessageMediaContact:
                            return SetContactTemplate(message, Title);
                        case TLType.MessageMediaGame:
                            return SetGameTemplate(message, Title);
                        case TLType.MessageMediaEmpty:
                            return SetUnsupportedTemplate(message, Title);
                        case TLType.MessageMediaWebPage:
                            return SetWebPageTemplate(message, Title);
                        case TLType.MessageMediaDocument:
                            if (message.IsSticker())
                            {
                                return SetStickerTemplate(message, Title);
                            }
                            else if (message.IsGif())
                            {
                                return SetGifTemplate(message, Title);
                            }
                            else if (message.IsVoice())
                            {
                                return SetVoiceMessageTemplate(message, Title);
                            }
                            else if (message.IsVideo())
                            {
                                return SetVideoTemplate(message, Title);
                            }
                            else if (message.IsRoundVideo())
                            {
                                return SetRoundVideoTemplate(message, Title);
                            }
                            else if (message.IsAudio())
                            {
                                return SetAudioTemplate(message, Title);
                            }

                            return SetDocumentTemplate(message, Title);
                        case TLType.MessageMediaUnsupported:
                            return SetUnsupportedMediaTemplate(message, Title);
                    }
                }
            }

            var serviceMessage = obj as TLMessageService;
            if (serviceMessage != null)
            {
                var action = serviceMessage.Action;
                if (action is TLMessageActionChatEditPhoto)
                {
                    return SetServicePhotoTemplate(serviceMessage, Title);
                }

                return SetServiceTextTemplate(serviceMessage, Title);
            }
            else
            {
                var emptyMessage = obj as TLMessageEmpty;
                if (emptyMessage != null)
                {
                    return SetEmptyTemplate(emptyMessage, Title);
                }

                return SetUnsupportedTemplate(message, Title);
            }
        }

        private bool SetForwardedMessagesTemplate(TLVector<TLMessage> messages)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = string.Empty;
            ServiceLabel.Text = LocaleHelper.Declension("ForwardedMessageCount",  messages.Count);
            MessageLabel.Text = string.Empty;

            var users = messages.Select(x => x.From).Distinct(new EqualityComparerDelegate<TLUser>((x, y) => x.Id == y.Id)).ToList();
            if (users.Count > 2)
            {
                TitleLabel.Text = users[0].FullName + LocaleHelper.Declension("AndOther", users.Count);
            }
            else if (users.Count == 2)
            {
                TitleLabel.Text = $"{users[0].FullName}, {users[1].FullName}";
            }
            else if (users.Count == 1)
            {
                TitleLabel.Text = users[0].FullName;
            }

            return true;
        }

        private bool SetTextTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = string.Empty;
            MessageLabel.Text = message.Message.Replace("\r\n", "\n").Replace('\n', ' ');

            return true;
        }

        private bool SetPhotoTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            // 🖼

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachPhoto;
            MessageLabel.Text = string.Empty;

            if (message.Media is TLMessageMediaPhoto photoMedia)
            {
                if (photoMedia.HasTTLSeconds)
                {
                    if (ThumbRoot != null)
                        ThumbRoot.Visibility = Visibility.Collapsed;
                }
                else
                {
                    FindName(nameof(ThumbRoot));
                    if (ThumbRoot != null)
                        ThumbRoot.Visibility = Visibility.Visible;

                    ThumbRoot.CornerRadius = ThumbEllipse.CornerRadius = default(CornerRadius);
                    ThumbImage.ImageSource = (ImageSource)DefaultPhotoConverter.Convert(photoMedia, true);
                }

                if (!string.IsNullOrWhiteSpace(photoMedia.Caption))
                {
                    ServiceLabel.Text += ", ";
                    MessageLabel.Text += photoMedia.Caption.Replace("\r\n", "\n").Replace('\n', ' ');
                }
            }

            return true;
        }

        private bool SetGeoTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachLocation;
            MessageLabel.Text = string.Empty;

            return true;
        }

        private bool SetGeoLiveTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachLiveLocation;
            MessageLabel.Text = string.Empty;

            return true;
        }

        private bool SetVenueTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachLocation;
            MessageLabel.Text = string.Empty;

            var venueMedia = message.Media as TLMessageMediaVenue;
            if (venueMedia != null && !string.IsNullOrWhiteSpace(venueMedia.Title))
            {
                ServiceLabel.Text += ", ";
                MessageLabel.Text = venueMedia.Title.Replace("\r\n", "\n").Replace('\n', ' ');
            }

            return true;
        }

        private bool SetGameTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            FindName(nameof(ThumbRoot));
            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Visible;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = $"🎮 {Strings.Android.AttachGame}";
            MessageLabel.Text = string.Empty;

            var gameMedia = message.Media as TLMessageMediaGame;
            if (gameMedia != null && gameMedia.Game != null)
            {
                ServiceLabel.Text = $"🎮 {gameMedia.Game.Title}";

                ThumbRoot.CornerRadius = ThumbEllipse.CornerRadius = default(CornerRadius);
                ThumbImage.ImageSource = (ImageSource)DefaultPhotoConverter.Convert(gameMedia.Game.Photo, true);
            }

            return true;
        }

        private bool SetContactTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachContact;
            MessageLabel.Text = string.Empty;

            return true;
        }

        private bool SetAudioTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachAudio;
            MessageLabel.Text = string.Empty;

            var documentMedia = message.Media as TLMessageMediaDocument;
            if (documentMedia != null)
            {
                var document = documentMedia.Document as TLDocument;
                if (document != null)
                {
                    ServiceLabel.Text = document.Title;
                }

                if (!string.IsNullOrWhiteSpace(documentMedia.Caption))
                {
                    ServiceLabel.Text += ", ";
                    MessageLabel.Text += documentMedia.Caption.Replace('\n', ' ');
                }
            }

            return true;
        }

        private bool SetVoiceMessageTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachAudio;
            MessageLabel.Text = string.Empty;

            var documentMedia = message.Media as TLMessageMediaDocument;
            if (documentMedia != null)
            {
                if (!string.IsNullOrWhiteSpace(documentMedia.Caption))
                {
                    ServiceLabel.Text += ", ";
                    MessageLabel.Text += documentMedia.Caption.Replace("\r\n", "\n").Replace('\n', ' ');
                }
            }

            return true;
        }

        private bool SetWebPageTemplate(TLMessage message, string title)
        {
            var webPageMedia = message.Media as TLMessageMediaWebPage;
            if (webPageMedia != null)
            {
                var webPage = webPageMedia.WebPage as TLWebPage;
                if (webPage != null && webPage.Photo != null && webPage.Type != null)
                {
                    Visibility = Visibility.Visible;

                    FindName(nameof(ThumbRoot));
                    if (ThumbRoot != null)
                        ThumbRoot.Visibility = Visibility.Visible;

                    TitleLabel.Text = GetFromLabel(message, title);
                    ServiceLabel.Text = string.Empty;
                    MessageLabel.Text = message.Message.Replace("\r\n", "\n").Replace('\n', ' ');

                    ThumbRoot.CornerRadius = ThumbEllipse.CornerRadius = default(CornerRadius);
                    ThumbImage.ImageSource = (ImageSource)DefaultPhotoConverter.Convert(webPage.Photo, true);
                }
                else
                {
                    return SetTextTemplate(message, title);
                }
            }

            return true;
        }

        private bool SetVideoTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachVideo;
            MessageLabel.Text = string.Empty;

            if (message.Media is TLMessageMediaDocument documentMedia)
            {
                if (documentMedia.HasTTLSeconds)
                {
                    if (ThumbRoot != null)
                        ThumbRoot.Visibility = Visibility.Collapsed;
                }
                else
                {
                    FindName(nameof(ThumbRoot));
                    if (ThumbRoot != null)
                        ThumbRoot.Visibility = Visibility.Visible;

                    ThumbRoot.CornerRadius = ThumbEllipse.CornerRadius = default(CornerRadius);
                    ThumbImage.ImageSource = (ImageSource)DefaultPhotoConverter.Convert(documentMedia.Document, true);
                }

                if (!string.IsNullOrWhiteSpace(documentMedia.Caption))
                {
                    ServiceLabel.Text += ", ";
                    MessageLabel.Text += documentMedia.Caption.Replace("\r\n", "\n").Replace('\n', ' ');
                }
            }

            return true;
        }

        private bool SetRoundVideoTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            FindName(nameof(ThumbRoot));
            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Visible;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachRound;
            MessageLabel.Text = string.Empty;

            var documentMedia = message.Media as TLMessageMediaDocument;
            if (documentMedia != null)
            {
                if (!string.IsNullOrWhiteSpace(documentMedia.Caption))
                {
                    ServiceLabel.Text += ", ";
                    MessageLabel.Text += documentMedia.Caption.Replace("\r\n", "\n").Replace('\n', ' ');
                }

                ThumbRoot.CornerRadius = ThumbEllipse.CornerRadius = new CornerRadius(18);
                ThumbImage.ImageSource = (ImageSource)DefaultPhotoConverter.Convert(documentMedia.Document, true);
            }

            return true;
        }

        private bool SetGifTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            FindName(nameof(ThumbRoot));
            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Visible;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachGif;
            MessageLabel.Text = string.Empty;

            var documentMedia = message.Media as TLMessageMediaDocument;
            if (documentMedia != null)
            {
                if (!string.IsNullOrWhiteSpace(documentMedia.Caption))
                {
                    ServiceLabel.Text += ", ";
                    MessageLabel.Text += documentMedia.Caption.Replace("\r\n", "\n").Replace('\n', ' ');
                }

                ThumbRoot.CornerRadius = ThumbEllipse.CornerRadius = default(CornerRadius);
                ThumbImage.ImageSource = (ImageSource)DefaultPhotoConverter.Convert(documentMedia.Document, true);
            }

            return true;
        }

        private bool SetStickerTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.AttachSticker;
            MessageLabel.Text = string.Empty;

            var documentMedia = message.Media as TLMessageMediaDocument;
            if (documentMedia != null)
            {
                var document = documentMedia.Document as TLDocument;
                if (document != null)
                {
                    var attribute = document.Attributes.OfType<TLDocumentAttributeSticker>().FirstOrDefault();
                    if (attribute != null)
                    {
                        if (!string.IsNullOrEmpty(attribute.Alt))
                        {
                            ServiceLabel.Text = $"{attribute.Alt} {Strings.Android.AttachSticker}";
                        }
                    }
                }
            }

            return true;
        }

        private bool SetDocumentTemplate(TLMessage message, string title)
        {
            var documentMedia = message.Media as TLMessageMediaDocument;
            if (documentMedia != null)
            {
                var document = documentMedia.Document as TLDocument;
                if (document != null)
                {
                    var photoSize = document.Thumb as TLPhotoSize;
                    var photoCachedSize = document.Thumb as TLPhotoCachedSize;
                    if (photoCachedSize != null || photoSize != null)
                    {
                        Visibility = Visibility.Visible;

                        FindName(nameof(ThumbRoot));
                        if (ThumbRoot != null)
                            ThumbRoot.Visibility = Visibility.Visible;

                        ThumbRoot.CornerRadius = ThumbEllipse.CornerRadius = default(CornerRadius);
                        ThumbImage.ImageSource = (ImageSource)DefaultPhotoConverter.Convert(documentMedia.Document, true);
                    }
                    else
                    {
                        Visibility = Visibility.Visible;

                        if (ThumbRoot != null)
                            ThumbRoot.Visibility = Visibility.Collapsed;
                    }

                    TitleLabel.Text = GetFromLabel(message, title);
                    ServiceLabel.Text = document.FileName;
                    MessageLabel.Text = string.Empty;

                    if (!string.IsNullOrWhiteSpace(documentMedia.Caption))
                    {
                        ServiceLabel.Text += ", ";
                        MessageLabel.Text += documentMedia.Caption.Replace("\r\n", "\n").Replace('\n', ' ');
                    }
                }
            }
            return true;
        }

        private bool SetServiceTextTemplate(TLMessageService message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = string.Empty;
            MessageLabel.Text = ServiceHelper.Convert(message);

            return true;
        }

        private bool SetServicePhotoTemplate(TLMessageService message, string title)
        {
            Visibility = Visibility.Visible;

            FindName(nameof(ThumbRoot));
            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Visible;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = string.Empty;
            MessageLabel.Text = ServiceHelper.Convert(message);

            var action = message.Action as TLMessageActionChatEditPhoto;
            if (action != null)
            {
                ThumbRoot.CornerRadius = ThumbEllipse.CornerRadius = default(CornerRadius);
                ThumbImage.ImageSource = (ImageSource)DefaultPhotoConverter.Convert(action.Photo, true);
            }

            return true;
        }

        private bool SetEmptyTemplate(TLMessageBase message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = string.Empty;
            ServiceLabel.Text = message is TLMessageEmpty ? Strings.Resources.DeletedMessage : string.Empty;
            MessageLabel.Text = string.Empty;
            return true;
        }

        private bool SetUnsupportedMediaTemplate(TLMessage message, string title)
        {
            Visibility = Visibility.Visible;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = GetFromLabel(message, title);
            ServiceLabel.Text = Strings.Android.UnsupportedAttachment;
            MessageLabel.Text = string.Empty;

            return true;
        }

        private bool SetUnsupportedTemplate(TLMessageBase message, string title)
        {
            Visibility = Visibility.Collapsed;

            if (ThumbRoot != null)
                ThumbRoot.Visibility = Visibility.Collapsed;

            TitleLabel.Text = string.Empty;
            ServiceLabel.Text = string.Empty;
            MessageLabel.Text = string.Empty;
            return false;
        }

        #endregion

        //private string GetFromLabel(TLMessage message, string title)
        //{
        //    return GetFromLabelInternal(message, title) + Environment.NewLine;
        //}

        private string GetFromLabel(TLMessage message, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (title.Equals("forward"))
                {
                    if (message.FwdFromChannel is TLChannel channel)
                    {
                        return channel.Title;
                    }
                    else if (message.FwdFromUser is TLUser user)
                    {
                        return user.FullName;
                    }
                }
                else
                {
                    return title;
                }
            }

            if (message.IsPost && (message.ToId is TLPeerChat || message.ToId is TLPeerChannel))
            {
                return message.Parent?.DisplayName ?? string.Empty;
            }
            else if (message.IsSaved() && message.FwdFromUser is TLUser user)
            {
                return user.FullName;
            }

            var from = message.From?.FullName ?? string.Empty;
            if (message.ViaBot != null && message.FwdFrom == null)
            {
                from += $" via @{message.ViaBot.Username}";
            }

            return from;
        }

        //private string GetFromLabel(TLMessageService message, string title)
        //{
        //    return GetFromLabelInternal(message, title) + Environment.NewLine;
        //}

        private string GetFromLabel(TLMessageService message, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                return Title;
            }

            if (message.IsPost && (message.ToId is TLPeerChat || message.ToId is TLPeerChannel))
            {
                return message.Parent?.DisplayName ?? string.Empty;
            }
            else
            {
                return message.From?.FullName ?? string.Empty;
            }
        }

        //#region Cursor

        //// Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);

        //protected override void OnPointerEntered(PointerRoutedEventArgs e)
        //{
        //    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
        //    base.OnPointerEntered(e);
        //}

        //protected override void OnPointerExited(PointerRoutedEventArgs e)
        //{
        //    Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        //    base.OnPointerExited(e);
        //}

        //#endregion
    }
}
