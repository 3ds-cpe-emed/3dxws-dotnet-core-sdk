using ds.authentication;
using ds.authentication.redirection;

using ds.enovia.common.collection;
using ds.enovia.common.model;
using ds.enovia.common.search;
using ds.enovia.dsdo.model;
using ds.enovia.dseng.mask;
using ds.enovia.dseng.model;
using ds.enovia.dseng.service;
using ds.enovia.dsxcad.model;
using ds.enovia.dsxcad.service;
using ds.enovia.model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ds.enovia.dsdo.tests
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

        [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "C437FF982377000063358D0600000C23", "C437FF982377000063358DB20010EF0F")]
        public async Task Get_DerivedOutpusByEngineeringItemId(string _securityContext, string _engineeringItemId1, string _engineeringItemId2)
        {
            //Authenticate
            IPassportAuthentication passport = await Authenticate();

            //Instantiate the Derived Output Service wrapper
            DerivedOutputService dsdoService = new DerivedOutputService(m_enoviaUrl, passport);

            dsdoService.SecurityContext = _securityContext;
            dsdoService.Tenant          = m_tenant;

            BusinessObjectId boId1 = new BusinessObjectId();

            boId1.id = _engineeringItemId1;
            boId1.type = "VPMReference";
            boId1.relativePath = "resource/v1/dsxcad/dsxcad:Part/" + _engineeringItemId1;
            boId1.source = "3DSpace";

            BusinessObjectId boId2 = new BusinessObjectId();

            boId2.id = _engineeringItemId2;
            boId2.type = "VPMReference";
            boId2.relativePath = "resource/v1/dsxcad/dsxcad:Part/" + _engineeringItemId2;
            boId2.source = "3DSpace";

            DerivedOutputsReferences derivedOutputsReferences = new DerivedOutputsReferences();
            derivedOutputsReferences.referencedObject.Add(boId1);
            derivedOutputsReferences.referencedObject.Add(boId2);

            ItemSet<DerivedOutputsLocate> locateSet = await dsdoService.LocateAsync(derivedOutputsReferences);

            Assert.IsNotNull(locateSet);

        }

      [TestCase("VPLMProjectLeader.Company Name.AAA27 Personal", "AAA27 Personal")]
      public async Task Search_DerivedOutpusByEngineeringItemId(string _securityContext, string _collaborativeSpace)
      {
         //Authenticate
         IPassportAuthentication passport = await Authenticate();

         //Instantiate the Derived Output Service wrapper
         EngineeringServices dsengService = new EngineeringServices(m_enoviaUrl, passport);

         dsengService.SecurityContext = _securityContext;
         dsengService.Tenant = m_tenant;

         xCADPartService xCadService = new xCADPartService(m_enoviaUrl, passport);
         xCadService.SecurityContext = _securityContext;
         xCadService.Tenant = m_tenant;

         SearchByCollaborativeSpace searchByCollaborativeSpace = new SearchByCollaborativeSpace(_collaborativeSpace);

         //IList<NlsLabeledItemSet<EngineeringItem>> engItems  = await dsengService.SearchAll(searchByCollaborativeSpace, EngineeringSearchMask.Default);
         IList<xCADPart> engItems = await xCadService.SearchAll<xCADPart>(searchByCollaborativeSpace);

         //Instantiate the Derived Output Service wrapper
         DerivedOutputService dsdoService = new DerivedOutputService(m_enoviaUrl, passport);

         dsdoService.SecurityContext = _securityContext;
         dsdoService.Tenant = m_tenant;

         try
         {
            foreach (xCADPart engItemSet in engItems)
            {
               BusinessObjectId boId1 = new BusinessObjectId();

               boId1.id = engItemSet.id;
               boId1.type = "VPMReference";
               boId1.relativePath = "resource/v1/dsxcad/dsxcad:Part/" + engItemSet.id;
               boId1.source = "3DSpace";

               DerivedOutputsReferences derivedOutputsReferences = new DerivedOutputsReferences();
               derivedOutputsReferences.referencedObject.Add(boId1);

               ItemSet<DerivedOutputsLocate> locateSet = await dsdoService.LocateAsync(derivedOutputsReferences);

               Assert.IsNotNull(locateSet);
            }
         }
         catch (DerivedOutputException _ex)
         {
            TestContext.Out.WriteLine(await _ex.GetErrorMessage());

         }
      }
   }
}