// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Web.UI.HtmlControls;

namespace System.Web.UI.Features;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0044 // Add readonly modifier

internal class FormWriterFeature : IFormWriterFeature
{
    private const string HiddenClassName = "aspNetHidden";

    public FormWriterFeature(ClientScriptManager clientScript)
    {
        ClientScript = clientScript;
    }

    public ClientScriptManager ClientScript { get; }

    private bool _inOnFormRender;
    private bool _fOnFormRenderCalled;
    private bool _fRequireWebFormsScript;
    private bool _fWebFormsScriptRendered;
    private bool _fRequirePostBackScript;
    private bool _fPostBackScriptRendered;
    private bool _containsCrossPagePost;
    private Dictionary<String, String>? _hiddenFieldsToRender;

    public bool ClientSupportsJavaScript => true;

    public void OnFormRender()
    {
        if (_fOnFormRenderCalled)
        {
            throw new HttpException(SR.GetString("Multiple_forms_not_allowed"));
        }

        _fOnFormRenderCalled = true;
        _inOnFormRender = true;
    }

    public HtmlForm? Form { get; set; }

    public void BeginFormRender(HtmlTextWriter writer, string? formUniqueID)
    {
        writer.AddAttribute("class", HiddenClassName);
        writer.RenderBeginTag("div");
        writer.RenderEndTag();

        ClientScript.RenderHiddenFields(writer);
        RenderViewStateFields(writer);

        writer.WriteEndTag("div");
    }

    private void RenderViewStateFields(HtmlTextWriter writer)
    {
    }

    /// <devdoc>
    ///     Called by both adapters and default rendering after form rendering.
    /// </devdoc>
    public void OnFormPostRender(HtmlTextWriter writer)
    {
        _inOnFormRender = false;
        //if (_postFormRenderDelegate != null)
        //{
        //    _postFormRenderDelegate(writer, null);
        //}
    }

    public void ResetOnFormRenderCalled()
    {
        _fOnFormRenderCalled = false;
    }

    public void EndFormRender(HtmlTextWriter writer, string formUniqueID)
    {
        EndFormRenderArrayAndExpandoAttribute(writer, formUniqueID);
        EndFormRenderHiddenFields(writer, formUniqueID);
        EndFormRenderPostBackAndWebFormsScript(writer, formUniqueID);
    }

    private void EndFormRenderArrayAndExpandoAttribute(HtmlTextWriter writer, string formUniqueID)
    {
#if FALSE
        if (ClientSupportsJavaScript)
        {
            // Devdiv 9409 - Register the array for reenabling only after the controls have been processed,
            // so that list controls can have their children registered.
            if (RenderDisabledControlsScript)
            {
                foreach (Control control in EnabledControls)
                {
                    ClientScript.RegisterArrayDeclaration(EnabledControlArray, "'" + control.ClientID + "'");
                }
            }
            ClientScript.RenderArrayDeclares(writer);
            ClientScript.RenderExpandoAttribute(writer);
        }
#endif
    }

    internal void EndFormRenderHiddenFields(HtmlTextWriter writer, string formUniqueID)
    {
#if FALSE
        if (RequiresViewStateEncryptionInternal)
        {
            ClientScript.RegisterHiddenField(ViewStateEncryptionID, String.Empty);
        }

        if (_containsCrossPagePost)
        {
            string path = EncryptString(Request.CurrentExecutionFilePath, Purpose.WebForms_Page_PreviousPageID);
            ClientScript.RegisterHiddenField(previousPageID, path);
        }

        if (EnableEventValidation)
        {
            ClientScript.SaveEventValidationField();
        }

        if (ClientScript.HasRegisteredHiddenFields)
        {
            bool renderDivAroundHiddenInputs = RenderDivAroundHiddenInputs(writer);
            if (renderDivAroundHiddenInputs)
            {
                writer.WriteLine();
                writer.AddAttribute(HtmlTextWriterAttribute.Class, HiddenClassName);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
            }

            ClientScript.RenderHiddenFields(writer);

            if (renderDivAroundHiddenInputs)
            {
                writer.RenderEndTag(); // DIV
            }
        }
#endif
    }

    private void EndFormRenderPostBackAndWebFormsScript(HtmlTextWriter writer, string formUniqueID)
    {
#if FALSE
        if (ClientSupportsJavaScript)
        {
            if (_fRequirePostBackScript && !_fPostBackScriptRendered)
            {
                RenderPostBackScript(writer, formUniqueID);
            }

            if (_fRequireWebFormsScript && !_fWebFormsScriptRendered)
                RenderWebFormsScript(writer);
        }

        ClientScript.RenderClientStartupScripts(writer);
#endif
    }
}
