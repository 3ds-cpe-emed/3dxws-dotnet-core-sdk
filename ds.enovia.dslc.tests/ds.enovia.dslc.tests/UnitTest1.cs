using ds.authentication;
using ds.authentication.redirection;
using ds.enovia.common.collection;
using ds.enovia.common.search;
using ds.enovia.dseng.model;
using ds.enovia.dseng.service;
using ds.enovia.dslc.exception;
using ds.enovia.dslc.model;
using ds.enovia.dslc.service;
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

        public BusinessObjectData GetBusinessObjectData(string _physicalId, string _type, string _relativePath)
        {
            BusinessObjectDualIdentifier boId = new BusinessObjectDualIdentifier();
            boId.id = _physicalId;
            boId.identifier = _physicalId;
            //Note : It was verified that the source property of the input should be null (R2022XGA) this might change in future versions
            //boId.source = m_enoviaUrl; "$3DSpace";
            boId.type = _type;
            boId.relativePath = _relativePath;

            BusinessObjectData __boData = new BusinessObjectData();
            __boData.data = new List<BusinessObjectDualIdentifier>();
            __boData.data.Add(boId);

            return __boData;
        }

        [TestCase("VPLMAdmin.Company Name.Default", "prd-R1132100982379-00342846", "C.1" )]
        //[TestCase("VPLMAdmin.Company Name.Default", "prd-R1132100982379-00355456", "A.1")]
        public async Task Exercise_GetGraphVersion(string _securityContext, string _name, string _revision)
        {
            #region Arrange
            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Search for an Engineering product
            EngineeringServices engineeringServices = new EngineeringServices(m_enoviaUrl, passport);
            engineeringServices.SecurityContext = _securityContext;
            engineeringServices.Tenant = m_tenant;

            SearchByNameRevision searchByNameRevision = new SearchByNameRevision(_name, _revision);

            NlsLabeledItemSet<EngineeringItem> engItems = await engineeringServices.Search(searchByNameRevision);

            Assert.IsNotNull(engItems);

            Assert.AreEqual(engItems.totalItems, 1);

            EngineeringItem engItem = engItems.member[0];

            Assert.IsNotNull(engItem);

            string pid  = engItem.id;
            string type = engItem.type;
            string relativePath = $"/resources/v1/modeler/dseng/dseng:EngItem/{pid}";

            //Prepare Data
            BusinessObjectData data = GetBusinessObjectData(pid, type, relativePath);

            //Act - Initialize Collaborative Lifecycle Services
            CollaborativeLifecycleService lifecycleService = new CollaborativeLifecycleService(m_enoviaUrl, passport);

            lifecycleService.SecurityContext = _securityContext;
            lifecycleService.Tenant = m_tenant;
            #endregion
            
            #region Act
            //Call Collaborative Lifecycle Services Get Graph
            VersionGraph versionGraph =  await lifecycleService.GetVersionGraph(data);
            #endregion

            #region Assert
            Assert.IsNotNull(versionGraph);
            #endregion
        }


      [TestCase("VPLMAdmin.Company Name.Default", "863FB0B4307E495BB6543BBB90163077", "35B0B3DB487B00005F0388A60006EEB9", "Company Name", "AAA27 Personal", "eifuseremed")]
      //[TestCase("VPLMAdmin.Company Name.Default", "prd-R1132100982379-00355456", "A.1")]
      public async Task Exercise_TransferOwnership(string _securityContext, string _id1, string _id2, string _organization, string _collabSpace, string _newOwnerId)
      {
         #region Arrange
         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         OwnershipTransferData ownershipTransferData = new OwnershipTransferData();
         ownershipTransferData.collabspace = _collabSpace;
         ownershipTransferData.organization = _organization;
         ownershipTransferData.owner = _newOwnerId;

         ownershipTransferData.data.Add(new PhysicalId(_id1));
         ownershipTransferData.data.Add(new PhysicalId(_id2));
         
         CollaborativeLifecycleService lifecycleService = new CollaborativeLifecycleService(m_enoviaUrl, passport);

         lifecycleService.SecurityContext = _securityContext;
         lifecycleService.Tenant = m_tenant;
         #endregion

         #region Act
         //Call Collaborative Lifecycle Services Get Graph
         try
         {
            IOwnershipTransferResponse ownershipTransferResponse = await lifecycleService.TransferOwnership(ownershipTransferData);

            Assert.IsNotNull(ownershipTransferResponse);

            Assert.IsNotNull(ownershipTransferResponse.results);
            Assert.AreEqual(2, ownershipTransferResponse.results.Count);

            Assert.AreEqual(200, ownershipTransferResponse.results[0].status);
            Assert.AreEqual(200, ownershipTransferResponse.results[1].status);

            Assert.AreEqual(_newOwnerId, ownershipTransferResponse.results[0].owner);
            Assert.AreEqual(_newOwnerId, ownershipTransferResponse.results[1].owner);

            Assert.AreEqual(_organization, ownershipTransferResponse.results[0].organization);
            Assert.AreEqual(_organization, ownershipTransferResponse.results[1].organization);

            Assert.AreEqual(_collabSpace, ownershipTransferResponse.results[0].collabspace);
            Assert.AreEqual(_collabSpace, ownershipTransferResponse.results[1].collabspace);

         }
         catch (TransferOwnershipException _ex)
         {
            Assert.Fail(await _ex.GetErrorMessage());
         }
         catch (Exception _ex)
         {
            Assert.Fail(_ex.Message);
         }

         #endregion

      }
   }
}