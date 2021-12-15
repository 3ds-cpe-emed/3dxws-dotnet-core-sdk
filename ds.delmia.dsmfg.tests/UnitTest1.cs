using ds.authentication;
using ds.authentication.redirection;
using ds.delmia.dsmfg.mask;
using ds.delmia.dsmfg.model;
using ds.delmia.dsmfg.model.process;
using ds.delmia.dsmfg.service;
using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.model;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ds.delmia.dsmfg.tests
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

        [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "CreatedMaterial Item Title", "CreatedMaterial Item Description")]
        public async Task Create_ManufacturedMaterial(string _securityContext, string _title, string _description)
        {
            #region Arrange

            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Engineering Service wrapper
            ManufacturingItemService manufacturingServices = new ManufacturingItemService(m_enoviaUrl, passport);
            manufacturingServices.SecurityContext = _securityContext;
            manufacturingServices.Tenant = m_tenant;

            ManufacturedMaterialCreate mfgCreate = new ManufacturedMaterialCreate();

            ((CreateMaterialEnterpriseAttributes)mfgCreate.attributes).title = _title;
            ((CreateMaterialEnterpriseAttributes)mfgCreate.attributes).description = _description;

            #endregion

            #region Act - Create Manufacturing Item

            NlsLabeledItemSet<ManufacturingItem> itemSet = await manufacturingServices.CreateManufacturingItem(mfgCreate);
            #endregion

            #region Assert
            Assert.IsNotNull(itemSet);
            Assert.AreEqual(itemSet.totalItems, 1);

            StringAssert.AreEqualIgnoringCase(itemSet.member[0].title, _title);
            StringAssert.AreEqualIgnoringCase(itemSet.member[0].description, _description);

            #endregion
        }

        //dsmfg_MfgItem schema
        //2=Yes - 1=No 
        [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "CreatedMaterial Title", "CreatedMaterial Description", "2", "1", false, false)]
        public async Task Create_ManufacturedMaterial_withCustomAndMfgEnterpriseAtts(string _securityContext, string _title, string _description, string _outsourced, string _planningRequired, bool _isLotNumberRequired, bool _isSerialNumberRequired)
        {
            #region Arrange

            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Engineering Service wrapper
            ManufacturingItemService manufacturingServices = new ManufacturingItemService(m_enoviaUrl, passport);
            manufacturingServices.SecurityContext = _securityContext;
            manufacturingServices.Tenant = m_tenant;

            ManufacturedMaterialCreate mfgCreate = new ManufacturedMaterialCreate();

            ((CreateMaterialEnterpriseAttributes)mfgCreate.attributes).title = _title;
            ((CreateMaterialEnterpriseAttributes)mfgCreate.attributes).description = _description;

            //dsmfg_MfgItem schema
            //2=Yes - 1=No 
            ((CreateMaterialEnterpriseAttributes)mfgCreate.attributes).outsourced = _outsourced;
            ((CreateMaterialEnterpriseAttributes)mfgCreate.attributes).planningRequired = _planningRequired;
            ((CreateMaterialEnterpriseAttributes)mfgCreate.attributes).isLotNumberRequired = _isLotNumberRequired;
            ((CreateMaterialEnterpriseAttributes)mfgCreate.attributes).isSerialNumberRequired = _isSerialNumberRequired;

            #endregion

            #region Act - Create Manufacturing Item
            ManufacturingItemMask manufacturingItemMask = ManufacturingItemMask.Details;
            
            NlsLabeledItemSet<ManufacturingItem> itemSet = await manufacturingServices.CreateManufacturingItem(mfgCreate, manufacturingItemMask);
            #endregion

            #region Assert
            Assert.IsNotNull(itemSet);
            Assert.AreEqual(itemSet.totalItems, 1);

            StringAssert.AreEqualIgnoringCase(itemSet.member[0].title, _title);
            StringAssert.AreEqualIgnoringCase(itemSet.member[0].description, _description);

            #endregion
        }


        [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "cmt-R1132100982379-00000453", "A.1")]
        public async Task Get_ManufacturedMaterial_withCustomAndMfgEnterpriseAtts(string _securityContext, string _name, string _revision)
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
            NlsLabeledItemSet<ManufacturingItem> searchReturnSet = await manufacturingServices.Search(searchMfgNameRev);

            string pid = searchReturnSet.member[0].id;

            NlsLabeledItemSet<ManufacturingItem> getReturnSet = await manufacturingServices.GetManufacturingItemFieldsDetails(pid);

            ManufacturingItem mfgItem = getReturnSet.member[0];

            #endregion

            #region Assert
            #endregion
        }

        //public async Task Create_ManufacturingProcess(string _securityContext, string _title, string _description)
        //{
        //    #region Arrange

        //    //Authenticate
        //    IPassportAuthentication passport = await Authenticate();

        //    //Instantiate the Engineering Service wrapper
        //    EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
        //    engineeringServices.SecurityContext = _securityContext;
        //    engineeringServices.Tenant = m_tenant;

        //    EngineeringItemCreate itemCreate = new EngineeringItemCreate();

        //    ((EngineeringItemAttributes)itemCreate.attributes).title = _title;
        //    ((EngineeringItemAttributes)itemCreate.attributes).description = _description;

        //    #endregion

        //    #region Act - Create Engineering Item

        //    NlsLabeledItemSet<EngineeringItem> itemSet = await engineeringServices.CreateEngineeringItem(itemCreate);
        //    #endregion

        //    #region Assert
        //    Assert.IsNotNull(itemSet);
        //    Assert.AreEqual(itemSet.totalItems, 1);

        //    StringAssert.AreEqualIgnoringCase(itemSet.member[0].title, _title);
        //    StringAssert.AreEqualIgnoringCase(itemSet.member[0].description, _description);

        //    #endregion
        //}
    }
}