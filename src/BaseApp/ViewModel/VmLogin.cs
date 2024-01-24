// (C) 2024 FOTEC Forschungs- und Technologietransfer GmbH
// Das Forschungsunternehmen der Fachhochschule Wiener Neustadt
// 
// Kontakt biss@fotec.at / www.fotec.at
// 
// Erstversion vom 15.11.2023 10:55
// Entwickler      Mandl Matthias (MMa)
// Projekt         IXchange

using System;
using System.Linq;
using System.Threading.Tasks;
using BaseApp.Connectivity;
using BDA.Common.Exchange.Enum;
using BDA.Common.Exchange.Model.ConfigApp;
using Biss.Apps.Attributes;
using Biss.Apps.Base;
using Biss.Apps.Enum;
using Biss.Apps.Interfaces;
using Biss.Apps.ViewModel;
using Biss.Dc.Core;
using Biss.EMail;
using Biss.Log.Producer;
using Exchange.Resources;
using Microsoft.Extensions.Logging;
using PropertyChanged;

namespace BaseApp.ViewModel
{
    #region Hilfsklassen

    /// <summary>
    ///     States der View
    /// </summary>
    public enum EnumViewLoginStates
    {
        /// <summary>
        ///     Login wird angezigt
        /// </summary>
        Login,

        /// <summary>
        ///     Login und Passwort
        /// </summary>
        Password,

        /// <summary>
        ///     Passwort ist falsch - neues zusenden anzeigen
        /// </summary>
        PasswordWrong,

        /// <summary>
        ///     User hat seine E-Mail (noch) nicht validiert
        /// </summary>
        UserNotValidated,

        /// <summary>
        ///     Account ist gesperrt
        /// </summary>
        UserAccountLocked
    }

    #endregion

    /// <summary>
    ///     <para>Login</para>
    /// Klasse VmLogin. (C) 2021 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    [ViewName("ViewLogin")]
    public class VmLogin : VmProjectBase
    {
        /// <summary>
        ///     Design Instanz für XAML d:DataContext="{x:Static viewmodels:VmLogin.DesignInstance}"
        /// </summary>
        public static VmLogin DesignInstance = new VmLogin();

        /// <summary>
        /// User checken
        /// </summary>
        private DcCheckUserLoginResult? _checkUser;

        /// <summary>
        /// Derzeitiger Status
        /// </summary>
        private EnumViewLoginStates _currentState = EnumViewLoginStates.Login;

        /// <summary>
        ///     VmLogin
        /// </summary>
        public VmLogin() : base(ResViewLogin.LblTitle, subTitle: ResViewLogin.LblSubTitle)
        {
            SetViewProperties(true);

            EntryLoginName = new VmEntry(title: ResViewLogin.EntryTitleLoginName,
                placeholder: ResViewLogin.EntryPlaceholderLoginName,
                validateFunc: ValidateLoginName,
                returnAction: () =>
                {
                    EntryLoginName!.ValidateData();
                    if (!EntryLoginName.DataValid)
                    {
                        return;
                    }

                    if (IsPasswordEntryVisible)
                    {
                        EntryPassword!.Focus(EnumVmEntrySetFocusMode.FocusAndSelect);
                    }
                    else
                    {
                        CmdLogin.Execute(null!);
                    }
                },
                showTitle: false);
            EntryLoginName.ValidChanged += (sender, args) =>
            {
                if (!EntryLoginName.DataValid && _currentState != EnumViewLoginStates.Login)
                {
                    CurrentState = EnumViewLoginStates.Login;
                }

                CmdLogin.CanExecute();
            };

            EntryPassword = new VmEntry(title: ResViewLogin.EntryTitlePassword,
                returnAction: () =>
                {
                    EntryPassword!.ValidateData();
                    if (EntryPassword.DataValid)
                    {
                        CmdLogin.Execute(null!);
                    }
                },
                showTitle: false
            );
        }

        #region Properties

        /// <summary>
        ///     Im Web angemeldet bleiben
        /// </summary>
        public bool KeepLogin { get; set; } = true;

        /// <summary>
        ///     Aktueller State
        /// </summary>
        public EnumViewLoginStates CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState == value)
                {
                    return;
                }

                var nextState = value;
                switch (nextState)
                {
                    case EnumViewLoginStates.Login:
                        _checkUser = null;
                        CmdLogin.DisplayName = ResViewLogin.CmdContinue;
                        break;
                    case EnumViewLoginStates.Password:
                        CmdLogin.DisplayName = ResViewLogin.CmdLogin;
                        break;
                    case EnumViewLoginStates.PasswordWrong:
                        CmdLogin.DisplayName = ResViewLogin.CmdLogin;
                        break;
                    case EnumViewLoginStates.UserNotValidated:
                        CmdLogin.DisplayName = ResViewLogin.CmdContinue;
                        break;
                    case EnumViewLoginStates.UserAccountLocked:
                        _checkUser = null;
                        CmdLogin.DisplayName = ResViewLogin.CmdContinue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _currentState = nextState;
            }
        }


        /// <summary>
        ///     Passwort Eingabe
        /// </summary>
        [DependsOn(nameof(CurrentState))]
        public bool IsPasswordEntryVisible => CurrentState == EnumViewLoginStates.Password || CurrentState == EnumViewLoginStates.PasswordWrong;

        /// <summary>
        ///     E-Mail nicht bestätigt
        /// </summary>
        [DependsOn(nameof(CurrentState))]
        public bool IsResendAccessLinkVisible => CurrentState == EnumViewLoginStates.UserNotValidated;

        /// <summary>
        ///     Passwort zurücksetzen sichtbar
        /// </summary>
        [DependsOn(nameof(CurrentState))]
        public bool IsForgotPasswordVisible => CurrentState == EnumViewLoginStates.PasswordWrong;

        /// <summary>
        ///     Login Name
        /// </summary>
        public VmEntry EntryLoginName { get; set; }

        /// <summary>
        ///     Passwort
        /// </summary>
        public VmEntry EntryPassword { get; set; }

        /// <summary>
        ///     Login Command
        /// </summary>
        public VmCommand CmdLogin { get; set; } = null!;

        /// <summary>
        ///     Passwort vergessen Command
        /// </summary>
        public VmCommand CmdForgotPassword { get; set; } = null!;

        /// <summary>
        ///     Passwort vergessen Command
        /// </summary>
        public VmCommand CmdResendAccessLink { get; set; } = null!;

        #endregion

        /// <summary>
        ///     Commands Initialisieren (aufruf im Kostruktor von VmBase)
        /// </summary>
        protected override void InitializeCommands()
        {
            CmdLogin = new VmCommand(ResViewLogin.CmdContinue, async () =>
                {
                    if (!await CheckConnected().ConfigureAwait(true))
                    {
                        return;
                    }

                    if (CurrentState == EnumViewLoginStates.Login || CurrentState == EnumViewLoginStates.UserAccountLocked)
                    {
                        //Weiter würde gedrückt
                        _checkUser = await Dc.CheckUserLoginName(EntryLoginName.Value).ConfigureAwait(true);
                        if (_checkUser != null && _checkUser.UserId != -1)
                        {
                            if (_checkUser.Locked)
                            {
                                CurrentState = EnumViewLoginStates.UserAccountLocked;
                                await MsgBox.Show(ResCommon.MsgLocked, ResCommon.MsgTitleLocked).ConfigureAwait(true);
                            }
                            else
                            {
                                if (_checkUser.LoginConfirmed)
                                {
                                    CurrentState = EnumViewLoginStates.Password;
                                }
                                else
                                {
                                    CurrentState = EnumViewLoginStates.UserNotValidated;
                                    await MsgBox.Show(ResViewLogin.MsgUserNotValidated, ResCommon.MsgTitleEmail).ConfigureAwait(true);
                                }
                            }
                        }
                        else
                        {
                            var yes = await MsgBox.Show(ResViewLogin.MsgNewUser, ResViewLogin.MsgTitleNewUser, VmMessageBoxButton.YesNo).ConfigureAwait(true);
                            if (yes == VmMessageBoxResult.Yes)
                            {
                                Dc.DcExUser.SetDefault();
                                Dc.DcExUser.Data.LoginName = EntryLoginName.Value;
                                // ReSharper disable once RedundantAssignment
                                object r = false;

                                r = await Nav.ToViewWithResult(typeof(VmEditUser)).ConfigureAwait(true) ?? true; // ViewResult kommt nicht wie gewollt

                                if (r is bool result && result)
                                {
                                    _checkUser = await Dc.CheckUserLoginName(EntryLoginName.Value).ConfigureAwait(true);

                                    if (_checkUser != null && _checkUser.UserId != -1)
                                    {
                                        CurrentState = EnumViewLoginStates.Password;
                                        await PwdProcess(Dc.DcExUser.Data.PasswordHash4NewUser).ConfigureAwait(true);
                                        Dc.DcExUser.Data.LoginConfirmed = true;
                                        await Dc.DcExUser.StoreData(true).ConfigureAwait(true);

                                        await OnLoginCreateCompany().ConfigureAwait(true);
                                        await OnLoginCreateGateway().ConfigureAwait(true);

                                        await Dc.DcExUser.WaitDataFromServerAsync(forceUpdate: true).ConfigureAwait(true);

                                        (new VmUser()).CmdAddToken.Execute(null!);

                                        // ReSharper disable once UnusedVariable
                                        var xx = Dc.DcExGateways.ToList();
                                    }
                                }
                                else
                                {
                                    Dc.DcExUser.SetDefault();
                                }
                            }
                        }
                    }
                    else if (CurrentState == EnumViewLoginStates.Password || CurrentState == EnumViewLoginStates.PasswordWrong)
                    {
                        await PwdProcess(AppCrypt.CumputeHash(EntryPassword.Value)).ConfigureAwait(true);
                    }
                    else if (CurrentState == EnumViewLoginStates.UserNotValidated)
                    {
                        //Weiter würde gedrückt
                        _checkUser = await Dc.CheckUserLoginName(EntryLoginName.Value).ConfigureAwait(true);
                        if (_checkUser != null && _checkUser.UserId != -1)
                        {
                            if (_checkUser.LoginConfirmed)
                            {
                                CurrentState = EnumViewLoginStates.Password;
                            }
                            else
                            {
                                CurrentState = EnumViewLoginStates.UserNotValidated;
                                await MsgBox.Show(ResViewLogin.MsgUserNotValidated, ResCommon.MsgTitleEmail).ConfigureAwait(true);
                            }
                        }
                    }
                }
                , CanExecuteLoginCommand);


            CmdForgotPassword = new VmCommand(ResViewLogin.CmdForgotPassword, async () =>
                {
                    if (_checkUser == null || _checkUser.UserId <= 0)
                    {
                        return;
                    }

                    var r = await Dc.ResetPassword(_checkUser.UserId).ConfigureAwait(true);
                    if (!r.Ok)
                    {
                        Logging.Log.LogWarning($"[{nameof(VmLogin)}]({nameof(CmdForgotPassword)}-Command): Error: {r.ServerExceptionText}");
                        await MsgBox.Show(ResViewLogin.MsgSendError, ResCommon.MsgTitleEmail).ConfigureAwait(true);
                    }
                    else
                    {
                        await MsgBox.Show(ResViewLogin.MsgResetSent, ResCommon.MsgTitleEmail).ConfigureAwait(true);
                    }
                }
                , () => EntryLoginName != null! && EntryLoginName.DataValid);


            CmdResendAccessLink = new VmCommand(ResViewLogin.CmdResendAccessLink, async () =>
                {
                    var r = await Dc.ResendAccessEMail(_checkUser!.UserId).ConfigureAwait(true);
                    if (r.Ok)
                    {
                        await MsgBox.Show(ResViewLogin.MsgConfirmationSent, ResCommon.MsgTitleEmail).ConfigureAwait(true);
                    }
                    else
                    {
                        Logging.Log.LogWarning($"[{nameof(VmLogin)}]({nameof(CmdForgotPassword)}-Command): Error: {r.ServerExceptionText}");
                        await MsgBox.Show(ResViewLogin.MsgSendError, ResCommon.MsgTitleEmail).ConfigureAwait(true);
                    }
                }
                , () => EntryLoginName != null! && EntryLoginName.DataValid);
        }

        /// <summary>
        /// Prozess zum einloggen
        /// </summary>
        /// <param name="pwdHash">Passwort string</param>
        /// <returns></returns>
        private async Task PwdProcess(string pwdHash)
        {
            ProjectDataLoadedAfterDcConnected = false;
            var pwdCheck = await Dc.CheckUserPassword(_checkUser!.UserId, pwdHash).ConfigureAwait(true);
            if (pwdCheck)
            {
                View.BusySet(ResViewLogin.BsyLogin);
                await Dc.DcExUser.WaitDataFromServerAsync(forceUpdate: true).ConfigureAwait(true);
#pragma warning disable CS0618 // Type or member is obsolete
                await Dc.DcExCompanies.WaitDataFromServerAsync(reload: true).ConfigureAwait(true);
#pragma warning restore CS0618 // Type or member is obsolete
                CurrentVmMenu?.UpdateMenu();
                GCmdHome.Execute(null!);
            }
            else
            {
                await MsgBox.Show(ResViewLogin.MsgWrongPassword, ResViewLogin.MsgTitleWrongPassword).ConfigureAwait(true);
                CurrentState = EnumViewLoginStates.PasswordWrong;
            }
        }

        /// <summary>
        ///     Validierung für Login Name
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private (string hint, bool valid) ValidateLoginName(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                CurrentState = EnumViewLoginStates.Login;
                return (ResCommon.ValNotEmpty, false);
            }

            if (!Validator.Check(arg))
            {
                CurrentState = EnumViewLoginStates.Login;
                return (ResCommon.ValNoEmail, false);
            }

            if (arg.Contains(' ', StringComparison.CurrentCultureIgnoreCase))
            {
                CurrentState = EnumViewLoginStates.Login;
                return (ResCommon.ValNoWhitespace, false);
            }

            return (string.Empty, true);
        }

        /// <summary>
        ///     Kann der Button Login ausgeführt werden
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteLoginCommand()
        {
            if (EntryLoginName == null!)
            {
                return false;
            }

            return EntryLoginName.DataValid;
        }

        /// <summary>
        /// Erstellen eines Gateways beim Login
        /// </summary>
        /// <returns>ob erfolgreich</returns>
        /// <exception cref="ArgumentException"></exception>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task<bool> OnLoginCreateGateway()
        {
            // ReSharper disable once RedundantAssignment
            DcListTypeGateway? item = null;

            var currC = Dc.DcExCompanies.FirstOrDefault(c => c.Data.Information.Name == Dc.DcExUser.Data.LoginName);
            if (currC == null)
            {
                throw new ArgumentException();
            }

            var newGateway = new ExGateway
                             {
                                 Information = new ExInformation
                                               {
                                                   Name = Dc.DcExUser.Data.LoginName,
                                                   Description = "Gateway des Users mit der Email Addresse " + Dc.DcExUser.Data.LoginName,
                                                   CreatedDate = DateTime.UtcNow
                                               },
                                 TblCompanyId = currC.Index,
                                 CompanyId = currC.Index,
                                 DeviceCommon = new ExCommonInfo {Secret = Dc.DcExUser.Data.LoginName}
                             };

            item = new DcListTypeGateway(newGateway);
            Dc.DcExGateways.Add(item);
            await Dc.DcExGateways.StoreAll().ConfigureAwait(true);

            return true;
        }

        /// <summary>
        /// Erstellen einer Company beim Login
        /// </summary>
        /// <returns>ob erfolgreich</returns>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task<bool> OnLoginCreateCompany()
        {
            // ReSharper disable once RedundantAssignment
            DcListTypeCompany? itemC = null;
            var newCompany = new ExCompany
                             {
                                 Information = new ExInformation
                                               {
                                                   Name = Dc.DcExUser.Data.LoginName,
                                                   Description = "Company des Users mit der Email Addresse " + Dc.DcExUser.Data.LoginName,
                                                   CreatedDate = DateTime.UtcNow
                                               },
                                 CompanyType = EnumCompanyTypes.Company
                             };

            itemC = new DcListTypeCompany(newCompany);
            Dc.DcExCompanies.Add(itemC);
            try
            {
                await Dc.DcExCompanies.StoreAll().ConfigureAwait(true);
            }
            catch (Exception e)
            {
                Logging.Log.LogError($"{e}");
                throw;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            await Dc.DcExCompanies.WaitDataFromServerAsync(reload: true).ConfigureAwait(true);
#pragma warning restore CS0618 // Type or member is obsolete
            return true;
        }
    }
}