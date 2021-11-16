//------------------------------------------------------------------------------------------------------------------------------------
// Copyright 2020 Dassault Systèmes - CPE EMED
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

namespace ds.delmia
{
    public static class MFGResourceNames
    {
        //DSMFG_PROCESS
        public static readonly string[] MANUFACTURED_MATERIAL  = { "CreateMaterial", "dsmfg:CreateMaterialEnterpriseAttributes" } ;
        public static readonly string[] PROVIDED_PART          = { "Provide", "dsmfg:ProvideEnterpriseAttributes" };
        public static readonly string[] MANUFACTURING_ASSEMBLY = { "CreateAssembly", "dsmfg:CreateAssemblyEnterpriseAttributes" };
        public static readonly string[] MANUFACTURING_KIT      = { "CreateKit", "dsmfg:CreateKitEnterpriseAttributes" };
        public static readonly string[] MANUFACTURING_INSTALLATION = { "Installation", "dsmfg:InstallationEnterpriseAttributes" };
        public static readonly string[] MANUFACTURED_PART = { "ElementaryEndItem", "dsmfg:ElementaryEndItemEnterpriseAttributes" };

        // Verify: Currently it seems that, even if can create them, we cannot read ServiceAssembly and ServiceKit through the Manufacturing Item APIs
        // 404 error (URI not Found)
        public static readonly string[] SERVICE_ASSEMBLY       = { "DELServiceAssemblyReference", "dsmfg:ServiceAssemblyEnterpriseAttributes" };
        public static readonly string[] SERVICE_KIT            = { "DELServiceKitReference", "dsmfg:ServiceKitEnterpriseAttributes" };

        // ProcessContinuousCreateMaterial
        // ProcessContinuousProvide
        // "status": 400,
        // "message": "Create Failed com.dassault_systemes.platform.model.CommonWebException"

        public static readonly string[] TRANSFORM       = { "Transform", "dsmfg:MarkEnterpriseAttributes" };
        public static readonly string[] MARK            = { "Marking", "dsmfg:MarkEnterpriseAttributes" };
        public static readonly string[] MACHINING       = { "Machine", "dsmfg:MachiningEnterpriseAttributes" };
        public static readonly string[] ANNOTATION      = { "Annotation", "dsmfg:AnnotationEnterpriseAttributes" };
        public static readonly string[] NO_DRILL        = { "NoDrill", "dsmfg:NoDrillEnterpriseAttributes" };
        public static readonly string[] CUT             = { "Cutting", "dsmfg:CutEnterpriseAttributes" };
        public static readonly string[] PRE_DRILL       = { "PreDrill", "dsmfg:PreDrillEnterpriseAttributes" };
        public static readonly string[] REMOVE_MATERIAL = { "RemoveMaterial", "dsmfg:RemoveMaterialEnterpriseAttributes" };
        public static readonly string[] GRIND           = { "Grinding", "dsmfg:GrindEnterpriseAttributes" };
        public static readonly string[] BEVEL           = { "Beveling", "dsmfg:BevelEnterpriseAttributes" };
        public static readonly string[] DRILL           = { "Drill", "dsmfg:DrillEnterpriseAttributes" };

        //DSPRCS_SYSTEM
        public static readonly string[] WORKPLAN              = { "DELLmiWorkPlanSystemReference", "dsprcs:WorkplanEnterpriseAttributes" };
        public static readonly string[] GENERAL_SYSTEM        = { "DELLmiGeneralSystemReference", "dsprcs:GeneralSystemEnterpriseAttributes" }; //?
        public static readonly string[] TRANSFORMATION_SYSTEM = { "DELLmiTransformationSystemReference", "dsprcs:TransformationSystemEnterpriseAttributes" }; //?
        public static readonly string[] SOURCE_SYSTEM         = { "SourceSystem", "dsprcs:SourceSystemEnterpriseAttributes" }; //?
        public static readonly string[] BUFFER_SYSTEM         = { "BufferSystem", "dsprcs:BufferSystemEnterpriseAttributes" }; //?
        public static readonly string[] SINK_SYSTEM           = { "SinkSystem", "dsprcs:SinkSystemEnterpriseAttributes" }; //?
        public static readonly string[] TRANSFER_SYSTEM       = { "DELLmiTransferSystemReference", "dsprcs:TransferSystemEnterpriseAttributes" };

        // Verify: Currently it seems that, even if can create them, we cannot read LogisticStation and LogisticTransfer through the Manufacturing Process APIs
        // 404 error (URI not Found)

        public static readonly string[] LOGISTIC_TRANSFER     = { "DELSCPLogisticsTransferSiteRef", "" };
        public static readonly string[] LOGISTIC_STATION      = { "DELSCPLogisticsSiteRef", "" };

    }
}
