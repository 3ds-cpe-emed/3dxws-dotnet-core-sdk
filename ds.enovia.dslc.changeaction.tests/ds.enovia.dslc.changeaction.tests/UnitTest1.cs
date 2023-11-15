using ds.authentication;
using ds.authentication.redirection;

using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.dslc.changeaction.model;
using ds.enovia.dslc.changeaction.service;
using ds.enovia.model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ds.enovia.dslc.tests
{
    public class Tests
    {
        const string DS3DXWS_AUTH_USERNAME = "DS3DXWS_AUTH_USERNAME";
        const string DS3DXWS_AUTH_PASSWORD = "DS3DXWS_AUTH_PASSWORD";
        const string DS3DXWS_AUTH_PASSPORT = "DS3DXWS_AUTH_PASSPORT";
        const string DS3DXWS_AUTH_ENOVIA   = "DS3DXWS_AUTH_ENOVIA";
        const string DS3DXWS_AUTH_TENANT   = "DS3DXWS_AUTH_TENANT";

        string m_username    = string.Empty;
        string m_password    = string.Empty;
        string m_passportUrl = string.Empty;
        string m_enoviaUrl   = string.Empty;
        string m_tenant      = string.Empty;

        UserInfo m_userInfo = null;

        [SetUp]
        public void Setup()
        {
            m_username    = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_USERNAME, EnvironmentVariableTarget.User); // e.g. AAA27
            m_password    = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_PASSWORD, EnvironmentVariableTarget.User); // e.g. your password
            m_passportUrl = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_PASSPORT, EnvironmentVariableTarget.User); // e.g. https://eu1-ds-iam.3dexperience.3ds.com:443/3DPassport

            m_enoviaUrl   = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_ENOVIA, EnvironmentVariableTarget.User); // e.g. https://r1132100982379-eu1-space.3dexperience.3ds.com:443/enovia
            m_tenant      = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_TENANT, EnvironmentVariableTarget.User); // e.g. R1132100982379
        }

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

      [TestCase("CA-R1132100982379-00000710")]
      public async Task Exercise_SearchChangeActions(string _name)
      {
         IPassportAuthentication passportAuth = await Authenticate();

         //Search for an Engineering product
         ChangeActionServices caServices = new ChangeActionServices(m_enoviaUrl, passportAuth);
         caServices.SecurityContext = m_userInfo.preferredcredentials.ToString();
         caServices.Tenant = m_tenant;

         SearchByFreeText searchByName = new SearchByFreeText(_name);

         IList<BusinessObjectIdentifier> caList = await caServices.Search(searchByName);

         Assert.IsNotEmpty(caList);

         Assert.NotZero(caList.Count);

         ChangeAction ca = await caServices.GetChangeAction(caList[0].identifier);

         Assert.IsNotNull(ca);
      }
   }
}