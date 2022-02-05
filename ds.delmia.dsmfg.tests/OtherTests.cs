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
    public class OtherTests
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

        UserInfo m_userInfo  = null;

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

        [Test]
        public void Serialization()
        {
            ManufacturingItemSetCreate mfgModelSetCreate = new ManufacturingItemSetCreate();

            ManufacturedMaterialCreate mfgCreate = new ManufacturedMaterialCreate
            {
                Title = "title",
                Description = "description"
            };

            mfgModelSetCreate.items.Add(mfgCreate);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonSerializableConverter<ManufacturedMaterialCreate>());

            JsonConverter converter = options.GetConverter(typeof(ManufacturedMaterialCreate));

            converter = options.GetConverter(typeof(JsonSerializable));

            string mfgItemSetPayload = JsonSerializer.Serialize<ManufacturingItemSetCreate>(mfgModelSetCreate,  options);
        }

    }
}