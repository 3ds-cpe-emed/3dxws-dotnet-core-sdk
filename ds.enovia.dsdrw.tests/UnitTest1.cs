using ds.authentication;
using ds.authentication.redirection;

using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.dsdrw.model;
using ds.enovia.dsdrw.service;
using ds.enovia.model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ds.enovia.dsdrw.tests
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
            m_username    = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_USERNAME, EnvironmentVariableTarget.User); // e.g. AAA27
            m_password    = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_PASSWORD, EnvironmentVariableTarget.User); // e.g. your password
            m_passportUrl = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_PASSPORT, EnvironmentVariableTarget.User); // e.g. https://eu1-ds-iam.3dexperience.3ds.com:443/3DPassport

            m_enoviaUrl   = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_ENOVIA, EnvironmentVariableTarget.User); // e.g. https://r1132100982379-eu1-space.3dexperience.3ds.com:443/enovia
            m_tenant      = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_TENANT, EnvironmentVariableTarget.User); // e.g. R1132100982379
        }

        [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "AAA27 Personal")]
        public async Task Search_DrawingsByCollaborativeSpace_Test_1(string _securityContext, string _collaborativeSpace)
        {
            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Bookmark Service wrapper
            DrawingService drawingService = new DrawingService(m_enoviaUrl, passport);

            drawingService.SecurityContext = _securityContext;
            drawingService.Tenant = m_tenant;

            SearchByCollaborativeSpace search = new SearchByCollaborativeSpace(_collaborativeSpace);

            List<NlsLabeledItemSet<Drawing>> pagedSearchResult  = await drawingService.SearchAll(search);

            long totalItemsMember = 0;
            long totalItems = 0;

            foreach (NlsLabeledItemSet<Drawing> searchPage in pagedSearchResult)
            {
                  totalItems += searchPage.totalItems;
                  totalItemsMember += searchPage.member.Count;   
            }

            Assert.AreEqual(totalItems, totalItemsMember);

            Console.WriteLine($"Total Items: {totalItems}");

        }

      [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "AAA27 Personal")]
      public async Task Get_Drawing_Test_1(string _securityContext, string _collaborativeSpace)
      {
         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Bookmark Service wrapper
         DrawingService drawingService = new DrawingService(m_enoviaUrl, passport);

         drawingService.SecurityContext = _securityContext;
         drawingService.Tenant = m_tenant;

         SearchByCollaborativeSpace search = new SearchByCollaborativeSpace(_collaborativeSpace);

         List<NlsLabeledItemSet<Drawing>> pagedSearchResult = await drawingService.SearchAll(search);

         long totalItemsMember = 0;
         long totalItems = 0;

         foreach (NlsLabeledItemSet<Drawing> searchPage in pagedSearchResult)
         {
            totalItems += searchPage.totalItems;
            totalItemsMember += searchPage.member.Count;
         }

         Assert.AreEqual(totalItems, totalItemsMember);

         Console.WriteLine($"Total Items: {totalItems}");

         if (totalItems < 1)
         {
            throw new ArgumentOutOfRangeException("The Collaborative Space has no drawings to get");
         }

         string drawingId = pagedSearchResult[0].member[0].id;

         NlsLabeledItemSet<Drawing> info = await drawingService.GetDrawing(drawingId);

         Assert.AreEqual(drawingId, info.member[0].id);

      }
   }
}