// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 13.12.2023 16:31
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biss.Dc.Core;
using Biss.Dc.Core.DcChat;
using Biss.Dc.Server.DcChat;
using Biss.Log.Producer;
using IXchange.Service.AppConnectivity.DataConnector;
using IXchangeDatabase;
using Microsoft.Extensions.Logging;


// ReSharper disable once CheckNamespace
namespace ConnectivityHost.DataConnector.Chat
{
    /// <summary>
    /// <para>DcChat</para>
    /// Klasse DcChat. (C) 2022 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class DcChat : DcChatServerBase
    {
        /// <summary>
        /// DcChat
        /// </summary>
        /// <param name="src"></param>
        // ReSharper disable once UnusedParameter.Local
        public DcChat(ServerRemoteCalls src)
        {
        }

        /// <inheritdoc />
        public override async Task<IDcChatUser> GetUser(long userId)
        {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var db = new Db();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            return null!;
        }

        /// <inheritdoc />
        public override Task<(List<IDcChatUser>, List<long>)> GetChatUsers(long userId, List<DcSyncElement> currentUsers, DcEnumChatSyncMode chatSyncMode)
        {
            Logging.Log.LogWarning($"[{nameof(DcChat)}]({nameof(GetChatUsers)}): Chat noch nicht fertig!");
            return Task.FromResult((new List<IDcChatUser>(), new List<long>()));
        }


        /// <inheritdoc />
        public override Task<(List<IDcChat> chats, List<long> removeChats)> GetChats(long userId, List<DcSyncElement> currentChats, DcEnumChatSyncMode chatSyncMode)
        {
            Logging.Log.LogWarning($"[{nameof(DcChat)}]({nameof(GetChats)}): Chat noch nicht fertig!");
            return Task.FromResult((new List<IDcChat>(), new List<long>()));
        }

        /// <summary>Chat-Einträge</summary>
        /// <param name="userId"></param>
        /// <param name="currentChatEntries"></param>
        /// <param name="chatSyncMode"></param>
        /// <returns></returns>
        public override Task<(Dictionary<long, List<IDcChatEntry>> chatEntries, List<long> removeChatEntries)> GetChatEntries(long userId, Dictionary<long, List<DcSyncElement>> currentChatEntries, DcEnumChatSyncMode chatSyncMode)
        {
            Logging.Log.LogWarning($"[{nameof(DcChat)}]({nameof(GetChatEntries)}): Chat noch nicht fertig!");
            return Task.FromResult((new Dictionary<long, List<IDcChatEntry>>(), new List<long>()));
        }

        /// <inheritdoc />
        public override Task<DcChatData> Post(long deviceId, long userId, string msg, long? chatId = null, long? otherUserId = null, string chatName = "", string addData = "")
        {
            return Task.FromResult(new DcChatData());
        }
    }
}