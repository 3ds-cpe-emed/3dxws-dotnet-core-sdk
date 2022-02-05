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
        public const string DSMFG_MFGINST_MASK_DEFAULT                 = "dsmfg:MfgItemInstanceMask.Default";
        public const string DSMFG_MFGINST_MASK_DETAILS                 = "dsmfg:MfgItemInstanceMask.Details";
        public const string DSMFG_MFGPROC_MASK_DEFAULT                 = "dsprcs:MfgProcessMask.Default";
        public const string DSMFG_MFGPROC_MASK_DETAILS                 = "dsprcs:MfgProcessMask.Details";
        public const string DSMFG_MFGPROC_MASK_STRUCTURE               = "dsprcs:MfgProcessMask.Structure.ModelView.Index";
        public const string DSMFG_SCOPEENGITEM_MASK_DEFAULT            = "dsmfg:ScopeEngItemMask.Default";
        public const string DSMFG_MFGITEM_MASK_DEFAULT                 = "dsmfg:MfgItemMask.Default";
        public const string DSMFG_MFGITEM_MASK_DETAILS                 = "dsmfg:MfgItemMask.Details";
        public const string TYPE_ATT                                   = "type";

        public const string DSMFG_MFGENTERPRISEATTRIBUTES              = "dsmfg:MfgItemEnterpriseAttributes";

        public const string DSMFG_OUTSOURCED                           = "outsourced";
        public const string DSMFG_PLANNING_REQUIRED                    = "planningRequired";
        public const string DSMFG_IS_LOT_NUMBER_REQUIRED               = "isLotNumberRequired";
        public const string DSMFG_IS_SERIAL_NUMBER_REQUIRED            = "isSerialNumberRequired";

        //DSMFG_PROCESS
        public const string MANUFACTURED_MATERIAL_ENTERPRISE_ATTS      = "dsmfg:CreateMaterialEnterpriseAttributes" ;
        public const string MANUFACTURING_ASSEMBLY_ENTERPRISE_ATTS     = "dsmfg:CreateAssemblyEnterpriseAttributes";
        public const string PROVIDED_PART_ENTERPRISE_ATTS              = "dsmfg:ProvideEnterpriseAttributes";
        public const string MANUFACTURED_PART_ENTERPRISE_ATTS          = "dsmfg:ElementaryEndItemEnterpriseAttributes";
        public const string MANUFACTURING_INSTALLATION_ENTERPRISE_ATTS = "dsmfg:InstallationEnterpriseAttributes";
        public const string MANUFACTURING_KIT_ENTERPRISE_ATTS          = "dsmfg:CreateKitEnterpriseAttributes";
        public const string PROC_CONT_MANUF_MAT_ENTERPRISE_ATTS        = "dsmfg:ProcessContinuousCreateMaterialEnterpriseAttributes";
        public const string PROC_CONT_PROVIDED_PART_ENTERPRISE_ATTS    = "dsmfg:ProcessContinuousProvideEnterpriseAttributes";
        
        public const string MANUFACTURED_MATERIAL_TYPE                 = "CreateMaterial";
        public const string MANUFACTURING_ASSEMBLY_TYPE                = "CreateAssembly";
        public const string PROVIDED_PART_TYPE                         = "Provide";
        public const string MANUFACTURED_PART_TYPE                     = "ElementaryEndItem";
        public const string MANUFACTURING_INSTALLATION_TYPE            = "Installation";
        public const string MANUFACTURING_KIT_TYPE                     = "CreateKit";
        public const string PROC_CONT_MANUF_MAT_TYPE                   = "ProcessContinuousCreateMaterial";
        public const string PROC_CONT_PROVIDED_PART_TYPE               = "ProcessContinuousProvide";

        // Verify: Currently it seems that, even if can create them, we cannot read ServiceAssembly and ServiceKit through the Manufacturing Item APIs
        // 404 error (URI not Found) - Classes have been updated with an NotImplementedException
        public const string SERVICE_ASSEMBLY_ENTERPRISE_ATTS           = "dsmfg:ServiceAssemblyEnterpriseAttributes";
        public const string SERVICE_KIT_ENTERPRISE_ATTS                = "dsmfg:ServiceKitEnterpriseAttributes";
        public const string SERVICE_KIT_TYPE                           = "DELServiceKitReference";
        public const string SERVICE_ASSEMBLY_TYPE                      = "DELServiceAssemblyReference";


        // ProcessContinuousCreateMaterial
        // ProcessContinuousProvide
        // "status": 400,
        // "message": "Create Failed com.dassault_systemes.platform.model.CommonWebException"


        //public static readonly string[] TRANSFORM       = { "Transform", "dsmfg:MarkEnterpriseAttributes" };
        //public static readonly string[] MARK            = { "Marking", "dsmfg:MarkEnterpriseAttributes" };
        //public static readonly string[] MACHINING       = { "Machine", "dsmfg:MachiningEnterpriseAttributes" };
        //public static readonly string[] ANNOTATION      = { "Annotation", "dsmfg:AnnotationEnterpriseAttributes" };
        //public static readonly string[] NO_DRILL        = { "NoDrill", "dsmfg:NoDrillEnterpriseAttributes" };
        //public static readonly string[] CUT             = { "Cutting", "dsmfg:CutEnterpriseAttributes" };
        //public static readonly string[] PRE_DRILL       = { "PreDrill", "dsmfg:PreDrillEnterpriseAttributes" };
        //public static readonly string[] REMOVE_MATERIAL = { "RemoveMaterial", "dsmfg:RemoveMaterialEnterpriseAttributes" };
        //public static readonly string[] GRIND           = { "Grinding", "dsmfg:GrindEnterpriseAttributes" };
        //public static readonly string[] BEVEL           = { "Beveling", "dsmfg:BevelEnterpriseAttributes" };
        //public static readonly string[] DRILL           = { "Drill", "dsmfg:DrillEnterpriseAttributes" };

        //DSPRCS_SYSTEM
        public const string WORKPLAN_TYPE                               = "DELLmiWorkPlanSystemReference";
        public const string BUFFER_SYSTEM_TYPE                          = "BufferSystem";
        public const string SINK_SYSTEM_TYPE                            = "SinkSystem";
        public const string TRANSFER_SYSTEM_TYPE                        = "DELLmiTransferSystemReference";
        public const string SOURCE_SYSTEM_TYPE                          = "SourceSystem";
        public const string TRANSFORMATION_SYSTEM_TYPE                  = "DELLmiTransformationSystemReference";
        public const string GENERAL_SYSTEM_TYPE                         = "DELLmiGeneralSystemReference";

        public const string DSMFG_MFGPROC_ENTERPRISEATTRIBUTES          = "dsprcs:MfgProcessEnterpriseAttributes";
        public const string WORKPLAN_ENTERPRISE_ATTRIBUTES              = "dsprcs:WorkPlanEnterpriseAttributes";
        public const string BUFFER_SYSTEM_ENTERPRISE_ATTRIBUTES         = "dsprcs:BufferSystemEnterpriseAttributes";
        public const string TRANSFER_SYSTEM_ENTERPRISE_ATTRIBUTES       = "dsprcs:TransferSystemEnterpriseAttributes";        
        public const string SINK_SYSTEM_ENTERPRISE_ATTRIBUTES           = "dsprcs:SinkSystemEnterpriseAttributes";
        public const string SOURCE_SYSTEM_ENTERPRISE_ATTRIBUTES         = "dsprcs:SourceSystemEnterpriseAttributes";
        public const string TRANSFORMATION_SYSTEM_ENTERPRISE_ATTRIBUTES = "dsprcs:TransformationSystemEnterpriseAttributes";
        public const string GENERAL_SYSTEM_ENTERPRISE_ATTRIBUTES        = "dsprcs:GeneralSystemEnterpriseAttributes";

        // Verify: Currently it seems that, even if can create them, we cannot read LogisticStation and LogisticTransfer through the Manufacturing Process APIs
        // 404 error (URI not Found) - Classes have been updated with an NotImplementedException
        public const string LOGISTIC_TRANSFER_TYPE                      = "DELSCPLogisticsTransferSiteRef";
        public const string LOGISTIC_TRANSFER_ENTERPRISE_ATTRIBUTES     = "";
        public const string LOGISTIC_STATION_TYPE                       = "DELSCPLogisticsSiteRef";
        public const string LOGISTIC_STATION_ENTERPRISE_ATTRIBUTES      = "";

    }
}