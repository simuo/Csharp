﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace OQC_OUT.oqcsi {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="oqcsiSoap", Namespace="http://tempuri.org/")]
    public partial class oqcsi : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback dataupOperationCompleted;
        
        private System.Threading.SendOrPostCallback historyupOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetInspetorInfoOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public oqcsi() {
            this.Url = App.Config.POSTData.JGPUrl; //global::OQC_OUT.Properties.Settings.Default.JabilTray_IO_oqcsi_oqcsi;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event dataupCompletedEventHandler dataupCompleted;
        
        /// <remarks/>
        public event historyupCompletedEventHandler historyupCompleted;
        
        /// <remarks/>
        public event GetInspetorInfoCompletedEventHandler GetInspetorInfoCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/dataup", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string dataup(
                    string sn, 
                    string stationid, 
                    string ins1code, 
                    string ins2code, 
                    string ins3code, 
                    string ins4code, 
                    string ins5code, 
                    string ins6code, 
                    string ins1name, 
                    string ins2name, 
                    string ins3name, 
                    string ins4name, 
                    string ins5name, 
                    string ins6name, 
                    string clor, 
                    string region, 
                    string project, 
                    string location, 
                    string pahse) {
            object[] results = this.Invoke("dataup", new object[] {
                        sn,
                        stationid,
                        ins1code,
                        ins2code,
                        ins3code,
                        ins4code,
                        ins5code,
                        ins6code,
                        ins1name,
                        ins2name,
                        ins3name,
                        ins4name,
                        ins5name,
                        ins6name,
                        clor,
                        region,
                        project,
                        location,
                        pahse});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void dataupAsync(
                    string sn, 
                    string stationid, 
                    string ins1code, 
                    string ins2code, 
                    string ins3code, 
                    string ins4code, 
                    string ins5code, 
                    string ins6code, 
                    string ins1name, 
                    string ins2name, 
                    string ins3name, 
                    string ins4name, 
                    string ins5name, 
                    string ins6name, 
                    string clor, 
                    string region, 
                    string project, 
                    string location, 
                    string pahse) {
            this.dataupAsync(sn, stationid, ins1code, ins2code, ins3code, ins4code, ins5code, ins6code, ins1name, ins2name, ins3name, ins4name, ins5name, ins6name, clor, region, project, location, pahse, null);
        }
        
        /// <remarks/>
        public void dataupAsync(
                    string sn, 
                    string stationid, 
                    string ins1code, 
                    string ins2code, 
                    string ins3code, 
                    string ins4code, 
                    string ins5code, 
                    string ins6code, 
                    string ins1name, 
                    string ins2name, 
                    string ins3name, 
                    string ins4name, 
                    string ins5name, 
                    string ins6name, 
                    string clor, 
                    string region, 
                    string project, 
                    string location, 
                    string pahse, 
                    object userState) {
            if ((this.dataupOperationCompleted == null)) {
                this.dataupOperationCompleted = new System.Threading.SendOrPostCallback(this.OndataupOperationCompleted);
            }
            this.InvokeAsync("dataup", new object[] {
                        sn,
                        stationid,
                        ins1code,
                        ins2code,
                        ins3code,
                        ins4code,
                        ins5code,
                        ins6code,
                        ins1name,
                        ins2name,
                        ins3name,
                        ins4name,
                        ins5name,
                        ins6name,
                        clor,
                        region,
                        project,
                        location,
                        pahse}, this.dataupOperationCompleted, userState);
        }
        
        private void OndataupOperationCompleted(object arg) {
            if ((this.dataupCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.dataupCompleted(this, new dataupCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/historyup", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string historyup(string events, string incoming) {
            object[] results = this.Invoke("historyup", new object[] {
                        events,
                        incoming});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void historyupAsync(string events, string incoming) {
            this.historyupAsync(events, incoming, null);
        }
        
        /// <remarks/>
        public void historyupAsync(string events, string incoming, object userState) {
            if ((this.historyupOperationCompleted == null)) {
                this.historyupOperationCompleted = new System.Threading.SendOrPostCallback(this.OnhistoryupOperationCompleted);
            }
            this.InvokeAsync("historyup", new object[] {
                        events,
                        incoming}, this.historyupOperationCompleted, userState);
        }
        
        private void OnhistoryupOperationCompleted(object arg) {
            if ((this.historyupCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.historyupCompleted(this, new historyupCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetInspetorInfo", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetInspetorInfo(string InspectorID) {
            object[] results = this.Invoke("GetInspetorInfo", new object[] {
                        InspectorID});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetInspetorInfoAsync(string InspectorID) {
            this.GetInspetorInfoAsync(InspectorID, null);
        }
        
        /// <remarks/>
        public void GetInspetorInfoAsync(string InspectorID, object userState) {
            if ((this.GetInspetorInfoOperationCompleted == null)) {
                this.GetInspetorInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetInspetorInfoOperationCompleted);
            }
            this.InvokeAsync("GetInspetorInfo", new object[] {
                        InspectorID}, this.GetInspetorInfoOperationCompleted, userState);
        }
        
        private void OnGetInspetorInfoOperationCompleted(object arg) {
            if ((this.GetInspetorInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetInspetorInfoCompleted(this, new GetInspetorInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void dataupCompletedEventHandler(object sender, dataupCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class dataupCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal dataupCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void historyupCompletedEventHandler(object sender, historyupCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class historyupCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal historyupCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void GetInspetorInfoCompletedEventHandler(object sender, GetInspetorInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetInspetorInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetInspetorInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591