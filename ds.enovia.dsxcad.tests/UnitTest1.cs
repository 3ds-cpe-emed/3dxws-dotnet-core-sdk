using ds.authentication;
using ds.authentication.redirection;
using ds.enovia.common.search;
using ds.enovia.dsxcad.model;
using ds.enovia.dsxcad.service;
using ds.enovia.model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ds.enovia.dsxcad.tests
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
            m_username = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_USERNAME, EnvironmentVariableTarget.User); // e.g. AAA27
            m_password = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_PASSWORD, EnvironmentVariableTarget.User); // e.g. your password
            m_passportUrl = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_PASSPORT, EnvironmentVariableTarget.User); // e.g. https://eu1-ds-iam.3dexperience.3ds.com:443/3DPassport

            m_enoviaUrl = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_ENOVIA, EnvironmentVariableTarget.User); // e.g. https://r1132100982379-eu1-space.3dexperience.3ds.com:443/enovia
            m_tenant = Environment.GetEnvironmentVariable(DS3DXWS_AUTH_TENANT, EnvironmentVariableTarget.User); // e.g. R1132100982379
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

        [TestCase("VPLMAdmin.Company Name.Default", "c:\\temp", "xcadmodel-R1132100982379-00000190", "A.1", 180)]
        public async Task Download_XCADModelAuthoringFile(string _securityContext, string _downloadPath, string _cadfamilymodelName, string _cadfamilymodelRevision, int _timeout)
        {
            #region Arrange

            Assert.IsTrue(Directory.Exists(_downloadPath), $"Cannot find '{_downloadPath}'");

            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Search for the xCAD Family by name and revision
            xCADFamilyRepresentationService xcadService = new xCADFamilyRepresentationService(m_enoviaUrl, passport);
            xcadService.SecurityContext = _securityContext;
            xcadService.Tenant = m_tenant;

            SearchByNameRevision query = new SearchByNameRevision(_cadfamilymodelName, _cadfamilymodelRevision);

            IList<xCADFamilyRepresentation> searchReturnSet = await xcadService.Search(query);

            Assert.IsNotNull(searchReturnSet);
            Assert.NotZero(searchReturnSet.Count, $"Cannot find XCADFamilyRepresentation with name = '{_cadfamilymodelName}' and revision = '{_cadfamilymodelRevision}'");

            xCADFamilyRepresentation part = searchReturnSet[0];
            string partId = part.id;
            
            HttpClient downloadHttpClient = new ServiceCollection().AddHttpClient().BuildServiceProvider().GetService<IHttpClientFactory>().CreateClient("download");

            downloadHttpClient.Timeout = TimeSpan.FromSeconds(_timeout);
            #endregion

            #region Act - download authoring file
            FileInfo downloadedFile = await xcadService.DownloadAuthoringFile(downloadHttpClient, partId, _downloadPath);
            #endregion

            #region Assert
            Assert.IsNotNull(downloadedFile);
            #endregion
        }
      [TestCase("VPLMAdmin.Company Name.Default", "AAA27 Personal")]
      public async Task Search_Products_And_Get_Details_of_First(string _securityContext, string _collaborativeSpace)
      {
        
         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Search for the xCAD Family by name and revision
         xCADProductService xcadService = new xCADProductService(m_enoviaUrl, passport);
         xcadService.SecurityContext = _securityContext;
         xcadService.Tenant = m_tenant;

         SearchByCollaborativeSpace query = new SearchByCollaborativeSpace(_collaborativeSpace);

         IList<xCADProduct> searchReturnSet = await xcadService.Search(query);

         Assert.IsNotNull(searchReturnSet);
         Assert.NotZero(searchReturnSet.Count, $"Cannot find Products in Collaborative Space'{_collaborativeSpace}'");

         xCADProduct product = searchReturnSet[0];
         string productId = product.id;

         xCADProduct productDetails  = await xcadService.GetXCADProduct(productId, xCADProductDetails.Details);

         #region Assert
         Assert.IsNotNull(productDetails);

         Assert.AreEqual(productId, productDetails.id);
         #endregion
      }

      [TestCase("VPLMAdmin.Company Name.Default", "AAA27 Personal")]
      public async Task Search_Parts_And_Get_Details_of_First(string _securityContext, string _collaborativeSpace)
      {

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Search for the xCAD Family by name and revision
         xCADPartService xcadService = new xCADPartService(m_enoviaUrl, passport);
         xcadService.SecurityContext = _securityContext;
         xcadService.Tenant = m_tenant;

         SearchByCollaborativeSpace query = new SearchByCollaborativeSpace(_collaborativeSpace);

         IList<xCADPart> searchReturnSet = await xcadService.Search(query);

         Assert.IsNotNull(searchReturnSet);
         Assert.NotZero(searchReturnSet.Count, $"Cannot find Products in Collaborative Space'{_collaborativeSpace}'");

         xCADPart part = searchReturnSet[0];
         string partId = part.id;

         xCADPart partDetails = await xcadService.GetXCADPart(partId, xCADPartDetails.Details);

         #region Assert
         Assert.IsNotNull(partDetails);

         Assert.AreEqual(partId, partDetails.id);

         Assert.IsNotNull(partDetails.AuthoringFile);
         #endregion
      }

      [TestCase("VPLMAdmin.Company Name.Default", "AAA27 Personal")]
      public async Task Search_Representations_And_Get_Details_of_First(string _securityContext, string _collaborativeSpace)
      {

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Search for the xCAD Family by name and revision
         xCADRepresentationService xcadService = new xCADRepresentationService(m_enoviaUrl, passport);
         xcadService.SecurityContext = _securityContext;
         xcadService.Tenant = m_tenant;

         SearchByCollaborativeSpace query = new SearchByCollaborativeSpace(_collaborativeSpace);

         IList<xCADRepresentation> searchReturnSet = await xcadService.Search(query);

         Assert.IsNotNull(searchReturnSet);
         Assert.NotZero(searchReturnSet.Count, $"Cannot find Representations in Collaborative Space'{_collaborativeSpace}'");

         xCADRepresentation rep = searchReturnSet[0];
         string repId = rep.id;

         xCADRepresentation repDetails = await xcadService.GetXCADRepresentation(repId, xCADRepresentationDetails.Details);

         #region Assert
         Assert.IsNotNull(repDetails);

         Assert.AreEqual(repId, repDetails.id);
         #endregion
      }


      [TestCase("VPLMAdmin.Company Name.Default", "AAA27 Personal")]
      public async Task Search_Templates_And_Get_Details_of_First(string _securityContext, string _collaborativeSpace)
      {

         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Search for the xCAD Family by name and revision
         xCADTemplateService xcadService = new xCADTemplateService(m_enoviaUrl, passport);
         xcadService.SecurityContext = _securityContext;
         xcadService.Tenant = m_tenant;

         SearchByCollaborativeSpace query = new SearchByCollaborativeSpace(_collaborativeSpace);

         IList<xCADTemplate> searchReturnSet = await xcadService.Search(query);

         Assert.IsNotNull(searchReturnSet);
         Assert.NotZero(searchReturnSet.Count, $"Cannot find Templates in Collaborative Space'{_collaborativeSpace}'");

         xCADTemplate template = searchReturnSet[0];
         string templateId = template.id;

         xCADTemplate templateDetails = await xcadService.GetXCADTemplate(templateId, xCADTemplateDetails.Default);

         #region Assert
         Assert.IsNotNull(templateDetails);

         Assert.AreEqual(templateId, templateDetails.id);
         #endregion
      }
   }
}