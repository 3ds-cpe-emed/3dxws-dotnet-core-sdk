//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2022 Dassault Systèmes - CPE EMED
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//------------------------------------------------------------------------------------------------------------------------------------


using ds.authentication;
using ds.authentication.redirection;
using ds.delmia.dsmfg.exception;
using ds.delmia.dsmfg.interfaces;
using ds.delmia.dsmfg.model.reference;
using ds.delmia.dsmfg.service;
using ds.delmia.model.collections;
using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.common.serialization;
using ds.enovia.model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ds.delmia.dsmfg.tests
{
    public class UpdateTests
    {
        const string DS3DXWS_AUTH_USERNAME  = "DS3DXWS_AUTH_USERNAME";
        const string DS3DXWS_AUTH_PASSWORD  = "DS3DXWS_AUTH_PASSWORD";
        const string DS3DXWS_AUTH_PASSPORT  = "DS3DXWS_AUTH_PASSPORT";
        const string DS3DXWS_AUTH_ENOVIA    = "DS3DXWS_AUTH_ENOVIA";
        const string DS3DXWS_AUTH_TENANT    = "DS3DXWS_AUTH_TENANT";
        const string SECURITY_CONTEXT       = "VPLMProjectLeader.Company Name.AAA27 Personal";

        const string CUSTOM_PROP_NAME_1_DBL = "AAA27_REAL_TEST";
        const string CUSTOM_PROP_NAME_2_INT = "AAA27_INT_TEST";

        string m_username    = string.Empty;
        string m_password    = string.Empty;
        string m_passportUrl = string.Empty;
        string m_enoviaUrl   = string.Empty;
        string m_tenant      = string.Empty;

        UserInfo m_userInfo  = null;

        public async Task<IPassportAuthentication> Authenticate()
        {
            UserPassport passport = new UserPassport(m_passportUrl);

            UserInfoRedirection userInfoRedirection = new UserInfoRedirection(m_enoviaUrl, m_tenant)
            {
                Current = true,
                IncludeCollaborativeSpaces = true,
                IncludePreferredCredentials = true
            };

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


        [TestCase(SECURITY_CONTEXT, "cmt-R1132100982379-00000465", "A.1")]
        public async Task Get_ManufacturedMaterial_withCustomAndMfgEnterpriseAtts2(string _securityContext, string _name, string _revision)
        {
            #region Arrange

            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Engineering Service wrapper
            ManufacturingItemService manufacturingServices = new ManufacturingItemService(m_enoviaUrl, passport);
            manufacturingServices.SecurityContext = _securityContext;
            manufacturingServices.Tenant = m_tenant;

            SearchByNameRevision searchMfgNameRev = new SearchByNameRevision(_name, _revision);

            #endregion

            #region Act - Search Manufacturing Item
            NlsLabeledItemSet2<ManufacturingItem> searchReturnSet = await manufacturingServices.Search(searchMfgNameRev);

            string pid = searchReturnSet.member[0].Id;

            List<string> fieldsToInclude = new List<string>() { "dsmvcfg:attribute.isConfigured",
                "dsmveno:CustomerAttributes", "dsmveno:SupportedTypes" };

            NlsLabeledItemSet2<ManufacturingItem> getReturnSet
                = await manufacturingServices.GetManufacturingItem2(pid, MFGResourceNames.DSMFG_MFGITEM_MASK_DETAILS, fieldsToInclude);

            ManufacturingItem mfgItem = getReturnSet.member[0];

            IList<string> NewProperty = ((dynamic)(mfgItem)).SupportedTypes;

            //NlsLabeledItemSet2<ManufacturingItemRead> getReturnSet2
            //  = await manufacturingServices.GetManufacturingItem2(pid, ManufacturingItemDefaultMask.Instance(), new List<IFields>() { new SupportedTypes() });

            //ManufacturingItemRead mfgItem2 = getReturnSet2.member[0];
            //dynamic supportedFields = (ISupportedTypesFields)mfgItem2;//?

            #endregion

            #region Assert
            StringAssert.AreEqualIgnoringCase(getReturnSet.member[0].Name, _name);
            StringAssert.AreEqualIgnoringCase(getReturnSet.member[0].Revision, _revision);

            #endregion
        }
    }
}