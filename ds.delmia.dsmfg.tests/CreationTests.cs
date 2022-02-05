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

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using ds.authentication;
using ds.authentication.redirection;

using ds.enovia.model;

using ds.delmia.dsmfg.exception;
using ds.delmia.dsmfg.interfaces;
using ds.delmia.dsmfg.model.reference;
using ds.delmia.dsmfg.service;

namespace ds.delmia.dsmfg.tests
{
    public class CreationTests
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

        [TestCase(SECURITY_CONTEXT)]
        public async Task Create_ManufacturedMaterial(string _securityContext)
        {
            #region Arrange

            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Engineering Service wrapper
            ManufacturingItemService manufacturingServices = new ManufacturingItemService(m_enoviaUrl, passport);
            manufacturingServices.SecurityContext = _securityContext;
            manufacturingServices.Tenant = m_tenant;

            Type[] typesToCreate = new Type[] { typeof(ManufacturedMaterialCreate), typeof(ManufacturingInstallationCreate) , 
                typeof(ManufacturedPartCreate),typeof(ManufacturingAssemblyCreate), typeof(ManufacturingKitCreate)};

            //ProcessContinuousManufacturedMaterialCreate, 
            //ProcessContinuousProvidedPartCreate

            foreach (Type typeToCreate in typesToCreate)
            {
                string title = "Title of " + typeToCreate.Name;
                string description = "Description of  " + typeToCreate.Name;

                var instance = (dynamic)Activator.CreateInstance(typeToCreate);
                instance.Title       = title;
                instance.Description = description;
                
                #endregion

                #region Act - Create Manufacturing Item

                //NlsLabeledItemSet2<ManufacturingItem> itemSet = await manufacturingServices.CreateManufacturingItem(mfgCreate);
                IManufacturingItem item = await manufacturingServices.CreateManufacturingItem(instance);
                #endregion

                #region Assert
                Assert.IsNotNull(item);

                StringAssert.AreEqualIgnoringCase(item.Title, title);
                StringAssert.AreEqualIgnoringCase(item.Description, description);

                Console.WriteLine("item " + item.Id + " created successfully.");
            }
            #endregion
        }

        [TestCase(SECURITY_CONTEXT)]
        public async Task Create_Multiple_ManufacturedItem_Types(string _securityContext)
        {
            #region Arrange

            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Engineering Service wrapper
            ManufacturingItemService manufacturingServices = new ManufacturingItemService(m_enoviaUrl, passport)
            {
                SecurityContext = _securityContext,
                Tenant = m_tenant
            };

            Type[] typesToCreate = new Type[] { typeof(ManufacturedMaterialCreate), typeof(ManufacturingInstallationCreate), typeof(ManufacturedPartCreate),typeof(ManufacturingAssemblyCreate), typeof(ManufacturingKitCreate), typeof(ProvidedPartCreate)};

            //TODO:
            //ProcessContinuousManufacturedMaterialCreate, 
            //ProcessContinuousProvidedPartCreate

            foreach (Type typeToCreate in typesToCreate)
            {
                string title = typeToCreate.Name + " Item Title";
                string description = typeToCreate.Name + " Item Description";

                // Manufacturing Item Properties (for most, but not all)
                bool isTrue             = true;
                string planningRequired = "2"; //Yes
                string outsourced       = "1"; //No
                double customAttDouble  = 1234.567;
                int customAttInteger    = 1234;

                var instance = (dynamic)Activator.CreateInstance(typeToCreate);
                instance.Title = title;
                instance.Description = description;

                SetIfPropertyExists(typeToCreate, "IsLotNumberRequired", instance, isTrue);
                SetIfPropertyExists(typeToCreate, "PlanningRequired", instance, planningRequired);
                SetIfPropertyExists(typeToCreate, "Outsourced", instance, outsourced);
                SetIfPropertyExists(typeToCreate, "IsSerialNumberRequired", instance, isTrue);

                // Custom Attributes to be set against "All Manufacturing Items Types" (DELFmiFunctionPPRReference)
                // in Platform Administration - Content - Attributes Management
                instance.MfgItemEnterpriseAttributes = new Dictionary<string, object>() { };
                instance.MfgItemEnterpriseAttributes[CUSTOM_PROP_NAME_1_DBL] = customAttDouble;
                instance.MfgItemEnterpriseAttributes[CUSTOM_PROP_NAME_2_INT]  = customAttInteger;
                #endregion

                try
                {
                    #region Act - Create Manufacturing Item
                    var item = await manufacturingServices.CreateManufacturingItem(instance, MFGResourceNames.DSMFG_MFGITEM_MASK_DETAILS);               
                    #endregion

                    #region Assert
                    Assert.IsNotNull(item);

                    StringAssert.AreEqualIgnoringCase(item.Title, title);
                    StringAssert.AreEqualIgnoringCase(item.Description, description);

                    if (PropertyExists(item, "IsLotNumberRequired"))
                        Assert.IsTrue(item.IsLotNumberRequired);

                    if (PropertyExists(item, "IsSerialNumberRequired"))
                        Assert.IsTrue(item.IsSerialNumberRequired);

                    if (PropertyExists(item, "Outsourced"))
                        Assert.IsTrue(item.Outsourced.Equals("No") || item.Outsourced.Equals(outsourced));

                    if (PropertyExists(item, "PlanningRequired"))
                        StringAssert.AreEqualIgnoringCase(item.PlanningRequired, planningRequired);

                    //FIX Dictionary<string,object> returns where we have Jtokens
                    //Assert.AreEqual(customAttDouble, item.MfgItemEnterpriseAttributes[CUSTOM_PROP_NAME_1_DBL]);
                    //Assert.AreEqual(customAttInteger, item.MfgItemEnterpriseAttributes[CUSTOM_PROP_NAME_2_INT]);

                    Console.WriteLine("item " + item.Id + " created successfully.");
                }
                catch (ManufacturingResponseException _mex)
                {
                    Assert.Fail("ManufacturingResponseException: " + await _mex.GetErrorMessage());
                }
                catch (Exception ex)
                {
                    Assert.Fail("Exception: " + ex.Message);
                }
            }
            #endregion
        }
      
        private void SetIfPropertyExists(Type _type, string _propName, object objInstance, object value)
        {
            PropertyInfo prop = _type.GetProperty(_propName);
            if (prop != null)
            {
                prop.SetValue(objInstance, value);
            }
        }
        private bool PropertyExists(object _obj, string _propName)
        {
            return (_obj.GetType().GetProperty(_propName) != null);
        }

        #region Individual Sub Type Creation

        [TestCase(SECURITY_CONTEXT, "Manufactured Part Item Title", "Manufactured Part Item Description")]
        public async Task Create_ManufacturedPart_Basic(string _securityContext, string _title, string _description)
        {
            #region Arrange

            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate and initialize the Engineering Service wrapper
            ManufacturingItemService manufacturingServices = new ManufacturingItemService(m_enoviaUrl, passport)
            {
                SecurityContext = _securityContext,
                Tenant = m_tenant
            };

            ManufacturedPartCreate mfgCreate = new ManufacturedPartCreate
            {
                Title       = _title,
                Description = _description
            };

            #endregion

            #region Act - Create Manufacturing Item

            IManufacturingItem item = await manufacturingServices.CreateManufacturingItem(mfgCreate);
            
            #endregion

            #region Assert

            Assert.IsNotNull(item);
            
            StringAssert.AreEqualIgnoringCase(item.Title, _title);
            StringAssert.AreEqualIgnoringCase(item.Description, _description);

            #endregion
        }

        [TestCase(SECURITY_CONTEXT, "Manufactured Material Item Title", "Manufactured Material Item Description")]
        public async Task Create_ManufacturedMaterial_Basic(string _securityContext, string _title, string _description)
        {
            #region Arrange

            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Engineering Service wrapper
            ManufacturingItemService manufacturingServices = new ManufacturingItemService(m_enoviaUrl, passport)
            {
                SecurityContext = _securityContext,
                Tenant          = m_tenant
            };

            ManufacturedMaterialCreate mfgMatCreate = new ManufacturedMaterialCreate
            {
                Title       = _title,
                Description = _description
            };

            #endregion

            #region Act - Create Manufacturing Item

            IManufacturingItem item = await manufacturingServices.CreateManufacturingItem(mfgMatCreate);
            
            #endregion

            #region Assert
            
            Assert.IsNotNull(item);

            StringAssert.AreEqualIgnoringCase(item.Title, _title);
            StringAssert.AreEqualIgnoringCase(item.Description, _description);

            #endregion
        }

        #endregion
    }
}