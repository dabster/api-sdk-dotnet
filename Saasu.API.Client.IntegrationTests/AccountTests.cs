using System;
using System.Linq;
using NUnit.Framework;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Models.Accounts;

namespace Saasu.API.Client.IntegrationTests
{
	[TestFixture]
	public class AccountTests
	{
		//private int _nonBankAcctId;
		//private int _bankAcctId;
		//private int _accountToBeUpdated;
		//private int _bankAccountToBeUpdated;
		//private int _inactiveAccountId;
  //      private int _headerAccountId;
  //      private int _accountToAssignToHeaderAccount;


		public AccountTests()
		{
			//CreateTestData();
		}

		[Test]
		public void GetAccountsNoFilter()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts();

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");
		}

		[Test]
		public void GetNonBankAccount()
		{
		    var accountId = CreateNonBankAccount();

            var accountsProxy = new AccountProxy();
			var response = accountsProxy.GetAccount(accountId);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.IsFalse(Convert.ToBoolean(response.DataObject.IsBankAccount), "Account returned is a bank account");
		}

		[Test]
		public void GetBankAccount()
		{
		    var accountId = CreateBankAccount();
			var accountsProxy = new AccountProxy();
			var response = accountsProxy.GetAccount(accountId);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.IsTrue(Convert.ToBoolean(response.DataObject.IsBankAccount), "Account returned is not a bank account");
		}

		[Test]
		public void GetAccountsFilterOnIsBankAccount()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(isBankAccount: true);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a => Assert.IsTrue(Convert.ToBoolean(a.IsBankAccount), "Non bank accounts have been returned"));
		}

		[Test]
		public void GetAccountsFilterOnIsNotBankAccount()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(isBankAccount: false);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a => Assert.IsFalse(Convert.ToBoolean(a.IsBankAccount), "Bank accounts have been returned"));
		}

		[Test]
		public void GetAccountsFilterOnActive()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(isActive: true);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a => Assert.IsTrue(Convert.ToBoolean(a.IsActive), "inactive accounts have been returned"));
		}

		[Test]
		public void GetAccountsFilterOnInactive()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(isActive: false);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a => Assert.IsFalse(Convert.ToBoolean(a.IsActive)));
		}

		[Test]
		public void GetAccountsFilterOnIncludeBuiltIn()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(includeBuiltIn: true);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");

			var builtInAccounts = response.DataObject.Accounts.Where(a => Convert.ToBoolean(a.IsBuiltIn));
			Assert.Greater(builtInAccounts.Count(), 0);
		}

		[Test]
		public void GetAccountsFilterOnNotIncludeBuiltIn()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(includeBuiltIn: false);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");

			var builtInAccounts = response.DataObject.Accounts.Where(a => Convert.ToBoolean(a.IsBuiltIn));
			Assert.AreEqual(builtInAccounts.Count(), 0);
		}

		[Test]
		public void GetAccountsFilterOnAccountType()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(accountType: "Income");

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a => Assert.AreEqual(a.AccountType, "Income"));
		}

        [Test]
        public void GetAccountsFilterOnHeaderAccountId()
        {
            var headerAccountId = CreateTestHeaderAccount();
            var assignedAccountId = CreateAndAssignTestAccountToHeaderAccount(headerAccountId);

            var accountsProxy = new AccountsProxy();
            var response = accountsProxy.GetAccounts(headerAccountId: headerAccountId);

            Assert.IsNotNull(response, "Reponse is null");
            Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
            Assert.AreEqual(response.DataObject.Accounts.Count, 1, "Incorrect number of accounts returned");
            Assert.AreEqual(response.DataObject.Accounts[0].Id, assignedAccountId, "Incorrect account assigned to header account.");
        }

		[Test]
		public void GetAccountsPageSize()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(pageSize: 10);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.AreEqual(response.DataObject.Accounts.Count, 10, "10 records should have been returned");
		}

		[Test]
		public void GetAccountsSecondPage()
		{
			var accountsProxy = new AccountsProxy();
			var response = accountsProxy.GetAccounts(pageNumber: 1, pageSize: 10);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.AreEqual(response.DataObject.Accounts.Count, 10, "10 records should have been returned");

			var acctIdsFirstPage = response.DataObject.Accounts.Select(a => a.Id).ToList();

			response = accountsProxy.GetAccounts(pageNumber: 2, pageSize: 10);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.Greater(response.DataObject.Accounts.Count, 0, "Zero accounts returned");

			response.DataObject.Accounts.ForEach(a => Assert.IsFalse(acctIdsFirstPage.Contains(a.Id), "Record from page 1 returned"));
		}

		[Test]
		public void InsertNonBankAccount()
		{
			//Create and Insert
			var account = GetTestAccount();

			var accountProxy = new AccountProxy();
			var response = accountProxy.InsertAccount(account);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.InsertedEntityId, 0, "Zero accounts returned");

			//Get account again and verify inserted fields.
			var acct = accountProxy.GetAccount(response.DataObject.InsertedEntityId);

			Assert.AreEqual(acct.DataObject.Name, account.Name, "Names not equal");
			Assert.AreEqual(acct.DataObject.AccountType, account.AccountType, "Account types not equal");
			Assert.AreEqual(acct.DataObject.DefaultTaxCode, account.DefaultTaxCode, "Tax codes not equal");
			Assert.AreEqual(acct.DataObject.LedgerCode, account.LedgerCode, "Leadge codes not equal");
			Assert.AreEqual(acct.DataObject.Currency, account.Currency, "Currencies not equal");
			Assert.AreEqual(acct.DataObject.IsBankAccount, account.IsBankAccount, "IsBankAccount not equal");
			Assert.AreEqual(acct.DataObject.IncludeInForecaster, false, "IncludeInForecaster should be false for non bank accounts");
            Assert.Null(acct.DataObject.HeaderAccountId, "Account should not have a header account");
		}

		[Test]
		public void InsertBankAccount()
		{
			//Create and Insert
			var account = GetTestBankAccount();

			var accountProxy = new AccountProxy();
			var response = accountProxy.InsertAccount(account);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
			Assert.Greater(response.DataObject.InsertedEntityId, 0, "Zero accounts returned");

			//Get account again and verify inserted fields.
			var acct = accountProxy.GetAccount(response.DataObject.InsertedEntityId);

			Assert.AreEqual(acct.DataObject.Name, account.Name, "Names not equal");
			Assert.AreEqual(acct.DataObject.AccountType, account.AccountType, "Account types not equal");
			Assert.AreEqual(acct.DataObject.DefaultTaxCode, account.DefaultTaxCode, "Tax codes not equal");
			Assert.AreEqual(acct.DataObject.LedgerCode, account.LedgerCode, "Ledger codes not equal");
			Assert.AreEqual(acct.DataObject.Currency, account.Currency, "Currencis not equal");
			Assert.AreEqual(acct.DataObject.IsBankAccount, account.IsBankAccount, "IsBankAccount not equal");
			Assert.AreEqual(acct.DataObject.IncludeInForecaster, account.IncludeInForecaster, "Include in Forecaster not equal");
			Assert.AreEqual(acct.DataObject.BSB, account.BSB, "BSBs not equal");
			Assert.AreEqual(acct.DataObject.Number, account.Number, "Account numbers not equal");
			Assert.AreEqual(acct.DataObject.BankAccountName, account.BankAccountName, "Bank account names not equal");
			Assert.AreEqual(acct.DataObject.BankFileCreationEnabled, account.BankFileCreationEnabled, "BankFileCreationEnabled not equal");
			Assert.AreEqual(acct.DataObject.BankCode, account.BankCode, "Bank codes not equal");
			Assert.AreEqual(acct.DataObject.UserNumber, account.UserNumber, "User numbers not equal");
			Assert.AreEqual(acct.DataObject.MerchantFeeAccountId, account.MerchantFeeAccountId, "Merchant accounts not equal");
			Assert.AreEqual(acct.DataObject.IncludePendingTransactions, account.IncludePendingTransactions, "IncludePendingTransactions not equal");
		}


        [Test]
        public void CanNotInsertPendingBankAccount()
        {

            var nameGuid = Guid.NewGuid();
            //Create and Insert
            var account = GetTestBankAccount();
            account.AccountType = "Pending";
            account.Name = $"Pending_{nameGuid}";
            account.BankAccountName = $"Pending_{nameGuid}";

            var accountProxy = new AccountProxy();
            var response = accountProxy.InsertAccount(account);

            Assert.IsNotNull(response, "Reponse is null");
            Assert.IsFalse(response.IsSuccessfull, "Reponse has not been successful");
        }

        [Test]
        public void InsertAccountWithHeader()
        {
            //Create and Insert
            var headerAccount = GetTestHeaderAccount();
            var accountProxy = new AccountProxy();
            var headerInsertResponse = accountProxy.InsertAccount(headerAccount);
            Assert.IsNotNull(headerInsertResponse, "Reponse is null");
            Assert.IsTrue(headerInsertResponse.IsSuccessfull, "Reponse has not been successful");
            Assert.Greater(headerInsertResponse.DataObject.InsertedEntityId, 0, "Zero accounts returned");

            var headerAccountId = headerInsertResponse.DataObject.InsertedEntityId;

            var account = GetTestAccount();
            account.HeaderAccountId = headerAccountId;

            var response = accountProxy.InsertAccount(account);

            Assert.IsNotNull(response, "Reponse is null");
            Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
            Assert.Greater(response.DataObject.InsertedEntityId, 0, "Zero accounts returned");

            //Get account again and verify inserted fields.
            var acct = accountProxy.GetAccount(response.DataObject.InsertedEntityId);

            Assert.AreEqual(acct.DataObject.Name, account.Name, "Names not equal");
            Assert.AreEqual(acct.DataObject.AccountType, account.AccountType, "Account types not equal");
            Assert.AreEqual(acct.DataObject.DefaultTaxCode, account.DefaultTaxCode, "Tax codes not equal");
            Assert.AreEqual(acct.DataObject.LedgerCode, account.LedgerCode, "Leadge codes not equal");
            Assert.AreEqual(acct.DataObject.Currency, account.Currency, "Currencies not equal");
            Assert.AreEqual(acct.DataObject.IsBankAccount, account.IsBankAccount, "IsBankAccount not equal");
            Assert.AreEqual(acct.DataObject.IncludeInForecaster, false, "IncludeInForecaster should be false for non bank accounts");
            Assert.AreEqual(acct.DataObject.HeaderAccountId, headerAccountId);
        }

        [Test]
        public void InsertHeaderAccount()
        {
            //Create and Insert
            var account = GetTestHeaderAccount();

            var accountProxy = new AccountProxy();
            var response = accountProxy.InsertAccount(account);

            Assert.IsNotNull(response, "Reponse is null");
            Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
            Assert.Greater(response.DataObject.InsertedEntityId, 0, "Zero accounts returned");

            //Get account again and verify inserted fields.
            var acct = accountProxy.GetAccount(response.DataObject.InsertedEntityId);

            Assert.AreEqual(acct.DataObject.Name, account.Name, "Names not equal");
            Assert.AreEqual(acct.DataObject.AccountLevel.ToLower(), "header");
            Assert.AreEqual(acct.DataObject.AccountType, account.AccountType, "Account types not equal");
            Assert.IsNull(acct.DataObject.DefaultTaxCode, "Tax code should be null");
            Assert.AreEqual(acct.DataObject.LedgerCode, account.LedgerCode, "Ledger codes not equal");
            Assert.IsFalse(Convert.ToBoolean(acct.DataObject.IsBankAccount), "Header accounts cannot be bank accounts");
            Assert.IsFalse(Convert.ToBoolean(acct.DataObject.IncludeInForecaster), "Header accounts cannot be included in forecaster");
        }

		[Test]
		public void UpdateAccount()
		{
		    var accountToUpdateId = CreateNonBankAccount();
			var accountProxy = new AccountProxy();

			//Get account, change name then update.
			var acct = accountProxy.GetAccount(accountToUpdateId);

			var newName = string.Format("UpdatedAccount_{0}", Guid.NewGuid());

			var updatedAccount = new AccountDetail
			{
				Name = newName,
				AccountType = "Equity",
				IsActive = false,
				IsBankAccount = false,
				LastUpdatedId = acct.DataObject.LastUpdatedId,
				DefaultTaxCode = "G1,G4",
				Currency = "AUD",
				LedgerCode = "BB"
			};

			var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), updatedAccount);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify change.
			acct = accountProxy.GetAccount(accountToUpdateId);

			Assert.IsNotNull(acct, "Account is null");
			Assert.AreEqual(acct.DataObject.Name, newName, "Names not equal");
			Assert.AreEqual(acct.DataObject.AccountType, "Equity", "Account types not equal");
			Assert.AreEqual(acct.DataObject.IsActive, false, "IsActive not equal");
			Assert.AreEqual(acct.DataObject.DefaultTaxCode, "G1,G4", "Default tax codes not equal");
			Assert.AreEqual(acct.DataObject.Currency, "AUD", "Currencies not equal");
			Assert.AreEqual(acct.DataObject.LedgerCode, "BB", "Ledger codes not equal");
			Assert.AreEqual(acct.DataObject.IncludeInForecaster, false, "Include in Forecaster should be false for non bank accounts");
		}

        [Test]
	    public void CannotUpdateToPendingAccountType()
	    {
	        var accountProxy = new AccountProxy();

	        var accountToUpdateId = CreateBankAccount();
	        var bankAccountId = CreateBankAccount();

	        //Get account, change name then update.
	        var acct = accountProxy.GetAccount(accountToUpdateId);

	        var newName = string.Format("UpdatedAccount_{0}", Guid.NewGuid());
	        var newBankAccountName = string.Format("Update Bank Account_{0}", Guid.NewGuid());

	        var updatedAccount = new AccountDetail
	        {
	            Name = newName,
	            AccountType = "Pending",
	            IsActive = false,
	            IsBankAccount = true,
	            LastUpdatedId = acct.DataObject.LastUpdatedId,
	            DefaultTaxCode = null,
	            Currency = "AUD",
	            LedgerCode = "BB",
	            IncludeInForecaster = false,
	            BSB = "020202",
	            Number = "22222222",
	            BankAccountName = newBankAccountName,
	            BankFileCreationEnabled = true,
	            BankCode = "B",
	            UserNumber = "333",
	            MerchantFeeAccountId = bankAccountId,
	            IncludePendingTransactions = false
	        };

	        var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), updatedAccount);


	        Assert.IsNotNull(response, "Reponse is null");
	        Assert.IsFalse(response.IsSuccessfull, "Reponse has not been successful");
        }

		[Test]
		public void UpdateBankAccount()
		{
			var accountProxy = new AccountProxy();

            var accountToUpdateId = CreateBankAccount();
		    var bankAccountId = CreateBankAccount();

            //Get account, change name then update.
            var acct = accountProxy.GetAccount(accountToUpdateId);

			var newName = string.Format("UpdatedAccount_{0}", Guid.NewGuid());
			var newBankAccountName = string.Format("Update Bank Account_{0}", Guid.NewGuid());

			var updatedAccount = new AccountDetail
			{
				Name = newName,
				AccountType = "Equity",
				IsActive = false,
				IsBankAccount = true,
				LastUpdatedId = acct.DataObject.LastUpdatedId,
				DefaultTaxCode = null,
				Currency = "AUD",
				LedgerCode = "BB",
				IncludeInForecaster = false,
				BSB = "020202",
				Number = "22222222",
				BankAccountName = newBankAccountName,
				BankFileCreationEnabled = true,
				BankCode = "B",
				UserNumber = "333",
				MerchantFeeAccountId = bankAccountId,
				IncludePendingTransactions = false
			};

			var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), updatedAccount);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify change.
			acct = accountProxy.GetAccount(accountToUpdateId);

			Assert.IsNotNull(acct, "Account in null");
			Assert.AreEqual(acct.DataObject.Name, newName, "Names not equal");
			Assert.AreEqual(acct.DataObject.AccountType, "Equity", "Account types not equal");
			Assert.AreEqual(acct.DataObject.IsActive, false, "IsAcive not equal");
			Assert.IsNull(acct.DataObject.DefaultTaxCode, "Default should be null");
			Assert.AreEqual(acct.DataObject.Currency, "AUD", "Currencies not equal");
			Assert.AreEqual(acct.DataObject.LedgerCode, "BB", "Ledger codes not equal");
			Assert.AreEqual(acct.DataObject.IncludeInForecaster, false, "Include in Forecaster not equal");
			Assert.AreEqual(acct.DataObject.BSB, "020202", "BSBs not equal");
			Assert.AreEqual(acct.DataObject.Number, "22222222", "Account Numbers not equal");
			Assert.AreEqual(acct.DataObject.BankAccountName, newBankAccountName, "Bank account names not equal");
			Assert.AreEqual(acct.DataObject.BankFileCreationEnabled, true, "BankFileCreationEnabled not equal");
			Assert.AreEqual(acct.DataObject.BankCode, "B", "Bank codes not equal");
			Assert.AreEqual(acct.DataObject.UserNumber, "333", "User numbers not equal");
			Assert.AreEqual(acct.DataObject.MerchantFeeAccountId, bankAccountId, "Merchant accounts not equal");
			Assert.AreEqual(acct.DataObject.IncludePendingTransactions, false, "IncludePendingTransactions not equal");
		}

		[Test]
		public void UpdateBankAccountBankFileCreationEnabled()
		{
			var accountProxy = new AccountProxy();
		    var bankAccountId = CreateBankAccount();

            //Get account, change name then update.
            var acct = accountProxy.GetAccount(bankAccountId);

			var newBankName = string.Format("UpdatedBankName_{0}", Guid.NewGuid());

			acct.DataObject.BankAccountName = newBankName;
			acct.DataObject.BankFileCreationEnabled = true;
			acct.DataObject.BankCode = "AAA";
			acct.DataObject.UserNumber = "222";

			var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), acct.DataObject);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify change.
			acct = accountProxy.GetAccount(bankAccountId);

			Assert.IsNotNull(acct, "Account in null");
			Assert.AreEqual(acct.DataObject.BankAccountName, newBankName, "Bank account names not equal");
			Assert.AreEqual(acct.DataObject.BankFileCreationEnabled, true, "BankFileCreationEnabled not equal");
			Assert.AreEqual(acct.DataObject.BankCode, "AAA", "Bank codes not equal");
			Assert.AreEqual(acct.DataObject.UserNumber, "222", "User numbers not equal");

			//Reset Bank Code and Customer Number for other tests.
			acct.DataObject.BankFileCreationEnabled = true;
			acct.DataObject.BankCode = "TBA";
			acct.DataObject.UserNumber = "111";

			accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), acct.DataObject);
		}

		[Test]
		public void UpdateBankAccountBankFileCreationNotEnabled()
		{
			var accountProxy = new AccountProxy();
		    var bankAccountId = CreateBankAccount();
            //Get account, change fields then update.
            var acct = accountProxy.GetAccount(bankAccountId);

			var newBankName = string.Format("UpdatedBankName_{0}", Guid.NewGuid());

			acct.DataObject.BankAccountName = newBankName;
			acct.DataObject.BankFileCreationEnabled = false;
			acct.DataObject.BankCode = null;
			acct.DataObject.UserNumber = null;

			var response = accountProxy.UpdateAccount(Convert.ToInt32(acct.DataObject.Id), acct.DataObject);

			Assert.IsNotNull(response, "Reponse is null");
			Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");

			//Get account again and verify change.
			acct = accountProxy.GetAccount(bankAccountId);

			Assert.IsNotNull(acct, "Account in null");
			Assert.AreEqual(acct.DataObject.BankAccountName, newBankName, "Bank account names not equal");
			Assert.AreEqual(acct.DataObject.BankFileCreationEnabled, false, "BankFileCreationEnabled not equal");

			//Bank code and user number should not have changed because BankFileCreationEnabled was false.
			Assert.IsNull(acct.DataObject.BankCode, "Bank code not null");
			Assert.IsNull(acct.DataObject.UserNumber, "User number not null");
		}

        [Test]
        public void UpdateHeaderAccount()
        {
            //Create and Insert
            var account = GetTestHeaderAccount();

            var accountProxy = new AccountProxy();
            var response = accountProxy.InsertAccount(account);

            Assert.IsNotNull(response, "Reponse is null");
            Assert.IsTrue(response.IsSuccessfull, "Reponse has not been successful");
            Assert.Greater(response.DataObject.InsertedEntityId, 0, "Zero accounts returned");

            var accountId = response.DataObject.InsertedEntityId;

            //Get account again and verify inserted fields.
            var insertedAcctFromDb = accountProxy.GetAccount(accountId);

            var newName = string.Format("TestAccount_{0}", Guid.NewGuid());
            account.Name = newName;
            account.LastUpdatedId = insertedAcctFromDb.DataObject.LastUpdatedId;

            var updateResponse = accountProxy.UpdateAccount(response.DataObject.InsertedEntityId, account);
            Assert.IsNotNull(updateResponse, "Reponse is null");
            Assert.IsTrue(updateResponse.IsSuccessfull, "Reponse has not been successful");

            //Get account again and verify inserted fields.
            var updatedAcctFromDb = accountProxy.GetAccount(accountId);
            Assert.IsNotNull(updatedAcctFromDb, "Reponse is null");
            Assert.IsTrue(updatedAcctFromDb.IsSuccessfull, "Reponse has not been successful");

            Assert.AreEqual(updatedAcctFromDb.DataObject.Name, newName);
        }

		#region Test Data

	    private int CreateNonBankAccount()
	    {
	        var accountProxy = new AccountProxy();

            var account = GetTestAccount();
            return accountProxy.InsertAccount(account).DataObject.InsertedEntityId;
        }

	    private int CreateBankAccount()
	    {
	        var accountProxy = new AccountProxy();

	        var account = GetTestBankAccount();
	        return accountProxy.InsertAccount(account).DataObject.InsertedEntityId;
	    }


	    private int CreateTestHeaderAccount()
	    {
	        var accountProxy = new AccountProxy();

            var account = GetTestHeaderAccount();
            var insertResult = accountProxy.InsertAccount(account);
            return insertResult.DataObject.InsertedEntityId;
        }

	    private int CreateAndAssignTestAccountToHeaderAccount(int headerAccountId)
	    {
	        var accountProxy = new AccountProxy();

            var account = GetTestAccount();
	        account.HeaderAccountId = headerAccountId;
	        var insertResult = accountProxy.InsertAccount(account);
	        return insertResult.DataObject.InsertedEntityId;
	    }
        //private void CreateTestData()
        //{
        //	var accountProxy = new AccountProxy();

        //	if (_nonBankAcctId == 0)
        //	{
        //		var account = GetTestAccount();
        //		var insertResult = accountProxy.InsertAccount(account);

        //		_nonBankAcctId = insertResult.DataObject.InsertedEntityId;
        //	}

        //	if (_bankAcctId == 0)
        //	{
        //		var account = GetTestBankAccount();
        //		var insertResult = accountProxy.InsertAccount(account);

        //		_bankAcctId = insertResult.DataObject.InsertedEntityId;
        //	}

        //	if (_inactiveAccountId == 0)
        //	{
        //		var account = GetTestAccount();
        //		account.IsActive = false;
        //		var insertResult = accountProxy.InsertAccount(account);

        //		_inactiveAccountId = insertResult.DataObject.InsertedEntityId;
        //	}

        //	if (_accountToBeUpdated == 0)
        //	{
        //		var account = GetTestAccount();
        //		var insertResult = accountProxy.InsertAccount(account);

        //		_accountToBeUpdated = insertResult.DataObject.InsertedEntityId;
        //	}

        //	if (_bankAccountToBeUpdated == 0)
        //	{
        //		var account = GetTestBankAccount();
        //		var insertResult = accountProxy.InsertAccount(account);

        //		_bankAccountToBeUpdated = insertResult.DataObject.InsertedEntityId;
        //	}

        //          if (_headerAccountId == 0)
        //          {
        //              var account = GetTestHeaderAccount();

        //              var insertResult = accountProxy.InsertAccount(account);

        //              _headerAccountId = insertResult.DataObject.InsertedEntityId;
        //          }

        //    if (_accountToAssignToHeaderAccount == 0)
        //    {
        //        var account = GetTestAccount();
        //        account.HeaderAccountId = _headerAccountId;
        //        var insertResult = accountProxy.InsertAccount(account);
        //        _accountToAssignToHeaderAccount = insertResult.DataObject.InsertedEntityId;
        //    }
        //}

        private AccountDetail GetTestAccount()
		{
			return new AccountDetail
			{
				Name = string.Format("TestAccount_{0}", Guid.NewGuid()),
				AccountType = "Income",
				IsActive = true,
				DefaultTaxCode = "G1",
				LedgerCode = "AA",
				Currency = "AUD",
				IsBankAccount = false
			};
		}

		private AccountDetail GetTestBankAccount()
		{
			return new AccountDetail
			{
				Name = string.Format("TestAccount_{0}", Guid.NewGuid()),
				AccountType = "Asset",
				IsActive = true,
				DefaultTaxCode = null,
				LedgerCode = "BB",
				Currency = "AUD",
				IsBankAccount = true,
				IncludeInForecaster = true,
				BSB = "010101",
				Number = "11111111",
				BankAccountName = string.Format("Test Bank Account_{0}", Guid.NewGuid()),
				BankFileCreationEnabled = true,
				BankCode = "TBA",
				UserNumber = "111",
				MerchantFeeAccountId = null,
				IncludePendingTransactions = true
			};
		}

        private AccountDetail GetTestHeaderAccount()
        {
            return new AccountDetail
            {
                Name = string.Format("TestAccount_{0}", Guid.NewGuid()),
                AccountLevel = "Header",
                AccountType = "Income",
                LedgerCode = "AA"
            };
        }


    }
		#endregion
}



















