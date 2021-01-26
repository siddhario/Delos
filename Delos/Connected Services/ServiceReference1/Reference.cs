﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceReference1
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ProductGroup", Namespace="http://www.ct4partners.ba/B2B")]
    public partial class ProductGroup : object
    {
        
        private string CodeField;
        
        private string GroupDescriptionField;
        
        private ServiceReference1.ProductAttribute[] AttributesField;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string Code
        {
            get
            {
                return this.CodeField;
            }
            set
            {
                this.CodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string GroupDescription
        {
            get
            {
                return this.GroupDescriptionField;
            }
            set
            {
                this.GroupDescriptionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public ServiceReference1.ProductAttribute[] Attributes
        {
            get
            {
                return this.AttributesField;
            }
            set
            {
                this.AttributesField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ProductAttribute", Namespace="http://www.ct4partners.ba/B2B")]
    public partial class ProductAttribute : object
    {
        
        private string AttributeNameField;
        
        private string AttributeValueField;
        
        private string AttributeCodeField;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string AttributeName
        {
            get
            {
                return this.AttributeNameField;
            }
            set
            {
                this.AttributeNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string AttributeValue
        {
            get
            {
                return this.AttributeValueField;
            }
            set
            {
                this.AttributeValueField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string AttributeCode
        {
            get
            {
                return this.AttributeCodeField;
            }
            set
            {
                this.AttributeCodeField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CTPRODUCT", Namespace="http://www.ct4partners.ba/B2B")]
    public partial class CTPRODUCT : object
    {
        
        private string CODEField;
        
        private string PRODUCTGROUPCODEField;
        
        private string NAMEField;
        
        private string MANUFACTURERField;
        
        private string MANUFACTURERCODEField;
        
        private string QTTYINSTOCKField;
        
        private string TAXField;
        
        private string PRICEField;
        
        private string PRICENOTAXField;
        
        private string RETAILPRICEField;
        
        private string SHORT_DESCRIPTIONField;
        
        private string WARRANTYField;
        
        private string EUR_ExchangeRateField;
        
        private string BARCODEField;
        
        private string IMAGE_URLField;
        
        private ServiceReference1.ArrayOfString IMAGE_URLSField;
        
        private string SPECIALOFFERField;
        
        private ServiceReference1.ArrayOfProductAttribute1 ATTRIBUTESField;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string CODE
        {
            get
            {
                return this.CODEField;
            }
            set
            {
                this.CODEField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public string PRODUCTGROUPCODE
        {
            get
            {
                return this.PRODUCTGROUPCODEField;
            }
            set
            {
                this.PRODUCTGROUPCODEField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string NAME
        {
            get
            {
                return this.NAMEField;
            }
            set
            {
                this.NAMEField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string MANUFACTURER
        {
            get
            {
                return this.MANUFACTURERField;
            }
            set
            {
                this.MANUFACTURERField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string MANUFACTURERCODE
        {
            get
            {
                return this.MANUFACTURERCODEField;
            }
            set
            {
                this.MANUFACTURERCODEField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string QTTYINSTOCK
        {
            get
            {
                return this.QTTYINSTOCKField;
            }
            set
            {
                this.QTTYINSTOCKField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string TAX
        {
            get
            {
                return this.TAXField;
            }
            set
            {
                this.TAXField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string PRICE
        {
            get
            {
                return this.PRICEField;
            }
            set
            {
                this.PRICEField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=8)]
        public string PRICENOTAX
        {
            get
            {
                return this.PRICENOTAXField;
            }
            set
            {
                this.PRICENOTAXField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=9)]
        public string RETAILPRICE
        {
            get
            {
                return this.RETAILPRICEField;
            }
            set
            {
                this.RETAILPRICEField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=10)]
        public string SHORT_DESCRIPTION
        {
            get
            {
                return this.SHORT_DESCRIPTIONField;
            }
            set
            {
                this.SHORT_DESCRIPTIONField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=11)]
        public string WARRANTY
        {
            get
            {
                return this.WARRANTYField;
            }
            set
            {
                this.WARRANTYField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=12)]
        public string EUR_ExchangeRate
        {
            get
            {
                return this.EUR_ExchangeRateField;
            }
            set
            {
                this.EUR_ExchangeRateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=13)]
        public string BARCODE
        {
            get
            {
                return this.BARCODEField;
            }
            set
            {
                this.BARCODEField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=14)]
        public string IMAGE_URL
        {
            get
            {
                return this.IMAGE_URLField;
            }
            set
            {
                this.IMAGE_URLField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=15)]
        public ServiceReference1.ArrayOfString IMAGE_URLS
        {
            get
            {
                return this.IMAGE_URLSField;
            }
            set
            {
                this.IMAGE_URLSField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=16)]
        public string SPECIALOFFER
        {
            get
            {
                return this.SPECIALOFFERField;
            }
            set
            {
                this.SPECIALOFFERField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=17)]
        public ServiceReference1.ArrayOfProductAttribute1 ATTRIBUTES
        {
            get
            {
                return this.ATTRIBUTESField;
            }
            set
            {
                this.ATTRIBUTESField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ArrayOfString", Namespace="http://www.ct4partners.ba/B2B", ItemName="URL")]
    public class ArrayOfString : System.Collections.Generic.List<string>
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ArrayOfProductAttribute1", Namespace="http://www.ct4partners.ba/B2B", ItemName="ATTRIBUTE")]
    public class ArrayOfProductAttribute1 : System.Collections.Generic.List<ServiceReference1.ProductAttribute>
    {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.ct4partners.ba/B2B", ConfigurationName="ServiceReference1.CTProductsInStockSoap")]
    public interface CTProductsInStockSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ct4partners.ba/B2B/GetCTProductGroups", ReplyAction="*")]
        System.Threading.Tasks.Task<ServiceReference1.GetCTProductGroupsResponse> GetCTProductGroupsAsync(ServiceReference1.GetCTProductGroupsRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ct4partners.ba/B2B/GetCTProductGroups_WithAttributes", ReplyAction="*")]
        System.Threading.Tasks.Task<ServiceReference1.GetCTProductGroups_WithAttributesResponse> GetCTProductGroups_WithAttributesAsync(ServiceReference1.GetCTProductGroups_WithAttributesRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ct4partners.ba/B2B/GetCTProducts", ReplyAction="*")]
        System.Threading.Tasks.Task<ServiceReference1.GetCTProductsResponse> GetCTProductsAsync(ServiceReference1.GetCTProductsRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://www.ct4partners.ba/B2B/GetCTProducts_WithAttributes", ReplyAction="*")]
        System.Threading.Tasks.Task<ServiceReference1.GetCTProducts_WithAttributesResponse> GetCTProducts_WithAttributesAsync(ServiceReference1.GetCTProducts_WithAttributesRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCTProductGroupsRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCTProductGroups", Namespace="http://www.ct4partners.ba/B2B", Order=0)]
        public ServiceReference1.GetCTProductGroupsRequestBody Body;
        
        public GetCTProductGroupsRequest()
        {
        }
        
        public GetCTProductGroupsRequest(ServiceReference1.GetCTProductGroupsRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.ct4partners.ba/B2B")]
    public partial class GetCTProductGroupsRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        public GetCTProductGroupsRequestBody()
        {
        }
        
        public GetCTProductGroupsRequestBody(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCTProductGroupsResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCTProductGroupsResponse", Namespace="http://www.ct4partners.ba/B2B", Order=0)]
        public ServiceReference1.GetCTProductGroupsResponseBody Body;
        
        public GetCTProductGroupsResponse()
        {
        }
        
        public GetCTProductGroupsResponse(ServiceReference1.GetCTProductGroupsResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.ct4partners.ba/B2B")]
    public partial class GetCTProductGroupsResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public ServiceReference1.ProductGroup[] GetCTProductGroupsResult;
        
        public GetCTProductGroupsResponseBody()
        {
        }
        
        public GetCTProductGroupsResponseBody(ServiceReference1.ProductGroup[] GetCTProductGroupsResult)
        {
            this.GetCTProductGroupsResult = GetCTProductGroupsResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCTProductGroups_WithAttributesRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCTProductGroups_WithAttributes", Namespace="http://www.ct4partners.ba/B2B", Order=0)]
        public ServiceReference1.GetCTProductGroups_WithAttributesRequestBody Body;
        
        public GetCTProductGroups_WithAttributesRequest()
        {
        }
        
        public GetCTProductGroups_WithAttributesRequest(ServiceReference1.GetCTProductGroups_WithAttributesRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.ct4partners.ba/B2B")]
    public partial class GetCTProductGroups_WithAttributesRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        public GetCTProductGroups_WithAttributesRequestBody()
        {
        }
        
        public GetCTProductGroups_WithAttributesRequestBody(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCTProductGroups_WithAttributesResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCTProductGroups_WithAttributesResponse", Namespace="http://www.ct4partners.ba/B2B", Order=0)]
        public ServiceReference1.GetCTProductGroups_WithAttributesResponseBody Body;
        
        public GetCTProductGroups_WithAttributesResponse()
        {
        }
        
        public GetCTProductGroups_WithAttributesResponse(ServiceReference1.GetCTProductGroups_WithAttributesResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.ct4partners.ba/B2B")]
    public partial class GetCTProductGroups_WithAttributesResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public ServiceReference1.ProductGroup[] GetCTProductGroups_WithAttributesResult;
        
        public GetCTProductGroups_WithAttributesResponseBody()
        {
        }
        
        public GetCTProductGroups_WithAttributesResponseBody(ServiceReference1.ProductGroup[] GetCTProductGroups_WithAttributesResult)
        {
            this.GetCTProductGroups_WithAttributesResult = GetCTProductGroups_WithAttributesResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCTProductsRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCTProducts", Namespace="http://www.ct4partners.ba/B2B", Order=0)]
        public ServiceReference1.GetCTProductsRequestBody Body;
        
        public GetCTProductsRequest()
        {
        }
        
        public GetCTProductsRequest(ServiceReference1.GetCTProductsRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.ct4partners.ba/B2B")]
    public partial class GetCTProductsRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string productGroupCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string manufacturerCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string searchphrase;
        
        public GetCTProductsRequestBody()
        {
        }
        
        public GetCTProductsRequestBody(string username, string password, string productGroupCode, string manufacturerCode, string searchphrase)
        {
            this.username = username;
            this.password = password;
            this.productGroupCode = productGroupCode;
            this.manufacturerCode = manufacturerCode;
            this.searchphrase = searchphrase;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCTProductsResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCTProductsResponse", Namespace="http://www.ct4partners.ba/B2B", Order=0)]
        public ServiceReference1.GetCTProductsResponseBody Body;
        
        public GetCTProductsResponse()
        {
        }
        
        public GetCTProductsResponse(ServiceReference1.GetCTProductsResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.ct4partners.ba/B2B")]
    public partial class GetCTProductsResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public ServiceReference1.CTPRODUCT[] GetCTProductsResult;
        
        public GetCTProductsResponseBody()
        {
        }
        
        public GetCTProductsResponseBody(ServiceReference1.CTPRODUCT[] GetCTProductsResult)
        {
            this.GetCTProductsResult = GetCTProductsResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCTProducts_WithAttributesRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCTProducts_WithAttributes", Namespace="http://www.ct4partners.ba/B2B", Order=0)]
        public ServiceReference1.GetCTProducts_WithAttributesRequestBody Body;
        
        public GetCTProducts_WithAttributesRequest()
        {
        }
        
        public GetCTProducts_WithAttributesRequest(ServiceReference1.GetCTProducts_WithAttributesRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.ct4partners.ba/B2B")]
    public partial class GetCTProducts_WithAttributesRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string username;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string password;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string productGroupCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string manufacturerCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string searchphrase;
        
        public GetCTProducts_WithAttributesRequestBody()
        {
        }
        
        public GetCTProducts_WithAttributesRequestBody(string username, string password, string productGroupCode, string manufacturerCode, string searchphrase)
        {
            this.username = username;
            this.password = password;
            this.productGroupCode = productGroupCode;
            this.manufacturerCode = manufacturerCode;
            this.searchphrase = searchphrase;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetCTProducts_WithAttributesResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetCTProducts_WithAttributesResponse", Namespace="http://www.ct4partners.ba/B2B", Order=0)]
        public ServiceReference1.GetCTProducts_WithAttributesResponseBody Body;
        
        public GetCTProducts_WithAttributesResponse()
        {
        }
        
        public GetCTProducts_WithAttributesResponse(ServiceReference1.GetCTProducts_WithAttributesResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.ct4partners.ba/B2B")]
    public partial class GetCTProducts_WithAttributesResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public ServiceReference1.CTPRODUCT[] GetCTProducts_WithAttributesResult;
        
        public GetCTProducts_WithAttributesResponseBody()
        {
        }
        
        public GetCTProducts_WithAttributesResponseBody(ServiceReference1.CTPRODUCT[] GetCTProducts_WithAttributesResult)
        {
            this.GetCTProducts_WithAttributesResult = GetCTProducts_WithAttributesResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface CTProductsInStockSoapChannel : ServiceReference1.CTProductsInStockSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class CTProductsInStockSoapClient : System.ServiceModel.ClientBase<ServiceReference1.CTProductsInStockSoap>, ServiceReference1.CTProductsInStockSoap
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public CTProductsInStockSoapClient(EndpointConfiguration endpointConfiguration) : 
                base(CTProductsInStockSoapClient.GetBindingForEndpoint(endpointConfiguration), CTProductsInStockSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CTProductsInStockSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(CTProductsInStockSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CTProductsInStockSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(CTProductsInStockSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CTProductsInStockSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ServiceReference1.GetCTProductGroupsResponse> ServiceReference1.CTProductsInStockSoap.GetCTProductGroupsAsync(ServiceReference1.GetCTProductGroupsRequest request)
        {
            return base.Channel.GetCTProductGroupsAsync(request);
        }
        
        public System.Threading.Tasks.Task<ServiceReference1.GetCTProductGroupsResponse> GetCTProductGroupsAsync(string username, string password)
        {
            ServiceReference1.GetCTProductGroupsRequest inValue = new ServiceReference1.GetCTProductGroupsRequest();
            inValue.Body = new ServiceReference1.GetCTProductGroupsRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            return ((ServiceReference1.CTProductsInStockSoap)(this)).GetCTProductGroupsAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ServiceReference1.GetCTProductGroups_WithAttributesResponse> ServiceReference1.CTProductsInStockSoap.GetCTProductGroups_WithAttributesAsync(ServiceReference1.GetCTProductGroups_WithAttributesRequest request)
        {
            return base.Channel.GetCTProductGroups_WithAttributesAsync(request);
        }
        
        public System.Threading.Tasks.Task<ServiceReference1.GetCTProductGroups_WithAttributesResponse> GetCTProductGroups_WithAttributesAsync(string username, string password)
        {
            ServiceReference1.GetCTProductGroups_WithAttributesRequest inValue = new ServiceReference1.GetCTProductGroups_WithAttributesRequest();
            inValue.Body = new ServiceReference1.GetCTProductGroups_WithAttributesRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            return ((ServiceReference1.CTProductsInStockSoap)(this)).GetCTProductGroups_WithAttributesAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ServiceReference1.GetCTProductsResponse> ServiceReference1.CTProductsInStockSoap.GetCTProductsAsync(ServiceReference1.GetCTProductsRequest request)
        {
            return base.Channel.GetCTProductsAsync(request);
        }
        
        public System.Threading.Tasks.Task<ServiceReference1.GetCTProductsResponse> GetCTProductsAsync(string username, string password, string productGroupCode, string manufacturerCode, string searchphrase)
        {
            ServiceReference1.GetCTProductsRequest inValue = new ServiceReference1.GetCTProductsRequest();
            inValue.Body = new ServiceReference1.GetCTProductsRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.productGroupCode = productGroupCode;
            inValue.Body.manufacturerCode = manufacturerCode;
            inValue.Body.searchphrase = searchphrase;
            return ((ServiceReference1.CTProductsInStockSoap)(this)).GetCTProductsAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ServiceReference1.GetCTProducts_WithAttributesResponse> ServiceReference1.CTProductsInStockSoap.GetCTProducts_WithAttributesAsync(ServiceReference1.GetCTProducts_WithAttributesRequest request)
        {
            return base.Channel.GetCTProducts_WithAttributesAsync(request);
        }
        
        public System.Threading.Tasks.Task<ServiceReference1.GetCTProducts_WithAttributesResponse> GetCTProducts_WithAttributesAsync(string username, string password, string productGroupCode, string manufacturerCode, string searchphrase)
        {
            ServiceReference1.GetCTProducts_WithAttributesRequest inValue = new ServiceReference1.GetCTProducts_WithAttributesRequest();
            inValue.Body = new ServiceReference1.GetCTProducts_WithAttributesRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            inValue.Body.productGroupCode = productGroupCode;
            inValue.Body.manufacturerCode = manufacturerCode;
            inValue.Body.searchphrase = searchphrase;
            return ((ServiceReference1.CTProductsInStockSoap)(this)).GetCTProducts_WithAttributesAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.CTProductsInStockSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.CTProductsInStockSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.CTProductsInStockSoap))
            {
                return new System.ServiceModel.EndpointAddress("http://www.ct4partners.ba/webservices/ctproductsinstock.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.CTProductsInStockSoap12))
            {
                return new System.ServiceModel.EndpointAddress("http://www.ct4partners.ba/webservices/ctproductsinstock.asmx");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            CTProductsInStockSoap,
            
            CTProductsInStockSoap12,
        }
    }
}
