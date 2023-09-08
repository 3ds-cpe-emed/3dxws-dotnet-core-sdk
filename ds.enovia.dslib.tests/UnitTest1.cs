using ds.authentication;
using ds.authentication.redirection;
using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.dslib.model;
using ds.enovia.dslib.service;
using ds.enovia.model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ws3dx.dseng.core.data.impl;

namespace ds.enovia.dseng.tests
{
   public class Tests
   {
      private const string DS3DXWS_AUTH_USERNAME = "DS3DXWS_AUTH_USERNAME";
      private const string DS3DXWS_AUTH_PASSWORD = "DS3DXWS_AUTH_PASSWORD";
      private const string DS3DXWS_AUTH_PASSPORT = "DS3DXWS_AUTH_PASSPORT";
      private const string DS3DXWS_AUTH_ENOVIA = "DS3DXWS_AUTH_ENOVIA";
      private const string DS3DXWS_AUTH_TENANT = "DS3DXWS_AUTH_TENANT";

      private string m_username = string.Empty;
      private string m_password = string.Empty;
      private string m_passportUrl = string.Empty;
      private string m_enoviaUrl = string.Empty;
      private string m_tenant = string.Empty;

      private UserInfo m_userInfo = null;

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

      private string GetCurrentUserPreferredCredentials()
      {
         return m_userInfo.preferredcredentials.ToString();
      }

      //Each of the physical ids above correspond to an existing classified Physical Product
      [TestCase("44C2728F4E2300006405B1EF000CFEB5")]
      [TestCase("AF02EA6D06390000624AEBDE0005EF55")]
      public async Task Get_ClassificationInformation(string _id)
      {
         #region Arrange

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the IP Classification client wrapper
         ClassificationServices classificationServices = new ClassificationServices(m_enoviaUrl, passport);
         classificationServices.SecurityContext = GetCurrentUserPreferredCredentials();
         classificationServices.Tenant = m_tenant;
         #endregion

         #region Act
         EngItemUriIdentifier engItemUriId = new EngItemUriIdentifier(_id, m_enoviaUrl);

         ClassifiedItemInstance classifiedItemInst = await classificationServices.Locate(engItemUriId);
         Assert.IsNotNull(classifiedItemInst);

         ItemsClassificationAttributes itemsClassificationAttributes = await classificationServices.GetClassificationAttributes(classifiedItemInst);
         Assert.IsNotNull(itemsClassificationAttributes);

         #endregion

         #region Assert
         Assert.Pass();
         #endregion

      }
   }
}