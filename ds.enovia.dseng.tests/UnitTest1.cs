using ds.authentication;
using ds.authentication.redirection;
using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.dseng.exception;
using ds.enovia.dseng.model;
using ds.enovia.dseng.service;
using ds.enovia.model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ds.enovia.dseng.tests
{
   public class Tests
   {
      const string DS3DXWS_AUTH_USERNAME = "DS3DXWS_AUTH_USERNAME";
      const string DS3DXWS_AUTH_PASSWORD = "DS3DXWS_AUTH_PASSWORD";
      const string DS3DXWS_AUTH_PASSPORT = "DS3DXWS_AUTH_PASSPORT";
      const string DS3DXWS_AUTH_ENOVIA = "DS3DXWS_AUTH_ENOVIA";
      const string DS3DXWS_AUTH_TENANT = "DS3DXWS_AUTH_TENANT";

      string m_username = string.Empty;
      string m_password = string.Empty;
      string m_passportUrl = string.Empty;
      string m_enoviaUrl = string.Empty;
      string m_tenant = string.Empty;

      UserInfo m_userInfo = null;

      public async Task<IPassportAuthentication> Authenticate()
      {
         UserPassport passport = new UserPassport(m_passportUrl);

         UserInfoRedirection userInfoRedirection = new UserInfoRedirection(m_enoviaUrl, m_tenant);
         userInfoRedirection.Current = true;
         userInfoRedirection.IncludeCollaborativeSpaces = true;
         userInfoRedirection.IncludePreferredCredentials = true;

         m_userInfo = await passport.CASLoginWithRedirection<UserInfo>(m_username, m_password, false, userInfoRedirection);

         Assert.IsNotNull(m_userInfo);

         StringAssert.AreEqualIgnoringCase(m_userInfo.name, m_username);

         Assert.IsTrue(passport.IsCookieAuthenticated);

         return passport;
      }


      [SetUp]
      public void Setup()
      {
         m_username = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_USERNAME, EnvironmentVariableTarget.User); // e.g. AAA27
         m_password = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_PASSWORD, EnvironmentVariableTarget.User); // e.g. your password
         m_passportUrl = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_PASSPORT, EnvironmentVariableTarget.User); // e.g. https://eu1-ds-iam.3dexperience.3ds.com:443/3DPassport

         m_enoviaUrl = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_ENOVIA, EnvironmentVariableTarget.User); // e.g. https://r1132100982379-eu1-space.3dexperience.3ds.com:443/enovia
         m_tenant = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_TENANT, EnvironmentVariableTarget.User); // e.g. R1132100982379
      }

      private string CurrentUserPreferredCredentials()
      {
         return m_userInfo.preferredcredentials.ToString();
      }

      [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "Engineering Item Title", "Engineering Item Description")]
      public async Task Create_EngineeringItem(string _securityContext, string _title, string _description)
      {
         #region Arrange

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Engineering Service wrapper
         EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
         engineeringServices.SecurityContext = _securityContext;
         engineeringServices.Tenant = m_tenant;

         EngineeringItemCreate itemCreate = new EngineeringItemCreate();

         ((EngineeringItemAttributes)itemCreate.attributes).title = _title;
         ((EngineeringItemAttributes)itemCreate.attributes).description = _description;

         #endregion

         #region Act - Create Engineering Item

         NlsLabeledItemSet<EngineeringItem> itemSet = await engineeringServices.CreateEngineeringItem(itemCreate);
         #endregion

         #region Assert
         Assert.IsNotNull(itemSet);
         Assert.AreEqual(itemSet.totalItems, 1);

         StringAssert.AreEqualIgnoringCase(itemSet.member[0].title, _title);
         StringAssert.AreEqualIgnoringCase(itemSet.member[0].description, _description);

         #endregion
      }

      [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "A437358E00006E50621D045E0000F2A8")]
      public async Task Get_EngineeringItem_Usage(string _securityContext, string _engineeringItemId)
      {
         #region Arrange

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Engineering Service wrapper
         EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
         engineeringServices.SecurityContext = _securityContext;
         engineeringServices.Tenant = m_tenant;

         #endregion

         #region Act - Get Engineering Item Common attributes

         EngineeringItemCommon engItemCommon = await engineeringServices.GetEngineeringItemCommon(_engineeringItemId);
         #endregion

         #region Assert
         Assert.IsNotNull(engItemCommon);
         Assert.AreEqual(engItemCommon.usage, "3DPart");

         #endregion
      }

      [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "221CD96C0A1B000064C928600012881F", "221CD96C0A1B000064C970100016855F")]
      public async Task Create_EngineeringInstances(string _securityContext, string _sourceItemId, string _targetItemId)
      {
         #region Arrange

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Engineering Service wrapper
         EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
         engineeringServices.SecurityContext = _securityContext;
         engineeringServices.Tenant = m_tenant;

         #endregion

         #region Act - Get Engineering Item Common attributes
         EngineeringInstanceCreate engineeringInstanceCreate = new EngineeringInstanceCreate();
         engineeringInstanceCreate.attributes.name = "i1";
         engineeringInstanceCreate.referencedObject = new EngineeringItemIdentifier(_targetItemId, m_enoviaUrl);

         EngineeringInstancesCreate engineeringInstancesCreate = new EngineeringInstancesCreate();
         engineeringInstancesCreate.instances.Add(engineeringInstanceCreate);

         try
         {
            NlsLabeledItemSet<EngineeringItem> engItem = await engineeringServices.CreateEngineeringInstances(_sourceItemId, engineeringInstancesCreate);
            Assert.IsNotNull(engItem);
         }
         catch (CreateEngineeringInstancesException _ex)
         {
            Assert.Fail(await _ex.GetErrorMessage());

         }
         catch (Exception _ex)
         {
            Assert.Fail(_ex.Message);
         }
         #endregion

         #region Assert


         #endregion
      }

      [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "221CD96C0A1B000064C928600012881F", "221CD96C0A1B000064C970100016855F")]
      public async Task Create_MultipleEngineeringInstances(string _securityContext, string _sourceItemId, string _targetItemId)
      {
         #region Arrange

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Engineering Service wrapper
         EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
         engineeringServices.SecurityContext = _securityContext;
         engineeringServices.Tenant = m_tenant;

         #endregion

         #region Act - Get Engineering Item Common attributes
         EngineeringInstanceCreate engineeringInstanceCreate = new EngineeringInstanceCreate();
         engineeringInstanceCreate.referencedObject = new EngineeringItemIdentifier(_targetItemId, m_enoviaUrl);

         EngineeringInstancesCreate engineeringInstancesCreate = new EngineeringInstancesCreate();

         for (int i = 0; i < 10; i++)
         {
            engineeringInstanceCreate.attributes.name = $"i-{i}";
            engineeringInstancesCreate.instances.Add(engineeringInstanceCreate);
         }
         
         try
         {
            NlsLabeledItemSet<EngineeringItem> engItem = await engineeringServices.CreateEngineeringInstances(_sourceItemId, engineeringInstancesCreate);
            Assert.IsNotNull(engItem);
         }
         catch (CreateEngineeringInstancesException _ex)
         {
            Assert.Fail(await _ex.GetErrorMessage());

         }
         catch (Exception _ex)
         {
            Assert.Fail(_ex.Message);
         }
         #endregion

         #region Assert


         #endregion
      }

      #region Bulk Fetch Tests
      [TestCase("AAA27")]
      public async Task Get_BulkFetch(string _searchText)
      {
         #region Arrange

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Engineering Service wrapper
         EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
         engineeringServices.SecurityContext = CurrentUserPreferredCredentials();
         engineeringServices.Tenant = m_tenant;

         #endregion

         #region Act - Get Engineering Item Common attributes
         SearchByFreeText searchByFreeText = new SearchByFreeText(_searchText);

         List<EngineeringItem> allResults = BulkFetchUtils<EngineeringItem>.AsList(await engineeringServices.SearchAll(searchByFreeText));
         
         List<List<string>> bulkFetchLists = BulkFetchUtils<EngineeringItem>.GetSplitIds(allResults);
         
         try
         {
            for (int i = 0; i < bulkFetchLists.Count; i++)
            {
               IList<EngineeringItem> resultAsDefaultMask = await engineeringServices.BulkFetchAsDefaultMask(bulkFetchLists[i].ToArray());

               Assert.AreEqual(bulkFetchLists[i].Count, resultAsDefaultMask.Count);

               IList<EngineeringItemCommon> resultAsCommonMask = await engineeringServices.BulkFetchAsCommonMask(bulkFetchLists[i].ToArray());

               Assert.AreEqual(bulkFetchLists[i].Count, resultAsCommonMask.Count);

               IList<EngineeringItem> resultAsDetailMask = await engineeringServices.BulkFetchAsDetailMask(bulkFetchLists[i].ToArray());

               Assert.AreEqual(bulkFetchLists[i].Count, resultAsDetailMask.Count);
            }
         }
         catch (BulkFetchException _ex)
         {
            Assert.Fail(await _ex.GetErrorMessage());
         }
         catch (Exception _ex)
         {
            Assert.Fail(_ex.Message);
         }
         #endregion

         #region Assert

         #endregion
      }

      #endregion

      [TestCase("5BAFBB4D5EAE4772ACE791986F12B668")]
      public async Task Get_EngineeringItem(string _engineeringItemId)
      {
         #region Arrange

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Engineering Service wrapper
         EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
         engineeringServices.SecurityContext = CurrentUserPreferredCredentials();
         engineeringServices.Tenant = m_tenant;

         #endregion

         #region Act - Get Engineering Item Common attributes

         EngineeringItem engItem = await engineeringServices.GetEngineeringItemDetails(_engineeringItemId);
         #endregion

         #region Assert
         Assert.IsNotNull(engItem);
         #endregion
      }

      [TestCase("5BAFBB4D5EAE4772ACE791986F12B668")]
      public async Task Modify_EngineeringItem(string _engineeringItemId)
      {
         #region Arrange

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Engineering Service wrapper
         EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
         engineeringServices.SecurityContext = CurrentUserPreferredCredentials();
         engineeringServices.Tenant = m_tenant;

         #endregion

         #region Act - Get Engineering Item Common attributes

         try
         {
            EngineeringItem engItem = await engineeringServices.GetEngineeringItemDetails(_engineeringItemId);

            engItem.isManufacturable = GetOppositeValue(engItem.isManufacturable);

            EngineeringItem engItemUpdate1 = await engineeringServices.UpdateEngineeringItem(engItem, true);

            engItemUpdate1.isManufacturable = GetOppositeValue(engItemUpdate1.isManufacturable);

            EngineeringItem engItemUpdate2 = await engineeringServices.UpdateEngineeringItem(engItemUpdate1, true);

            #region Assert
            Assert.That(engItemUpdate1.isManufacturable.Equals(engItemUpdate2.isManufacturable, StringComparison.InvariantCultureIgnoreCase));
            #endregion
         }
         catch (EngineeringResponseException _ex)
         {
            Assert.Fail (await _ex.GetErrorMessage());
         }
         #endregion
      }

      private string GetOppositeValue(string _value)
      {
         if (_value?.Equals(false.ToString(), StringComparison.InvariantCultureIgnoreCase) != false)
         {
            return FormatBoolean(true);
         }
         else
         {
            return FormatBoolean(false);
         }
      }

      private string FormatBoolean(bool _bool)
      {
         return _bool.ToString().ToUpperInvariant();
      }
   }
}