﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControlLibrary.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ControlLibrary.ControlLibrary.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function AjaxValidatorFunction(source, arguments)
        ///{
        ///	var webRequest = false;
        ///	if(window.XMLHttpRequest)
        ///	{ 
        ///		webRequest = new XMLHttpRequest();
        ///	} 
        ///	else if(window.ActiveXObject)
        ///	{
        ///		try 
        ///		{ 
        ///			webRequest = new ActiveXObject(&apos;Msxml2.XMLHTTP&apos;);
        ///		} 
        ///		catch (e1)
        ///		{ 
        ///			try 
        ///			{ 
        ///				webRequest = new ActiveXObject(&apos;Microsoft.XMLHTTP&apos;);
        ///			} 
        ///			catch (e2)  
        ///			{}
        ///		}
        ///	}
        ///	if(webRequest)
        ///	{
        ///		var Ctrl=document.getElementById(source.controltovalidate); 
        ///		var formText=&apos;QueryCtrl=&apos; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AjaxValidatorScript {
            get {
                return ResourceManager.GetString("AjaxValidatorScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function ReportViewSaveLog(id,actionType,url)
        ///{
        ///	var webRequest = false;
        ///	if(window.XMLHttpRequest)
        ///	{ 
        ///		webRequest = new XMLHttpRequest();
        ///	} 
        ///	else if(window.ActiveXObject)
        ///	{
        ///		try 
        ///		{ 
        ///			webRequest = new ActiveXObject(&apos;Msxml2.XMLHTTP&apos;);
        ///		} 
        ///		catch (e1)
        ///		{ 
        ///			try 
        ///			{ 
        ///				webRequest = new ActiveXObject(&apos;Microsoft.XMLHTTP&apos;);
        ///			} 
        ///			catch (e2)  
        ///			{}
        ///		}
        ///	}
        ///	if(webRequest)
        ///	{
        ///		var Ctrl=document.getElementById(id);
        ///		var formText=&apos;QueryCtrl=&apos;+encodeURI(id)+&apos;&amp;QueryData= [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ReportViewLogScript {
            get {
                return ResourceManager.GetString("ReportViewLogScript", resourceCulture);
            }
        }
    }
}
