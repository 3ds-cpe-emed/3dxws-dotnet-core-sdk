using ds.authentication;
using ds.authentication.redirection;
using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.document.model;
using ds.enovia.document.service;
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

        model.UserInfo m_userInfo = null;

        public async Task<IPassportAuthentication> Authenticate()
        {
            UserPassport passport = new UserPassport(m_passportUrl);

            UserInfoRedirection userInfoRedirection = new UserInfoRedirection(m_enoviaUrl, m_tenant);
            userInfoRedirection.Current = true;
            userInfoRedirection.IncludeCollaborativeSpaces = true;
            userInfoRedirection.IncludePreferredCredentials = true;

            m_userInfo = await passport.CASLoginWithRedirection<model.UserInfo>(m_username, m_password, false, userInfoRedirection);

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
        public async Task Search_DocumentsByCollaborativeSpace(string _securityContext, string _collaborativeSpace)
        {
            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Document Service wrapper
            DocumentService documentService = new DocumentService(m_enoviaUrl, passport);

            documentService.SecurityContext = _securityContext;
            documentService.Tenant = m_tenant;

            SearchByCollaborativeSpace search = new SearchByCollaborativeSpace(_collaborativeSpace);

            List<NlsLabeledItemSet<Document>> pagedSearchResult  = await documentService.SearchAll(search);

            long totalItemsMember = 0;
            long totalItems = 0;

            foreach (NlsLabeledItemSet<Document> searchPage in pagedSearchResult)
            {
                  totalItems += searchPage.totalItems;
                  totalItemsMember += searchPage.member.Count;   
            }

            Assert.AreEqual(totalItems, totalItemsMember);

            Console.WriteLine($"Total Items: {totalItems}");

      }
      
      [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "B27E90CCBB334129A7B73D9B0883CBA8", "863FB0B4307E495BB6543BBB90163077")]
      public async Task Get_AttachedDocuments(string _securityContext, string _parentId0, string _parentId1)
      {
         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Document Service wrapper
         DocumentService documentService = new DocumentService(m_enoviaUrl, passport);

         documentService.SecurityContext = _securityContext;
         documentService.Tenant = m_tenant;

         DocumentResponse<Document> attachedDocs0 = await documentService.GetAttachedDocuments(_parentId0);

         Assert.IsNotNull(attachedDocs0);

         Assert.IsNotNull(attachedDocs0.data);

         Assert.AreEqual(0, attachedDocs0.data.Count);


         DocumentResponse<Document> attachedDocs1 = await documentService.GetAttachedDocuments(_parentId1);

         Assert.IsNotNull(attachedDocs1);

         Assert.IsNotNull(attachedDocs1.data);

         Assert.AreEqual(1, attachedDocs1.data.Count);

      }

      [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "C437FF98237700006331747200068B1F")]
      public async Task Get_SpecificationDocuments(string _securityContext, string _parentId0)
      {
         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Document Service wrapper
         DocumentService documentService = new DocumentService(m_enoviaUrl, passport);

         documentService.SecurityContext = _securityContext;
         documentService.Tenant = m_tenant;

         DocumentResponse<Document> specDocs0 = await documentService.GetSpecificationDocuments(_parentId0);

         Assert.IsNotNull(specDocs0);

         Assert.IsNotNull(specDocs0.data);

         Assert.AreEqual(1, specDocs0.data.Count);

      }
   }
}