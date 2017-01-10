using System;
using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using AdvantShop.Payment;

namespace ClientPages
{
    public partial class install_UserContols_PaymentView : AdvantShop.Controls.InstallerStep
    {
        private bool _isMethodsExist = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            //_isMethodsExist = PaymentService.GetCountPaymentMethods() > 0;

            divFizBank.Visible = AdvantShop.Core.AdvantshopConfigService.GetActivityPayment("SberBank");
            divUrBank.Visible = AdvantShop.Core.AdvantshopConfigService.GetActivityPayment("Bill");
            divCreditCard.Visible = AdvantShop.Core.AdvantshopConfigService.GetActivityPayment("Robokassa");
            divTerminals.Visible = AdvantShop.Core.AdvantshopConfigService.GetActivityPayment("Robokassa");
            divElectronMoney.Visible = AdvantShop.Core.AdvantshopConfigService.GetActivityPayment("Robokassa");
            divIphone.Visible = AdvantShop.Core.AdvantshopConfigService.GetActivityPayment("Robokassa");


            mvPayment.SetActiveView(_isMethodsExist ? vExistPayment : vNew);
        }

        public new void LoadData()
        {
            var pm = PaymentService.GetPaymentMethodByName(chbCash.Text);
            chbCash.Checked = pm != null && pm.Type == PaymentType.Cash;

            pm = PaymentService.GetPaymentMethodByName(chbFizBank.Text);
            chbFizBank.Checked = pm != null && pm.Type == PaymentType.SberBank;

            pm = PaymentService.GetPaymentMethodByName(chbUrBank.Text);
            chbUrBank.Checked = pm != null && pm.Type == PaymentType.Bill;

            pm = PaymentService.GetPaymentMethodByName(chbCreditCard.Text);
            chbCreditCard.Checked = pm != null;

            pm = PaymentService.GetPaymentMethodByName(chbElectronMoney.Text);
            chbElectronMoney.Checked = pm != null;

            pm = PaymentService.GetPaymentMethodByName(chbTerminals.Text);
            chbTerminals.Checked = pm != null;

            pm = PaymentService.GetPaymentMethodByName(chbIPhone.Text);
            chbIPhone.Checked = pm != null;

            LoadCreditCard();
            LoadElectronMoney();
            LoadTerminals();

            pm = PaymentService.GetPaymentMethodByName(chbIPhone.Text);
            if (pm != null && pm.Type == PaymentType.Robokassa)
            {
                chbIPhone.Checked = true;
                txtLoginRobokassaIPhone.Text = pm.Parameters.ElementOrDefault(RobokassaTemplate.MerchantLogin);
                txtPassRobokassaIPhone.Text = pm.Parameters.ElementOrDefault(RobokassaTemplate.Password);
            }
        }

        public new void SaveData()
        {
            if (_isMethodsExist) return;
            var pm = PaymentService.GetPaymentMethodByName(chbCash.Text);
            if (pm != null)
                PaymentService.DeletePaymentMethod(pm.PaymentMethodId);

            if (chbCash.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Cash);
                method.Name = chbCash.Text;
                method.Description = chbCash.Text;
                method.SortOrder = 0;
                method.Enabled = true;
                var id = PaymentService.AddPaymentMethod(method);
            }

            pm = PaymentService.GetPaymentMethodByName(chbFizBank.Text);
            if (pm != null)
                PaymentService.DeletePaymentMethod(pm.PaymentMethodId);

            if (chbFizBank.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.SberBank);
                method.Name = chbFizBank.Text;
                method.Description = chbFizBank.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {SberBankTemplate.CurrencyValue, "1"},
                    {SberBankTemplate.CompanyName, SettingsBank.CompanyName},
                    {SberBankTemplate.TransAccount, SettingsBank.RS},
                    {SberBankTemplate.INN, SettingsBank.INN},
                    {SberBankTemplate.KPP, SettingsBank.KPP},
                    {SberBankTemplate.BankName, SettingsBank.BankName},
                    {SberBankTemplate.CorAccount, SettingsBank.KS},
                    {SberBankTemplate.BIK, SettingsBank.BIK}
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            pm = PaymentService.GetPaymentMethodByName(chbUrBank.Text);
            if (pm != null)
                PaymentService.DeletePaymentMethod(pm.PaymentMethodId);

            if (chbUrBank.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Bill);
                method.Name = chbUrBank.Text;
                method.Description = chbUrBank.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {BillTemplate.Accountant, SettingsBank.Accountant},
                    {BillTemplate.CurrencyValue, "1"},
                    {BillTemplate.CompanyName, SettingsBank.CompanyName},
                    {BillTemplate.TransAccount, SettingsBank.RS},
                    {BillTemplate.CorAccount, SettingsBank.KS},
                    {BillTemplate.Address, ""},
                    {BillTemplate.Telephone, SettingsMain.Phone},
                    {BillTemplate.INN, SettingsBank.INN},
                    {BillTemplate.KPP, SettingsBank.KPP},
                    {BillTemplate.BIK, SettingsBank.BIK},
                    {BillTemplate.BankName, SettingsBank.BankName},
                    {BillTemplate.Director, SettingsBank.Director},
                    {BillTemplate.Manager, SettingsBank.Manager}
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (chbCreditCard.Checked)
            {
                SaveCreditCard();
            }

            if (chbElectronMoney.Checked)
            {
                SaveElectronMoney();
            }

            if (chbTerminals.Checked)
            {
                SaveTerminals();
            }

            pm = PaymentService.GetPaymentMethodByName(chbIPhone.Text);
            if (pm != null)
                PaymentService.DeletePaymentMethod(pm.PaymentMethodId);

            if (chbIPhone.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Robokassa);
                method.Name = chbIPhone.Text;
                method.Description = chbIPhone.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {RobokassaTemplate.MerchantLogin, txtLoginRobokassaIPhone.Text},
                    {RobokassaTemplate.Password, txtPassRobokassaIPhone.Text},
                    {RobokassaTemplate.CurrencyLabel, "RUR"},
                    {RobokassaTemplate.CurrencyValue, "1"}
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }
        }

        private void LoadCreditCard()
        {
            var pm = PaymentService.GetPaymentMethodByName(chbCreditCard.Text);
            if (pm != null)
            {
                if (pm.Type == PaymentType.Robokassa)
                {
                    rbRobokassaCreditcard.Checked = true;
                    txtLoginRobokassaCreditcard.Text = pm.Parameters.ElementOrDefault(RobokassaTemplate.MerchantLogin);
                    txtPassRobokassaCreditcard.Text = pm.Parameters.ElementOrDefault(RobokassaTemplate.Password);
                }

                if (pm.Type == PaymentType.Assist)
                {
                    rbAssistCreditcard.Checked = true;
                    txtLoginAssistCreditcard.Text = pm.Parameters.ElementOrDefault(AssistTemplate.Login);
                    txtPassAssistCreditcard.Text = pm.Parameters.ElementOrDefault(AssistTemplate.Password);
                    txtShopIdAssistCreditcard.Text = pm.Parameters.ElementOrDefault(AssistTemplate.MerchantID);
                }

                if (pm.Type == PaymentType.Platron)
                {
                    rbPlatronCreditcard.Checked = true;
                    txtSellerIdPlatronCreditcard.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.MerchantId);
                    txtPaySystemCreditcard.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.PaymentSystem);
                    txtPayPassCreditcard.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.SecretKey);
                }

                if (pm.Type == PaymentType.ZPayment)
                {
                    rbZPaymentCreditcard.Checked = true;
                    txtPayPoketCreditcard.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.Purse);
                    txtPassZpaymentCreditcard.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.Password);
                    txtSecretKeyZpaymentCreditcard.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.SecretKey);
                }
            }
        }

        private void SaveCreditCard()
        {
            var pm = PaymentService.GetPaymentMethodByName(chbCreditCard.Text);
            if (pm != null)
            {
                PaymentService.DeletePaymentMethod(pm.PaymentMethodId);
            }

            if (rbRobokassaCreditcard.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Robokassa);
                method.Name = chbCreditCard.Text;
                method.Description = chbCreditCard.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {RobokassaTemplate.MerchantLogin, txtLoginRobokassaCreditcard.Text},
                    {RobokassaTemplate.Password, txtPassRobokassaCreditcard.Text},
                    {RobokassaTemplate.CurrencyLabel, "RUR"},
                    {RobokassaTemplate.CurrencyValue, "1"}
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbAssistCreditcard.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Assist);
                method.Name = chbCreditCard.Text;
                method.Description = chbCreditCard.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {AssistTemplate.Login, txtLoginAssistCreditcard.Text},
                    {AssistTemplate.Password, txtPassAssistCreditcard.Text},
                    {AssistTemplate.MerchantID, txtShopIdAssistCreditcard.Text},
                    {AssistTemplate.Sandbox, false.ToString()},
                    {AssistTemplate.CurrencyCode, "RUB"},
                    {AssistTemplate.CurrencyValue, "1"},
                    //{AssistTemplate.AssistIdCcPayment, false.ToString( ) },
                    //{AssistTemplate.CardPayment, true .ToString( ) },
                    //{AssistTemplate.PayCashPayment, false.ToString( ) },
                    //{AssistTemplate.WebMoneyPayment, false.ToString( ) },
                    //{AssistTemplate.QiwiBeelinePayment, false.ToString( ) }
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbPlatronCreditcard.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Platron);
                method.Name = chbCreditCard.Text;
                method.Description = chbCreditCard.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {PlatronTemplate.MerchantId, txtSellerIdPlatronCreditcard.Text},
                    {PlatronTemplate.Currency, "RUR"},
                    {PlatronTemplate.PaymentSystem, txtPaySystemCreditcard.Text},
                    {PlatronTemplate.CurrencyValue, "1"},
                    {PlatronTemplate.SecretKey, txtPayPassCreditcard.Text},
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbZPaymentCreditcard.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.ZPayment);
                method.Name = chbCreditCard.Text;
                method.Description = chbCreditCard.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {ZPaymentTemplate.Purse, txtPayPoketCreditcard.Text},
                    {ZPaymentTemplate.Password, txtPassZpaymentCreditcard.Text},
                    {ZPaymentTemplate.SecretKey, txtSecretKeyZpaymentCreditcard.Text},
                    {ZPaymentTemplate.CurrencyValue, "1"},
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }
        }

        private void LoadElectronMoney()
        {
            var pm = PaymentService.GetPaymentMethodByName(chbElectronMoney.Text);
            if (pm != null)
            {
                if (pm.Type == PaymentType.Robokassa)
                {
                    rbRobokassaElectronMoney.Checked = true;
                    txtLoginRobokassaElectronMoney.Text = pm.Parameters.ElementOrDefault(RobokassaTemplate.MerchantLogin);
                    txtPassRobokassaElectronMoney.Text = pm.Parameters.ElementOrDefault(RobokassaTemplate.Password);
                }

                if (pm.Type == PaymentType.Assist)
                {
                    rbAssistElectronMoney.Checked = true;
                    txtLoginAssistElectronMoney.Text = pm.Parameters.ElementOrDefault(AssistTemplate.Login);
                    txtPassAssistElectronMoney.Text = pm.Parameters.ElementOrDefault(AssistTemplate.Password);
                    txtShopIdAssistElectronMoney.Text = pm.Parameters.ElementOrDefault(AssistTemplate.MerchantID);
                }

                if (pm.Type == PaymentType.Platron)
                {
                    rbPlatronElectronMoney.Checked = true;
                    txtSellerIdPlatronElectronMoney.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.MerchantId);
                    txtPaySystemElectronMoney.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.PaymentSystem);
                    txtPayPassElectronMoney.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.SecretKey);
                }

                if (pm.Type == PaymentType.ZPayment)
                {
                    rbZPaymentElectronMoney.Checked = true;
                    txtPayPoketElectronMoney.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.Purse);
                    txtPassZpaymentElectronMoney.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.Password);
                    txtSecretKeyZpaymentElectronMoney.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.SecretKey);
                }
            }
        }

        private void SaveElectronMoney()
        {
            var pm = PaymentService.GetPaymentMethodByName(chbElectronMoney.Text);
            if (pm != null)
            {
                PaymentService.DeletePaymentMethod(pm.PaymentMethodId);
            }

            if (rbRobokassaElectronMoney.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Robokassa);
                method.Name = chbElectronMoney.Text;
                method.Description = chbElectronMoney.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {RobokassaTemplate.MerchantLogin, txtLoginRobokassaElectronMoney.Text},
                    {RobokassaTemplate.Password, txtPassRobokassaElectronMoney.Text},
                    {RobokassaTemplate.CurrencyLabel, "RUR"},
                    {RobokassaTemplate.CurrencyValue, "1"}
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbAssistElectronMoney.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Assist);
                method.Name = chbElectronMoney.Text;
                method.Description = chbElectronMoney.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {AssistTemplate.Login, txtLoginAssistElectronMoney.Text},
                    {AssistTemplate.Password, txtPassAssistElectronMoney.Text},
                    {AssistTemplate.MerchantID, txtShopIdAssistElectronMoney.Text},
                    {AssistTemplate.Sandbox, false.ToString()},
                    {AssistTemplate.CurrencyCode, "RUB"},
                    {AssistTemplate.CurrencyValue, "1"},
                    //{AssistTemplate.AssistIdCcPayment, false.ToString( ) },
                    //{AssistTemplate.CardPayment, true .ToString( ) },
                    //{AssistTemplate.PayCashPayment, false.ToString( ) },
                    //{AssistTemplate.WebMoneyPayment, false.ToString( ) },
                    //{AssistTemplate.QiwiBeelinePayment, false.ToString( ) }
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbPlatronElectronMoney.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Platron);
                method.Name = chbElectronMoney.Text;
                method.Description = chbElectronMoney.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {PlatronTemplate.MerchantId, txtSellerIdPlatronElectronMoney.Text},
                    {PlatronTemplate.Currency, "RUR"},
                    {PlatronTemplate.PaymentSystem, txtPaySystemElectronMoney.Text},
                    {PlatronTemplate.CurrencyValue, "1"},
                    {PlatronTemplate.SecretKey, txtPayPassElectronMoney.Text},
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbZPaymentElectronMoney.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Platron);
                method.Name = chbElectronMoney.Text;
                method.Description = chbElectronMoney.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {ZPaymentTemplate.Purse, txtPayPoketElectronMoney.Text},
                    {ZPaymentTemplate.Password, txtPassZpaymentElectronMoney.Text},
                    {ZPaymentTemplate.SecretKey, txtSecretKeyZpaymentElectronMoney.Text},
                    {ZPaymentTemplate.CurrencyValue, "1"},
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }
        }

        private void LoadTerminals()
        {
            var pm = PaymentService.GetPaymentMethodByName(chbTerminals.Text);
            if (pm != null)
            {
                if (pm.Type == PaymentType.Robokassa)
                {
                    rbRobokassaTerminals.Checked = true;
                    txtLoginRobokassaTerminals.Text = pm.Parameters.ElementOrDefault(RobokassaTemplate.MerchantLogin);
                    txtPassRobokassaTerminals.Text = pm.Parameters.ElementOrDefault(RobokassaTemplate.Password);
                }

                if (pm.Type == PaymentType.Assist)
                {
                    rbAssistTerminals.Checked = true;
                    txtLoginAssistTerminals.Text = pm.Parameters.ElementOrDefault(AssistTemplate.Login);
                    txtPassAssistTerminals.Text = pm.Parameters.ElementOrDefault(AssistTemplate.Password);
                    txtShopIdAssistTerminals.Text = pm.Parameters.ElementOrDefault(AssistTemplate.MerchantID);
                }

                if (pm.Type == PaymentType.Platron)
                {
                    rbPlatronTerminals.Checked = true;
                    txtSellerIdPlatronTerminals.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.MerchantId);
                    txtPaySystemTerminals.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.PaymentSystem);
                    txtPayPassTerminals.Text = pm.Parameters.ElementOrDefault(PlatronTemplate.SecretKey);
                }

                if (pm.Type == PaymentType.ZPayment)
                {
                    rbZPaymentTerminals.Checked = true;
                    txtPayPoketTerminals.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.Purse);
                    txtPassZpaymentTerminals.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.Password);
                    txtSecretKeyZpaymentTerminals.Text = pm.Parameters.ElementOrDefault(ZPaymentTemplate.SecretKey);
                }
            }
        }

        private void SaveTerminals()
        {
            var pm = PaymentService.GetPaymentMethodByName(chbTerminals.Text);
            if (pm != null)
            {
                PaymentService.DeletePaymentMethod(pm.PaymentMethodId);
            }

            if (rbRobokassaTerminals.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Robokassa);
                method.Name = chbTerminals.Text;
                method.Description = chbTerminals.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {RobokassaTemplate.MerchantLogin, txtLoginRobokassaTerminals.Text},
                    {RobokassaTemplate.Password, txtPassRobokassaTerminals.Text},
                    {RobokassaTemplate.CurrencyLabel, "RUR"},
                    {RobokassaTemplate.CurrencyValue, "1"}
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbAssistTerminals.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Assist);
                method.Name = chbTerminals.Text;
                method.Description = chbTerminals.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {AssistTemplate.Login, txtLoginAssistTerminals.Text},
                    {AssistTemplate.Password, txtPassAssistTerminals.Text},
                    {AssistTemplate.MerchantID, txtShopIdAssistTerminals.Text},
                    {AssistTemplate.Sandbox, false.ToString()},
                    {AssistTemplate.CurrencyCode, "RUB"},
                    {AssistTemplate.CurrencyValue, "1"},
                    //{AssistTemplate.AssistIdCcPayment, false.ToString( ) },
                    //{AssistTemplate.CardPayment, true .ToString( ) },
                    //{AssistTemplate.PayCashPayment, false.ToString( ) },
                    //{AssistTemplate.WebMoneyPayment, false.ToString( ) },
                    //{AssistTemplate.QiwiBeelinePayment, false.ToString( ) }
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbPlatronTerminals.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Platron);
                method.Name = chbTerminals.Text;
                method.Description = chbTerminals.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {PlatronTemplate.MerchantId, txtSellerIdPlatronTerminals.Text},
                    {PlatronTemplate.Currency, "RUR"},
                    {PlatronTemplate.PaymentSystem, txtPaySystemTerminals.Text},
                    {PlatronTemplate.CurrencyValue, "1"},
                    {PlatronTemplate.SecretKey, txtPayPassTerminals.Text},
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }

            if (rbZPaymentTerminals.Checked)
            {
                var method = PaymentMethod.Create(PaymentType.Platron);
                method.Name = chbTerminals.Text;
                method.Description = chbTerminals.Text;
                method.SortOrder = 0;
                method.Enabled = true;

                var id = PaymentService.AddPaymentMethod(method);
                var parameters = new Dictionary<string, string>
                {
                    {ZPaymentTemplate.Purse, txtPayPoketTerminals.Text},
                    {ZPaymentTemplate.Password, txtPassZpaymentTerminals.Text},
                    {ZPaymentTemplate.SecretKey, txtSecretKeyZpaymentTerminals.Text},
                    {ZPaymentTemplate.CurrencyValue, "1"},
                };
                PaymentService.UpdatePaymentParams(id, parameters);
            }
        }

        public new bool Validate()
        {
            var validList = new List<ValidElement>();

            //Credit card
            if (rbRobokassaCreditcard.Checked && chbCreditCard.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtLoginRobokassaCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedRobokassaLogin
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassRobokassaCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedRobokassaPass
                });
            }

            if (rbAssistCreditcard.Checked && chbCreditCard.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtLoginAssistCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistLogin
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassAssistCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistPass
                });
                validList.Add(new ValidElement
                {
                    Control = txtShopIdAssistCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistShopId
                });
            }

            if (rbPlatronCreditcard.Checked && chbCreditCard.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtSellerIdPlatronCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronSellerId
                });
                validList.Add(new ValidElement
                {
                    Control = txtPaySystemCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronPaySystem
                });
                validList.Add(new ValidElement
                {
                    Control = txtPayPassCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronPass
                });
            }

            if (rbZPaymentCreditcard.Checked && chbCreditCard.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtPayPoketCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentPayPoket
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassZpaymentCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentPass
                });
                validList.Add(new ValidElement
                {
                    Control = txtSecretKeyZpaymentCreditcard,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentSecretKey
                });
            }

            //Electron money
            if (rbRobokassaElectronMoney.Checked && chbElectronMoney.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtLoginRobokassaElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedRobokassaLogin
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassRobokassaElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedRobokassaPass
                });
            }

            if (rbAssistElectronMoney.Checked && chbElectronMoney.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtLoginAssistElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistLogin
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassAssistElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistPass
                });
                validList.Add(new ValidElement
                {
                    Control = txtShopIdAssistElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistShopId
                });
            }

            if (rbPlatronElectronMoney.Checked && chbElectronMoney.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtSellerIdPlatronElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronSellerId
                });
                validList.Add(new ValidElement
                {
                    Control = txtPaySystemElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronPaySystem
                });
                validList.Add(new ValidElement
                {
                    Control = txtPayPassElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronPass
                });
            }

            if (rbZPaymentElectronMoney.Checked && chbElectronMoney.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtPayPoketElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentPayPoket
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassZpaymentElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentPass
                });
                validList.Add(new ValidElement
                {
                    Control = txtSecretKeyZpaymentElectronMoney,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentSecretKey
                });
            }

            if (rbRobokassaTerminals.Checked && chbTerminals.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtLoginRobokassaTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedRobokassaLogin
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassRobokassaTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedRobokassaPass
                });
            }


            if (rbAssistTerminals.Checked && chbTerminals.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtLoginAssistTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistLogin
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassAssistTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistPass
                });
                validList.Add(new ValidElement
                {
                    Control = txtShopIdAssistTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedAssistShopId
                });
            }

            if (rbPlatronTerminals.Checked && chbTerminals.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtSellerIdPlatronTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronSellerId
                });
                validList.Add(new ValidElement
                {
                    Control = txtPaySystemTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronPaySystem
                });
                validList.Add(new ValidElement
                {
                    Control = txtPayPassTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedPlatronPass
                });
            }

            if (rbZPaymentTerminals.Checked && chbTerminals.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtPayPoketTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentPayPoket
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassZpaymentTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentPass
                });
                validList.Add(new ValidElement
                {
                    Control = txtSecretKeyZpaymentTerminals,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedZpaymentSecretKey
                });
            }

            if (chbIPhone.Checked)
            {
                validList.Add(new ValidElement
                {
                    Control = txtLoginRobokassaIPhone,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedRobokassaLogin
                });
                validList.Add(new ValidElement
                {
                    Control = txtPassRobokassaIPhone,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_PaymentView_Err_NeedRobokassaPass
                });
            }

            return ValidationHelper.Validate(validList);
        }
    }
}