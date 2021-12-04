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
            xCADFamilyRepresentationService xcadFamilyRepService = new xCADFamilyRepresentationService(m_enoviaUrl, passport);
            xcadFamilyRepService.SecurityContext = _securityContext;
            xcadFamilyRepService.Tenant = m_tenant;

            SearchByNameRevision query = new SearchByNameRevision(_cadfamilymodelName, _cadfamilymodelRevision);

            IList<xCADFamilyRepresentation> searchReturnSet = await xcadFamilyRepService.Search(query);

            Assert.IsNotNull(searchReturnSet);
            Assert.NotZero(searchReturnSet.Count, $"Cannot find XCADFamilyRepresentation with name = '{_cadfamilymodelName}' and revision = '{_cadfamilymodelRevision}'");

            xCADFamilyRepresentation part = searchReturnSet[0];
            string partId = part.id;
            
            HttpClient downloadHttpClient = new ServiceCollection().AddHttpClient().BuildServiceProvider().GetService<IHttpClientFactory>().CreateClient("download");

            downloadHttpClient.Timeout = TimeSpan.FromSeconds(_timeout);
            #endregion

            #region Act - download authoring file
            FileInfo downloadedFile = await xcadFamilyRepService.DownloadAuthoringFile(downloadHttpClient, partId, _downloadPath);
            #endregion

            #region Assert
            Assert.IsNotNull(downloadedFile);
            #endregion
        }
    }
}